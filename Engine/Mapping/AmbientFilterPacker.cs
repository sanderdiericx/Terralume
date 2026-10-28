using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Terralume.Engine.Buffers.Data;
using Terralume.Engine.Constants;
using Terralume.Engine.Utils;
using Terraria;
using Terraria.ModLoader;

namespace Terralume.Engine.Mapping
{
    internal class AmbientFilterPacker : ModSystem
    {
        public static Texture2D AmbientFilterTexture { get; private set; }

        // 2D boolean array which mirrors the on-screen tile grid, false means ambient lighting cannot reach a tile
        private static bool[,] reachableTiles;
        private static int[,] bestTilePassCounts;

        private static Color[] reachablePixels = new Color[RenderConstants.MATERIAL_DATA_RESOLUTION * RenderConstants.MATERIAL_DATA_RESOLUTION];

        private static List<(Vector2, int)> queuedPositions = new List<(Vector2, int)>();
        private static List<(Vector2, int)> newQueuedPositions = new List<(Vector2, int)>();

        public override void Load()
        {
            base.Load();

            GraphicsDevice device = Main.graphics.GraphicsDevice;

            Main.QueueMainThreadAction(() =>
            {
                AmbientFilterTexture = new Texture2D(device, RenderConstants.MATERIAL_DATA_RESOLUTION, RenderConstants.MATERIAL_DATA_RESOLUTION);
            });
        }

        public override void Unload()
        {
            base.Unload();

            Main.QueueMainThreadAction(() =>
            {
                AmbientFilterTexture.Dispose();
            });
        }

        // Determines which tiles on the screen can have ambient lighting. any pockets not reachable by a light or the player through tile grid traversal cannot have ambient lighting
        public static void CalculateReachableTiles(TileCaptureParameter tileCaptureParameter, List<(Vector2, LightProfile)> onScreenLights)
        {
            Vector2 stride = tileCaptureParameter.TileStride;
            Vector2 minTile = tileCaptureParameter.MinTile;
            Vector2 maxTile = tileCaptureParameter.MaxTile;

            SetQueuedPositions(onScreenLights, tileCaptureParameter);

            Tile[,] tiles = TileHelper.CopyTileMap(minTile, maxTile);

            reachableTiles = new bool[tiles.GetLength(0), tiles.GetLength(1)];
            bestTilePassCounts = new int[tiles.GetLength(0), tiles.GetLength(1)];

            // Initialize all values to int.maxValue
            Span<int> flatSpan = MemoryMarshal.CreateSpan(ref bestTilePassCounts[0, 0], bestTilePassCounts.Length);
            flatSpan.Fill(int.MaxValue);

            while (queuedPositions.Count > 0)
            {
                UpdateQueuedPositions(tiles);
            }
        }

        // Packs reachable tiles into a texture
        public static void PackAmbientFilterTexture(TileCaptureParameter tileCaptureParameter)
        {
            for (int x = 0; x < reachableTiles.GetLength(0); x++)
            {
                for (int y = 0; y < reachableTiles.GetLength(1); y++)
                {
                    int index = y * (int)tileCaptureParameter.TileStride.X + x;

                    reachablePixels[index] = reachableTiles[x, y] ? new Color(1.0f, 1.0f, 1.0f) : new Color(0.0f, 0.0f, 0.0f);
                }
            }

            AmbientFilterTexture.SetData(reachablePixels);
        }

        private static void SetQueuedPositions(List<(Vector2, LightProfile)> onScreenLights, TileCaptureParameter tileCaptureParameter)
        {
            queuedPositions.Clear();

            // Add player
            Vector2 playerWorldPos = Main.player[Main.myPlayer].position;

            int playerX = (int)MathF.Floor(playerWorldPos.X / 16f) - (int)tileCaptureParameter.MinTile.X;
            int playerY = (int)MathF.Floor(playerWorldPos.Y / 16f) - (int)tileCaptureParameter.MinTile.Y;

            Vector2 playerPos = new Vector2(playerX, playerY);
            queuedPositions.Add((playerPos, 0));

            // Add lights
            foreach ((Vector2 position, LightProfile lightProfile) in onScreenLights)
            {
                // Only lights with a great enough influence can trigger ambient lighting
                if (lightProfile.Range * lightProfile.Intensity < RenderConstants.AMBIENT_FILTER_MIN_LIGHT_INFLUENCE)
                {
                    continue;
                }

                Vector2 worldPosition = position / Main.GameViewMatrix.Zoom + tileCaptureParameter.ScreenPosition;

                int x = (int)MathF.Floor(worldPosition.X / 16f) - (int)tileCaptureParameter.MinTile.X;
                int y = (int)MathF.Floor(worldPosition.Y / 16f) - (int)tileCaptureParameter.MinTile.Y;

                queuedPositions.Add((new Vector2(x, y), 0));
            }
        }

        private static void UpdateQueuedPositions(Tile[,] tiles)
        {
            newQueuedPositions.Clear();

            for (int i = queuedPositions.Count - 1; i >= 0; i--)
            {
                Vector2 position = queuedPositions[i].Item1;
                int tilePassCount = queuedPositions[i].Item2;

                int x = (int)position.X;
                int y = (int)position.Y;

                // If we are out of bounds of the tiles array, do not propogate further
                if (x < 0 || x >= reachableTiles.GetLength(0) || y < 0 || y >= reachableTiles.GetLength(1))
                {
                    queuedPositions.RemoveAt(i);
                    continue;
                }

                // If the tile has been visited by a closer propogator, do not propogate further
                if (tilePassCount >= bestTilePassCounts[x, y])
                {
                    queuedPositions.RemoveAt(i);
                    continue;
                }

                // If we reach a solid tile
                if (tiles[x, y].HasTile && Main.tileSolid[tiles[x, y].TileType])
                {
                    tilePassCount++;

                    // If it has passed the max amount of solid tiles, do not propogate further
                    if (tilePassCount >= RenderConstants.AMBIENT_FILTER_MAX_TILE_PASS_COUNT)
                    {
                        queuedPositions.RemoveAt(i);
                        continue;
                    }
                }
                else // Reset the tile pass count when we hit air
                {
                    tilePassCount = 0;
                }

                // Tile is reachable
                bestTilePassCounts[x, y] = tilePassCount;
                reachableTiles[x, y] = true;
                queuedPositions.RemoveAt(i);

                // Propogate in a cross pattern
                newQueuedPositions.Add((new Vector2(x + 1, y), tilePassCount));
                newQueuedPositions.Add((new Vector2(x - 1, y), tilePassCount));
                newQueuedPositions.Add((new Vector2(x, y + 1), tilePassCount));
                newQueuedPositions.Add((new Vector2(x, y - 1), tilePassCount));
            }

            queuedPositions.AddRange(newQueuedPositions);
        }
    }
}

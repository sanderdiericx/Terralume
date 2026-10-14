using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terralume.Engine.Buffers.Data;
using Terralume.Engine.Constants;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terralume.Engine.Mapping
{
    internal class SunOcclusionPacker : ModSystem
    {
        private static readonly int MAX_RAY_STEPS = 100;
        private static readonly int MAX_TILE_PASSES = 20;
        private static readonly Vector2 TILEMAP_PADDING = new Vector2(100, 100);

        // Tile IDs that the sun occlusion ray skips when determining if it hit a solid tile
        private static readonly HashSet<int> SKIPPED_TILES = new HashSet<int>
        {
            TileID.LeafBlock,
            TileID.LivingWood,
            TileID.Cloud,
            TileID.RainCloud,
            TileID.SnowCloud,
            TileID.Glass
        };

        public static Texture2D SunOcclusionTexture { get; private set; }
        private static float[] occlusionPixels = new float[RenderConstants.MATERIAL_DATA_RESOLUTION * RenderConstants.MATERIAL_DATA_RESOLUTION];

        public override void Load()
        {
            base.Load();

            GraphicsDevice device = Main.graphics.GraphicsDevice;

            Main.QueueMainThreadAction(() =>
            {
                SunOcclusionTexture = new Texture2D(device, RenderConstants.MATERIAL_DATA_RESOLUTION, RenderConstants.MATERIAL_DATA_RESOLUTION, false, SurfaceFormat.Single);
            });
        }

        public override void Unload()
        {
            base.Unload();

            Main.QueueMainThreadAction(() =>
            {
                SunOcclusionTexture.Dispose();
            });
        }

        public static void UpdateSunOcclusionValues(TileCaptureParameter tileCaptureParameter)
        {
            // buffer sized to the max visible tile area, reused every frame
            Array.Clear(occlusionPixels, 0, occlusionPixels.Length);

            // Extract min and max tile positions
            int minX = (int)tileCaptureParameter.MinTile.X, minY = (int)tileCaptureParameter.MinTile.Y;
            int maxX = (int)tileCaptureParameter.MaxTile.X, maxY = (int)tileCaptureParameter.MaxTile.Y;
            int stride = (int)tileCaptureParameter.TileStride.X;

            Vector2 sunDirection = new Vector2(0.2f, 1f);

            Main.NewText(sunDirection);

            // Grab a chunk of the main tilemap into our own 2D array, so that we can multithread the ray casts
            // Note: startPos is clamped here so the copy and the indexing below share the same origin
            Vector2 startPos = new Vector2(Math.Clamp(minX - (int)TILEMAP_PADDING.X, 0, Main.tile.Width), Math.Clamp(minY - (int)TILEMAP_PADDING.Y, 0, Main.tile.Height));
            Vector2 endPos = new Vector2(maxX + (int)TILEMAP_PADDING.X, maxY + (int)TILEMAP_PADDING.Y);

            Tile[,] tiles = CopyTileMap(startPos, endPos);

            // For each tile currently on screen, cast a ray toward the sun
            for (int y = minY; y < maxY; y++)
            {
                for (int x = minX; x < maxX; x++)
                {
                    int index = (y - minY) * stride + (x - minX);
                    if (index < 0 || index >= occlusionPixels.Length) continue;

                    // We cast a ray in the direction of the sun, if it doesnt reach the sun, we cast a ray in the opposite direction of the sun (along the x axis)
                    // This prevents caves and overhangs from blocking a bunch of surface light
                    float occlusion = CastSunRay(tiles, x - (int)startPos.X, y - (int)startPos.Y, sunDirection);

                    if (occlusion < 0.999f)
                    {
                        float mirroredOcclusion = CastSunRay(tiles, x - (int)startPos.X, y - (int)startPos.Y, new Vector2(sunDirection.X, sunDirection.Y));

                        occlusion = MathF.Max(occlusion, mirroredOcclusion);
                    }

                    occlusionPixels[index] = occlusion;
                }
            }
        }

        public static void PackSunOcclusionTexture()
        {
            SunOcclusionTexture.SetData(occlusionPixels);
        }

        // Casts a ray from that steps along the tile grid from a tile space position until it hits a solid tile or reaches a max iteration count
        private static float CastSunRay(Tile[,] tiles, int x, int y, Vector2 sunDir)
        {
            Vector2 origin = new Vector2(x, y);

            // Tile space is y-down, so flip y to march toward the sun
            Vector2 toSun = new Vector2(sunDir.X, -sunDir.Y);

            int tilePassCount = 0;

            // Step through the tile grid in the direction of the sun
            for (int i = 1; i <= MAX_RAY_STEPS; i++)
            {
                Vector2 currentPos = origin + (toSun * i);

                int sampleX = (int)MathF.Floor(currentPos.X);
                int sampleY = (int)MathF.Floor(currentPos.Y);

                // Ray is out of bounds, assume unobstructed path to the sky
                if (sampleX < 0 || sampleX >= tiles.GetLength(0) || sampleY < 0 || sampleY >= tiles.GetLength(1))
                {
                    return 1f;
                }

                Tile sampled = tiles[sampleX, sampleY];

                // Check if we can skip collision on this tile
                if (!sampled.HasTile || !Main.tileSolid[sampled.TileType] || SKIPPED_TILES.Contains(sampled.TileType) || sampled.IsActuated)
                {
                    continue;
                }

                // Ray has hit a solid, non-actuated and non-skippable tile
                tilePassCount++;

                if (tilePassCount > MAX_TILE_PASSES)
                {
                    return 0f;
                }
            }

            // This tile has an unobstructed path to the sky
            return 1f;
        }

        // Copies the Main.tile tilemap into a 2D array, only copies values within a specified bounding box
        private static Tile[,] CopyTileMap(Vector2 startPos, Vector2 endPos)
        {
            // Ensure start and end positions are within bounds
            int startX = (int)Math.Clamp(startPos.X, 0, Main.tile.Width);
            int startY = (int)Math.Clamp(startPos.Y, 0, Main.tile.Height);

            int endX = (int)Math.Clamp(endPos.X, 0, Main.tile.Width);
            int endY = (int)Math.Clamp(endPos.Y, 0, Main.tile.Height);

            Tile[,] tiles = new Tile[endX - startX, endY - startY];

            // Copy Main.tile values into tiles
            for (int x = startX; x < endX; x++)
            {
                for (int y = startY; y < endY; y++)
                {
                    tiles[x - startX, y - startY] = Main.tile[x, y];
                }
            }

            return tiles;
        }
    }
}
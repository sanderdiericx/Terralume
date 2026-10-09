using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terralume.Engine.Buffers.Data;
using Terralume.Engine.Constants;
using Terraria;
using Terraria.ModLoader;

namespace Terralume.Engine.Hooks.Tracking
{
    internal class SunOcclusionTracker : ModSystem
    {
        private static readonly int MAX_RAY_STEPS = 100;
        private static readonly Vector2 TILEMAP_PADDING = new Vector2(100, 100);

        public static Texture2D SunOcclusionTexture { get; private set; }
        private static Vector4[] occlusionPixels = new Vector4[RenderConstants.MATERIAL_DATA_RESOLUTION * RenderConstants.MATERIAL_DATA_RESOLUTION];

        public override void Load()
        {
            base.Load();

            GraphicsDevice device = Main.graphics.GraphicsDevice;

            Main.QueueMainThreadAction(() =>
            {
                SunOcclusionTexture = new Texture2D(device, RenderConstants.MATERIAL_DATA_RESOLUTION, RenderConstants.MATERIAL_DATA_RESOLUTION);
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

            // Calculate sun direction
            double timePercent = Main.time / (Main.dayTime ? Main.dayLength : Main.nightLength);
            float angle = (float)(Math.PI - (timePercent * Math.PI));
            Vector2 sunDirection = Vector2.Normalize(new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)));

            // Grab a chunk of the main tilemap into our own 2D array, so that we can multithread the ray casts
            Vector2 startPos = new Vector2(minX - (int)TILEMAP_PADDING.X, minY - (int)TILEMAP_PADDING.Y);
            Vector2 endPos = new Vector2(maxX - (int)TILEMAP_PADDING.X, maxY - (int)TILEMAP_PADDING.Y);

            Tile[,] tiles = CopyTileMap(startPos, endPos);

            int count = 0;

            // For each tile currently on screen, cast a ray in the negative direction of the sun
            for (int x = minX; x < maxX; x++)
            {
                for (int y = minY; y < maxY; y++)
                {
                    int tileMapX = x + (int)TILEMAP_PADDING.X;
                    int tileMapY = x + (int)TILEMAP_PADDING.Y;

                    // Ray returns the level of occlusion this tile has
                    // Note: packing the occlusion float into a ARGB texture is kind of wasteful compared to IDPacker
                    occlusionPixels[count] = new Vector4(CastSunRay(tiles, tileMapX, tileMapY, sunDirection));

                    count++;
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

            // Step through the tile grid in the opposite direction of the sun
            for (int i = 1; i <= MAX_RAY_STEPS; i++)
            {
                Vector2 currentPos = origin + (-sunDir * i);

                int sampleX = (int)MathF.Floor(currentPos.X);
                int sampleY = (int)MathF.Floor(currentPos.Y);

                // Ray is out of bounds, assume unobstructed path to the sky
                if (sampleX < 0 || sampleX > tiles.GetLength(0) || sampleY < 0 || sampleY > tiles.GetLength(1))
                {
                    return 1f;
                }

                Tile sampled = tiles[sampleX, sampleY];

                // Ray has hit tile, no sunlight shall reach this tile
                if (sampled.HasTile)
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

            Tile[,] tiles = new Tile[endX - startX + 1, endY - startY + 1];

            // Copy Main.tile values into tiles
            for (int x = startX; x < endX; x++)
            {
                for (int y = startY; y < endY; y++)
                {
                    tiles[x - startX, y - startX] = Main.tile[x, y]; 
                }
            }

            return tiles;
        }
    }
}

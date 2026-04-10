using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace Terralume.Engine.Utils
{
    // TO DO, optimize by caching pixel color arrays and others
    public static class TileHelper
    {
        // Builds flat 2 arrays for all visible tiles, One array contains tile IDs, the other specifies which indices contain tiles.
        //
        // Note: Indices without tiles will recieve tile ID 0 by default.
        public static TileSearchResult GetVisibleTiles()
        {
            Vector2 screenPos = Main.screenPosition;
            Vector2 viewSize = new Vector2(Main.screenWidth, Main.screenHeight);
            if (Main.GameViewMatrix != null)
            {
                viewSize /= Main.GameViewMatrix.Zoom;
            }

            int strideX = (int)Math.Ceiling(viewSize.X / 16.0) + 2;
            int strideY = (int)Math.Ceiling(viewSize.Y / 16.0) + 2;

            int minTileX = (int)Math.Floor(screenPos.X / 16.0);
            int minTileY = (int)Math.Floor(screenPos.Y / 16.0);

            int maxTileX = minTileX + strideX - 1;
            int maxTileY = minTileY + strideY - 1;

            ushort[] tileIDs = new ushort[strideX * strideY];
            bool[] tiles = new bool[strideX * strideY];

            int count = 0;

            for (int i = minTileY; i <= maxTileY; i++)
            {
                for (int j = minTileX; j <= maxTileX; j++)
                {
                    if (j < 0 || j >= Main.maxTilesX || i < 0 || i >= Main.maxTilesY)
                    {
                        tileIDs[count] = 0;
                        tiles[count] = false;
                    }
                    else
                    {
                        tileIDs[count] = (ushort)(Main.tile[j, i].HasTile ? Main.tile[j, i].TileType : 0);
                        tiles[count] = Main.tile[j, i].HasTile;
                    }
                    count++;
                }
            }

            return new TileSearchResult(tileIDs, tiles);
        }

        public static int GetHorizontalTileStride()
        {
            Vector2 viewSize = new Vector2(Main.screenWidth, Main.screenHeight);
            if (Main.GameViewMatrix != null)
            {
                viewSize /= Main.GameViewMatrix.Zoom;
            }

            return (int)Math.Ceiling(viewSize.X / 16.0) + 2;
        }

        public static int GetVerticalTileStride()
        {
            Vector2 viewSize = new Vector2(Main.screenWidth, Main.screenHeight);
            if (Main.GameViewMatrix != null)
            {
                viewSize /= Main.GameViewMatrix.Zoom;
            }

            return (int)Math.Ceiling(viewSize.Y / 16.0) + 2;
        }

        // Encodes an array of ushorts into a 2D texture
        public static void PackTileIDTexture(Texture2D tileIDTexture, ushort[] tileIDs)
        {
            int pixelCount = (tileIDs.Length + 1) / 2; // Ceiling division

            Color[] pixels = new Color[tileIDTexture.Width * tileIDTexture.Height];

            int safePixelCount = Math.Min(pixelCount, pixels.Length);

            for (int i = 0; i < pixelCount; i++)
            {
                int index = i * 2;
                ushort u0 = tileIDs[index];
                ushort u1 = (index + 1 < tileIDs.Length) ? tileIDs[index + 1] : (ushort)0;

                pixels[i] = new Color(
                    (byte)(u0 & 0xFF), // low byte of u0
                    (byte)(u0 >> 8), // high byte of u0
                    (byte)(u1 & 0xFF), // low byte of u1
                    (byte)(u1 >> 8) // high byte of u1
                );
            }

            tileIDTexture.SetData(pixels);
        }

        // Encodes an array of booleans into a 2D texture
        public static void PackTileTexture(Texture2D tileTexture, bool[] tiles)
        {
            int pixelCount = (tiles.Length + 3) / 4; // Ceiling division

            Color[] pixels = new Color[tileTexture.Width * tileTexture.Height];

            int safePixelCount = Math.Min(pixelCount, pixels.Length);

            for (int i = 0; i < safePixelCount; i++)
            {
                byte[] values = { 0, 0, 0, 0 };

                // Take 4 booleans from the tiles array and convert them into bytes
                for (int j = 0; j < 4; j++)
                {
                    if (i * 4 + j < tiles.Length)
                    {
                        values[j] = tiles[i * 4 + j] ? (byte)255 : (byte)0;
                    }
                }

                // Encode into each color channel
                pixels[i] = new Color(values[0], values[1], values[2], values[3]);
            }

            tileTexture.SetData(pixels);
        }
    }
}
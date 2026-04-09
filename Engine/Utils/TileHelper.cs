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

            int minTileX = (int)Math.Floor(screenPos.X / 16.0);
            int maxTileX = (int)Math.Ceiling((screenPos.X + viewSize.X) / 16.0);

            int minTileY = (int)Math.Floor(screenPos.Y / 16.0);
            int maxTileY = (int)Math.Ceiling((screenPos.Y + viewSize.Y) / 16.0);

            int xSize = maxTileX - minTileX + 1;
            int ySize = maxTileY - minTileY + 1;

            ushort[] tileIDs = new ushort[xSize * ySize];
            bool[] tiles = new bool[xSize * ySize];

            int count = 0;

            for (int i = minTileX; i <= maxTileX; i++)
            {
                for (int j = minTileY; j <= maxTileY; j++)
                {
                    int localX = i - minTileX;
                    int localY = j - minTileY;

                    if (i < 0 || i >= Main.maxTilesX || j < 0 || j >= Main.maxTilesY)
                    {
                        tileIDs[count] = 0;
                        tiles[count] = false;

                        count++;

                        continue;
                    }

                    tileIDs[count] = (ushort)(Main.tile[i, j].HasTile ? Main.tile[i, j].TileType : 0);
                    tiles[count] = Main.tile[i, j].HasTile;

                    count++;
                }
            }

            return new TileSearchResult(tileIDs, tiles);
        }

        // Finds the amount of tiles spanning the x axis visible on screen
        public static int GetHorizontalTileStride()
        {
            Vector2 screenPos = Main.screenPosition;

            Vector2 viewSize = new Vector2(Main.screenWidth, Main.screenHeight);
            if (Main.GameViewMatrix != null)
            {
                viewSize /= Main.GameViewMatrix.Zoom;
            }

            int minTileX = (int)Math.Floor(screenPos.X / 16.0);
            int maxTileX = (int)Math.Ceiling((screenPos.X + viewSize.X) / 16.0);

            return maxTileX - minTileX + 1;
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
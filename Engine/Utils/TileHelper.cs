using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;

namespace Terralume.Engine.Utils
{
    // TO DO, optimize by caching pixel color arrays and others
    public static class TileHelper
    {
        // Builds a flat array with tile ids of all visible tiles
        //
        // Note: Indices without tiles will recieve tile ID -1 by default, this method supports max 32,767 different tiles
        public static short[] GetVisibleTiles()
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

            short[] tileIDs = new short[strideX * strideY];

            int count = 0;

            for (int i = minTileY; i <= maxTileY; i++)
            {
                for (int j = minTileX; j <= maxTileX; j++)
                {
                    if (j < 0 || j >= Main.maxTilesX || i < 0 || i >= Main.maxTilesY)
                    {
                        tileIDs[count] = 0;
                    }
                    else
                    {
                        tileIDs[count] = (short)(Main.tile[j, i].HasTile ? Main.tile[j, i].TileType : -1);
                    }

                    count++;
                }
            }

            return tileIDs;
        }


        public static Vector2 GetTileStride()
        {
            Vector2 viewSize = new Vector2(Main.screenWidth, Main.screenHeight);
            if (Main.GameViewMatrix != null)
            {
                viewSize /= Main.GameViewMatrix.Zoom;
            }

            return new Vector2((int)Math.Ceiling(viewSize.X / 16.0) + 2, (int)Math.Ceiling(viewSize.Y / 16.0) + 2);
        }


        // Encodes an array of shorts into a 2D texture
        public static void PackTileIDTexture(Texture2D tileIDTexture, short[] tileIDs)
        {
            int pixelCount = (tileIDs.Length + 1) / 2; // Ceiling division

            Color[] pixels = new Color[tileIDTexture.Width * tileIDTexture.Height];

            int safePixelCount = Math.Min(pixelCount, pixels.Length);

            for (int i = 0; i < pixelCount; i++)
            {
                int index = i * 2;
                short u0 = tileIDs[index];
                short u1 = (index + 1 < tileIDs.Length) ? tileIDs[index + 1] : (short)-1;

                pixels[i] = new Color(
                    (byte)(u0 & 0xFF), // low byte of u0
                    (byte)(u0 >> 8), // high byte of u0
                    (byte)(u1 & 0xFF), // low byte of u1
                    (byte)(u1 >> 8) // high byte of u1
                );
            }

            tileIDTexture.SetData(pixels);
        }

        // Encodes a dictionary mapping tile ID to material vector into a texture, the position inside of a color inside the texture refers to its tile ID
        public static void PackMaterialTexture(Texture2D materialTexture, Dictionary<int, Vector3> mappedMaterials)
        {
            Color[] pixels = new Color[materialTexture.Width * materialTexture.Height];

            for (int i = 0; i < pixels.Length; i++)
            {
                Vector3 color = new Vector3(0.0f, 0.0f, 0.0f);
                mappedMaterials.TryGetValue(i, out color);

                pixels[i] = new Color(color.X, color.Y, color.Z);
            }

            materialTexture.SetData(pixels);
        }
    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terralume.Engine.Buffers.Data;
using Terralume.Engine.Constants;
using Terraria;

namespace Terralume.Engine.Utils
{
    public static class TileHelper
    {
        private static Color[] materialPixels = new Color[RenderConstants.MATERIAL_DATA_RESOLUTION * RenderConstants.MATERIAL_DATA_RESOLUTION];
        private static Color[] tileIDPixels = new Color[RenderConstants.MATERIAL_DATA_RESOLUTION * RenderConstants.MATERIAL_DATA_RESOLUTION];

        public static TileCaptureParameter CalculateTileCaptureParameter()
        {
            Vector2 screenSize = new Vector2(Main.screenWidth, Main.screenHeight);

            Vector2 viewSize = screenSize;
            if (Main.GameViewMatrix != null)
            {
                viewSize /= Main.GameViewMatrix.Zoom;
            }

            Vector2 screenPosition = Main.screenPosition + (screenSize * 0.5f) - (viewSize * 0.5f);

            int strideX = (int)Math.Ceiling(viewSize.X / 16.0) + 2;
            int strideY = (int)Math.Ceiling(viewSize.Y / 16.0) + 2;

            int minTileX = (int)Math.Floor(screenPosition.X / 16.0);
            int minTileY = (int)Math.Floor(screenPosition.Y / 16.0);

            int maxTileX = minTileX + strideX - 1;
            int maxTileY = minTileY + strideY - 1;

            return new TileCaptureParameter(screenSize, screenPosition, viewSize, new Vector2(strideX, strideY), new Vector2(minTileX, minTileY), new Vector2(maxTileX, maxTileY));
        }


        // Builds a flat array with tile ids of all visible tiles
        //
        // Note: Indices without tiles will recieve tile ID -1 by default, this method supports max 32,767 different tiles
        public static short[] GetVisibleTiles(TileCaptureParameter tileCaptureParameter)
        {
            Vector2 stride = tileCaptureParameter.TileStride;
            Vector2 minTile = tileCaptureParameter.MinTile;
            Vector2 maxTile = tileCaptureParameter.MaxTile;

            short[] tileIDs = new short[(int)stride.X * (int)stride.Y];

            int count = 0;

            for (int i = (int)minTile.Y; i <= maxTile.Y; i++)
            {
                for (int j = (int)minTile.X; j <= maxTile.X; j++)
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

        // Encodes an array of shorts into a 2D texture
        public static void PackTileIDTexture(Texture2D tileIDTexture, short[] tileIDs)
        {
            int pixelCount = (tileIDs.Length + 1) / 2; // Ceiling division

            int safePixelCount = Math.Min(pixelCount, RenderConstants.MATERIAL_DATA_RESOLUTION * RenderConstants.MATERIAL_DATA_RESOLUTION);

            for (int i = 0; i < safePixelCount; i++)
            {
                int index = i * 2;
                short u0 = tileIDs[index];
                short u1 = (index + 1 < tileIDs.Length) ? tileIDs[index + 1] : (short)-1;

                tileIDPixels[i] = new Color(
                    (byte)(u0 & 0xFF), // low byte of u0
                    (byte)(u0 >> 8), // high byte of u0
                    (byte)(u1 & 0xFF), // low byte of u1
                    (byte)(u1 >> 8) // high byte of u1
                );
            }

            tileIDTexture.SetData(tileIDPixels);
        }

        // Encodes a dictionary mapping tile ID to material vector into a texture, the position inside of a color inside the texture refers to its tile ID
        public static void PackMaterialTexture(Texture2D materialTexture, Dictionary<int, Vector3> mappedMaterials)
        {
            int materialCount = Math.Min(mappedMaterials.Count, RenderConstants.MATERIAL_DATA_RESOLUTION * RenderConstants.MATERIAL_DATA_RESOLUTION);

            for (int i = 0; i < materialCount; i++)
            {
                Vector3 color = new Vector3(0.0f, 0.0f, 0.0f);
                mappedMaterials.TryGetValue(i, out color);

                materialPixels[i] = new Color(color.X, color.Y, color.Z);
            }

            materialTexture.SetData(materialPixels);
        }
    }
}
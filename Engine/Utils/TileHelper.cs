using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terralume.Engine.Buffers.Data;
using Terralume.Engine.Constants;
using Terraria;
using Terraria.ID;

namespace Terralume.Engine.Utils
{
    // TO DO: Optimize Texture.SetData()
    public static class TileHelper
    {
        private static Color[] shortPixels = new Color[RenderConstants.MATERIAL_DATA_RESOLUTION * RenderConstants.MATERIAL_DATA_RESOLUTION];
        private static Color[] materialPixels = new Color[RenderConstants.MATERIAL_DATA_RESOLUTION * RenderConstants.MATERIAL_DATA_RESOLUTION];

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


        // Builds a flat array with tile ids of all visible tiles, and a second flat array with ids of all visible wall tiles
        //
        // Note: Indices without tiles will recieve tile ID -1 by default, this method supports max 32,767 different tiles/walls
        public static void GetVisibleIDs(TileCaptureParameter tileCaptureParameter, out short[] tileIDs, out short[] wallIDs)
        {
            Vector2 stride = tileCaptureParameter.TileStride;
            Vector2 minTile = tileCaptureParameter.MinTile;
            Vector2 maxTile = tileCaptureParameter.MaxTile;

            tileIDs = new short[(int)stride.X * (int)stride.Y];
            wallIDs = new short[(int)stride.X * (int)stride.Y];

            int count = 0;

            for (int i = (int)minTile.Y; i <= maxTile.Y; i++)
            {
                for (int j = (int)minTile.X; j <= maxTile.X; j++)
                {
                    if (j < 0 || j >= Main.maxTilesX || i < 0 || i >= Main.maxTilesY) // Out of bounds;
                    {
                        tileIDs[count] = -1;
                        wallIDs[count] = -1;
                    }
                    else
                    {
                        tileIDs[count] = (short)(Main.tile[j, i].HasTile ? Main.tile[j, i].TileType : -1);
                        wallIDs[count] = (short)(Main.tile[j, i].WallType != WallID.None ? Main.tile[j, i].WallType : -1);
                    }

                    count++;
                }
            }
        }

        // Encodes an array of shorts into a 2D texture
        //
        // Note: Each texture index stores two shorts
        public static void PackShortsIntoTexture(Texture2D texture, short[] shorts)
        {
            int pixelCount = (shorts.Length + 1) / 2;

            int safePixelCount = Math.Min(pixelCount, RenderConstants.MATERIAL_DATA_RESOLUTION * RenderConstants.MATERIAL_DATA_RESOLUTION);

            for (int i = 0; i < safePixelCount; i++)
            {
                int index = i * 2;
                short u0 = shorts[index];
                short u1 = (index + 1 < shorts.Length) ? shorts[index + 1] : (short)-1;

                shortPixels[i] = new Color(
                    (byte)(u0 & 0xFF), // low byte of u0
                    (byte)(u0 >> 8), // high byte of u0
                    (byte)(u1 & 0xFF), // low byte of u1
                    (byte)(u1 >> 8) // high byte of u1
                );
            }

            texture.SetData(shortPixels);
        }

        // Encodes a dictionary (mapping ID to material vector) into a texture, the index of a pixel inside the texture refers to its ID
        public static void PackMaterialTexture(Texture2D texture, Dictionary<int, Vector3> mappedMaterials)
        {
            int maxPixels = RenderConstants.MATERIAL_DATA_RESOLUTION * RenderConstants.MATERIAL_DATA_RESOLUTION;

            Array.Clear(materialPixels, 0, materialPixels.Length);

            foreach (var kvp in mappedMaterials)
            {
                int id = kvp.Key;

                if (id >= 0 && id < maxPixels)
                {
                    Vector3 color = kvp.Value;
                    materialPixels[id] = new Color(color.X, color.Y, color.Z);
                }
            }

            texture.SetData(materialPixels);
        }
    }
}
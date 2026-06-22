using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System;
using System.Collections.Generic;
using Terralume.Engine.Buffers.Data;
using Terralume.Engine.Constants;
using Terraria;

namespace Terralume.Engine.Utils
{
    public static class TileHelper
    {
        private static HalfVector2[] noisePixels = new HalfVector2[RenderConstants.MATERIAL_DATA_RESOLUTION * RenderConstants.MATERIAL_DATA_RESOLUTION];

        public static TileCaptureParameter CalculateTileCaptureParameter()
        {
            Vector2 screenSize = RenderConstants.SCREEN_SIZE;

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

            return new TileCaptureParameter(screenPosition, viewSize, new Vector2(strideX, strideY), new Vector2(minTileX, minTileY), new Vector2(maxTileX, maxTileY));
        }

        // Encodes a dictionary (mapping ID to noise vector) into a texture (HalfVector2)
        public static void PackNoiseTexture(Texture2D texture, Dictionary<int, Vector2> mappedNoise)
        {
            int maxPixels = RenderConstants.MATERIAL_DATA_RESOLUTION * RenderConstants.MATERIAL_DATA_RESOLUTION;

            Array.Clear(noisePixels, 0, noisePixels.Length);

            foreach (var kvp in mappedNoise)
            {
                int id = kvp.Key;

                if (id >= 0 && id < maxPixels)
                {
                    Vector2 noise = kvp.Value;
                    noisePixels[id] = new HalfVector2(noise.X, noise.Y);
                }
            }

            texture.SetData(noisePixels);
        }
    }
}
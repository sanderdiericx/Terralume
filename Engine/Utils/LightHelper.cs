using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System;
using System.Collections.Generic;
using Terralume.Engine.Buffers.Data;
using Terralume.Engine.Constants;

namespace Terralume.Engine.Utils
{
    public static class LightHelper
    {
        private static HalfVector4[] lightPositionPixels = new HalfVector4[RenderConstants.LIGHT_DATA_RESOLUTION * RenderConstants.LIGHT_DATA_RESOLUTION];
        private static Color[] lightColorPixels = new Color[RenderConstants.LIGHT_DATA_RESOLUTION * RenderConstants.LIGHT_DATA_RESOLUTION];

        public static void PackLightTextures(Texture2D lightPositionTexture, Texture2D lightColorTexture, List<(Vector2, LightProfile)> lightProfiles)
        {
            int lightCount = Math.Min(lightProfiles.Count, RenderConstants.LIGHT_DATA_RESOLUTION * RenderConstants.LIGHT_DATA_RESOLUTION);

            for (int i = 0; i < lightCount; i++)
            {
                Vector2 position = lightProfiles[i].Item1;
                Vector4 color = lightProfiles[i].Item2.Color;

                lightPositionPixels[i] = new HalfVector4(position.X, position.Y, lightProfiles[i].Item2.Range, lightProfiles[i].Item2.Intensity);
                lightColorPixels[i] = new Color(color.X, color.Y, color.Z, color.W);
            }

            lightPositionTexture.SetData(lightPositionPixels);
            lightColorTexture.SetData(lightColorPixels);
        }
    }
}

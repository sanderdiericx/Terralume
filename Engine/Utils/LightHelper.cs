using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System.Collections.Generic;
using Terralume.Engine.Buffers.Data;

namespace Terralume.Engine.Utils
{
    public static class LightHelper
    {
        private const int MAX_PIXELS = 32 * 32;

        private static HalfVector4[] lightPositionPixels = new HalfVector4[MAX_PIXELS];
        private static Color[] lightColorPixels = new Color[MAX_PIXELS];

        public static void PackLightTextures(Texture2D lightPositionTexture, Texture2D lightColorTexture, List<(Vector2, LightProfile)> lightProfiles)
        {
            int lightCount = System.Math.Min(lightProfiles.Count, MAX_PIXELS);

            for (int i = 0; i < lightCount; i++)
            {
                Vector2 position = lightProfiles[i].Item1;
                Vector4 color = lightProfiles[i].Item2.LightColor;

                lightPositionPixels[i] = new HalfVector4(position.X, position.Y, lightProfiles[i].Item2.LightRadius, 1f);
                lightColorPixels[i] = new Color(color.X, color.Y, color.Z, color.W);
            }

            lightPositionTexture.SetData(lightPositionPixels);
            lightColorTexture.SetData(lightColorPixels);
        }
    }
}

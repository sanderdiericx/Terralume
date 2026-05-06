using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terralume.Engine.Constants;

namespace Terralume.Engine.Utils
{
    public static class OutputHelper
    {
        public static Vector3[] GenerateSSAOKernel()
        {
            int sampleCount = RenderConstants.SSAO_SAMPLE_COUNT;

            Random rng = new Random(42);
            Vector3[] kernel = new Vector3[sampleCount];

            for (int i = 0; i < sampleCount; i++)
            {
                float x = (float)rng.NextDouble() * 2.0f - 1.0f;
                float y = (float)rng.NextDouble() * 2.0f - 1.0f;
                float z = (float)rng.NextDouble();

                Vector3 sample = new Vector3(x, y, z);
                sample.Normalize();

                float scale = (float)i / sampleCount;
                scale = MathHelper.Lerp(0.1f, 1.0f, scale * scale);
                sample *= scale;

                kernel[i] = sample;
            }

            return kernel;
        }

        // Texture must be 4x4
        public static void GenerateSSAONoiseTexture(Texture2D noiseTexture)
        {
            Random rng = new Random(123);
            Color[] pixels = new Color[16];

            for (int i = 0; i < 16; i++)
            {
                float angle = (float)(rng.NextDouble() * Math.PI * 2.0f);

                float x = (float)(Math.Cos(angle) * 0.5f + 0.5f);
                float y = (float)(Math.Sin(angle) * 0.5f + 0.5f);

                pixels[i] = new Color(x, y, 0f);
            }

            noiseTexture.SetData(pixels);
        }
    }
}

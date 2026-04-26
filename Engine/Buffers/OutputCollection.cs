using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace Terralume.Engine.Buffers
{
    public class OutputCollection : IDisposable
    {
        // LightPositionTexture and LightColorTexture must be the same size for the lighting shader to sample correctly
        private const int DATA_TEXTURE_RESOLUTION = 32;

        public RenderTarget2D LightingBuffer;

        public Texture2D LightPositionTexture;
        public Texture2D LightColorTexture;

        public int Width;
        public int Height;

        public OutputCollection(GraphicsDevice graphicsDevice, int width, int height)
        {
            LightingBuffer = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.None);

            // Stores (posX, posY, radius, unused)
            LightPositionTexture = new RenderTarget2D(graphicsDevice, DATA_TEXTURE_RESOLUTION, DATA_TEXTURE_RESOLUTION, false, SurfaceFormat.HalfVector4, DepthFormat.None);

            // Stores (lightColorRed, lightColorBlue, lightColorGreen, unused)
            LightColorTexture = new RenderTarget2D(graphicsDevice, DATA_TEXTURE_RESOLUTION, DATA_TEXTURE_RESOLUTION, false, SurfaceFormat.Color, DepthFormat.None);

            Width = width;
            Height = height;
        }

        public bool IsValid()
        {
            return Width == Main.graphics.GraphicsDevice.PresentationParameters.BackBufferWidth && Height == Main.graphics.GraphicsDevice.PresentationParameters.BackBufferHeight;
        }

        public void Dispose()
        {
            LightingBuffer.Dispose();

            LightPositionTexture.Dispose();
            LightColorTexture.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}

using Microsoft.Xna.Framework.Graphics;
using System;
using Terralume.Engine.Constants;
using Terraria;

namespace Terralume.Engine.Buffers
{
    public class OutputCollection : IDisposable
    {
        public RenderTarget2D LightingBuffer;

        public Texture2D LightPositionTexture;
        public Texture2D LightColorTexture;

        public int Width;
        public int Height;

        public OutputCollection(GraphicsDevice graphicsDevice, int width, int height)
        {
            LightingBuffer = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.None);

            // Stores (posX, posY, range, intensity)
            LightPositionTexture = new RenderTarget2D(graphicsDevice, RenderConstants.LIGHT_DATA_RESOLUTION, RenderConstants.LIGHT_DATA_RESOLUTION, false, SurfaceFormat.HalfVector4, DepthFormat.None);

            // Stores (lightColorRed, lightColorBlue, lightColorGreen, unused)
            LightColorTexture = new RenderTarget2D(graphicsDevice, RenderConstants.LIGHT_DATA_RESOLUTION, RenderConstants.LIGHT_DATA_RESOLUTION, false, SurfaceFormat.Color, DepthFormat.None);

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

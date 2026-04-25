using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace Terralume.Engine.Buffers
{
    public class OutputCollection : IDisposable
    {
        public RenderTarget2D LightingBuffer;

        public int Width;
        public int Height;

        public OutputCollection(GraphicsDevice graphicsDevice, int width, int height)
        {
            LightingBuffer = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.None);

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

            GC.SuppressFinalize(this);
        }
    }
}

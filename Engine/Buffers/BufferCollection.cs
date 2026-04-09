using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace Terralume.Engine.Buffers
{
    public class BufferCollection : IDisposable
    {
        public RenderTarget2D BackgroundBuffer;
        public RenderTarget2D AlbedoBuffer;

        public int Width;
        public int Height;

        public BufferCollection(GraphicsDevice graphicsDevice, int width, int height)
        {
            BackgroundBuffer = new RenderTarget2D(graphicsDevice, width, height);
            AlbedoBuffer = new RenderTarget2D(graphicsDevice, width, height);

            Width = width;
            Height = height;
        }

        public bool IsValid()
        {
            return Width == Main.graphics.GraphicsDevice.PresentationParameters.BackBufferWidth && Height == Main.graphics.GraphicsDevice.PresentationParameters.BackBufferHeight;
        }

        public void Dispose()
        {
            BackgroundBuffer.Dispose();
            AlbedoBuffer.Dispose();
        }
    }
}

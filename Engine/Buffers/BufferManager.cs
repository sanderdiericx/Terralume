using Microsoft.Xna.Framework.Graphics;
using System;

namespace Terralume.Engine.Buffers
{
    public class BufferManager : IDisposable
    {
        public RenderTarget2D BackgroundBuffer;
        public RenderTarget2D AlbedoBuffer;

        public int Width;
        public int Height;

        public BufferManager(GraphicsDevice graphicsDevice, int width, int height)
        {
            BackgroundBuffer = new RenderTarget2D(graphicsDevice, width, height);
            AlbedoBuffer = new RenderTarget2D(graphicsDevice, width, height);

            Width = width;
            Height = height;
        }

        void IDisposable.Dispose()
        {
            BackgroundBuffer.Dispose();
            AlbedoBuffer.Dispose();
        }
    }
}

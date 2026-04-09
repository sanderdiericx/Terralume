using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace Terralume.Engine.Buffers
{
    public class BufferCollection : IDisposable
    {
        public RenderTarget2D BackgroundBuffer;
        public RenderTarget2D AlbedoBuffer;

        public Texture2D TileIDTexture;
        public Texture2D TileTexture;

        public int Width;
        public int Height;

        public BufferCollection(GraphicsDevice graphicsDevice, int width, int height)
        {
            BackgroundBuffer = new RenderTarget2D(graphicsDevice, width, height);
            AlbedoBuffer = new RenderTarget2D(graphicsDevice, width, height);

            TileIDTexture = new Texture2D(graphicsDevice, 100, 100);
            TileTexture = new Texture2D(graphicsDevice, 100, 100);

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
            TileIDTexture.Dispose();
            TileTexture.Dispose();
        }
    }
}

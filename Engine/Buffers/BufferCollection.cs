using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace Terralume.Engine.Buffers
{
    public class BufferCollection : IDisposable
    {
        public RenderTarget2D BackgroundBuffer;

        public RenderTarget2D UndergroundBackgroundBufferMaster;
        public RenderTarget2D UndergroundBackgroundBuffer;

        public RenderTarget2D AlbedoBuffer;
        public RenderTarget2D DepthBuffer;

        public RenderTarget2D StableTileTarget;

        public Texture2D TileIDTexture;
        public Texture2D TileTexture;

        public int Width;
        public int Height;

        public BufferCollection(GraphicsDevice graphicsDevice, int width, int height)
        {
            BackgroundBuffer = new RenderTarget2D(graphicsDevice, width, height);

            UndergroundBackgroundBufferMaster = new RenderTarget2D(graphicsDevice, Main.instance.backgroundTarget.Width, Main.instance.backgroundTarget.Height);
            UndergroundBackgroundBuffer = new RenderTarget2D(graphicsDevice, width, height);

            AlbedoBuffer = new RenderTarget2D(graphicsDevice, width, height);
            DepthBuffer = new RenderTarget2D(graphicsDevice, width, height);

            StableTileTarget = new RenderTarget2D(graphicsDevice, width, height);

            TileIDTexture = new Texture2D(graphicsDevice, 150, 150);
            TileTexture = new Texture2D(graphicsDevice, 150, 150);

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

            UndergroundBackgroundBufferMaster.Dispose();
            UndergroundBackgroundBuffer.Dispose();

            AlbedoBuffer.Dispose();
            DepthBuffer.Dispose();

            StableTileTarget.Dispose();

            TileIDTexture.Dispose();
            TileTexture.Dispose();
        }
    }
}

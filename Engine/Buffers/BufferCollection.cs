using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;

namespace Terralume.Engine.Buffers
{
    public class BufferCollection : IDisposable
    {
        private const int DATA_TEXTURE_RESOLUTION = 256;

        public RenderTarget2D BackgroundBuffer;

        public RenderTarget2D UndergroundBackgroundBufferMaster;
        public RenderTarget2D UndergroundBackgroundBuffer;

        public RenderTarget2D NPCBuffer;
        public RenderTarget2D PlayerBuffer;
        public RenderTarget2D ProjectileBuffer;

        public RenderTarget2D AlbedoBuffer;
        public RenderTarget2D DepthBuffer;
        public RenderTarget2D MaterialBuffer;

        public RenderTarget2D StableTileTarget;
        public RenderTarget2D StableWallTarget;

        public Texture2D TileIDTexture;
        public Texture2D MaterialTexture;

        public RenderTarget2D CopyBuffer;

        public int Width;
        public int Height;

        public BufferCollection(GraphicsDevice graphicsDevice, int width, int height)
        {
            BackgroundBuffer = new RenderTarget2D(graphicsDevice, width, height);

            UndergroundBackgroundBufferMaster = new RenderTarget2D(graphicsDevice, Main.instance.backgroundTarget.Width, Main.instance.backgroundTarget.Height);
            UndergroundBackgroundBuffer = new RenderTarget2D(graphicsDevice, width, height);

            NPCBuffer = new RenderTarget2D(graphicsDevice, width, height);
            PlayerBuffer = new RenderTarget2D(graphicsDevice, width, height);
            ProjectileBuffer = new RenderTarget2D(graphicsDevice, width, height);

            AlbedoBuffer = new RenderTarget2D(graphicsDevice, width, height);
            DepthBuffer = new RenderTarget2D(graphicsDevice, width, height);
            MaterialBuffer = new RenderTarget2D(graphicsDevice, width, height);

            StableTileTarget = new RenderTarget2D(graphicsDevice, width, height);
            StableWallTarget = new RenderTarget2D(graphicsDevice, width, height);

            TileIDTexture = new Texture2D(graphicsDevice, DATA_TEXTURE_RESOLUTION, DATA_TEXTURE_RESOLUTION);
            MaterialTexture = new Texture2D(graphicsDevice, DATA_TEXTURE_RESOLUTION, DATA_TEXTURE_RESOLUTION);

            CopyBuffer = new RenderTarget2D(graphicsDevice, width, height, false, graphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

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

            NPCBuffer.Dispose();
            PlayerBuffer.Dispose();
            ProjectileBuffer.Dispose();

            AlbedoBuffer.Dispose();
            DepthBuffer.Dispose();
            MaterialBuffer.Dispose();

            StableTileTarget.Dispose();
            StableWallTarget.Dispose();

            TileIDTexture.Dispose();
            MaterialTexture.Dispose();

            CopyBuffer.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}

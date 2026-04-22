using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace Terralume.Engine.Buffers
{
    public class TargetCollection
    {
        public RenderTarget2D BackgroundBuffer;

        public RenderTarget2D UndergroundBackgroundBufferMaster;
        public RenderTarget2D UndergroundBackgroundBuffer;

        public RenderTarget2D EntityBuffer;
        public RenderTarget2D PlayerBuffer;
        public RenderTarget2D ProjectileBuffer;
        public RenderTarget2D NonSolidTileBuffer;

        public RenderTarget2D StableTileTarget;
        public RenderTarget2D StableWallTarget;

        public RenderTarget2D CopyBuffer;

        public int Width;
        public int Height;

        public TargetCollection(GraphicsDevice graphicsDevice, int width, int height)
        {
            BackgroundBuffer = new RenderTarget2D(graphicsDevice, width, height);

            UndergroundBackgroundBufferMaster = new RenderTarget2D(graphicsDevice, Main.instance.backgroundTarget.Width, Main.instance.backgroundTarget.Height);
            UndergroundBackgroundBuffer = new RenderTarget2D(graphicsDevice, width, height);

            EntityBuffer = new RenderTarget2D(graphicsDevice, width, height);
            PlayerBuffer = new RenderTarget2D(graphicsDevice, width, height);
            ProjectileBuffer = new RenderTarget2D(graphicsDevice, width, height);
            NonSolidTileBuffer = new RenderTarget2D(graphicsDevice, width, height);

            StableTileTarget = new RenderTarget2D(graphicsDevice, width, height);
            StableWallTarget = new RenderTarget2D(graphicsDevice, width, height);

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

            EntityBuffer.Dispose();
            PlayerBuffer.Dispose();
            ProjectileBuffer.Dispose();

            NonSolidTileBuffer.Dispose();

            StableTileTarget.Dispose();
            StableWallTarget.Dispose();

            CopyBuffer.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}

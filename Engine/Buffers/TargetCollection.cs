using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace Terralume.Engine.Buffers
{
    public class TargetCollection
    {
        public RenderTarget2D BackgroundTarget;

        public RenderTarget2D UndergroundBackgroundTargetMaster;
        public RenderTarget2D UndergroundBackgroundTarget;

        public RenderTarget2D EntityTarget;
        public RenderTarget2D PlayerTarget;
        public RenderTarget2D ProjectileTarget;
        public RenderTarget2D NonSolidTileTarget;

        public RenderTarget2D StableTileTarget;
        public RenderTarget2D StableWallTarget;

        public RenderTarget2D CopyTarget;

        public int Width;
        public int Height;

        public TargetCollection(GraphicsDevice graphicsDevice, int width, int height)
        {
            BackgroundTarget = new RenderTarget2D(graphicsDevice, width, height);

            UndergroundBackgroundTargetMaster = new RenderTarget2D(graphicsDevice, Main.instance.backgroundTarget.Width, Main.instance.backgroundTarget.Height);
            UndergroundBackgroundTarget = new RenderTarget2D(graphicsDevice, width, height);

            EntityTarget = new RenderTarget2D(graphicsDevice, width, height);
            PlayerTarget = new RenderTarget2D(graphicsDevice, width, height);
            ProjectileTarget = new RenderTarget2D(graphicsDevice, width, height);
            NonSolidTileTarget = new RenderTarget2D(graphicsDevice, width, height);

            StableTileTarget = new RenderTarget2D(graphicsDevice, width, height);
            StableWallTarget = new RenderTarget2D(graphicsDevice, width, height);

            CopyTarget = new RenderTarget2D(graphicsDevice, width, height, false, graphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

            Width = width;
            Height = height;
        }

        public bool IsValid()
        {
            return Width == Main.graphics.GraphicsDevice.PresentationParameters.BackBufferWidth && Height == Main.graphics.GraphicsDevice.PresentationParameters.BackBufferHeight;
        }

        public void Dispose()
        {
            BackgroundTarget.Dispose();

            UndergroundBackgroundTargetMaster.Dispose();
            UndergroundBackgroundTarget.Dispose();

            EntityTarget.Dispose();
            PlayerTarget.Dispose();
            ProjectileTarget.Dispose();

            NonSolidTileTarget.Dispose();

            StableTileTarget.Dispose();
            StableWallTarget.Dispose();

            CopyTarget.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}

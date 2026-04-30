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

        public RenderTarget2D NonSolidTileTarget;
        public RenderTarget2D TileTarget;
        public RenderTarget2D WallTarget;

        public RenderTarget2D EntityTarget;
        public RenderTarget2D PlayerTarget;
        public RenderTarget2D ProjectileTarget;

        public RenderTarget2D CopyTarget;

        public int Width;
        public int Height;

        public TargetCollection(GraphicsDevice graphicsDevice, int width, int height)
        {
            BackgroundTarget = new RenderTarget2D(graphicsDevice, width, height);

            UndergroundBackgroundTargetMaster = new RenderTarget2D(graphicsDevice, Main.instance.backgroundTarget.Width, Main.instance.backgroundTarget.Height);
            UndergroundBackgroundTarget = new RenderTarget2D(graphicsDevice, width, height);

            NonSolidTileTarget = new RenderTarget2D(graphicsDevice, width, height);
            TileTarget = new RenderTarget2D(graphicsDevice, width, height);
            WallTarget = new RenderTarget2D(graphicsDevice, width, height);

            EntityTarget = new RenderTarget2D(graphicsDevice, width, height);
            PlayerTarget = new RenderTarget2D(graphicsDevice, width, height);
            ProjectileTarget = new RenderTarget2D(graphicsDevice, width, height);

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

            NonSolidTileTarget.Dispose();
            TileTarget.Dispose();
            WallTarget.Dispose();

            EntityTarget.Dispose();
            PlayerTarget.Dispose();
            ProjectileTarget.Dispose();

            CopyTarget.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}

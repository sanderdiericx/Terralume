using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace Terralume.Engine.Buffers
{
    internal class TargetCollection : IDisposable
    {
        public RenderTarget2D Background { get; private set; }

        public RenderTarget2D UndergroundBackgroundMaster { get; private set; }
        public RenderTarget2D UndergroundBackground { get; private set; }

        public RenderTarget2D NonSolidTile { get; private set; }
        public RenderTarget2D Tile { get; private set; }
        public RenderTarget2D Wall { get; private set; }

        public RenderTarget2D Entity { get; private set; }
        public RenderTarget2D Player { get; private set; }
        public RenderTarget2D Projectile { get; private set; }
        public RenderTarget2D Liquid { get; private set; }

        public RenderTarget2D Copy { get; private set; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public TargetCollection(GraphicsDevice graphicsDevice, int width, int height)
        {
            Background = new RenderTarget2D(graphicsDevice, width, height);

            UndergroundBackgroundMaster = new RenderTarget2D(graphicsDevice, Main.instance.backgroundTarget.Width, Main.instance.backgroundTarget.Height);
            UndergroundBackground = new RenderTarget2D(graphicsDevice, width, height);

            NonSolidTile = new RenderTarget2D(graphicsDevice, width, height);
            Tile = new RenderTarget2D(graphicsDevice, width, height);
            Wall = new RenderTarget2D(graphicsDevice, width, height);

            Entity = new RenderTarget2D(graphicsDevice, width, height);
            Player = new RenderTarget2D(graphicsDevice, width, height);
            Projectile = new RenderTarget2D(graphicsDevice, width, height);

            Liquid = new RenderTarget2D(graphicsDevice, width, height);

            Copy = new RenderTarget2D(graphicsDevice, width, height, false, graphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

            Width = width;
            Height = height;
        }

        public bool IsValid()
        {
            return Width == Main.graphics.GraphicsDevice.PresentationParameters.BackBufferWidth && Height == Main.graphics.GraphicsDevice.PresentationParameters.BackBufferHeight;
        }

        public void Dispose()
        {
            Background.Dispose();

            UndergroundBackgroundMaster.Dispose();
            UndergroundBackground.Dispose();

            NonSolidTile.Dispose();
            Tile.Dispose();
            Wall.Dispose();

            Entity.Dispose();
            Player.Dispose();
            Projectile.Dispose();

            Liquid.Dispose();

            Copy.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}

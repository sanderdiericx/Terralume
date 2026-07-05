using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace Terralume.Engine.Buffers
{
    public class TargetCollection
    {
        public RenderTarget2D Background;

        public RenderTarget2D UndergroundBackgroundMaster;
        public RenderTarget2D UndergroundBackground;

        public RenderTarget2D NonSolidTile;
        public RenderTarget2D Tile;
        public RenderTarget2D Wall;

        public RenderTarget2D Entity;
        public RenderTarget2D Player;
        public RenderTarget2D Projectile;

        public RenderTarget2D Copy;

        public int Width;
        public int Height;

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

            Copy.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}

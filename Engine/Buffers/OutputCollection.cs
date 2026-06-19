using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace Terralume.Engine.Buffers
{
    public class OutputCollection : IDisposable
    {
        public RenderTarget2D RadianceBuffer;
        public RenderTarget2D OutputBuffer;

        public RenderTarget2D JFABufferA;
        public RenderTarget2D JFABufferB;

        public RenderTarget2D JFATileBuffer;
        public RenderTarget2D JFAAirBuffer;

        public RenderTarget2D SDFBufferFinal;

        public int Width;
        public int Height;

        public OutputCollection(GraphicsDevice graphicsDevice, int width, int height)
        {
            RadianceBuffer = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Vector4, DepthFormat.None);
            OutputBuffer = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.None);

            JFABufferA = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Vector2, DepthFormat.None);
            JFABufferB = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Vector2, DepthFormat.None);

            JFATileBuffer = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Vector2, DepthFormat.None);
            JFAAirBuffer = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Vector2, DepthFormat.None);

            SDFBufferFinal = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Vector2, DepthFormat.None);

            Width = width;
            Height = height;
        }

        public bool IsValid()
        {
            return Width == Main.graphics.GraphicsDevice.PresentationParameters.BackBufferWidth && Height == Main.graphics.GraphicsDevice.PresentationParameters.BackBufferHeight;
        }

        public void Dispose()
        {
            RadianceBuffer.Dispose();
            OutputBuffer.Dispose();

            JFABufferA.Dispose();
            JFABufferB.Dispose();

            JFATileBuffer.Dispose();
            JFAAirBuffer.Dispose();

            SDFBufferFinal.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
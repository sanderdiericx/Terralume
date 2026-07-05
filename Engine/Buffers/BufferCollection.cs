using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace Terralume.Engine.Buffers
{
    public class BufferCollection : IDisposable
    {
        public RenderTarget2D LayerBuffer;
        public RenderTarget2D NoiseBuffer;

        public RenderTarget2D DepthBuffer;
        public RenderTarget2D MaterialBuffer;
        public RenderTarget2D HeightBuffer;
        public RenderTarget2D NormalBuffer;
        public RenderTarget2D AlbedoBuffer;

        public RenderTarget2D JFABufferA;
        public RenderTarget2D JFABufferB;

        public RenderTarget2D JFATileBuffer;
        public RenderTarget2D JFAAirBuffer;

        public RenderTarget2D SDFBufferFinal;

        public RenderTarget2D BlurredUndergroundBackgroundBuffer;

        public int Width;
        public int Height;

        public BufferCollection(GraphicsDevice graphicsDevice, int width, int height)
        {
            LayerBuffer = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Single, DepthFormat.None);
            NoiseBuffer = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Single, DepthFormat.None);

            DepthBuffer = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Single, DepthFormat.None);
            MaterialBuffer = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.None);
            HeightBuffer = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Single, DepthFormat.None);
            NormalBuffer = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.None);
            AlbedoBuffer = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.None);

            JFABufferA = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Vector2, DepthFormat.None);
            JFABufferB = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Vector2, DepthFormat.None);

            JFATileBuffer = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Vector2, DepthFormat.None);
            JFAAirBuffer = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Vector2, DepthFormat.None);

            SDFBufferFinal = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Vector2, DepthFormat.None);

            BlurredUndergroundBackgroundBuffer = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.None);

            Width = width;
            Height = height;
        }

        public bool IsValid()
        {
            return Width == Main.graphics.GraphicsDevice.PresentationParameters.BackBufferWidth && Height == Main.graphics.GraphicsDevice.PresentationParameters.BackBufferHeight;
        }

        public void Dispose()
        {
            LayerBuffer.Dispose();
            NoiseBuffer.Dispose();

            DepthBuffer.Dispose();
            MaterialBuffer.Dispose();
            HeightBuffer.Dispose();
            NormalBuffer.Dispose();
            AlbedoBuffer.Dispose();

            JFABufferA.Dispose();
            JFABufferB.Dispose();

            JFATileBuffer.Dispose();
            JFAAirBuffer.Dispose();

            SDFBufferFinal.Dispose();

            BlurredUndergroundBackgroundBuffer.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}

using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace Terralume.Engine.Buffers
{
    internal class BufferCollection : IDisposable
    {
        public RenderTarget2D LayerBuffer { get; private set; }
        public RenderTarget2D NoiseBuffer { get; private set; }
        public RenderTarget2D DepthBuffer { get; private set; }

        public RenderTarget2D MaterialBuffer { get; private set; }
        public RenderTarget2D ReflectiveMaterialBuffer { get; private set; }

        public RenderTarget2D HeightBuffer { get; private set; }
        public RenderTarget2D NormalBuffer { get; private set; }
        public RenderTarget2D AlbedoBuffer { get; private set; }

        public RenderTarget2D SDFBufferFinal { get; private set; }
        public RenderTarget2D SDFBufferReflection { get; private set; }

        public RenderTarget2D BlurredUndergroundBackgroundBuffer { get; private set; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public BufferCollection(GraphicsDevice graphicsDevice, int width, int height)
        {
            LayerBuffer = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Single, DepthFormat.None);
            NoiseBuffer = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Single, DepthFormat.None);
            DepthBuffer = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Single, DepthFormat.None);

            MaterialBuffer = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.None);
            ReflectiveMaterialBuffer = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Vector4, DepthFormat.None);

            HeightBuffer = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Single, DepthFormat.None);
            NormalBuffer = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.None);
            AlbedoBuffer = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.None);

            SDFBufferFinal = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Vector2, DepthFormat.None);
            SDFBufferReflection = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Single, DepthFormat.None);

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
            ReflectiveMaterialBuffer.Dispose();

            HeightBuffer.Dispose();
            NormalBuffer.Dispose();
            AlbedoBuffer.Dispose();

            SDFBufferFinal.Dispose();
            SDFBufferReflection.Dispose();

            BlurredUndergroundBackgroundBuffer.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}

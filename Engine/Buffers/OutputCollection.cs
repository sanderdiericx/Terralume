using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace Terralume.Engine.Buffers
{
    internal class OutputCollection : IDisposable
    {
        public RenderTarget2D RadianceBuffer { get; private set; }
        public RenderTarget2D OutputBuffer { get; private set; }
        public RenderTarget2D BloomBuffer { get; private set; }
        public RenderTarget2D ReflectionBuffer { get; private set; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public OutputCollection(GraphicsDevice graphicsDevice, int width, int height)
        {
            RadianceBuffer = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Vector4, DepthFormat.None);
            OutputBuffer = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.None);
            BloomBuffer = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Vector4, DepthFormat.None);
            ReflectionBuffer = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Vector4, DepthFormat.None);

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
            BloomBuffer.Dispose();
            ReflectionBuffer.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
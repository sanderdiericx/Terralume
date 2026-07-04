using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace Terralume.Engine.Buffers
{
    public class OutputCollection : IDisposable
    {
        public RenderTarget2D RadianceBuffer;
        public RenderTarget2D OutputBuffer;

        public int Width;
        public int Height;

        public OutputCollection(GraphicsDevice graphicsDevice, int width, int height)
        {
            RadianceBuffer = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Vector4, DepthFormat.None);
            OutputBuffer = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.None);

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

            GC.SuppressFinalize(this);
        }
    }
}
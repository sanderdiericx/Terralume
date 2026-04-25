using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace Terralume.Engine.Buffers
{
    public class BufferCollection : IDisposable
    {
        private const int DATA_TEXTURE_RESOLUTION = 256;

        public RenderTarget2D LayerBuffer;
        public RenderTarget2D DepthBuffer;

        public RenderTarget2D AlbedoBuffer;
        public RenderTarget2D MaterialBuffer;
        public RenderTarget2D NormalBuffer;

        public Texture2D TileIDTexture;
        public Texture2D MaterialTexture;

        public int Width;
        public int Height;

        public BufferCollection(GraphicsDevice graphicsDevice, int width, int height)
        {
            LayerBuffer = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Single, DepthFormat.None);
            DepthBuffer = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Single, DepthFormat.None);

            AlbedoBuffer = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.None);
            MaterialBuffer = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.None);
            NormalBuffer = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.None);

            TileIDTexture = new Texture2D(graphicsDevice, DATA_TEXTURE_RESOLUTION, DATA_TEXTURE_RESOLUTION);
            MaterialTexture = new Texture2D(graphicsDevice, DATA_TEXTURE_RESOLUTION, DATA_TEXTURE_RESOLUTION);

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
            DepthBuffer.Dispose();

            AlbedoBuffer.Dispose();
            MaterialBuffer.Dispose();
            NormalBuffer.Dispose();

            TileIDTexture.Dispose();
            MaterialTexture.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}

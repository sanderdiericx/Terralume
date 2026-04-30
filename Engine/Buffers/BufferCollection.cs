using Microsoft.Xna.Framework.Graphics;
using System;
using Terralume.Engine.Constants;
using Terraria;

namespace Terralume.Engine.Buffers
{
    public class BufferCollection : IDisposable
    {
        public RenderTarget2D LayerBuffer;
        public RenderTarget2D DepthBuffer;

        public RenderTarget2D AlbedoBuffer;
        public RenderTarget2D MaterialBuffer;
        public RenderTarget2D NormalBuffer;

        public Texture2D TileIDTexture;
        public Texture2D WallIDTexture;

        public Texture2D TileMaterialTexture;
        public Texture2D WallMaterialTexture;

        public int Width;
        public int Height;

        public BufferCollection(GraphicsDevice graphicsDevice, int width, int height)
        {
            LayerBuffer = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Single, DepthFormat.None);
            DepthBuffer = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Single, DepthFormat.None);

            AlbedoBuffer = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.None);
            MaterialBuffer = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.None);
            NormalBuffer = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.None);

            TileIDTexture = new Texture2D(graphicsDevice, RenderConstants.MATERIAL_DATA_RESOLUTION, RenderConstants.MATERIAL_DATA_RESOLUTION);
            WallIDTexture = new Texture2D(graphicsDevice, RenderConstants.MATERIAL_DATA_RESOLUTION, RenderConstants.MATERIAL_DATA_RESOLUTION);

            TileMaterialTexture = new Texture2D(graphicsDevice, RenderConstants.MATERIAL_DATA_RESOLUTION, RenderConstants.MATERIAL_DATA_RESOLUTION);
            WallMaterialTexture = new Texture2D(graphicsDevice, RenderConstants.MATERIAL_DATA_RESOLUTION, RenderConstants.MATERIAL_DATA_RESOLUTION);

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
            WallIDTexture.Dispose();

            TileMaterialTexture.Dispose();
            WallMaterialTexture.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}

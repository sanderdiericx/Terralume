using Microsoft.Xna.Framework.Graphics;
using System;
using Terralume.Engine.Constants;
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

        public Texture2D TileIDTexture;
        public Texture2D WallIDTexture;

        public Texture2D TileMaterialTexture;
        public Texture2D WallMaterialTexture;

        public Texture2D TileNoiseTexture;
        public Texture2D WallNoiseTexture;

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

            TileIDTexture = new Texture2D(graphicsDevice, RenderConstants.MATERIAL_DATA_RESOLUTION, RenderConstants.MATERIAL_DATA_RESOLUTION);
            WallIDTexture = new Texture2D(graphicsDevice, RenderConstants.MATERIAL_DATA_RESOLUTION, RenderConstants.MATERIAL_DATA_RESOLUTION);

            TileMaterialTexture = new Texture2D(graphicsDevice, RenderConstants.MATERIAL_DATA_RESOLUTION, RenderConstants.MATERIAL_DATA_RESOLUTION);
            WallMaterialTexture = new Texture2D(graphicsDevice, RenderConstants.MATERIAL_DATA_RESOLUTION, RenderConstants.MATERIAL_DATA_RESOLUTION);

            TileNoiseTexture = new Texture2D(graphicsDevice, RenderConstants.MATERIAL_DATA_RESOLUTION, RenderConstants.MATERIAL_DATA_RESOLUTION, false, SurfaceFormat.HalfVector2);
            WallNoiseTexture = new Texture2D(graphicsDevice, RenderConstants.MATERIAL_DATA_RESOLUTION, RenderConstants.MATERIAL_DATA_RESOLUTION, false, SurfaceFormat.HalfVector2);

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

            TileIDTexture.Dispose();
            WallIDTexture.Dispose();

            TileMaterialTexture.Dispose();
            WallMaterialTexture.Dispose();

            TileNoiseTexture.Dispose();
            WallNoiseTexture.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}

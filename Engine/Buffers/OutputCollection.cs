using Microsoft.Xna.Framework.Graphics;
using System;
using Terralume.Engine.Constants;
using Terraria;

namespace Terralume.Engine.Buffers
{
    public class OutputCollection : IDisposable
    {
        public RenderTarget2D LightingBuffer;

        public Texture2D LightPositionTexture;
        public Texture2D LightColorTexture;

        public Texture2D LightGridTexture;
        public Texture2D LightIndexTexture;

        public int Width;
        public int Height;

        public OutputCollection(GraphicsDevice graphicsDevice, int width, int height)
        {
            LightingBuffer = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.None);

            // LightPositionTexture: (posX, posY, range, intensity) LightColorTexture: (lightColorRed, lightColorBlue, lightColorGreen, unused)
            LightPositionTexture = new Texture2D(graphicsDevice, RenderConstants.LIGHT_DATA_RESOLUTION, RenderConstants.LIGHT_DATA_RESOLUTION, false, SurfaceFormat.HalfVector4);
            LightColorTexture = new Texture2D(graphicsDevice, RenderConstants.LIGHT_DATA_RESOLUTION, RenderConstants.LIGHT_DATA_RESOLUTION, false, SurfaceFormat.Color);

            int gridWidth = (int)Math.Ceiling((double)width / RenderConstants.LIGHT_GRID_TILE_SIZE);
            int gridHeight = (int)Math.Ceiling((double)height / RenderConstants.LIGHT_GRID_TILE_SIZE);

            int maxIndices = gridWidth * gridHeight * RenderConstants.MAX_LIGHTS_PER_TILE;
            int indexTextureSize = (int)Math.Ceiling(Math.Sqrt(maxIndices));
            indexTextureSize = Math.Min(indexTextureSize, 4096);

            // LightGridTexture: (start index for tile in index list, light count for each tile) LightIndexTexture (light data index)
            LightGridTexture = new Texture2D(graphicsDevice, gridWidth, gridHeight, false, SurfaceFormat.Vector4);
            LightIndexTexture = new Texture2D(graphicsDevice, indexTextureSize, indexTextureSize, false, SurfaceFormat.Vector4);

            Width = width;
            Height = height;
        }

        public bool IsValid()
        {
            return Width == Main.graphics.GraphicsDevice.PresentationParameters.BackBufferWidth && Height == Main.graphics.GraphicsDevice.PresentationParameters.BackBufferHeight;
        }

        public void Dispose()
        {
            LightingBuffer.Dispose();

            LightPositionTexture.Dispose();
            LightColorTexture.Dispose();

            LightGridTexture.Dispose();
            LightIndexTexture.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
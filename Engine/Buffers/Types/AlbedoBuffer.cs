using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;

namespace Terralume.Engine.Buffers.Types
{
    public class AlbedoBuffer
    {
        public RenderTarget2D RenderTarget;

        public AlbedoBuffer(GraphicsDevice graphicsDevice, int width, int height)
        {
            RenderTarget = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.None);
        }
    }
}
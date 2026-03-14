using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;
using Color = Microsoft.Xna.Framework.Color;
using Terraria;

namespace Terralume.Engine.Buffers.Types
{
    // Buffer storing pixel color data without lighting alterations
    public class AlbedoBuffer
    {
        public RenderTarget2D RenderTarget;
        private GraphicsDevice graphicsDevice;

        public AlbedoBuffer(GraphicsDevice graphicsDevice, int width, int height)
        {
            RenderTarget = new RenderTarget2D(graphicsDevice, width, height);

            this.graphicsDevice = graphicsDevice;
        }

        // Draw spriteBatch to stored RenderTarget
        public void Render(SpriteBatch spriteBatch, SamplerState samplerState, Matrix transformationMatrix)
        {
            graphicsDevice.SetRenderTarget(RenderTarget);
            graphicsDevice.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, samplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, transformationMatrix);

            graphicsDevice.SetRenderTarget(null);
            spriteBatch.End();
        }
    }
}

using Microsoft.Xna.Framework.Graphics;

namespace Terralume.Engine.Buffers
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
        public void BeginRender(SpriteBatch spritebatch)
        {
            graphicsDevice.SetRenderTarget(RenderTarget);
            spritebatch.Begin();
        }

        // End drawing of spritebatch to stored RenderTarget
        public void EndRender(SpriteBatch spriteBatch)
        {
            graphicsDevice.SetRenderTarget(null);
            spriteBatch.End();
        }
    }
}

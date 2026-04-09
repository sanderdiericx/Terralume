using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Terralume.Engine.Buffers
{
    public static class BufferRenderer
    {
        public static void RenderAlbedoBuffer(SpriteBatch spriteBatch, RenderTarget2D albedoBuffer)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            device.SetRenderTarget(albedoBuffer);
            device.Clear(Color.Transparent);

            // Bake underground background buffer into our albedo buffer.
            Vector2 drawPos = Main.sceneBackgroundPos - Main.screenPosition;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            spriteBatch.Draw(Main.instance.backgroundTarget, drawPos, Color.White);
            spriteBatch.End();

            // Copy the current screen target to our albedo buffer
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            device.SetRenderTarget(Main.instance.backgroundTarget);
            // device.Clear(Color.Transparent);

            // Reset Terraria drawing state
            device.SetRenderTargets(oldTargets);
        }

        public static void RenderBackgroundBuffer(SpriteBatch spriteBatch, RenderTarget2D backgroundBuffer)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            // Copy the current screen target to our background buffer
            device.SetRenderTarget(backgroundBuffer);
            device.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            device.SetRenderTargets(oldTargets);
            device.Clear(Color.Transparent);
        }
    }
}

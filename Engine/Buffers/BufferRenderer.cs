using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terralume.Engine.Shaders;
using Terralume.Engine.Utils;
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

            // Reset Terraria drawing state
            device.SetRenderTargets(oldTargets);
        }

        public static void RenderDepthBuffer(SpriteBatch spriteBatch, RenderTarget2D albedoBuffer, RenderTarget2D depthBuffer, Texture2D tileTexture)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            device.SetRenderTarget(depthBuffer);
            device.Clear(Color.Transparent);

            Effect effect = EffectManager.GetEffect(EffectType.Depth);

            effect.Parameters["TileTexture"]?.SetValue(tileTexture);
            effect.Parameters["TileStride"]?.SetValue(TileHelper.GetHorizontalTileStride());
            effect.Parameters["TileTextureSize"]?.SetValue(new Vector2(tileTexture.Width, tileTexture.Height));
            effect.Parameters["ScreenOffset"]?.SetValue(new Vector2(Main.screenPosition.X % 16, Main.screenPosition.Y % 16));

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, null, null, effect);
            spriteBatch.Draw(albedoBuffer, Vector2.Zero, Color.White);
            spriteBatch.End();

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

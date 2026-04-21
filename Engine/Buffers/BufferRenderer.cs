using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terralume.Engine.Shaders;
using Terraria;

namespace Terralume.Engine.Buffers
{
    public static class BufferRenderer
    {
        public static void RenderAlbedoBuffer(SpriteBatch spriteBatch, RenderTarget2D albedoBuffer, RenderTarget2D undergroundBackgroundBuffer)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            device.SetRenderTarget(albedoBuffer);
            device.Clear(Color.Transparent);

            // Copy the current screen target to our albedo buffer
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            spriteBatch.Draw(undergroundBackgroundBuffer, Vector2.Zero, Color.White);
            spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);

            spriteBatch.End();

            // Reset Terraria drawing state
            device.SetRenderTargets(oldTargets);
        }

        public static void RenderDepthBuffer(SpriteBatch spriteBatch, RenderTarget2D albedoBuffer, RenderTarget2D depthBuffer, RenderTarget2D tileTarget, RenderTarget2D undergroundBackgroundTarget)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            device.SetRenderTarget(depthBuffer);
            device.Clear(Color.Transparent);

            Effect effect = EffectManager.GetEffect(EffectType.Depth);

            effect.Parameters["TileTarget"].SetValue(tileTarget);
            effect.Parameters["UndergroundBackgroundTarget"].SetValue(undergroundBackgroundTarget);

            // Prevent monogame from blurring TileTarget data
            device.SamplerStates[1] = SamplerState.PointClamp;
            device.SamplerStates[2] = SamplerState.PointClamp;

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, null, null, effect);
            spriteBatch.Draw(albedoBuffer, Vector2.Zero, Color.White);
            spriteBatch.End();

            device.SetRenderTargets(oldTargets);
        }

        public static void CopyBackgroundBuffer(SpriteBatch spriteBatch, RenderTarget2D backgroundBuffer)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            device.SetRenderTarget(backgroundBuffer);
            device.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            device.SetRenderTargets(oldTargets);
            device.Clear(Color.Transparent);
        }

        public static void CopyTileTarget(SpriteBatch spriteBatch, RenderTarget2D stableTileTarget)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            device.SetRenderTarget(stableTileTarget);

            Vector2 tileDrawPos = Main.sceneTilePos - Main.screenPosition;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            spriteBatch.Draw(Main.instance.tileTarget, tileDrawPos, Color.White);
            spriteBatch.End();

            device.SetRenderTargets(oldTargets);
        }
    }
}

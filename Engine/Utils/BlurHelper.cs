using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terralume.Engine.Buffers.Cache;
using Terralume.Engine.Shaders;
using Terraria;

namespace Terralume.Engine.Utils
{
    internal static class BlurHelper
    {
        public static void DownsampledGaussianBlur(SpriteBatch spriteBatch, RenderTarget2D inputTarget, RenderTarget2D outputTarget, int downsampleWidth, int downsampleHeight, float blurDeviations)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            RenderTarget2D copyTargetA = RenderTargetCache.RequestTarget(downsampleWidth, downsampleHeight);
            RenderTarget2D copyTargetB = RenderTargetCache.RequestTarget(downsampleWidth, downsampleHeight);

            // Draw input target onto copy target A
            device.SetRenderTarget(copyTargetA);
            device.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.LinearClamp, null, null);
            spriteBatch.Draw(inputTarget, new Rectangle(0, 0, downsampleWidth, downsampleHeight), Color.White);
            spriteBatch.End();

            // Run horizontal and vertical blur passes
            Effect effect = EffectManager.GetEffect(EffectType.Gaussian);
            effect.Parameters["U_Sigma"].SetValue(blurDeviations);

            // Horizontal
            device.SetRenderTarget(copyTargetB);
            device.Clear(Color.Transparent);

            effect.Parameters["ScreenSize"].SetValue(copyTargetB.Size());
            effect.Parameters["Direction"].SetValue(new Vector2(1.0f, 0.0f));

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, null, null, effect);
            spriteBatch.Draw(copyTargetA, Vector2.Zero, Color.White);
            spriteBatch.End();

            // Vertical
            device.SetRenderTarget(copyTargetA);
            device.Clear(Color.Transparent);

            effect.Parameters["ScreenSize"].SetValue(copyTargetA.Size());
            effect.Parameters["Direction"].SetValue(new Vector2(0.0f, 1.0f));

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, null, null, effect);
            spriteBatch.Draw(copyTargetB, Vector2.Zero, Color.White);
            spriteBatch.End();

            // Blit blurred target back to native resolution
            device.SetRenderTarget(outputTarget);
            device.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.LinearClamp, null, null);
            spriteBatch.Draw(copyTargetA, new Rectangle(0, 0, inputTarget.Width, inputTarget.Height), Color.White);
            spriteBatch.End();

            RenderTargetCache.ReturnTarget(copyTargetA);
            RenderTargetCache.ReturnTarget(copyTargetB);

            device.SetRenderTargets(oldTargets);
        }
    }
}

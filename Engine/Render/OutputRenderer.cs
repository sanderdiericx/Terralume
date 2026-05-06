using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terralume.Engine.Buffers;
using Terralume.Engine.Buffers.Data;
using Terralume.Engine.Constants;
using Terralume.Engine.Shaders;
using Terralume.Engine.Utils;
using Terraria;

namespace Terralume.Engine.Render
{
    public static class OutputRenderer
    {
        public static void RenderSSAOBuffer(SpriteBatch spriteBatch, OutputCollection outputCollection, BufferCollection bufferCollection, DepthParameter depthParameter)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            device.SetRenderTarget(outputCollection.SSAOBuffer);
            device.Clear(Color.Transparent);

            Effect effect = EffectManager.GetEffect(EffectType.SSAO);

            Vector2 screenSize = RenderConstants.SCREEN_SIZE;
            Vector2 halfScreenSize = screenSize / 2f;
            Vector2 noiseScale = halfScreenSize / 4f;

            float currentZoom = Main.GameViewMatrix.Zoom.X;

            effect.Parameters["ScreenSize"].SetValue(screenSize);
            effect.Parameters["HalfScreenSize"].SetValue(halfScreenSize);
            effect.Parameters["NoiseScale"].SetValue(noiseScale);

            effect.Parameters["Radius"].SetValue(RenderConstants.SSAO_RADIUS);
            effect.Parameters["Bias"].SetValue(RenderConstants.SSAO_BIAS);
            effect.Parameters["DepthScale"].SetValue(RenderConstants.DEPTH_SCALE * currentZoom);
            effect.Parameters["BackgroundDepth"].SetValue(depthParameter.BackgroundDepth);
            effect.Parameters["Samples"].SetValue(OutputHelper.GenerateSSAOKernel());

            effect.Parameters["NormalBuffer"].SetValue(bufferCollection.NormalBuffer);
            effect.Parameters["NoiseTexture"].SetValue(outputCollection.SSAONoiseTexture);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, null, null, effect);
            spriteBatch.Draw(bufferCollection.DepthBuffer, new Rectangle(0, 0, outputCollection.SSAOBuffer.Width, outputCollection.SSAOBuffer.Height), Color.White);
            spriteBatch.End();

            device.SetRenderTargets(oldTargets);
        }

        public static void RenderSSAOBlurBuffer(SpriteBatch spriteBatch, OutputCollection outputCollection, BufferCollection bufferCollection)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            device.SetRenderTarget(outputCollection.SSAOBlurBuffer);
            device.Clear(Color.Transparent);

            Effect effect = EffectManager.GetEffect(EffectType.SSAOBlur);

            effect.Parameters["ScreenSize"].SetValue(RenderConstants.SCREEN_SIZE);
            effect.Parameters["DepthFalloff"].SetValue(RenderConstants.SSAO_DEPTH_FALLOFF);

            effect.Parameters["SSAOBuffer"].SetValue(outputCollection.SSAOBuffer);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, null, null, effect);
            spriteBatch.Draw(bufferCollection.DepthBuffer, new Rectangle(0, 0, outputCollection.SSAOBlurBuffer.Width, outputCollection.SSAOBlurBuffer.Height), Color.White);
            spriteBatch.End();

            device.SetRenderTargets(oldTargets);
        }

        // Uses the composed G-Buffer to compute lighting for the albedo buffer
        public static void RenderRadianceBuffer(SpriteBatch spriteBatch, OutputCollection outputCollection, BufferCollection bufferCollection, DepthParameter depthParameter)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            device.SetRenderTarget(outputCollection.RadianceBuffer);
            device.Clear(Color.Transparent);

            Effect effect = EffectManager.GetEffect(EffectType.Radiance);

            float currentZoom = Main.GameViewMatrix.Zoom.X;

            effect.Parameters["ScreenSize"].SetValue(RenderConstants.SCREEN_SIZE);
            effect.Parameters["DepthScale"].SetValue(RenderConstants.DEPTH_SCALE * currentZoom);
            effect.Parameters["LightDepth"].SetValue(RenderConstants.LIGHT_DEPTH);

            effect.Parameters["TileDepth"].SetValue(depthParameter.TileDepth);
            effect.Parameters["WallDepth"].SetValue(depthParameter.WallDepth);

            effect.Parameters["AlbedoVariationFactor"].SetValue(RenderConstants.ALBEDO_VARIATION_FACTOR);

            effect.Parameters["LightDataTextureSize"].SetValue(new Vector2(RenderConstants.LIGHT_DATA_RESOLUTION, RenderConstants.LIGHT_DATA_RESOLUTION));
            effect.Parameters["LightIndexTextureSize"].SetValue(new Vector2(outputCollection.LightIndexTexture.Width, outputCollection.LightIndexTexture.Height));

            effect.Parameters["DepthBuffer"].SetValue(bufferCollection.DepthBuffer);
            effect.Parameters["NormalBuffer"].SetValue(bufferCollection.NormalBuffer);
            effect.Parameters["MaterialBuffer"].SetValue(bufferCollection.MaterialBuffer);
            effect.Parameters["NoiseBuffer"].SetValue(bufferCollection.NoiseBuffer);

            effect.Parameters["LightPositionTexture"].SetValue(outputCollection.LightPositionTexture);
            effect.Parameters["LightColorTexture"].SetValue(outputCollection.LightColorTexture);
            effect.Parameters["LightGridTexture"].SetValue(outputCollection.LightGridTexture);
            effect.Parameters["LightIndexTexture"].SetValue(outputCollection.LightIndexTexture);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, effect);
            spriteBatch.Draw(bufferCollection.AlbedoBuffer, Vector2.Zero, Color.White);
            spriteBatch.End();

            device.SetRenderTargets(oldTargets);
        }

        public static void RenderOutputBuffer(SpriteBatch spriteBatch, OutputCollection outputCollection, BufferCollection bufferCollection, TargetCollection targetCollection)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            device.SetRenderTarget(outputCollection.OutputBuffer);
            device.Clear(Color.Transparent);

            Effect effect = EffectManager.GetEffect(EffectType.Output);

            effect.Parameters["Ambient"].SetValue(RenderConstants.AMBIENT);
            effect.Parameters["Exposure"].SetValue(RenderConstants.TONE_MAPPING_EXPOSURE);

            effect.Parameters["RadianceBuffer"].SetValue(outputCollection.RadianceBuffer);
            effect.Parameters["SSAOBlurBuffer"].SetValue(outputCollection.SSAOBlurBuffer);

            device.SamplerStates[1] = SamplerState.PointClamp;

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            spriteBatch.Draw(targetCollection.BackgroundTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, effect);
            spriteBatch.Draw(bufferCollection.AlbedoBuffer, Vector2.Zero, Color.White);
            spriteBatch.End();

            device.SetRenderTargets(oldTargets);
        }
    }
}

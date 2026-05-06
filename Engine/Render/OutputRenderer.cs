using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terralume.Engine.Buffers;
using Terralume.Engine.Constants;
using Terralume.Engine.Shaders;
using Terraria;

namespace Terralume.Engine.Render
{
    public static class OutputRenderer
    {
        // Uses the composed G-Buffer to compute lighting for the albedo buffer
        public static void RenderRadianceBuffer(SpriteBatch spriteBatch, OutputCollection outputCollection, BufferCollection bufferCollection)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            device.SetRenderTarget(outputCollection.RadianceBuffer);
            device.Clear(Color.Transparent);

            Effect effect = EffectManager.GetEffect(EffectType.Radiance);

            Vector2 screenSize = new Vector2(bufferCollection.AlbedoBuffer.Width, bufferCollection.AlbedoBuffer.Height);

            float currentZoom = Main.GameViewMatrix.Zoom.X;

            effect.Parameters["ScreenSize"].SetValue(screenSize);
            effect.Parameters["DepthScale"].SetValue(100f * currentZoom);
            effect.Parameters["LightDepth"].SetValue(0.85f);
            
            effect.Parameters["TileDepth"].SetValue(RenderConstants.DEPTH_PARAMETER.TileDepth);
            effect.Parameters["WallDepth"].SetValue(RenderConstants.DEPTH_PARAMETER.WallDepth);
            
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

            effect.Parameters["Ambient"].SetValue(new Vector3(0.002f, 0.002f, 0.0025f));
            effect.Parameters["Exposure"].SetValue(0.8f);

            effect.Parameters["RadianceBuffer"].SetValue(outputCollection.RadianceBuffer);

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

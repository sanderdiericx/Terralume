using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terralume.Engine.Buffers;
using Terralume.Engine.Buffers.Data;
using Terralume.Engine.Constants;
using Terralume.Engine.Shaders;
using Terraria;

namespace Terralume.Engine.Render
{
    public static class OutputRenderer
    {
        public static void RenderSDFBuffer(SpriteBatch spriteBatch, OutputCollection outputCollection, BufferCollection bufferCollection, LayerParameter layerParameter)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            RenderTarget2D readTarget = outputCollection.SDFBufferA;
            RenderTarget2D writeTarget = outputCollection.SDFBufferB;

            device.SetRenderTarget(readTarget);
            device.Clear(Color.Transparent);

            Effect jfaInitEffect = EffectManager.GetEffect(EffectType.JFAInit);

            jfaInitEffect.Parameters["TileLayer"].SetValue(layerParameter.Tile);
            jfaInitEffect.Parameters["ScreenSize"].SetValue(RenderConstants.SCREEN_SIZE);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, null, null, jfaInitEffect);
            spriteBatch.Draw(bufferCollection.LayerBuffer, Vector2.Zero, Color.White);
            spriteBatch.End();

            Effect jfaEffect = EffectManager.GetEffect(EffectType.JFA);

            jfaEffect.Parameters["ScreenSize"].SetValue(RenderConstants.SCREEN_SIZE);

            Vector2 size = readTarget.Size();
            int initJumpSize = (int)Math.Ceiling(Math.Max(size.X, size.Y) / 2);

            for (int jumpSize = initJumpSize; jumpSize >= 1; jumpSize /= 2)
            {
                jfaEffect.Parameters["JumpSizeUV"].SetValue(new Vector2(jumpSize, jumpSize) / RenderConstants.SCREEN_SIZE);

                device.SetRenderTarget(writeTarget);

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, null, null, jfaEffect);
                spriteBatch.Draw(readTarget, Vector2.Zero, Color.White);
                spriteBatch.End();

                RenderTarget2D temp = readTarget;
                readTarget = writeTarget;
                writeTarget = temp;
            }

            device.SetRenderTarget(outputCollection.SDFBufferFinal);
            device.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, null, null);
            spriteBatch.Draw(readTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            device.SetRenderTargets(oldTargets);
        }

        // Uses the composed G-Buffer to compute lighting for the albedo buffer
        public static void RenderRadianceBuffer(SpriteBatch spriteBatch, OutputCollection outputCollection, BufferCollection bufferCollection, LayerParameter layerParameter)
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

            effect.Parameters["BackgroundLayer"].SetValue(layerParameter.Background);
            effect.Parameters["TileLayer"].SetValue(layerParameter.Tile);
            effect.Parameters["WallLayer"].SetValue(layerParameter.Wall);

            effect.Parameters["AlbedoVariationFactor"].SetValue(RenderConstants.ALBEDO_VARIATION_FACTOR);

            effect.Parameters["LightDataTextureSize"].SetValue(new Vector2(RenderConstants.LIGHT_DATA_RESOLUTION, RenderConstants.LIGHT_DATA_RESOLUTION));
            effect.Parameters["LightIndexTextureSize"].SetValue(new Vector2(outputCollection.LightIndexTexture.Width, outputCollection.LightIndexTexture.Height));

            effect.Parameters["LayerBuffer"].SetValue(bufferCollection.LayerBuffer);
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

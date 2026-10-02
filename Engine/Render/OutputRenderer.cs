using log4net.Filter;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;
using Terralume.Engine.Buffers;
using Terralume.Engine.Buffers.Cache;
using Terralume.Engine.Buffers.Data;
using Terralume.Engine.Constants;
using Terralume.Engine.Hooks.Tracking;
using Terralume.Engine.Mapping;
using Terralume.Engine.Shaders;
using Terralume.Engine.Utils;
using Terraria;

namespace Terralume.Engine.Render
{
    internal static class OutputRenderer
    {
        // Uses the composed G-Buffer to compute radiance and bloom mask
        public static void RenderRadianceAndBloomBuffer(SpriteBatch spriteBatch, OutputCollection outputCollection, BufferCollection bufferCollection, LayerParameter layerParameter, TileCaptureParameter tileCaptureParameter)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            device.SetRenderTargets(outputCollection.RadianceBuffer, outputCollection.BloomBuffer);
            device.Clear(Color.Transparent);

            Effect effect = EffectManager.GetEffect(EffectType.Radiance);

            float currentZoom = Main.GameViewMatrix.Zoom.X;

            effect.Parameters["ScreenSize"].SetValue(outputCollection.RadianceBuffer.Size());
            effect.Parameters["ViewSize"].SetValue(tileCaptureParameter.ViewSize);
            effect.Parameters["ScreenPosition"].SetValue(tileCaptureParameter.ScreenPosition);

            effect.Parameters["DepthScale"].SetValue(RenderConstants.DEPTH_SCALE * currentZoom);
            effect.Parameters["LightDepth"].SetValue(RenderConstants.LIGHT_DEPTH);

            effect.Parameters["BackgroundLayer"].SetValue(layerParameter.Background);
            effect.Parameters["TileLayer"].SetValue(layerParameter.Tile);
            effect.Parameters["WallLayer"].SetValue(layerParameter.Wall);
            effect.Parameters["PlayerLayer"].SetValue(layerParameter.Player);
            effect.Parameters["ProjectileLayer"].SetValue(layerParameter.Projectile);

            effect.Parameters["OcclusionFactor"].SetValue(0.08f / currentZoom);

            effect.Parameters["VolumetricStrength"].SetValue(0.04f);
            effect.Parameters["VolumetricRange"].SetValue(0.1f);
            effect.Parameters["VolumetricNoiseContrast"].SetValue(0.6f);

            AmbientLightProfile ambientLight = AmbientLightTracker.GetAmbientLightProfile();

            effect.Parameters["AmbientColor"].SetValue(ambientLight.AmbientColor);
            effect.Parameters["AmbientStrength"].SetValue(ambientLight.AmbientStrength);
            effect.Parameters["AmbientDiffuseWeight"].SetValue(0.2f);

            effect.Parameters["Gamma"].SetValue(1.9f);

            effect.Parameters["LightDataTextureSize"].SetValue(new Vector2(RenderConstants.LIGHT_DATA_RESOLUTION, RenderConstants.LIGHT_DATA_RESOLUTION));
            effect.Parameters["LightIndexTextureSize"].SetValue(new Vector2(LightPacker.LightIndexTexture.Width, LightPacker.LightIndexTexture.Height));
            effect.Parameters["SunOcclusionTextureSize"].SetValue(new Vector2(SunOcclusionTracker.SunOcclusionTexture.Width, SunOcclusionTracker.SunOcclusionTexture.Height));

            effect.Parameters["LayerBuffer"].SetValue(bufferCollection.LayerBuffer);
            effect.Parameters["DepthBuffer"].SetValue(bufferCollection.DepthBuffer);
            effect.Parameters["NormalBuffer"].SetValue(bufferCollection.NormalBuffer);
            effect.Parameters["MaterialBuffer"].SetValue(bufferCollection.MaterialBuffer);
            effect.Parameters["NoiseBuffer"].SetValue(bufferCollection.NoiseBuffer);
            effect.Parameters["SDFBuffer"].SetValue(bufferCollection.SDFBufferFinal);

            effect.Parameters["LightPositionTexture"].SetValue(LightPacker.LightPositionTexture);
            effect.Parameters["LightColorTexture"].SetValue(LightPacker.LightColorTexture);
            effect.Parameters["LightGridTexture"].SetValue(LightPacker.LightGridTexture);
            effect.Parameters["LightIndexTexture"].SetValue(LightPacker.LightIndexTexture);
            effect.Parameters["SunOcclusionTexture"].SetValue(SunOcclusionTracker.SunOcclusionTexture);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, effect);
            spriteBatch.Draw(bufferCollection.AlbedoBuffer, Vector2.Zero, Color.White);
            spriteBatch.End();

            device.SetRenderTargets(oldTargets);
        }

        public static void RenderBlurredBloomBuffer(SpriteBatch spriteBatch, OutputCollection outputCollection, int levels)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            RenderTarget2D bloomBuffer = outputCollection.BloomBuffer;

            RenderTarget2D[] downsampled = new RenderTarget2D[levels];
            int w = bloomBuffer.Width, h = bloomBuffer.Height;
            RenderTarget2D prev = bloomBuffer;

            Effect effect = EffectManager.GetEffect(EffectType.Downsample);

            // Downsample chain
            for (int i = 0; i < levels; i++)
            {
                w /= 2; h /= 2;
                RenderTarget2D target = RenderTargetCache.RequestTarget(w, h, SurfaceFormat.Vector4);

                effect.Parameters["ScreenSize"].SetValue(target.Size());

                device.SetRenderTarget(target);
                device.Clear(Color.Transparent);
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.LinearClamp, null, null, effect);
                spriteBatch.Draw(prev, new Rectangle(0, 0, w, h), Color.White);
                spriteBatch.End();

                BlurHelper.GaussianBlur(spriteBatch, target, 0.2f);

                downsampled[i] = target;
                prev = target;
            }

            // Upsample and additive combine
            RenderTarget2D composite = downsampled[levels - 1];
            for (int i = levels - 2; i >= 0; i--)
            {
                RenderTarget2D target = downsampled[i];
                int tw = target.Width, th = target.Height;

                device.SetRenderTarget(target);
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearClamp, null, null);
                spriteBatch.Draw(composite, new Rectangle(0, 0, tw, th), Color.White * 0.8f);
                spriteBatch.End();

                composite = target;
            }

            device.SetRenderTarget(bloomBuffer);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearClamp, null, null);
            spriteBatch.Draw(composite, new Rectangle(0, 0, bloomBuffer.Width, bloomBuffer.Height), Color.White);
            spriteBatch.End();

            device.SetRenderTargets(oldTargets);

            foreach (var t in downsampled)
            {
                RenderTargetCache.ReturnTarget(t);
            }
        }

        // Note: Radiance buffer must be produced before this method is called
        public static void RenderReflectionBuffer(SpriteBatch spriteBatch, OutputCollection outputCollection, BufferCollection bufferCollection)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            device.SetRenderTarget(outputCollection.ReflectionBuffer);
            device.Clear(Color.Transparent);

            Effect effect = EffectManager.GetEffect(EffectType.Reflection);

            effect.Parameters["ScreenSize"].SetValue(outputCollection.ReflectionBuffer.Size());

            effect.Parameters["SDFReflectionBuffer"].SetValue(bufferCollection.SDFBufferReflection);
            effect.Parameters["ReflectiveMaterialBuffer"].SetValue(bufferCollection.ReflectiveMaterialBuffer);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, null, null, effect);
            spriteBatch.Draw(outputCollection.RadianceBuffer, Vector2.Zero, Color.White);
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

            effect.Parameters["ScreenSize"].SetValue(outputCollection.OutputBuffer.Size());

            effect.Parameters["AORadius"].SetValue(40f);
            effect.Parameters["AOStrength"].SetValue(0.001f);

            effect.Parameters["BloomIntensity"].SetValue(1.1f);

            effect.Parameters["Exposure"].SetValue(RenderConstants.TONE_MAPPING_EXPOSURE);
            effect.Parameters["Gamma"].SetValue(1.9f);

            effect.Parameters["RadianceBuffer"].SetValue(outputCollection.RadianceBuffer);
            effect.Parameters["SDFBuffer"].SetValue(bufferCollection.SDFBufferFinal);
            effect.Parameters["BloomBuffer"].SetValue(outputCollection.BloomBuffer);
            effect.Parameters["ReflectionBuffer"].SetValue(outputCollection.ReflectionBuffer);

            device.SamplerStates[1] = SamplerState.PointClamp;

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            spriteBatch.Draw(targetCollection.Background, Vector2.Zero, Color.White);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, effect);
            spriteBatch.Draw(bufferCollection.AlbedoBuffer, Vector2.Zero, Color.White);
            spriteBatch.End();

            device.SetRenderTargets(oldTargets);
        }
    }
}

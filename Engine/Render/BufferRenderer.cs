using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Numerics;
using Terralume.Engine.Buffers;
using Terralume.Engine.Buffers.Cache;
using Terralume.Engine.Buffers.Data;
using Terralume.Engine.Constants;
using Terralume.Engine.Hooks.Tracking;
using Terralume.Engine.Mapping;
using Terralume.Engine.Mapping.Registry;
using Terralume.Engine.Shaders;
using Terralume.Engine.Utils;
using Terraria;
using Terraria.DataStructures;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace Terralume.Engine.Render
{
    // Note: All of these methods require Main.screenTarget from before drawing interface, without background textures
    internal static class BufferRenderer
    {
        // Renders a "layer" buffer where each pixel represents a different layer, this avoids repeated texture lookups in the depth and material mapping
        // Note: Layer colors are defined by the LayerParameter
        public static void RenderLayerBuffer(SpriteBatch spriteBatch, BufferCollection bufferCollection, TargetCollection targetCollection, LayerParameter layerParameter)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            device.SetRenderTarget(bufferCollection.LayerBuffer);
            device.Clear(Color.Transparent);

            Effect effect = EffectManager.GetEffect(EffectType.Layer);

            layerParameter.SetUniforms(effect);

            effect.Parameters["TileTarget"].SetValue(targetCollection.Tile);
            effect.Parameters["NonSolidTileTarget"].SetValue(targetCollection.NonSolidTile);
            effect.Parameters["WallTarget"].SetValue(targetCollection.Wall);
            effect.Parameters["EntityTarget"].SetValue(targetCollection.Entity);
            effect.Parameters["PlayerTarget"].SetValue(targetCollection.Player);
            effect.Parameters["ProjectileTarget"].SetValue(targetCollection.Projectile);
            effect.Parameters["LiquidTarget"].SetValue(targetCollection.Liquid);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, null, null, effect);
            spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            device.SetRenderTargets(oldTargets);
        }

        // Renders the noise texture for material variation on tiles and walls
        public static void RenderNoiseBuffer(SpriteBatch spriteBatch, BufferCollection bufferCollection, LayerParameter layerParameter, TileCaptureParameter tileCaptureParameter)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            device.SetRenderTarget(bufferCollection.NoiseBuffer);
            device.Clear(Color.Transparent);

            Effect effect = EffectManager.GetEffect(EffectType.Noise);

            effect.Parameters["ScreenPosition"].SetValue(tileCaptureParameter.ScreenPosition);
            effect.Parameters["ViewSize"].SetValue(tileCaptureParameter.ViewSize);

            effect.Parameters["DefaultNoise"].SetValue(RenderConstants.DEFAULT_NOISE);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, null, null, effect);
            spriteBatch.Draw(bufferCollection.LayerBuffer, Vector2.Zero, Color.White);
            spriteBatch.End();

            device.SetRenderTargets(oldTargets);
        }

        // Renders a depth map by comparing layers and then fetching the depth value for that layer from the depth parameter
        public static void RenderDepthBuffer(SpriteBatch spriteBatch, BufferCollection bufferCollection, LayerParameter layerParameter, DepthParameter depthParameter)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            device.SetRenderTarget(bufferCollection.DepthBuffer);
            device.Clear(Color.Transparent);

            Effect effect = EffectManager.GetEffect(EffectType.Depth);

            depthParameter.SetUniforms(effect);
            layerParameter.SetUniforms(effect);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, null, null, effect);
            spriteBatch.Draw(bufferCollection.LayerBuffer, Vector2.Zero, Color.White);
            spriteBatch.End();

            device.SetRenderTargets(oldTargets);
        }

        // Renders a material buffer
        // Format: r = metallic, g = roughness, b = emissive, a = unused (default 1.0)
        public static void RenderMaterialBuffer(SpriteBatch spriteBatch, BufferCollection bufferCollection, LayerParameter layerParameter, TileCaptureParameter tileCaptureParameter)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            device.SetRenderTargets(bufferCollection.MaterialBuffer);
            device.Clear(Color.Transparent);

            Effect effect = EffectManager.GetEffect(EffectType.Material);

            effect.Parameters["TileStride"].SetValue(tileCaptureParameter.TileStride);
            effect.Parameters["MinTilePosition"].SetValue(tileCaptureParameter.MinTile);

            effect.Parameters["ScreenPosition"].SetValue(tileCaptureParameter.ScreenPosition);
            effect.Parameters["ViewSize"].SetValue(tileCaptureParameter.ViewSize);

            effect.Parameters["DefaultMaterial"].SetValue(MaterialMapper.DefaultMaterial);

            effect.Parameters["DefaultProjectileMaterial"].SetValue(MaterialMapper.DefaultProjectileMaterial);

            effect.Parameters["MaterialTextureSize"].SetValue(new Vector2(RenderConstants.MATERIAL_DATA_RESOLUTION, RenderConstants.MATERIAL_DATA_RESOLUTION));
            effect.Parameters["TileIDTextureSize"].SetValue(new Vector2(RenderConstants.MATERIAL_DATA_RESOLUTION, RenderConstants.MATERIAL_DATA_RESOLUTION));

            layerParameter.SetUniforms(effect);

            effect.Parameters["TileIDTexture"].SetValue(IDPacker.TileIDTexture);
            effect.Parameters["WallIDTexture"].SetValue(IDPacker.WallIDTexture);
            effect.Parameters["LiquidIDTexture"].SetValue(IDPacker.LiquidIDTexture);

            effect.Parameters["TileMaterialTexture"].SetValue(MaterialPacker.TileMaterialTexture);
            effect.Parameters["WallMaterialTexture"].SetValue(MaterialPacker.WallMaterialTexture);
            effect.Parameters["LiquidMaterialTexture"].SetValue(MaterialPacker.LiquidMaterialTexture);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.LinearClamp, null, null, effect);
            spriteBatch.Draw(bufferCollection.LayerBuffer, Vector2.Zero, Color.White);
            spriteBatch.End();

            // Stamp player materials
            Dictionary<Vector3, List<DrawData>> drawData = PlayerMaterialTracker.DrawData;

            Effect injectEffect = EffectManager.GetEffect(EffectType.MaterialInject);

            foreach (var kvp in drawData)
            {
                injectEffect.Parameters["Material"].SetValue(kvp.Key);

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, injectEffect, Main.GameViewMatrix.TransformationMatrix);

                foreach (DrawData drawItem in kvp.Value)
                {
                    spriteBatch.Draw(drawItem.texture, drawItem.position, drawItem.sourceRect, Color.White, drawItem.rotation, drawItem.origin, drawItem.scale, drawItem.effect, 0f);
                }

                spriteBatch.End();
            }

            device.SetRenderTargets(oldTargets);
        }

        // Renders a reflective material buffer, used for screen space reflectionss
        // Format: r = reflection strength, g = unused, b = unused, a = liquid id (if liquid; else -1)
        public static void RenderReflectiveMaterialBuffer(SpriteBatch spriteBatch, BufferCollection bufferCollection, LayerParameter layerParameter, TileCaptureParameter tileCaptureParameter)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            device.SetRenderTarget(bufferCollection.ReflectiveMaterialBuffer);

            Effect effect = EffectManager.GetEffect(EffectType.ReflectiveMaterial);

            effect.Parameters["TileStride"].SetValue(tileCaptureParameter.TileStride);
            effect.Parameters["MinTilePosition"].SetValue(tileCaptureParameter.MinTile);

            effect.Parameters["ScreenPosition"].SetValue(tileCaptureParameter.ScreenPosition);
            effect.Parameters["ViewSize"].SetValue(tileCaptureParameter.ViewSize);

            effect.Parameters["DefaultReflection"].SetValue(ReflectionMapper.DefaultReflection);

            effect.Parameters["ReflectionTextureSize"].SetValue(new Vector2(RenderConstants.MATERIAL_DATA_RESOLUTION, RenderConstants.MATERIAL_DATA_RESOLUTION));
            effect.Parameters["TileIDTextureSize"].SetValue(new Vector2(RenderConstants.MATERIAL_DATA_RESOLUTION, RenderConstants.MATERIAL_DATA_RESOLUTION));

            effect.Parameters["NonSolidTileLayer"].SetValue(layerParameter.NonSolidTile);
            effect.Parameters["TileLayer"].SetValue(layerParameter.Tile);
            effect.Parameters["LiquidLayer"].SetValue(layerParameter.Liquid);

            effect.Parameters["TileIDTexture"].SetValue(IDPacker.TileIDTexture);
            effect.Parameters["LiquidIDTexture"].SetValue(IDPacker.LiquidIDTexture);

            effect.Parameters["TileReflectionTexture"].SetValue(ReflectionPacker.TileReflectionTexture);
            effect.Parameters["LiquidReflectionTexture"].SetValue(ReflectionPacker.LiquidReflectionTexture);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, null, null, effect);
            spriteBatch.Draw(bufferCollection.LayerBuffer, Vector2.Zero, Color.White);
            spriteBatch.End();

            device.SetRenderTargets(oldTargets);
        }

        // Pass over Main.screenTarget to convert pixels to greyscale, this then acts as our height map
        public static void RenderHeightBuffer(SpriteBatch spriteBatch, BufferCollection bufferCollection)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            device.SetRenderTarget(bufferCollection.HeightBuffer);
            device.Clear(Color.Transparent);

            Effect effect = EffectManager.GetEffect(EffectType.Height);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, null, null, effect);
            spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            device.SetRenderTargets(oldTargets);
        }

        // Renders a normal buffer by taking the heightmap and then running a weighted sobel filter over it
        public static void RenderNormalBuffer(SpriteBatch spriteBatch, BufferCollection bufferCollection)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            device.SetRenderTarget(bufferCollection.NormalBuffer);
            device.Clear(Color.Transparent);

            Effect effect = EffectManager.GetEffect(EffectType.Normal);

            effect.Parameters["ScreenSize"].SetValue(bufferCollection.NormalBuffer.Size());

            effect.Parameters["NormalStrength"].SetValue(RenderConstants.NORMAL_STRENGTH);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.LinearClamp, null, null, effect);
            spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            BlurHelper.GaussianBlur(spriteBatch, bufferCollection.NormalBuffer, RenderConstants.NORMAL_BLUR_DEVIATIONS);

            device.SetRenderTargets(oldTargets);
        }

        // Copies Main.screen target into a custom render target, the albedo map represents each pixels diffuse color
        // Note: For this to copy a true albedo buffer, vanilla lighting must be disabled
        public static void RenderAlbedoBuffer(SpriteBatch spriteBatch, BufferCollection bufferCollection)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            device.SetRenderTarget(bufferCollection.AlbedoBuffer);
            device.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            // We bake the underground background into the albedo buffer because it is affected by lights (and is not present in Main.screenTarget)
            spriteBatch.Draw(bufferCollection.BlurredUndergroundBackgroundBuffer, Vector2.Zero, Color.White);

            spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);

            spriteBatch.End();

            device.SetRenderTargets(oldTargets);
        }

        public static void RenderSDFBuffer(SpriteBatch spriteBatch, BufferCollection bufferCollection, LayerParameter layerParameter)
        {
            RenderTarget2D JFATileBuffer = RenderTargetCache.RequestTarget(bufferCollection.SDFBufferFinal.Width, bufferCollection.SDFBufferFinal.Height, SurfaceFormat.Vector2);
            RenderTarget2D JFAAirBuffer = RenderTargetCache.RequestTarget(bufferCollection.SDFBufferFinal.Width, bufferCollection.SDFBufferFinal.Height, SurfaceFormat.Vector2);

            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            JFAPass(device, spriteBatch, bufferCollection, JFATileBuffer, layerParameter.Tile, false);
            JFAPass(device, spriteBatch, bufferCollection, JFAAirBuffer, layerParameter.Tile, true);

            // Combine into final signed distance field
            Effect combineEffect = EffectManager.GetEffect(EffectType.SDFCombine);

            combineEffect.Parameters["ScreenSize"].SetValue(bufferCollection.SDFBufferFinal.Size());
            combineEffect.Parameters["SeedLayer"].SetValue(layerParameter.Tile);

            combineEffect.Parameters["JFATileBuffer"].SetValue(JFATileBuffer);
            combineEffect.Parameters["JFAAirBuffer"].SetValue(JFAAirBuffer);

            device.SetRenderTarget(bufferCollection.SDFBufferFinal);
            device.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, null, null, combineEffect);
            spriteBatch.Draw(bufferCollection.LayerBuffer, Vector2.Zero, Color.White);
            spriteBatch.End();

            device.SetRenderTargets(oldTargets);

            RenderTargetCache.ReturnTarget(JFATileBuffer);
            RenderTargetCache.ReturnTarget(JFAAirBuffer);
        }

        private static void JFAPass(GraphicsDevice device, SpriteBatch spriteBatch, BufferCollection bufferCollection, RenderTarget2D outputTarget, float seedLayer, bool exclusive)
        {
            RenderTarget2D readTarget = RenderTargetCache.RequestTarget(outputTarget.Width, outputTarget.Height, SurfaceFormat.Vector2);
            RenderTarget2D writeTarget = RenderTargetCache.RequestTarget(outputTarget.Width, outputTarget.Height, SurfaceFormat.Vector2);

            device.SetRenderTarget(readTarget);
            device.Clear(Color.Transparent);

            Effect jfaInitEffect = EffectManager.GetEffect(EffectType.JFAInit);

            jfaInitEffect.Parameters["SeedLayer"].SetValue(seedLayer);
            jfaInitEffect.Parameters["ScreenSize"].SetValue(readTarget.Size());
            jfaInitEffect.Parameters["Exclusive"].SetValue(exclusive);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, null, null, jfaInitEffect);
            spriteBatch.Draw(bufferCollection.LayerBuffer, Vector2.Zero, Color.White);
            spriteBatch.End();

            Effect jfaEffect = EffectManager.GetEffect(EffectType.JFA);

            jfaEffect.Parameters["ScreenSize"].SetValue(writeTarget.Size());

            Vector2 size = readTarget.Size();
            int initJumpSize = (int)Math.Max(size.X, size.Y);

            initJumpSize = (int)BitOperations.RoundUpToPowerOf2((uint)initJumpSize);

            for (int jumpSize = initJumpSize; jumpSize >= 1; jumpSize /= 2)
            {
                jfaEffect.Parameters["JumpSizeUV"].SetValue(new Vector2(jumpSize, jumpSize) / writeTarget.Size());

                device.SetRenderTarget(writeTarget);

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, null, null, jfaEffect);
                spriteBatch.Draw(readTarget, Vector2.Zero, Color.White);
                spriteBatch.End();

                RenderTarget2D temp = readTarget;
                readTarget = writeTarget;
                writeTarget = temp;
            }

            device.SetRenderTarget(outputTarget);
            device.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, null, null);
            spriteBatch.Draw(readTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            RenderTargetCache.ReturnTarget(readTarget);
            RenderTargetCache.ReturnTarget(writeTarget);
        }

        public static void RenderSDFBufferReflection(SpriteBatch spriteBatch, BufferCollection bufferCollection)
        {
            RenderTarget2D JFAReflectionBuffer = RenderTargetCache.RequestTarget(bufferCollection.SDFBufferFinal.Width, bufferCollection.SDFBufferFinal.Height, SurfaceFormat.Vector2);

            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            JFAPassReflection(device, spriteBatch, bufferCollection, JFAReflectionBuffer);

            // Combine into final buffer with distance to nearest reflective surface
            Effect combineEffect = EffectManager.GetEffect(EffectType.SDFCombineReflection);

            combineEffect.Parameters["ScreenSize"].SetValue(bufferCollection.SDFBufferReflection.Size());

            combineEffect.Parameters["JFAReflectionBuffer"].SetValue(JFAReflectionBuffer);

            device.SetRenderTarget(bufferCollection.SDFBufferReflection);
            device.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, null, null, combineEffect);
            spriteBatch.Draw(bufferCollection.ReflectiveMaterialBuffer, Vector2.Zero, Color.White);
            spriteBatch.End();

            device.SetRenderTargets(oldTargets);

            RenderTargetCache.ReturnTarget(JFAReflectionBuffer);
        }

        private static void JFAPassReflection(GraphicsDevice device, SpriteBatch spriteBatch, BufferCollection bufferCollection, RenderTarget2D outputTarget)
        {
            RenderTarget2D readTarget = RenderTargetCache.RequestTarget(outputTarget.Width, outputTarget.Height, SurfaceFormat.Vector2);
            RenderTarget2D writeTarget = RenderTargetCache.RequestTarget(outputTarget.Width, outputTarget.Height, SurfaceFormat.Vector2);

            device.SetRenderTarget(readTarget);
            device.Clear(Color.Transparent);

            Effect jfaInitEffect = EffectManager.GetEffect(EffectType.JFAReflectionInit);

            jfaInitEffect.Parameters["ScreenSize"].SetValue(readTarget.Size());

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, null, null, jfaInitEffect);
            spriteBatch.Draw(bufferCollection.ReflectiveMaterialBuffer, Vector2.Zero, Color.White);
            spriteBatch.End();

            Effect jfaEffect = EffectManager.GetEffect(EffectType.JFAReflection);

            jfaEffect.Parameters["ScreenSize"].SetValue(writeTarget.Size());

            Vector2 size = readTarget.Size();
            int initJumpSize = (int)size.Y;

            initJumpSize = (int)BitOperations.RoundUpToPowerOf2((uint)initJumpSize);

            for (int jumpSize = initJumpSize; jumpSize >= 1; jumpSize /= 2)
            {
                jfaEffect.Parameters["JumpSizeUV"].SetValue(new Vector2(jumpSize, jumpSize) / writeTarget.Size());

                device.SetRenderTarget(writeTarget);

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, null, null, jfaEffect);
                spriteBatch.Draw(readTarget, Vector2.Zero, Color.White);
                spriteBatch.End();

                RenderTarget2D temp = readTarget;
                readTarget = writeTarget;
                writeTarget = temp;
            }

            device.SetRenderTarget(outputTarget);
            device.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, null, null);
            spriteBatch.Draw(readTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            RenderTargetCache.ReturnTarget(readTarget);
            RenderTargetCache.ReturnTarget(writeTarget);
        }

        // Note: SDF buffer must be before this generated for valid blur
        public static void BlurUndergroundBackgroundBuffer(SpriteBatch spriteBatch, BufferCollection bufferCollection, TargetCollection targetCollection)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            int width = bufferCollection.BlurredUndergroundBackgroundBuffer.Width;
            int height = bufferCollection.BlurredUndergroundBackgroundBuffer.Height;

            RenderTarget2D wideBlurTarget = RenderTargetCache.RequestTarget(width, height, SurfaceFormat.Color);
            RenderTarget2D tightBlurTarget = RenderTargetCache.RequestTarget(width, height, SurfaceFormat.Color);

            BlurHelper.DownsampledGaussianBlur(spriteBatch, targetCollection.UndergroundBackground, wideBlurTarget, width / 2, height / 2, 1f);
            BlurHelper.DownsampledGaussianBlur(spriteBatch, targetCollection.UndergroundBackground, tightBlurTarget, width / 2, height / 2, 0.5f);

            device.SetRenderTarget(bufferCollection.BlurredUndergroundBackgroundBuffer);
            device.Clear(Color.Transparent);

            Effect effect = EffectManager.GetEffect(EffectType.BackgroundBlur);

            effect.Parameters["BlurStartDistance"].SetValue(RenderConstants.BACKGROUND_BLUR_START_DISTANCE);
            effect.Parameters["BlurEndDistance"].SetValue(RenderConstants.BACKGROUND_BLUR_END_DISTANCE);

            effect.Parameters["WideBlurTarget"].SetValue(wideBlurTarget);
            effect.Parameters["TightBlurTarget"].SetValue(tightBlurTarget);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, null, null, effect);
            spriteBatch.Draw(bufferCollection.SDFBufferFinal, Vector2.Zero, Color.White);
            spriteBatch.End();

            device.SetRenderTargets(oldTargets);

            RenderTargetCache.ReturnTarget(wideBlurTarget);
            RenderTargetCache.ReturnTarget(tightBlurTarget);
        }

        // Note SunOcclusionTexture must be packed for this method to work properly
        public static void RenderSunOcclusionBuffer(SpriteBatch spriteBatch, BufferCollection bufferCollection, TileCaptureParameter tileCaptureParameter)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            device.SetRenderTarget(bufferCollection.SunOcclusionBuffer);
            device.Clear(Color.Transparent);

            Effect effect = EffectManager.GetEffect(EffectType.SunOcclusion);

            effect.Parameters["TileStride"].SetValue(tileCaptureParameter.TileStride);
            effect.Parameters["MinTilePosition"].SetValue(tileCaptureParameter.MinTile);

            effect.Parameters["ScreenPosition"].SetValue(tileCaptureParameter.ScreenPosition);
            effect.Parameters["ViewSize"].SetValue(tileCaptureParameter.ViewSize);

            effect.Parameters["SunOcclusionTextureSize"].SetValue(SunOcclusionPacker.SunOcclusionTexture.Size());

            effect.Parameters["SunOcclusionTexture"].SetValue(SunOcclusionPacker.SunOcclusionTexture);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, null, null, effect);
            spriteBatch.Draw(bufferCollection.LayerBuffer, Vector2.Zero, Color.White);
            spriteBatch.End();

            // Heavy gaussian blur because the sun occlusion data is in tile-space
            RenderTarget2D blurTarget = RenderTargetCache.RequestTarget(bufferCollection.SunOcclusionBuffer.Width, bufferCollection.SunOcclusionBuffer.Height, SurfaceFormat.Single);

            device.SetRenderTarget(blurTarget);
            device.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, null, null);
            spriteBatch.Draw(bufferCollection.SunOcclusionBuffer, Vector2.Zero, Color.White);
            spriteBatch.End();

            Vector2 blurSize = bufferCollection.SunOcclusionBuffer.Size() / 4f;

            BlurHelper.DownsampledGaussianBlur(spriteBatch, blurTarget, bufferCollection.SunOcclusionBuffer, (int)blurSize.X, (int)blurSize.Y, 10f);

            device.SetRenderTargets(oldTargets);

            RenderTargetCache.ReturnTarget(blurTarget);
        }
    }
}

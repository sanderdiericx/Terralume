using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terralume.Engine.Buffers;
using Terralume.Engine.Buffers.Data;
using Terralume.Engine.Constants;
using Terralume.Engine.Mapping.Registry;
using Terralume.Engine.Shaders;
using Terraria;

namespace Terralume.Engine.Render
{
    // Note: All of these methods require Main.screenTarget from before drawing interface, without background textures
    public static class BufferRenderer
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

            effect.Parameters["TileTarget"].SetValue(targetCollection.TileTarget);
            effect.Parameters["NonSolidTileTarget"].SetValue(targetCollection.NonSolidTileTarget);
            effect.Parameters["WallTarget"].SetValue(targetCollection.WallTarget);
            effect.Parameters["EntityTarget"].SetValue(targetCollection.EntityTarget);
            effect.Parameters["PlayerTarget"].SetValue(targetCollection.PlayerTarget);
            effect.Parameters["ProjectileTarget"].SetValue(targetCollection.ProjectileTarget);

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

            effect.Parameters["TileStride"].SetValue(tileCaptureParameter.TileStride);
            effect.Parameters["MinTilePosition"].SetValue(tileCaptureParameter.MinTile);

            effect.Parameters["ScreenPosition"].SetValue(tileCaptureParameter.ScreenPosition);
            effect.Parameters["ViewSize"].SetValue(tileCaptureParameter.ViewSize);

            effect.Parameters["DefaultNoise"].SetValue(NoiseMapper.DefaultNoise);
            effect.Parameters["BackgroundNoise"].SetValue(NoiseMapper.BackgroundNoise);

            effect.Parameters["NoiseTextureSize"].SetValue(new Vector2(RenderConstants.MATERIAL_DATA_RESOLUTION, RenderConstants.MATERIAL_DATA_RESOLUTION));
            effect.Parameters["IDTextureSize"].SetValue(new Vector2(RenderConstants.MATERIAL_DATA_RESOLUTION, RenderConstants.MATERIAL_DATA_RESOLUTION));

            effect.Parameters["TileLayer"].SetValue(layerParameter.Tile);
            effect.Parameters["WallLayer"].SetValue(layerParameter.Wall);
            effect.Parameters["BackgroundLayer"].SetValue(layerParameter.Background);

            effect.Parameters["TileIDTexture"].SetValue(bufferCollection.TileIDTexture);
            effect.Parameters["WallIDTexture"].SetValue(bufferCollection.WallIDTexture);

            effect.Parameters["TileNoiseTexture"].SetValue(bufferCollection.TileNoiseTexture);
            effect.Parameters["WallNoiseTexture"].SetValue(bufferCollection.WallNoiseTexture);

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
        public static void RenderMaterialBuffer(SpriteBatch spriteBatch, BufferCollection bufferCollection, LayerParameter layerParameter, TileCaptureParameter tileCaptureParameter)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            device.SetRenderTarget(bufferCollection.MaterialBuffer);
            device.Clear(Color.Transparent);

            Effect effect = EffectManager.GetEffect(EffectType.Material);

            effect.Parameters["TileStride"].SetValue(tileCaptureParameter.TileStride);
            effect.Parameters["MinTilePosition"].SetValue(tileCaptureParameter.MinTile);

            effect.Parameters["ScreenPosition"].SetValue(tileCaptureParameter.ScreenPosition);
            effect.Parameters["ViewSize"].SetValue(tileCaptureParameter.ViewSize);

            effect.Parameters["DefaultMaterial"].SetValue(MaterialMapper.DefaultMaterial);
            effect.Parameters["MaterialVariationFactor"].SetValue(RenderConstants.MATERIAL_VARIATION_FACTOR);

            effect.Parameters["MaterialTextureSize"].SetValue(new Vector2(RenderConstants.MATERIAL_DATA_RESOLUTION, RenderConstants.MATERIAL_DATA_RESOLUTION));
            effect.Parameters["TileIDTextureSize"].SetValue(new Vector2(RenderConstants.MATERIAL_DATA_RESOLUTION, RenderConstants.MATERIAL_DATA_RESOLUTION));

            layerParameter.SetUniforms(effect);

            effect.Parameters["NoiseBuffer"].SetValue(bufferCollection.NoiseBuffer);

            effect.Parameters["TileIDTexture"].SetValue(bufferCollection.TileIDTexture);
            effect.Parameters["WallIDTexture"].SetValue(bufferCollection.WallIDTexture);

            effect.Parameters["TileMaterialTexture"].SetValue(bufferCollection.TileMaterialTexture);
            effect.Parameters["WallMaterialTexture"].SetValue(bufferCollection.WallMaterialTexture);

            device.SamplerStates[2] = SamplerState.PointClamp;
            device.SamplerStates[3] = SamplerState.PointClamp;
            device.SamplerStates[4] = SamplerState.PointClamp;
            device.SamplerStates[5] = SamplerState.PointClamp;

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.LinearClamp, null, null, effect);
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

            effect.Parameters["ScreenSize"].SetValue(RenderConstants.SCREEN_SIZE);

            effect.Parameters["NormalStrength"].SetValue(RenderConstants.NORMAL_STRENGTH);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.LinearClamp, null, null, effect);
            spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            device.SetRenderTargets(oldTargets);
        }

        // Copies Main.screen target into a custom render target, the albedo map represents each pixels diffuse color
        // Note: For this to copy a true albedo buffer, vanilla lighting must be disabled
        public static void RenderAlbedoBuffer(SpriteBatch spriteBatch, BufferCollection bufferCollection, TargetCollection targetCollection)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            device.SetRenderTarget(bufferCollection.AlbedoBuffer);
            device.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            // We bake the underground background into the albedo buffer because it is affected by lights (and is not present in Main.screenTarget)
            spriteBatch.Draw(targetCollection.UndergroundBackgroundTarget, Vector2.Zero, Color.White);

            spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);

            spriteBatch.End();

            device.SetRenderTargets(oldTargets);
        }
    }
}

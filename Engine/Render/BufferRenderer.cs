using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terralume.Engine.Buffers;
using Terralume.Engine.Buffers.Data;
using Terralume.Engine.Shaders;
using Terralume.Engine.Utils;
using Terraria;

namespace Terralume.Engine.Render
{
    public static class BufferRenderer
    {
        // Renders a "layer" buffer where each pixel represents a different layer, this avoids repeated texture lookups in the depth and material mapping
        // Note: Layer colors are defined by the LayerParameter
        public static void RenderLayerBuffer(SpriteBatch spriteBatch, BufferCollection bufferCollection, TargetCollection targetCollection, RenderTarget2D screenTarget, LayerParameter layerParameter)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            device.SetRenderTarget(bufferCollection.LayerBuffer);
            device.Clear(Color.Transparent);

            Effect effect = EffectManager.GetEffect(EffectType.Layer);

            // Params
            effect.Parameters["LayerParameter_Background"].SetValue(layerParameter.Background);
            effect.Parameters["LayerParameter_Tile"].SetValue(layerParameter.Tile);
            effect.Parameters["LayerParameter_NonSolidTile"].SetValue(layerParameter.NonSolidTile);
            effect.Parameters["LayerParameter_Wall"].SetValue(layerParameter.Wall);
            effect.Parameters["LayerParameter_Entity"].SetValue(layerParameter.Entity);
            effect.Parameters["LayerParameter_Player"].SetValue(layerParameter.Player);
            effect.Parameters["LayerParameter_Projectile"].SetValue(layerParameter.Projectile);
            effect.Parameters["LayerParameter_Other"].SetValue(layerParameter.Other);

            // Targets
            effect.Parameters["TileTarget"].SetValue(targetCollection.StableTileTarget);
            effect.Parameters["NonSolidTileTarget"].SetValue(targetCollection.NonSolidTileTarget);
            effect.Parameters["WallTarget"].SetValue(targetCollection.StableWallTarget);
            effect.Parameters["EntityTarget"].SetValue(targetCollection.EntityTarget);
            effect.Parameters["PlayerTarget"].SetValue(targetCollection.PlayerTarget);
            effect.Parameters["ProjectileTarget"].SetValue(targetCollection.ProjectileTarget);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, null, null, effect);
            spriteBatch.Draw(screenTarget, Vector2.Zero, Color.White);
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

        // Renders a depth map by sampling pixels from multiple captured render targets as layers to asign depth values
        public static void RenderDepthBuffer(SpriteBatch spriteBatch, BufferCollection bufferCollection, TargetCollection targetCollection, RenderTarget2D screenTarget, DepthParameter depthParameter)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            device.SetRenderTarget(bufferCollection.DepthBuffer);
            device.Clear(Color.Transparent);

            Effect effect = EffectManager.GetEffect(EffectType.Depth);

            // Params
            effect.Parameters["DepthParameter_BackgroundDepth"].SetValue(depthParameter.BackgroundDepth);
            effect.Parameters["DepthParameter_TileDepth"].SetValue(depthParameter.TileDepth);
            effect.Parameters["DepthParameter_NonSolidTileDepth"].SetValue(depthParameter.NonSolidTileDepth);
            effect.Parameters["DepthParameter_WallDepth"].SetValue(depthParameter.WallDepth);
            effect.Parameters["DepthParameter_EntityDepth"].SetValue(depthParameter.EntityDepth);
            effect.Parameters["DepthParameter_PlayerDepth"].SetValue(depthParameter.PlayerDepth);
            effect.Parameters["DepthParameter_ProjectileDepth"].SetValue(depthParameter.ProjectileDepth);
            effect.Parameters["DepthParameter_OtherDepth"].SetValue(depthParameter.OtherDepth);

            // Targets
            effect.Parameters["TileTarget"].SetValue(targetCollection.StableTileTarget);
            effect.Parameters["NonSolidTileTarget"].SetValue(targetCollection.NonSolidTileTarget);
            effect.Parameters["WallTarget"].SetValue(targetCollection.StableWallTarget);
            effect.Parameters["EntityTarget"].SetValue(targetCollection.EntityTarget);
            effect.Parameters["PlayerTarget"].SetValue(targetCollection.PlayerTarget);
            effect.Parameters["ProjectileTarget"].SetValue(targetCollection.ProjectileTarget);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, null, null, effect);
            spriteBatch.Draw(screenTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            device.SetRenderTargets(oldTargets);
        }

        public static void RenderMaterialBuffer(SpriteBatch spriteBatch, BufferCollection bufferCollection, TargetCollection targetCollection, RenderTarget2D screenTarget)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            device.SetRenderTarget(bufferCollection.MaterialBuffer);
            device.Clear(Color.Transparent);

            Effect effect = EffectManager.GetEffect(EffectType.Material);

            effect.Parameters["TileStride"].SetValue(TileHelper.GetTileStride());
            effect.Parameters["TileIDTextureSize"].SetValue(new Vector2(bufferCollection.TileIDTexture.Width, bufferCollection.TileIDTexture.Height));

            effect.Parameters["ScreenPosition"].SetValue(Main.screenPosition);

            Vector2 viewSize = new Vector2(Main.screenWidth, Main.screenHeight);
            effect.Parameters["ViewSize"].SetValue(viewSize);

            int minTileX = (int)Math.Floor(Main.screenPosition.X / 16f);
            int minTileY = (int)Math.Floor(Main.screenPosition.Y / 16f);

            effect.Parameters["MinTilePosition"].SetValue(new Vector2(minTileX, minTileY));

            effect.Parameters["MaterialTextureSize"].SetValue(new Vector2(bufferCollection.MaterialTexture.Width, bufferCollection.MaterialTexture.Height));

            effect.Parameters["TileTarget"].SetValue(targetCollection.StableTileTarget);
            effect.Parameters["NonSolidTileTarget"].SetValue(targetCollection.NonSolidTileTarget);
            effect.Parameters["EntityTarget"].SetValue(targetCollection.EntityTarget);
            effect.Parameters["PlayerTarget"].SetValue(targetCollection.PlayerTarget);
            effect.Parameters["ProjectileTarget"].SetValue(targetCollection.ProjectileTarget);

            effect.Parameters["TileIDTexture"].SetValue(bufferCollection.TileIDTexture);
            effect.Parameters["MaterialTexture"].SetValue(bufferCollection.MaterialTexture);

            device.SamplerStates[6] = SamplerState.PointClamp;
            device.SamplerStates[7] = SamplerState.PointClamp;

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, null, null, effect);
            spriteBatch.Draw(screenTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            device.SetRenderTargets(oldTargets);
        }
    }
}

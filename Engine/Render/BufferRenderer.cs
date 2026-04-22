using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terralume.Engine.Buffers;
using Terralume.Engine.Shaders;
using Terralume.Engine.Utils;
using Terraria;

namespace Terralume.Engine.Render
{
    public static class BufferRenderer
    {
        // Copies Main.screen target into a custom render target, the albedo map represents each pixels diffuse color
        // Note: For this to copy a true albedo buffer, vanilla lighting must be disabled
        public static void RenderAlbedoBuffer(SpriteBatch spriteBatch, BufferCollection bufferCollection, TargetCollection targetCollection)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            device.SetRenderTarget(bufferCollection.AlbedoBuffer);
            device.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            // We bake the underground background into the albedo buffer because it is affected by lights
            spriteBatch.Draw(targetCollection.UndergroundBackgroundBuffer, Vector2.Zero, Color.White);

            spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);

            spriteBatch.End();

            device.SetRenderTargets(oldTargets);
        }

        // Renders a depth map by sampling pixels from multiple captured render targets as layers to asign depth values
        public static void RenderDepthBuffer(SpriteBatch spriteBatch, BufferCollection bufferCollection, TargetCollection targetCollection, RenderTarget2D screenTarget)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            device.SetRenderTarget(bufferCollection.DepthBuffer);
            device.Clear(Color.Transparent);

            Effect effect = EffectManager.GetEffect(EffectType.Depth);

            effect.Parameters["TileTarget"].SetValue(targetCollection.StableTileTarget);
            effect.Parameters["WallTarget"].SetValue(targetCollection.StableWallTarget);

            device.SamplerStates[1] = SamplerState.PointClamp;
            device.SamplerStates[2] = SamplerState.PointClamp;

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
            effect.Parameters["TileIDTexture"].SetValue(bufferCollection.TileIDTexture);
            effect.Parameters["MaterialTexture"].SetValue(bufferCollection.MaterialTexture);

            device.SamplerStates[1] = SamplerState.PointClamp;
            device.SamplerStates[2] = SamplerState.PointClamp;
            device.SamplerStates[3] = SamplerState.PointClamp;

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, null, null, effect);
            spriteBatch.Draw(screenTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            device.SetRenderTargets(oldTargets);
        }
    }
}

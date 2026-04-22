using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terralume.Engine.Shaders;
using Terralume.Engine.Utils;
using Terraria;

namespace Terralume.Engine.Buffers
{
    public static class BufferRenderer
    {
        // Copies Main.screen target into a custom render target, the albedo map represents each pixels diffuse color
        // Note: For this to copy a true albedo buffer, vanilla lighting must be disabled
        public static void RenderAlbedoBuffer(SpriteBatch spriteBatch, BufferCollection bufferCollection)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            device.SetRenderTarget(bufferCollection.AlbedoBuffer);
            device.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            // We bake the underground background into the albedo buffer because it is affected by lights
            spriteBatch.Draw(bufferCollection.UndergroundBackgroundBuffer, Vector2.Zero, Color.White);

            spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);

            spriteBatch.End();

            device.SetRenderTargets(oldTargets);
        }

        // Renders a depth map by sampling pixels from multiple captured render targets as layers to asign depth values
        public static void RenderDepthBuffer(SpriteBatch spriteBatch, BufferCollection bufferCollection, RenderTarget2D screenTarget)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            device.SetRenderTarget(bufferCollection.DepthBuffer);
            device.Clear(Color.Transparent);

            Effect effect = EffectManager.GetEffect(EffectType.Depth);

            effect.Parameters["TileTarget"].SetValue(bufferCollection.StableTileTarget);
            effect.Parameters["WallTarget"].SetValue(bufferCollection.StableWallTarget);

            device.SamplerStates[1] = SamplerState.PointClamp;
            device.SamplerStates[2] = SamplerState.PointClamp;

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, null, null, effect);
            spriteBatch.Draw(screenTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            device.SetRenderTargets(oldTargets);
        }

        public static void RenderMaterialBuffer(SpriteBatch spriteBatch, BufferCollection bufferCollection, RenderTarget2D screenTarget)
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

            effect.Parameters["TileTarget"].SetValue(bufferCollection.StableTileTarget);
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

        // Copies the background (surface/underworld texture) into a custom background render target
        // Note: It's stored in a seperate render target because these backgrounds should not be affected by lighting
        public static void CopyBackgroundBuffer(SpriteBatch spriteBatch, BufferCollection bufferCollection)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            device.SetRenderTarget(bufferCollection.BackgroundBuffer);
            device.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            // Clear the old targets, this ensures the surface/underworld background does not appear within Main.screenTarget and the albedo buffer
            device.SetRenderTargets(oldTargets);
            device.Clear(Color.Transparent);
        }

        // Copies the underground background into a custom rendertarget with the correct offset applied
        public static void CopyUndergroundBackgroundBuffer(SpriteBatch spriteBatch, BufferCollection bufferCollection, ref bool backgroundRedrawn)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            // On frames where Terraria redraws the backgroundTarget, we copy this into our master underground background buffer
            if (backgroundRedrawn)
            {
                device.SetRenderTarget(bufferCollection.UndergroundBackgroundBufferMaster);
                device.Clear(Color.Transparent);

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);

                spriteBatch.Draw(Main.instance.backgroundTarget, Vector2.Zero, Color.White);

                spriteBatch.End();

                backgroundRedrawn = false;
            }

            // Every frame the master underground background buffer gets copied into the main underground background buffer
            device.SetRenderTarget(bufferCollection.UndergroundBackgroundBuffer);
            device.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, null, null);

            Vector2 drawPos = Main.sceneBackgroundPos - Main.screenPosition;

            spriteBatch.Draw(bufferCollection.UndergroundBackgroundBufferMaster, drawPos, Color.White);
            spriteBatch.End();

            // Clear the background target so that the underground background texture does not appear within Main.screenTarget
            device.SetRenderTarget(Main.instance.backgroundTarget);
            device.Clear(Color.Transparent);

            device.SetRenderTargets(oldTargets);
        }

        // Copies Terrarias tile target into a custom rendertarget with the correct offset applied
        public static void CopyTileTarget(SpriteBatch spriteBatch, BufferCollection bufferCollection)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            device.SetRenderTarget(bufferCollection.StableTileTarget);

            Vector2 tileDrawPos = Main.sceneTilePos - Main.screenPosition;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            spriteBatch.Draw(Main.instance.tileTarget, tileDrawPos, Color.White);
            spriteBatch.End();

            device.SetRenderTargets(oldTargets);
        }

        // Copies Terrarias wall target into a custom rendertarget with the correct offset applied
        public static void CopyWallTarget(SpriteBatch spriteBatch, BufferCollection bufferCollection)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            device.SetRenderTarget(bufferCollection.StableWallTarget);

            Vector2 tileDrawPos = Main.sceneWallPos - Main.screenPosition;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            spriteBatch.Draw(Main.instance.wallTarget, tileDrawPos, Color.White);
            spriteBatch.End();

            device.SetRenderTargets(oldTargets);
        }

        // Uses vanilla NPC drawing logic to draw NPCs to a custom rendertarget
        // Note: There is a lot of spritebatch restarting and rendertarget switching here, might need something better in the future
        public static void CopyNPCTarget(SpriteBatch spriteBatch, BufferCollection bufferCollection, On_Main.orig_DrawNPCs orig, Main self, bool behindTiles)
        {
            Main.spriteBatch.End();

            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            // Copy screenTarget
            device.SetRenderTarget(bufferCollection.CopyBuffer);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            // Draw Entities
            device.SetRenderTarget(bufferCollection.NPCBuffer);

            if (behindTiles)
            {
                device.Clear(Color.Transparent);
            }

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            orig(self, behindTiles);
            Main.spriteBatch.End();

            // Paste back into screenTarget
            device.SetRenderTargets(oldTargets);
            device.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            spriteBatch.Draw(bufferCollection.CopyBuffer, Vector2.Zero, Color.White);
            spriteBatch.End();

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        }

        public static void CopyPlayerTarget(SpriteBatch spriteBatch, BufferCollection bufferCollection, On_Main.orig_DrawPlayers_AfterProjectiles orig, Main self)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            // Copy screenTarget
            device.SetRenderTarget(bufferCollection.CopyBuffer);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            // Draw Entities
            device.SetRenderTarget(bufferCollection.PlayerBuffer);
            
            orig(self);

            // Paste back into screenTarget
            device.SetRenderTargets(oldTargets);
            device.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            spriteBatch.Draw(bufferCollection.CopyBuffer, Vector2.Zero, Color.White);
            spriteBatch.End();
        }

        public static void CopyProjectileTarget(SpriteBatch spriteBatch, BufferCollection bufferCollection, On_Main.orig_DrawProjectiles orig, Main self)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            // Copy screenTarget
            device.SetRenderTarget(bufferCollection.CopyBuffer);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            // Draw Entities
            device.SetRenderTarget(bufferCollection.ProjectileBuffer);

            orig(self);

            // Paste back into screenTarget
            device.SetRenderTargets(oldTargets);
            device.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            spriteBatch.Draw(bufferCollection.CopyBuffer, Vector2.Zero, Color.White);
            spriteBatch.End();
        }
    }
}

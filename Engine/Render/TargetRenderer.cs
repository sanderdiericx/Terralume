using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terralume.Engine.Buffers;
using Terraria;

namespace Terralume.Engine.Render
{
    public static class TargetRenderer
    {
        // Copies the background (surface/underworld texture) into a custom background render target
        // Note: It's stored in a seperate render target because these backgrounds should not be affected by lighting
        public static void CopyBackgroundBuffer(SpriteBatch spriteBatch, TargetCollection targetCollection)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            device.SetRenderTarget(targetCollection.BackgroundTarget);
            device.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            // Clear the old targets, this ensures the surface/underworld background does not appear within Main.screenTarget and the albedo buffer
            device.SetRenderTargets(oldTargets);
            device.Clear(Color.Transparent);
        }

        // Copies the underground background into a custom rendertarget with the correct offset applied
        public static void CopyUndergroundBackgroundBuffer(SpriteBatch spriteBatch, TargetCollection targetCollection, ref bool backgroundRedrawn)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            // On frames where Terraria redraws the backgroundTarget, we copy this into our master underground background buffer
            if (backgroundRedrawn)
            {
                device.SetRenderTarget(targetCollection.UndergroundBackgroundTargetMaster);
                device.Clear(Color.Transparent);

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);

                spriteBatch.Draw(Main.instance.backgroundTarget, Vector2.Zero, Color.White);

                spriteBatch.End();

                backgroundRedrawn = false;
            }

            // Every frame the master underground background buffer gets copied into the main underground background buffer
            device.SetRenderTarget(targetCollection.UndergroundBackgroundTarget);
            device.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, null, null);

            Vector2 drawPos = Main.sceneBackgroundPos - Main.screenPosition;

            spriteBatch.Draw(targetCollection.UndergroundBackgroundTargetMaster, drawPos, Color.White);
            spriteBatch.End();

            // Clear the background target so that the underground background texture does not appear within Main.screenTarget
            device.SetRenderTarget(Main.instance.backgroundTarget);
            device.Clear(Color.Transparent);

            device.SetRenderTargets(oldTargets);
        }

        // Copies Terrarias tile target into a custom rendertarget with the correct offset applied
        public static void CopyTileTarget(SpriteBatch spriteBatch, TargetCollection targetCollection)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            device.SetRenderTarget(targetCollection.StableTileTarget);
            device.Clear(Color.Transparent);

            Vector2 tileDrawPos = Main.sceneTilePos - Main.screenPosition;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            spriteBatch.Draw(Main.instance.tileTarget, tileDrawPos, Color.White);
            spriteBatch.End();

            device.SetRenderTargets(oldTargets);
        }

        // Copies Terrarias wall target into a custom rendertarget with the correct offset applied
        public static void CopyWallTarget(SpriteBatch spriteBatch, TargetCollection targetCollection)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            device.SetRenderTarget(targetCollection.StableWallTarget);
            device.Clear(Color.Transparent);

            Vector2 tileDrawPos = Main.sceneWallPos - Main.screenPosition;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            spriteBatch.Draw(Main.instance.wallTarget, tileDrawPos, Color.White);
            spriteBatch.End();

            device.SetRenderTargets(oldTargets);
        }

        // Uses vanilla NPC drawing logic to draw NPCs to a custom rendertarget
        // Note: There is a lot of spritebatch restarting and rendertarget switching here, might need something better in the future
        public static void CopyEntityTarget(SpriteBatch spriteBatch, TargetCollection targetCollection, On_Main.orig_DrawNPCs orig, Main self, bool behindTiles)
        {
            Main.spriteBatch.End();

            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            // Copy screenTarget
            device.SetRenderTarget(targetCollection.CopyTarget);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            // Draw Entities
            device.SetRenderTarget(targetCollection.EntityTarget);
            device.Clear(Color.Transparent);

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            orig(self, behindTiles);
            Main.spriteBatch.End();

            // Paste back into screenTarget
            device.SetRenderTargets(oldTargets);
            device.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            spriteBatch.Draw(targetCollection.CopyTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        }

        public static void CopyPlayerTarget(SpriteBatch spriteBatch, TargetCollection targetCollection, On_Main.orig_DrawPlayers_AfterProjectiles orig, Main self)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            // Copy screenTarget
            device.SetRenderTarget(targetCollection.CopyTarget);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            // Draw Entities
            device.SetRenderTarget(targetCollection.PlayerTarget);
            device.Clear(Color.Transparent);

            orig(self);

            // Paste back into screenTarget
            device.SetRenderTargets(oldTargets);
            device.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            spriteBatch.Draw(targetCollection.CopyTarget, Vector2.Zero, Color.White);
            spriteBatch.End();
        }

        public static void CopyProjectileTarget(SpriteBatch spriteBatch, TargetCollection targetCollection, On_Main.orig_DrawProjectiles orig, Main self)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            // Copy screenTarget
            device.SetRenderTarget(targetCollection.CopyTarget);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            // Draw Entities
            device.SetRenderTarget(targetCollection.ProjectileTarget);
            device.Clear(Color.Transparent);

            orig(self);

            // Paste back into screenTarget
            device.SetRenderTargets(oldTargets);
            device.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            spriteBatch.Draw(targetCollection.CopyTarget, Vector2.Zero, Color.White);
            spriteBatch.End();
        }

        public static void CopyNonSolidTileTarget(SpriteBatch spriteBatch, TargetCollection targetCollection, On_Main.orig_DoDraw_Tiles_NonSolid orig, Main self)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            // Copy screenTarget
            device.SetRenderTarget(targetCollection.CopyTarget);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            // Draw Entities
            device.SetRenderTarget(targetCollection.NonSolidTileTarget);
            device.Clear(Color.Transparent);

            orig(self);

            // Paste back into screenTarget
            device.SetRenderTargets(oldTargets);
            device.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            spriteBatch.Draw(targetCollection.CopyTarget, Vector2.Zero, Color.White);
            spriteBatch.End();
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terralume.Engine.Buffers;
using Terraria;

namespace Terralume.Engine.Render
{
    // Note: There is a lot of double drawing, spritebatch restarting and rendertarget switching here, might need something better in the future
    public class TargetRenderer
    {
        // Copies the background (surface/underworld texture) into a custom background render target
        // Note: It's stored in a seperate render target because these backgrounds should not be affected by lighting
        public static void CopyBackgroundBuffer(SpriteBatch spriteBatch, TargetCollection targetCollection)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            device.SetRenderTarget(targetCollection.Background);
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
                device.SetRenderTarget(targetCollection.UndergroundBackgroundMaster);
                device.Clear(Color.Transparent);

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);

                spriteBatch.Draw(Main.instance.backgroundTarget, Vector2.Zero, Color.White);

                spriteBatch.End();

                backgroundRedrawn = false;
            }

            // Every frame the master underground background buffer gets copied into the main underground background buffer
            device.SetRenderTarget(targetCollection.UndergroundBackground);
            device.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            Vector2 drawPos = Main.sceneBackgroundPos - Main.screenPosition;

            spriteBatch.Draw(targetCollection.UndergroundBackgroundMaster, drawPos, Color.White);
            spriteBatch.End();

            // Clear the background target so that the underground background texture does not appear within Main.screenTarget
            device.SetRenderTarget(Main.instance.backgroundTarget);
            device.Clear(Color.Transparent);

            device.SetRenderTargets(oldTargets);
        }

        // Renders 2 blurred versions of the underground background backround target by downsampling and then upsampling at different resolutions
        // Note: De-duplicate when motivated
        public static void RenderUndergroundBackgroundBlurTargets(SpriteBatch spriteBatch, TargetCollection targetCollection)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();


            // TIGHT BLUR

            // Render the underground background target into a 1/2 resolution copy target with bilinear sampling
            device.SetRenderTarget(targetCollection.UndergroundBackgroundBlurTightCopy);
            device.Clear(Color.Transparent);

            Vector2 tightBlurCopySize = targetCollection.UndergroundBackgroundBlurTightCopy.Size();

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.LinearClamp, null, null);
            spriteBatch.Draw(targetCollection.UndergroundBackground, new Rectangle(0, 0, (int) tightBlurCopySize.X, (int) tightBlurCopySize.Y), Color.White);
            spriteBatch.End();

            // Render the 1/2 resolution copy target into the tight blur target at native resolution with bilinear sampling
            device.SetRenderTarget(targetCollection.UndergroundBackgroundBlurTight);
            device.Clear(Color.Transparent);

            Vector2 tightBlurSize = targetCollection.UndergroundBackgroundBlurTight.Size();

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.LinearClamp, null, null);
            spriteBatch.Draw(targetCollection.UndergroundBackgroundBlurTightCopy, new Rectangle(0, 0, (int) tightBlurSize.X, (int) tightBlurSize.Y), Color.White);
            spriteBatch.End();


            // WIDE BLUR

            // Render the underground background target into a 1/4 resolution copy target with bilinear sampling
            device.SetRenderTarget(targetCollection.UndergroundBackgroundBlurWideCopy);
            device.Clear(Color.Transparent);

            Vector2 wideBlurCopySize = targetCollection.UndergroundBackgroundBlurWideCopy.Size();

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.LinearClamp, null, null);
            spriteBatch.Draw(targetCollection.UndergroundBackground, new Rectangle(0, 0, (int)wideBlurCopySize.X, (int)wideBlurCopySize.Y), Color.White);
            spriteBatch.End();

            // Render the 1/4 resolution copy target into the tight blur target at native resolution with bilinear sampling
            device.SetRenderTarget(targetCollection.UndergroundBackgroundBlurWide);
            device.Clear(Color.Transparent);

            Vector2 wideBlurSize = targetCollection.UndergroundBackgroundBlurWide.Size();

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.LinearClamp, null, null);
            spriteBatch.Draw(targetCollection.UndergroundBackgroundBlurWideCopy, new Rectangle(0, 0, (int)wideBlurSize.X, (int)wideBlurSize.Y), Color.White);
            spriteBatch.End();

            device.SetRenderTargets(oldTargets);
        }

        public static void CopyEntityTarget(SpriteBatch spriteBatch, TargetCollection targetCollection, On_Main.orig_DrawNPCs orig, Main self, bool behindTiles)
        {
            Main.spriteBatch.End();

            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            // Copy screenTarget
            device.SetRenderTarget(targetCollection.Copy);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            device.SetRenderTarget(targetCollection.Entity);
            device.Clear(Color.Transparent);

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            orig(self, behindTiles);
            Main.spriteBatch.End();

            // Paste back into screenTarget
            device.SetRenderTargets(oldTargets);
            device.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            spriteBatch.Draw(targetCollection.Copy, Vector2.Zero, Color.White);
            spriteBatch.End();

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        }

        public static void CopyPlayerTarget(SpriteBatch spriteBatch, TargetCollection targetCollection, On_Main.orig_DrawPlayers_AfterProjectiles orig, Main self)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            // Copy screenTarget
            device.SetRenderTarget(targetCollection.Copy);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            device.SetRenderTarget(targetCollection.Player);
            device.Clear(Color.Transparent);

            orig(self);

            // Paste back into screenTarget
            device.SetRenderTargets(oldTargets);
            device.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            spriteBatch.Draw(targetCollection.Copy, Vector2.Zero, Color.White);
            spriteBatch.End();
        }

        public static void CopyProjectileTarget(SpriteBatch spriteBatch, TargetCollection targetCollection, On_Main.orig_DrawProjectiles orig, Main self)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            // Copy screenTarget
            device.SetRenderTarget(targetCollection.Copy);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            device.SetRenderTarget(targetCollection.Projectile);
            device.Clear(Color.Transparent);

            orig(self);

            // Paste back into screenTarget
            device.SetRenderTargets(oldTargets);
            device.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            spriteBatch.Draw(targetCollection.Copy, Vector2.Zero, Color.White);
            spriteBatch.End();
        }

        public static void CopyTileTarget(SpriteBatch spriteBatch, TargetCollection targetCollection, On_Main.orig_DoDraw_Tiles_Solid orig, Main self)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            // Copy screenTarget
            device.SetRenderTarget(targetCollection.Copy);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            device.SetRenderTarget(targetCollection.Tile);
            device.Clear(Color.Transparent);

            orig(self);

            // Paste back into screenTarget
            device.SetRenderTargets(oldTargets);
            device.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            spriteBatch.Draw(targetCollection.Copy, Vector2.Zero, Color.White);
            spriteBatch.End();
        }

        public static void CopyNonSolidTileTarget(SpriteBatch spriteBatch, TargetCollection targetCollection, On_Main.orig_DoDraw_Tiles_NonSolid orig, Main self)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            // Copy screenTarget
            device.SetRenderTarget(targetCollection.Copy);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            device.SetRenderTarget(targetCollection.NonSolidTile);
            device.Clear(Color.Transparent);

            orig(self);

            // Paste back into screenTarget
            device.SetRenderTargets(oldTargets);
            device.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            spriteBatch.Draw(targetCollection.Copy, Vector2.Zero, Color.White);
            spriteBatch.End();
        }

        public static void CopyWallTarget(SpriteBatch spriteBatch, TargetCollection targetCollection)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            device.SetRenderTarget(targetCollection.Wall);
            device.Clear(Color.Transparent);

            // Draw wall target with correct offset into our custom target
            Vector2 wallOffset = Main.sceneWallPos - Main.screenPosition;

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            spriteBatch.Draw(Main.instance.wallTarget, wallOffset, Color.White);
            spriteBatch.End();

            device.SetRenderTargets(oldTargets);
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terralume.Engine.Buffers;
using Terralume.Engine.Constants;
using Terralume.Engine.Render;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Terralume.Engine.Pipeline
{
    internal class TargetCapturer : ModSystem
    {
        private static TargetCollection targetCollection;

        private static SpriteBatch deferredSpriteBatch;
        private static bool backgroundRedrawn;

        public override void Load()
        {
            Main.QueueMainThreadAction(() =>
            {
                targetCollection = new TargetCollection(Main.graphics.GraphicsDevice, (int)RenderConstants.SCREEN_SIZE.X, (int)RenderConstants.SCREEN_SIZE.Y);
                deferredSpriteBatch = new SpriteBatch(Main.graphics.GraphicsDevice);
            });


            On_Main.DoDraw += On_Main_DoDraw;
            On_Main.RenderBackground += On_Main_RenderBackground;
            On_Main.DoDraw_WallsAndBlacks += On_Main_DoDraw_WallsAndBlacks;

            On_Main.DoDraw_Tiles_Solid += On_Main_DoDraw_Tiles_Solid;
            On_Main.DoDraw_Tiles_NonSolid += On_Main_DoDraw_Tiles_NonSolid;

            On_Main.DrawNPCs += On_Main_DrawNPCs;
            On_Main.DrawPlayers_AfterProjectiles += On_Main_DrawPlayers_AfterProjectiles;
            On_Main.DrawProjectiles += On_Main_DrawProjectiles;

            backgroundRedrawn = false;
        }

        public override void Unload()
        {
            base.Unload();

            Main.QueueMainThreadAction(() =>
            {
                targetCollection.Dispose();
            });

            On_Main.DoDraw -= On_Main_DoDraw;
            On_Main.RenderBackground -= On_Main_RenderBackground;
            On_Main.DoDraw_WallsAndBlacks -= On_Main_DoDraw_WallsAndBlacks;

            On_Main.DoDraw_Tiles_Solid -= On_Main_DoDraw_Tiles_Solid;
            On_Main.DoDraw_Tiles_NonSolid -= On_Main_DoDraw_Tiles_NonSolid;

            On_Main.DrawNPCs -= On_Main_DrawNPCs;
            On_Main.DrawPlayers_AfterProjectiles -= On_Main_DrawPlayers_AfterProjectiles;
            On_Main.DrawProjectiles -= On_Main_DrawProjectiles;
        }

        private void On_Main_DoDraw(On_Main.orig_DoDraw orig, Main self, GameTime gameTime)
        {
            UpdateBuffer();

            orig(self, gameTime);
        }

        private void On_Main_RenderBackground(On_Main.orig_RenderBackground orig, Main self)
        {
            orig(self);

            backgroundRedrawn = true;
        }

        private void On_Main_DoDraw_WallsAndBlacks(On_Main.orig_DoDraw_WallsAndBlacks orig, Main self)
        {
            TargetRenderer.CopyBackgroundBuffer(deferredSpriteBatch, targetCollection);
            TargetRenderer.CopyUndergroundBackgroundBuffer(deferredSpriteBatch, targetCollection, ref backgroundRedrawn);

            orig(self);

            TargetRenderer.CopyWallTarget(deferredSpriteBatch, targetCollection);

            // Main.screenTarget cannot have background targets for g-buffer composition
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);
        }

        private void On_Main_DoDraw_Tiles_Solid(On_Main.orig_DoDraw_Tiles_Solid orig, Main self)
        {
            orig(self);

            TargetRenderer.CopyTileTarget(deferredSpriteBatch, targetCollection, orig, self);
        }

        private void On_Main_DoDraw_Tiles_NonSolid(On_Main.orig_DoDraw_Tiles_NonSolid orig, Main self)
        {
            orig(self);

            TargetRenderer.CopyNonSolidTileTarget(deferredSpriteBatch, targetCollection, orig, self);
        }

        private void On_Main_DrawNPCs(On_Main.orig_DrawNPCs orig, Main self, bool behindTiles)
        {
            orig(self, behindTiles);

            TargetRenderer.CopyEntityTarget(deferredSpriteBatch, targetCollection, orig, self, behindTiles);
        }

        private void On_Main_DrawPlayers_AfterProjectiles(On_Main.orig_DrawPlayers_AfterProjectiles orig, Main self)
        {
            orig(self);

            TargetRenderer.CopyPlayerTarget(deferredSpriteBatch, targetCollection, orig, self);
        }

        private void On_Main_DrawProjectiles(On_Main.orig_DrawProjectiles orig, Main self)
        {
            orig(self);

            TargetRenderer.CopyProjectileTarget(deferredSpriteBatch, targetCollection, orig, self);
        }

        public static TargetCollection GetTargetCollection()
        {
            TargetRenderer.CopyLiquidTarget(deferredSpriteBatch, targetCollection);

            return targetCollection;
        }

        // Make sure all buffers match screen size
        private void UpdateBuffer()
        {
            if (!targetCollection.IsValid())
            {
                targetCollection.Dispose();
                targetCollection = new TargetCollection(Main.graphics.GraphicsDevice, (int)RenderConstants.SCREEN_SIZE.X, (int)RenderConstants.SCREEN_SIZE.Y);
            }
        }
    }
}
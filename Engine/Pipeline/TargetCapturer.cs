using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terralume.Engine.Buffers;
using Terralume.Engine.Constants;
using Terralume.Engine.Render;
using Terraria;
using Terraria.ModLoader;

namespace Terralume.Engine.Pipeline
{
    public class TargetCapturer : ModSystem
    {
        public static TargetCollection TargetCollection;

        private static SpriteBatch deferredSpriteBatch;
        private bool backgroundRedrawn;

        public override void Load()
        {
            Main.QueueMainThreadAction(() =>
            {
                TargetCollection = new TargetCollection(Main.graphics.GraphicsDevice, (int)RenderConstants.SCREEN_SIZE.X, (int)RenderConstants.SCREEN_SIZE.Y);
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
                TargetCollection.Dispose();
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
            TargetRenderer.CopyBackgroundBuffer(deferredSpriteBatch, TargetCollection);
            TargetRenderer.CopyUndergroundBackgroundBuffer(deferredSpriteBatch, TargetCollection, ref backgroundRedrawn);

            orig(self);

            TargetRenderer.CopyWallTarget(deferredSpriteBatch, TargetCollection);

            // Main.screenTarget cannot have background targets for g-buffer composition
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);
        }

        private void On_Main_DoDraw_Tiles_Solid(On_Main.orig_DoDraw_Tiles_Solid orig, Main self)
        {
            orig(self);

            TargetRenderer.CopyTileTarget(deferredSpriteBatch, TargetCollection, orig, self);
        }

        private void On_Main_DoDraw_Tiles_NonSolid(On_Main.orig_DoDraw_Tiles_NonSolid orig, Main self)
        {
            orig(self);

            TargetRenderer.CopyNonSolidTileTarget(deferredSpriteBatch, TargetCollection, orig, self);
        }


        private void On_Main_DrawNPCs(On_Main.orig_DrawNPCs orig, Main self, bool behindTiles)
        {
            orig(self, behindTiles);

            TargetRenderer.CopyEntityTarget(deferredSpriteBatch, TargetCollection, orig, self, behindTiles);
        }

        private void On_Main_DrawPlayers_AfterProjectiles(On_Main.orig_DrawPlayers_AfterProjectiles orig, Main self)
        {
            orig(self);

            TargetRenderer.CopyPlayerTarget(deferredSpriteBatch, TargetCollection, orig, self);
        }

        private void On_Main_DrawProjectiles(On_Main.orig_DrawProjectiles orig, Main self)
        {
            orig(self);

            TargetRenderer.CopyProjectileTarget(deferredSpriteBatch, TargetCollection, orig, self);
        }

        // Make sure all buffers match screen size
        private void UpdateBuffer()
        {
            if (!TargetCollection.IsValid())
            {
                TargetCollection.Dispose();
                TargetCollection = new TargetCollection(Main.graphics.GraphicsDevice, (int)RenderConstants.SCREEN_SIZE.X, (int)RenderConstants.SCREEN_SIZE.Y);
            }
        }
    }
}
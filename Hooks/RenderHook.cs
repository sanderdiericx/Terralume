using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terralume.Engine.Buffers;
using Terralume.Engine.Materials;
using Terralume.Engine.Shaders;
using Terralume.Engine.Utils;
using Terraria;
using Terraria.ModLoader;

namespace Terralume.Hooks
{
    public class RenderHook : ModSystem
    {
        private BufferCollection bufferCollection;
        private SpriteBatch deferredSpriteBatch;

        private RenderTarget2D currentRenderTarget;

        private bool backgroundRedrawn;


        private int tick;

        public override void Load()
        {
            Main.QueueMainThreadAction(() =>
            {
                bufferCollection = new BufferCollection(Main.graphics.GraphicsDevice, Main.graphics.GraphicsDevice.PresentationParameters.BackBufferWidth, Main.graphics.GraphicsDevice.PresentationParameters.BackBufferHeight);
                deferredSpriteBatch = new SpriteBatch(Main.graphics.GraphicsDevice);

                EffectManager.LoadEffect(EffectType.Depth);
                EffectManager.LoadEffect(EffectType.Material);
            });

            On_Main.RenderBackground += On_Main_RenderBackground;
            On_Main.DoDraw_WallsAndBlacks += On_Main_DoDraw_WallsAndBlacks;
            On_Main.DrawInterface += On_Main_DrawInterface;

            On_Main.DrawNPCs += On_Main_DrawNPCs;
            On_Main.DrawPlayers_AfterProjectiles += On_Main_DrawPlayers_AfterProjectiles;
            On_Main.DrawProjectiles += On_Main_DrawProjectiles;

            backgroundRedrawn = false;

            currentRenderTarget = Main.screenTarget;

            tick = 0;
        }

        public override void Unload()
        {
            base.Unload();

            Main.QueueMainThreadAction(() =>
            {
                bufferCollection.Dispose();
            });

            On_Main.RenderBackground -= On_Main_RenderBackground;
            On_Main.DoDraw_WallsAndBlacks -= On_Main_DoDraw_WallsAndBlacks;
            On_Main.DrawInterface -= On_Main_DrawInterface;

            On_Main.DrawNPCs -= On_Main_DrawNPCs;
            On_Main.DrawPlayers_AfterProjectiles -= On_Main_DrawPlayers_AfterProjectiles;
            On_Main.DrawProjectiles -= On_Main_DrawProjectiles;
        }

        private void On_Main_RenderBackground(On_Main.orig_RenderBackground orig, Main self)
        {
            orig(self);

            backgroundRedrawn = true;
        }

        private void On_Main_DoDraw_WallsAndBlacks(On_Main.orig_DoDraw_WallsAndBlacks orig, Main self)
        {
            tick++;

            UpdateBuffer();

            BufferRenderer.CopyBackgroundBuffer(deferredSpriteBatch, bufferCollection);
            BufferRenderer.CopyUndergroundBackgroundBuffer(deferredSpriteBatch, bufferCollection, ref backgroundRedrawn);

            Main.graphics.GraphicsDevice.Clear(Color.Transparent);

            orig(self);
        }

        private void On_Main_DrawNPCs(On_Main.orig_DrawNPCs orig, Main self, bool behindTiles)
        {
            orig(self, behindTiles);

            BufferRenderer.CopyNPCTarget(deferredSpriteBatch, bufferCollection, orig, self, behindTiles);
        }

        private void On_Main_DrawPlayers_AfterProjectiles(On_Main.orig_DrawPlayers_AfterProjectiles orig, Main self)
        {
            orig(self);

            BufferRenderer.CopyPlayerTarget(deferredSpriteBatch, bufferCollection, orig, self);
        }

        private void On_Main_DrawProjectiles(On_Main.orig_DrawProjectiles orig, Main self)
        {
            orig(self);

            BufferRenderer.CopyProjectileTarget(deferredSpriteBatch, bufferCollection, orig, self);
        }


        private void On_Main_DrawInterface(On_Main.orig_DrawInterface orig, Main self, GameTime gameTime)
        {
            // TO DO: SAMPLE Main.screenTarget here to build normal and material buffer.
            short[] tileIDs = TileHelper.GetVisibleTiles();

            TileHelper.PackTileIDTexture(bufferCollection.TileIDTexture, tileIDs);
            TileHelper.PackMaterialTexture(bufferCollection.MaterialTexture, MaterialMapper.MappedTiles);

            BufferRenderer.CopyTileTarget(deferredSpriteBatch, bufferCollection);
            BufferRenderer.CopyWallTarget(deferredSpriteBatch, bufferCollection);

            BufferRenderer.RenderDepthBuffer(deferredSpriteBatch, bufferCollection, Main.screenTarget);
            BufferRenderer.RenderMaterialBuffer(deferredSpriteBatch, bufferCollection, Main.screenTarget);

            BufferRenderer.RenderAlbedoBuffer(deferredSpriteBatch, bufferCollection);

            /*
             *  START DEBUG
             */

            KeyboardState keyboardState = Keyboard.GetState();

            if (tick > 5)
            {
                tick = 0;

                if (keyboardState.IsKeyDown(Keys.NumPad1))
                {
                    Main.NewText("[DEBUG] Switching to albedo buffer view", Color.Green);

                    currentRenderTarget = bufferCollection.AlbedoBuffer;
                }
                else if (keyboardState.IsKeyDown(Keys.NumPad2))
                {
                    Main.NewText("[DEBUG] Switching to projectile buffer view", Color.Green);

                    currentRenderTarget = bufferCollection.ProjectileBuffer;
                }
                else if (keyboardState.IsKeyDown(Keys.NumPad3))
                {
                    Main.NewText("[DEBUG] Switching to depth buffer view", Color.Green);

                    currentRenderTarget = bufferCollection.DepthBuffer;
                }
                else if (keyboardState.IsKeyDown(Keys.NumPad4))
                {
                    Main.NewText("[DEBUG] Switching to tile buffer view", Color.Green);

                    currentRenderTarget = bufferCollection.StableTileTarget;
                }
                else if (keyboardState.IsKeyDown(Keys.NumPad5))
                {
                    Main.NewText("[DEBUG] Switching to screen target view", Color.Green);

                    currentRenderTarget = Main.screenTarget;
                }
                else if (keyboardState.IsKeyDown(Keys.NumPad6))
                {
                    Main.NewText("[DEBUG] Switching to player target view", Color.Green);

                    currentRenderTarget = bufferCollection.PlayerBuffer;
                }
                else if (keyboardState.IsKeyDown(Keys.NumPad7))
                {
                    Main.NewText("[DEBUG] Switching to wall target view", Color.Green);

                    currentRenderTarget = bufferCollection.StableWallTarget;
                }
                else if (keyboardState.IsKeyDown(Keys.NumPad8))
                {
                    Main.NewText("[DEBUG] Switching to material buffer view", Color.Green);

                    currentRenderTarget = bufferCollection.MaterialBuffer;
                }
                else if (keyboardState.IsKeyDown(Keys.NumPad9))
                {
                    Main.NewText("[DEBUG] Switching to entity buffer view", Color.Green);

                    currentRenderTarget = bufferCollection.NPCBuffer;
                }
            }

            deferredSpriteBatch.Begin();
            deferredSpriteBatch.Draw(currentRenderTarget, Vector2.Zero, Color.White);
            deferredSpriteBatch.End();

            /*
             *  STOP DEBUG
             */

            // Draw UI normally
            orig(self, gameTime);
        }

        // Make sure all buffers match screen size
        private void UpdateBuffer()
        {
            if (!bufferCollection.IsValid())
            {
                bufferCollection.Dispose();
                bufferCollection = new BufferCollection(Main.graphics.GraphicsDevice, Main.graphics.GraphicsDevice.PresentationParameters.BackBufferWidth, Main.graphics.GraphicsDevice.PresentationParameters.BackBufferHeight);
            }
        }
    }
}
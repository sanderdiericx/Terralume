using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using Terralume.Engine.Buffers;
using Terralume.Engine.Shaders;
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
            });

            On_Main.RenderBackground += On_Main_RenderBackground;
            On_Main.DoDraw_WallsAndBlacks += On_Main_DoDraw_WallsAndBlacks;
            On_Main.DrawInterface += On_Main_DrawInterface;

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

            BufferRenderer.CopyBackgroundBuffer(deferredSpriteBatch, bufferCollection.BackgroundBuffer);

            ///

            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            // STEP 1: CAPTURE (Only when the engine actually renders)
            if (backgroundRedrawn)
            {
                // Use BackgroundBuffer as your "Master Copy" of the raw pixels
                device.SetRenderTarget(bufferCollection.UndergroundBackgroundBufferMaster);
                device.Clear(Color.Transparent);

                deferredSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
                // We draw at Zero because we want the raw, unshifted texture
                deferredSpriteBatch.Draw(Main.instance.backgroundTarget, Vector2.Zero, Color.White);
                deferredSpriteBatch.End();

                backgroundRedrawn = false;
            }

            // STEP 2: BLIT (Every single frame)
            // This draws from your Master Copy to the final buffer using the movement math
            device.SetRenderTarget(bufferCollection.UndergroundBackgroundBuffer);
            device.Clear(Color.Transparent);

            deferredSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, null, null);

            Vector2 drawPos = Main.sceneBackgroundPos - Main.screenPosition;

            // Draw the Master Copy we saved earlier
            deferredSpriteBatch.Draw(bufferCollection.UndergroundBackgroundBufferMaster, drawPos, Color.White);
            deferredSpriteBatch.End();

            // STEP 3: CLEAR ENGINE TARGET
            // We clear the engine's target so it doesn't show up in the final screen blit
            device.SetRenderTarget(Main.instance.backgroundTarget);
            device.Clear(Color.Transparent);

            // Restore and continue
            device.SetRenderTargets(oldTargets);

            ///

            Main.graphics.GraphicsDevice.Clear(Color.Transparent);

            orig(self);
        }


        private void On_Main_DrawInterface(On_Main.orig_DrawInterface orig, Main self, GameTime gameTime)
        {
            // TO DO: SAMPLE Main.screenTarget here to build depth normal and material buffer.

            BufferRenderer.CopyTileTarget(deferredSpriteBatch, bufferCollection.StableTileTarget);

            BufferRenderer.RenderDepthBuffer(deferredSpriteBatch, Main.screenTarget, bufferCollection.DepthBuffer, bufferCollection.StableTileTarget, bufferCollection.UndergroundBackgroundBuffer);

            BufferRenderer.RenderAlbedoBuffer(deferredSpriteBatch, bufferCollection.AlbedoBuffer, bufferCollection.UndergroundBackgroundBuffer);

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
                    Main.NewText("[DEBUG] Switching to background buffer view", Color.Green);

                    currentRenderTarget = bufferCollection.BackgroundBuffer;
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
                    Main.NewText("[DEBUG] Switching to underground background view", Color.Green);

                    currentRenderTarget = bufferCollection.UndergroundBackgroundBuffer;
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
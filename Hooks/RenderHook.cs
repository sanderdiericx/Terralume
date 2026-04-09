using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using Terralume.Engine.Buffers;
using Terraria;
using Terraria.ModLoader;

namespace Terralume.Hooks
{
    public class RenderHook : ModSystem
    {
        private BufferCollection bufferCollection;
        private SpriteBatch deferredSpriteBatch;

        private RenderTarget2D currentRenderTarget;

        private int tick;

        public override void Load()
        {
            Main.QueueMainThreadAction(() =>
            {
                bufferCollection = new BufferCollection(Main.graphics.GraphicsDevice, Main.graphics.GraphicsDevice.PresentationParameters.BackBufferWidth, Main.graphics.GraphicsDevice.PresentationParameters.BackBufferHeight);
                deferredSpriteBatch = new SpriteBatch(Main.graphics.GraphicsDevice);
            });

            On_Main.DoDraw_WallsAndBlacks += ClearForAlbedoHook;
            On_Main.DrawInterface += CaptureAlbedoHook;

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

            On_Main.DoDraw_WallsAndBlacks -= ClearForAlbedoHook;
            On_Main.DrawInterface -= CaptureAlbedoHook;
        }

        private void ClearForAlbedoHook(On_Main.orig_DoDraw_WallsAndBlacks orig, Main self)
        {
            tick++;

            UpdateBuffer();

            BufferRenderer.RenderBackgroundBuffer(deferredSpriteBatch, bufferCollection.BackgroundBuffer);

            orig(self);
        }


        private void CaptureAlbedoHook(On_Main.orig_DrawInterface orig, Main self, GameTime gameTime)
        {
            // TO DO: SAMPLE Main.screenTarget here to build depth normal and material buffer.


            BufferRenderer.RenderAlbedoBuffer(deferredSpriteBatch, bufferCollection.AlbedoBuffer);

            KeyboardState keyboardState = Keyboard.GetState();

            if (tick > 10)
            {
                tick = 0;

                if (keyboardState.IsKeyDown(Keys.F5))
                {
                    Main.NewText("[DEBUG] Switching to albedo buffer view", Color.Green);

                    currentRenderTarget = bufferCollection.AlbedoBuffer;
                }
                else if (keyboardState.IsKeyDown(Keys.F6))
                {
                    Main.NewText("[DEBUG] Switching to background buffer view", Color.Green);

                    currentRenderTarget = bufferCollection.BackgroundBuffer;
                }
            }

            deferredSpriteBatch.Begin();
            deferredSpriteBatch.Draw(currentRenderTarget, Vector2.Zero, Color.White);
            deferredSpriteBatch.End();

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
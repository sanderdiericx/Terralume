using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terralume.Engine.Buffers;
using Terraria;
using Terraria.ModLoader;

namespace Terralume.Engine.Pipeline
{
    public class LightingProcessor : ModSystem
    {
        private SpriteBatch deferredSpriteBatch;

        // Debug
        private RenderTarget2D currentRenderTarget;
        private int tick;

        public override void Load()
        {
            Main.QueueMainThreadAction(() =>
            {
                deferredSpriteBatch = new SpriteBatch(Main.graphics.GraphicsDevice);
            });

            On_Main.DrawInterface += On_Main_DrawInterface;

            // Debug
            currentRenderTarget = Main.screenTarget;
            tick = 0;
        }

        public override void Unload()
        {
            base.Unload();

            On_Main.DrawInterface -= On_Main_DrawInterface;
        }

        private void On_Main_DrawInterface(On_Main.orig_DrawInterface orig, Main self, Microsoft.Xna.Framework.GameTime gameTime)
        {
            tick++;

            BufferComposer.ComposeGBuffer();

            TargetCollection targetCollection = TargetCapturer.TargetCollection;
            BufferCollection bufferCollection = BufferComposer.BufferCollection;


            /*
             *  START DEBUG
             */

            KeyboardState keyboardState = Keyboard.GetState();

            if (tick > 5)
            {
                tick = 0;

                if (keyboardState.IsKeyDown(Keys.NumPad0))
                {
                    Main.NewText("[DEBUG] Switching to normal buffer view", Color.Green);

                    currentRenderTarget = bufferCollection.NormalBuffer;
                }
                else if (keyboardState.IsKeyDown(Keys.NumPad1))
                {
                    Main.NewText("[DEBUG] Switching to albedo buffer view", Color.Green);

                    currentRenderTarget = bufferCollection.AlbedoBuffer;
                }
                else if (keyboardState.IsKeyDown(Keys.NumPad2))
                {
                    Main.NewText("[DEBUG] Switching to depth buffer view", Color.Green);

                    currentRenderTarget = bufferCollection.DepthBuffer;
                }
                else if (keyboardState.IsKeyDown(Keys.NumPad3))
                {
                    Main.NewText("[DEBUG] Switching to material buffer view", Color.Green);

                    currentRenderTarget = bufferCollection.MaterialBuffer;
                }
                else if (keyboardState.IsKeyDown(Keys.NumPad4))
                {
                    Main.NewText("[DEBUG] Switching to liquid target view", Color.Green);

                    currentRenderTarget = Main.instance.backWaterTarget;
                }
                else if (keyboardState.IsKeyDown(Keys.NumPad5))
                {
                    Main.NewText("[DEBUG] Switching to tile target view", Color.Green);

                    currentRenderTarget = targetCollection.StableTileTarget;
                }
                else if (keyboardState.IsKeyDown(Keys.NumPad6))
                {
                    Main.NewText("[DEBUG] Switching to wall target view", Color.Green);

                    currentRenderTarget = targetCollection.StableWallTarget;
                }
                else if (keyboardState.IsKeyDown(Keys.NumPad7))
                {
                    Main.NewText("[DEBUG] Switching to player target view", Color.Green);

                    currentRenderTarget = targetCollection.PlayerTarget;
                }
                else if (keyboardState.IsKeyDown(Keys.NumPad8))
                {
                    Main.NewText("[DEBUG] Switching to entity target view", Color.Green);

                    currentRenderTarget = targetCollection.EntityTarget;
                }
                else if (keyboardState.IsKeyDown(Keys.NumPad9))
                {
                    Main.NewText("[DEBUG] Switching to projectile view", Color.Green);

                    currentRenderTarget = targetCollection.ProjectileTarget;
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
    }
}

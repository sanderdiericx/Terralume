using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Terralume.Engine.Buffers;
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

        private int tick;

        public override void Load()
        {
            Main.QueueMainThreadAction(() =>
            {
                bufferCollection = new BufferCollection(Main.graphics.GraphicsDevice, Main.graphics.GraphicsDevice.PresentationParameters.BackBufferWidth, Main.graphics.GraphicsDevice.PresentationParameters.BackBufferHeight);
                deferredSpriteBatch = new SpriteBatch(Main.graphics.GraphicsDevice);

                EffectManager.LoadEffect(EffectType.Depth);
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

            TileSearchResult tileSearchResult = TileHelper.GetVisibleTiles();

            TileHelper.PackTileTexture(bufferCollection.TileTexture, tileSearchResult.Tiles);
            BufferRenderer.RenderDepthBuffer(deferredSpriteBatch, Main.screenTarget, bufferCollection.DepthBuffer, bufferCollection.TileTexture);

            BufferRenderer.RenderAlbedoBuffer(deferredSpriteBatch, bufferCollection.AlbedoBuffer);

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
                else if (keyboardState.IsKeyDown(Keys.H))
                {
                    bool[] tiles = tileSearchResult.Tiles;
                    int tileStride = TileHelper.GetHorizontalTileStride();
                    string output = "\n";

                    int height = tiles.Length / tileStride;

                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < tileStride; j++)
                        {
                            int index = j + (tileStride * i);

                            output += tiles[index] ? "#" : " ";
                        }

                        output += "\n";
                    }

                    Mod.Logger.Debug(output);
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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Terralume.Engine.Buffers;
using Terralume.Engine.Buffers.Data;
using Terralume.Engine.Render;
using Terralume.Engine.Shaders;
using Terralume.Engine.Utils;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terralume.Engine.Pipeline
{
    public class LightingProcessor : ModSystem
    {
        private OutputCollection outputCollection;
        private SpriteBatch deferredSpriteBatch;

        private List<(Vector2, LightProfile)> onScreenLights;

        private RenderTarget2D currentRenderTarget;

        public override void Load()
        {
            Main.QueueMainThreadAction(() =>
            {
                outputCollection = new OutputCollection(Main.graphics.GraphicsDevice, Main.graphics.GraphicsDevice.PresentationParameters.BackBufferWidth, Main.graphics.GraphicsDevice.PresentationParameters.BackBufferHeight);
                deferredSpriteBatch = new SpriteBatch(Main.graphics.GraphicsDevice);

                EffectManager.LoadEffect(EffectType.Lighting);
            });

            onScreenLights = new List<(Vector2, LightProfile)>();

            currentRenderTarget = Main.screenTarget;

            On_Main.DrawInterface += On_Main_DrawInterface;
        }

        public override void Unload()
        {
            base.Unload();

            On_Main.DrawInterface -= On_Main_DrawInterface;
        }

        public override void PostSetupContent()
        {
            base.PostSetupContent();

            string output = "\n";

            for (int i = 0; i < WallID.Count; i++)
            {
                string name = WallID.Search.GetName(i);

                output += $"{name}\n";
            }

            Mod.Logger.Debug(output);
        }

        private void On_Main_DrawInterface(On_Main.orig_DrawInterface orig, Main self, GameTime gameTime)
        {
            UpdateBuffer();

            TileCaptureParameter tileCaptureParameter = TileHelper.CalculateTileCaptureParameter();
            short[] tileIDs = TileHelper.GetVisibleTiles(tileCaptureParameter);

            BufferComposer.ComposeGBuffer(deferredSpriteBatch, tileCaptureParameter, tileIDs);

            TargetCollection targetCollection = TargetCapturer.TargetCollection;
            BufferCollection bufferCollection = BufferComposer.BufferCollection;

            onScreenLights = LightCapturer.GetOnScreenLights(tileCaptureParameter, tileIDs);
            LightHelper.PackLightTextures(outputCollection.LightPositionTexture, outputCollection.LightColorTexture, onScreenLights);

            OutputRenderer.RenderLightingBuffer(deferredSpriteBatch, outputCollection, bufferCollection, targetCollection, onScreenLights.Count);

            KeyboardState keyboardState = Main.keyState;

            if (keyboardState.IsKeyDown(Keys.LeftAlt))
            {
                if (keyboardState.IsKeyDown(Keys.D1))
                {
                    Main.NewText($"Switching to lighting buffer view", Color.Green);

                    currentRenderTarget = outputCollection.LightingBuffer;
                }
                else if (keyboardState.IsKeyDown(Keys.D2))
                {
                    Main.NewText($"Switching to albedo buffer view", Color.Green);

                    currentRenderTarget = bufferCollection.AlbedoBuffer;
                }
                else if (keyboardState.IsKeyDown(Keys.D3))
                {
                    Main.NewText($"Switching to material buffer view", Color.Green);

                    currentRenderTarget = bufferCollection.MaterialBuffer;
                }
                else if (keyboardState.IsKeyDown(Keys.D4))
                {
                    Main.NewText($"Switching to depth buffer view", Color.Green);

                    currentRenderTarget = bufferCollection.DepthBuffer;
                }
                else if (keyboardState.IsKeyDown(Keys.D5))
                {
                    Main.NewText($"Switching to normal buffer view", Color.Green);

                    currentRenderTarget = bufferCollection.NormalBuffer;
                }
                else if (keyboardState.IsKeyDown(Keys.D6))
                {
                    Main.NewText($"Switching to layer buffer view", Color.Green);

                    currentRenderTarget = bufferCollection.LayerBuffer;
                }
                else if (keyboardState.IsKeyDown(Keys.D7))
                {
                    Main.NewText($"Switching to screen target view", Color.Green);

                    currentRenderTarget = Main.screenTarget;
                }
            }

            Main.NewText(onScreenLights.Count);

            deferredSpriteBatch.Begin();
            deferredSpriteBatch.Draw(currentRenderTarget, Vector2.Zero, Color.White);
            deferredSpriteBatch.End();

            // Draw UI normally
            orig(self, gameTime);
        }

        // Make sure all buffers match screen size
        private void UpdateBuffer()
        {
            if (!outputCollection.IsValid())
            {
                outputCollection.Dispose();
                outputCollection = new OutputCollection(Main.graphics.GraphicsDevice, Main.graphics.GraphicsDevice.PresentationParameters.BackBufferWidth, Main.graphics.GraphicsDevice.PresentationParameters.BackBufferHeight);
            }
        }
    }
}

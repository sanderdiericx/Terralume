using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Terralume.Engine.Buffers;
using Terralume.Engine.Buffers.Data;
using Terralume.Engine.Constants;
using Terralume.Engine.Hooks.Tracking;
using Terralume.Engine.Mapping;
using Terralume.Engine.Render;
using Terralume.Engine.Shaders;
using Terralume.Engine.Utils;
using Terraria;
using Terraria.ModLoader;

namespace Terralume.Engine.Pipeline
{
    internal class LightingProcessor : ModSystem
    {
        private OutputCollection outputCollection;
        private SpriteBatch deferredSpriteBatch;

        private List<(Vector2, LightProfile)> onScreenLights;

        private RenderTarget2D currentRenderTarget;

        public override void Load()
        {
            Main.QueueMainThreadAction(() =>
            {
                outputCollection = new OutputCollection(Main.graphics.GraphicsDevice, (int)RenderConstants.SCREEN_SIZE.X, (int)RenderConstants.SCREEN_SIZE.Y);
                deferredSpriteBatch = new SpriteBatch(Main.graphics.GraphicsDevice);

                EffectManager.LoadEffect(EffectType.Downsample);
                EffectManager.LoadEffect(EffectType.Radiance);
                EffectManager.LoadEffect(EffectType.Output);
            });

            onScreenLights = new List<(Vector2, LightProfile)>();

            currentRenderTarget = Main.screenTarget;

            On_Main.DrawInterface += On_Main_DrawInterface;
        }

        public override void Unload()
        {
            base.Unload();

            Main.QueueMainThreadAction(() =>
            {
                outputCollection.Dispose();
            });

            On_Main.DrawInterface -= On_Main_DrawInterface;
        }

        private void On_Main_DrawInterface(On_Main.orig_DrawInterface orig, Main self, GameTime gameTime)
        {
            UpdateBuffer();

            AmbientLightTracker.GetAmbientLightProfile();

            LayerParameter layerParameter = RenderConstants.LAYER_PARAMETER;

            TileCaptureParameter tileCaptureParameter = TileHelper.CalculateTileCaptureParameter();

            TargetCollection targetCollection = TargetCapturer.GetTargetCollection();
            BufferCollection bufferCollection = BufferComposer.GetBufferCollection(deferredSpriteBatch, tileCaptureParameter);

            onScreenLights = LightTracker.GetOnScreenLights(tileCaptureParameter);
            LightPacker.PackLightTextures(onScreenLights, RenderConstants.SCREEN_SIZE);

            OutputRenderer.RenderRadianceAndBloomBuffer(deferredSpriteBatch, outputCollection, bufferCollection, layerParameter, tileCaptureParameter);
            OutputRenderer.RenderBlurredBloomBuffer(deferredSpriteBatch, outputCollection, 3);
            OutputRenderer.RenderReflectionBuffer(deferredSpriteBatch, outputCollection, bufferCollection, targetCollection);

            OutputRenderer.RenderOutputBuffer(deferredSpriteBatch, outputCollection, bufferCollection, targetCollection);

            // START DEBUG
            KeyboardState keyboardState = Main.keyState;
            if (keyboardState.IsKeyDown(Keys.LeftAlt))
            {
                if (keyboardState.IsKeyDown(Keys.D1))
                {
                    Main.NewText($"Switching to output buffer view", Color.Green);

                    currentRenderTarget = outputCollection.OutputBuffer;
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
                    Main.NewText($"Switching to radiance buffer view", Color.Green);

                    currentRenderTarget = outputCollection.RadianceBuffer;
                }
                else if (keyboardState.IsKeyDown(Keys.D8))
                {
                    Main.NewText($"Switching to sun occlusion buffer view", Color.Green);

                    currentRenderTarget = bufferCollection.SunOcclusionBuffer;
                }
                else if (keyboardState.IsKeyDown(Keys.D0))
                {
                    Main.NewText($"Switching to player ao buffer view", Color.Green);

                    currentRenderTarget = bufferCollection.PlayerAmbientOcclusionBuffer;
                }
            }

            // END DEBUG

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
                outputCollection = new OutputCollection(Main.graphics.GraphicsDevice, (int)RenderConstants.SCREEN_SIZE.X, (int)RenderConstants.SCREEN_SIZE.Y);
            }
        }
    }
}

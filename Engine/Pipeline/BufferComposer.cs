using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terralume.Engine.Buffers;
using Terralume.Engine.Buffers.Data;
using Terralume.Engine.Materials;
using Terralume.Engine.Render;
using Terralume.Engine.Shaders;
using Terralume.Engine.Utils;
using Terraria;
using Terraria.ModLoader;

namespace Terralume.Engine.Pipeline
{
    public class BufferComposer : ModSystem
    {
        private BufferCollection bufferCollection;
        private SpriteBatch deferredSpriteBatch;

        private readonly LayerParameter layerParameter = new LayerParameter(0.0f, 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f);
        private readonly DepthParameter depthParameter = new DepthParameter(0.9f, 0.3f, 0.5f, 0.7f, 0.5f, 0.5f, 0.5f, 0.5f);

        // Debug
        private RenderTarget2D currentRenderTarget;
        private int tick;

        public override void Load()
        {
            Main.QueueMainThreadAction(() =>
            {
                bufferCollection = new BufferCollection(Main.graphics.GraphicsDevice, Main.graphics.GraphicsDevice.PresentationParameters.BackBufferWidth, Main.graphics.GraphicsDevice.PresentationParameters.BackBufferHeight);
                deferredSpriteBatch = new SpriteBatch(Main.graphics.GraphicsDevice);

                EffectManager.LoadEffect(EffectType.Layer);
                EffectManager.LoadEffect(EffectType.Depth);
                EffectManager.LoadEffect(EffectType.Material);
                EffectManager.LoadEffect(EffectType.Normal);
            });

            On_Main.DrawInterface += On_Main_DrawInterface;

            // Debug
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

            On_Main.DrawInterface -= On_Main_DrawInterface;
        }

        // TO DO: SAMPLE Main.screenTarget here to build normal and material buffer.
        private void On_Main_DrawInterface(On_Main.orig_DrawInterface orig, Main self, GameTime gameTime)
        {
            tick++;
            UpdateBuffer();

            TargetCapturer.CaptureInterfaceTargets();
            TargetCollection targetCollection = TargetCapturer.TargetCollection;

            short[] tileIDs = TileHelper.GetVisibleTiles();

            TileHelper.PackTileIDTexture(bufferCollection.TileIDTexture, tileIDs);
            TileHelper.PackMaterialTexture(bufferCollection.MaterialTexture, MaterialMapper.MappedTiles);

            BufferRenderer.RenderLayerBuffer(deferredSpriteBatch, bufferCollection, targetCollection, layerParameter);

            BufferRenderer.RenderDepthBuffer(deferredSpriteBatch, bufferCollection, layerParameter, depthParameter);
            BufferRenderer.RenderMaterialBuffer(deferredSpriteBatch, bufferCollection, layerParameter);
            BufferRenderer.RenderNormalBuffer(deferredSpriteBatch, bufferCollection);

            BufferRenderer.RenderAlbedoBuffer(deferredSpriteBatch, bufferCollection, targetCollection);

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
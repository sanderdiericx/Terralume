using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
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

        private DepthParameter depthParameter;

        // Debug
        private RenderTarget2D currentRenderTarget;
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

            depthParameter = new DepthParameter(0.9f, 0.3f, 0.5f, 0.7f, 0.5f, 0.5f, 0.5f, 0.5f);


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

            BufferRenderer.RenderDepthBuffer(deferredSpriteBatch, bufferCollection, targetCollection, Main.screenTarget, depthParameter);
            BufferRenderer.RenderMaterialBuffer(deferredSpriteBatch, bufferCollection, targetCollection, Main.screenTarget);

            BufferRenderer.RenderAlbedoBuffer(deferredSpriteBatch, bufferCollection, targetCollection);

            /*
             *  START DEBUG
             */

            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.H))
            {
                Main.NewText("[DEBUG] Saving all debug targets");

                string debugDir = "C:\\Users\\sande\\Documents\\Terralume_Debug\\";
                Directory.CreateDirectory(debugDir);

                void SaveTarget(RenderTarget2D target, string name)
                {
                    string path = Path.Combine(debugDir, $"{name}.png");
                    using FileStream fs = new FileStream(path, FileMode.Create);
                    target.SaveAsPng(fs, target.Width, target.Height);
                }

                SaveTarget(Main.screenTarget, "screen_target_start");
                SaveTarget(bufferCollection.AlbedoBuffer, "albedo_buffer");
                SaveTarget(bufferCollection.DepthBuffer, "depth_buffer");
                SaveTarget(bufferCollection.MaterialBuffer, "material_buffer");
                SaveTarget(targetCollection.NonSolidTileTarget, "non_solid_tile_target");
                SaveTarget(targetCollection.StableTileTarget, "tile_target");
                SaveTarget(targetCollection.StableWallTarget, "wall_target");
                SaveTarget(targetCollection.PlayerTarget, "player_target");
                SaveTarget(targetCollection.EntityTarget, "entity_target");
                SaveTarget(targetCollection.ProjectileTarget, "projectile_target");
            }

            if (tick > 5)
            {
                tick = 0;

                if (keyboardState.IsKeyDown(Keys.NumPad0))
                {
                    Main.NewText("[DEBUG] Switching to screen target view", Color.Green);

                    currentRenderTarget = Main.screenTarget;
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
                    Main.NewText("[DEBUG] Switching to non solid tile target view", Color.Green);

                    currentRenderTarget = targetCollection.NonSolidTileTarget;
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
using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terralume.Engine.Buffers;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Terralume.RenderHooks
{
    public class DeferredRendererSystem : ModSystem
    {
        private BufferManager bufferManager;
        private SpriteBatch deferredSpriteBatch;

        private int tick;

        public override void Load()
        {
            Main.QueueMainThreadAction(() =>
            {
                bufferManager = new BufferManager(Main.graphics.GraphicsDevice, Main.graphics.GraphicsDevice.PresentationParameters.BackBufferWidth, Main.graphics.GraphicsDevice.PresentationParameters.BackBufferHeight);
                deferredSpriteBatch = new SpriteBatch(Main.graphics.GraphicsDevice);
            });

            On_Main.DrawWalls += StartHook;
            On_Main.DrawInterface += EndHook;

            tick = 0;
        }

        public override void Unload()
        {
            base.Unload();

            On_Main.DrawWalls -= StartHook;
            On_Main.DrawInterface -= EndHook;
        }

        private void StartHook(On_Main.orig_DrawWalls orig, Main self)
        {
            UpdateBuffer();

            RenderTargetBinding[] oldTargets = Main.graphics.GraphicsDevice.GetRenderTargets();
            Viewport oldViewport = Main.graphics.GraphicsDevice.Viewport;

            RenderTarget2D backgroundBuffer = bufferManager.BackgroundBuffer;

            // Copy the current screen target to our background buffer
            Main.graphics.GraphicsDevice.SetRenderTarget(backgroundBuffer);
            deferredSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            deferredSpriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            deferredSpriteBatch.End();

            Main.graphics.GraphicsDevice.SetRenderTargets(oldTargets);
            Main.graphics.GraphicsDevice.Viewport = oldViewport;

            tick++;

            orig(self);
        }

        private void EndHook(On_Main.orig_DrawInterface orig, Main self, GameTime gameTime)
        {
            RenderTargetBinding[] oldTargets = Main.graphics.GraphicsDevice.GetRenderTargets();
            Viewport oldViewport = Main.graphics.GraphicsDevice.Viewport;

            RenderTarget2D albedoBuffer = bufferManager.AlbedoBuffer;

            // Copy the current screen target to our albedo buffer
            Main.graphics.GraphicsDevice.SetRenderTarget(albedoBuffer);
            deferredSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            deferredSpriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            deferredSpriteBatch.End();

            // Temporary debug
            if (tick > 600)
            {
                tick = 0;
                ExportDebugImage(bufferManager.BackgroundBuffer, "background_buffer");
                ExportDebugImage(bufferManager.AlbedoBuffer, "albedo_buffer");
            }

            Main.graphics.GraphicsDevice.SetRenderTargets(oldTargets);
            Main.graphics.GraphicsDevice.Viewport = oldViewport;

            // Draw UI normally
            orig(self, gameTime);
        }

        // Make sure all buffers match screen size
        private void UpdateBuffer()
        {
            if (bufferManager == null || bufferManager.Width != Main.graphics.PreferredBackBufferWidth || bufferManager.Height != Main.graphics.PreferredBackBufferHeight)
            {
                ((IDisposable)bufferManager).Dispose();
                bufferManager = new BufferManager(Main.graphics.GraphicsDevice, Main.graphics.GraphicsDevice.PresentationParameters.BackBufferWidth, Main.graphics.GraphicsDevice.PresentationParameters.BackBufferHeight);
            }
        }

        // VERY temporary debug
        private void ExportDebugImage(Texture2D texture, string fileName)
        {
            // Ensure the path exists in your Terraria save folder
            string path = Path.Combine(@"C:\Users\marij\Documents\Terralume_Debug", fileName + ".png");

            using (FileStream stream = File.OpenWrite(path))
            {
                texture.SaveAsPng(stream, texture.Width, texture.Height);
            }

            Main.NewText($"G-Buffer exported to: {path}", Color.Green);
        }
    }
}
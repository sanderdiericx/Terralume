using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terralume.Engine.Buffers;
using Terraria;
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

            On_Main.DoDraw_WallsAndBlacks += ClearForAlbedoHook;
            On_Main.DrawInterface += CaptureAlbedoHook;

            tick = 0;
        }

        public override void Unload()
        {
            base.Unload();

            On_Main.DoDraw_WallsAndBlacks -= ClearForAlbedoHook;
            On_Main.DrawInterface -= CaptureAlbedoHook;
        }

        private void ClearForAlbedoHook(On_Main.orig_DoDraw_WallsAndBlacks orig, Main self)
        {
            tick++;

            UpdateBuffer();

            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            // Copy the current screen target to our background buffer
            device.SetRenderTarget(bufferManager.BackgroundBuffer);
            device.Clear(Color.Transparent);

            deferredSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            deferredSpriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            deferredSpriteBatch.End();

            // Bake backgroundTarget into BackgroundBuffer before wiping it
           

            // Clear everything drawn so far so that the albedo buffer contains a transparent background
            device.SetRenderTargets(oldTargets);
            device.Clear(Color.Transparent);

            orig(self);
        }


        private void CaptureAlbedoHook(On_Main.orig_DrawInterface orig, Main self, GameTime gameTime)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();


            // TO DO: SAMPLE Main.screenTarget here to build depth normal and material buffer.


            // Copy the current screen target to our albedo buffer
            device.SetRenderTarget(bufferManager.AlbedoBuffer);
            device.Clear(Color.Transparent);

            // Bake underground background buffer into our albedo buffer.
            Vector2 drawPos = Main.sceneBackgroundPos - Main.screenPosition;
            deferredSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            deferredSpriteBatch.Draw(Main.instance.backgroundTarget, drawPos, Color.White);
            deferredSpriteBatch.End();

            deferredSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            deferredSpriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            deferredSpriteBatch.End();

            device.SetRenderTarget(Main.instance.backgroundTarget);
            device.Clear(Color.Transparent);

            // Reset Terraria drawing state
            device.SetRenderTargets(oldTargets);


            // Temporary debug
            if (tick > 1000)
            {
                tick = 0;
                ExportDebugImage(bufferManager.BackgroundBuffer, "background_buffer");
                ExportDebugImage(bufferManager.AlbedoBuffer, "albedo_buffer");
            }


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
            string path = Path.Combine(@"C:\Users\sande\Documents\Terralume_Debug", fileName + ".png");

            using (FileStream stream = File.OpenWrite(path))
            {
                texture.SaveAsPng(stream, texture.Width, texture.Height);
            }

            Main.NewText($"G-Buffer exported to: {path}", Color.Green);
        }
    }
}
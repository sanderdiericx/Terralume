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

            On_Main.DoDraw_WallsAndBlacks += CaptureBackgroundHook;
            On_Main.DrawInterface += CaptureAlbedoHook;

            tick = 0;
        }

        public override void Unload()
        {
            base.Unload();

            On_Main.DoDraw_WallsAndBlacks -= CaptureBackgroundHook;
            On_Main.DrawInterface -= CaptureAlbedoHook;
        }

        private void CaptureBackgroundHook(On_Main.orig_DoDraw_WallsAndBlacks orig, Main self)
        {
            tick++;

            UpdateBuffer();

            bufferManager.RenderBackgroundBuffer(deferredSpriteBatch);

            orig(self);
        }


        private void CaptureAlbedoHook(On_Main.orig_DrawInterface orig, Main self, GameTime gameTime)
        {
            // TO DO: SAMPLE Main.screenTarget here to build depth normal and material buffer.


            bufferManager.RenderAlbedoBuffer(deferredSpriteBatch);

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
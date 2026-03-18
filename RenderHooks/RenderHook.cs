using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Terralume.RenderHooks
{
    public class DeferredRendererSystem : ModSystem
    {
        private RenderTarget2D albedoBuffer;

        private int tick;

        public override void Load()
        {
            // Nuke all background drawing
            On_Main.DrawBackground += (orig, self) => { return; };
            On_Main.DrawSurfaceBG += (orig, self) => { return; };
            On_Main.DrawUnderworldBackground += (orig, self, flat) => { return; };
            On_Main.DrawStarsInBackground += (orig, self, sceneArea, artificial) => { return; };
            On_Main.DrawSunAndMoon += (orig, self, sceneArea, moonColor, sunColor, tempMushroomInfluence) => { return; };

            On_Main.DrawWalls += StartHook;
            On_Main.DrawInterface += EndHook;

            tick = 0;
        }

        private void StartHook(On_Main.orig_DrawWalls orig, Main self)
        {
            UpdateBuffer();

            Main.graphics.GraphicsDevice.Clear(Color.Transparent);

            tick++;

            orig(self);
        }

        private void EndHook(On_Main.orig_DrawInterface orig, Main self, GameTime gameTime)
        {
            RenderTargetBinding[] oldTargets = Main.graphics.GraphicsDevice.GetRenderTargets();

            // Copy the current screen target to our albedo buffer
            Main.graphics.GraphicsDevice.SetRenderTarget(albedoBuffer);
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            Main.spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            Main.spriteBatch.End();

            // Temporary debug
            if (tick > 600)
            {
                tick = 0;
                ExportDebugImage(albedoBuffer, "albedo_buffer");
            }

            Main.graphics.GraphicsDevice.SetRenderTargets(oldTargets);

            // Draw UI normally
            orig(self, gameTime);
        }

        // Make sure all buffers match screen size
        private void UpdateBuffer()
        {
            if (albedoBuffer == null || albedoBuffer.Width != Main.graphics.PreferredBackBufferWidth || albedoBuffer.Height != Main.graphics.PreferredBackBufferHeight)
            {
                albedoBuffer?.Dispose();
                albedoBuffer = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.graphics.PreferredBackBufferWidth, Main.graphics.PreferredBackBufferHeight, false, SurfaceFormat.Color, DepthFormat.None);
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
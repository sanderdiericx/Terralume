using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Terralume.Engine.Buffers;
using Terralume.Engine.Buffers.Data;
using Terralume.Engine.Mapping;
using Terralume.Engine.Render;
using Terralume.Engine.Shaders;
using Terralume.Engine.Utils;
using Terraria;
using Terraria.ModLoader;

namespace Terralume.Engine.Pipeline
{
    public class LightingProcessor : ModSystem
    {
        private OutputCollection outputCollection;
        private SpriteBatch deferredSpriteBatch;

        public override void Load()
        {
            Main.QueueMainThreadAction(() =>
            {
                outputCollection = new OutputCollection(Main.graphics.GraphicsDevice, Main.graphics.GraphicsDevice.PresentationParameters.BackBufferWidth, Main.graphics.GraphicsDevice.PresentationParameters.BackBufferHeight);
                deferredSpriteBatch = new SpriteBatch(Main.graphics.GraphicsDevice);

                EffectManager.LoadEffect(EffectType.Lighting);
            });

            On_Main.DrawInterface += On_Main_DrawInterface;
        }

        public override void Unload()
        {
            base.Unload();

            On_Main.DrawInterface -= On_Main_DrawInterface;
        }

        private void On_Main_DrawInterface(On_Main.orig_DrawInterface orig, Main self, GameTime gameTime)
        {
            UpdateBuffer();

            BufferComposer.ComposeGBuffer(deferredSpriteBatch);

            TargetCollection targetCollection = TargetCapturer.TargetCollection;
            BufferCollection bufferCollection = BufferComposer.BufferCollection;

            List<(Vector2, LightProfile)> onScreenLights = LightCapturer.GetOnScreenLights(new Vector2(bufferCollection.AlbedoBuffer.Width, bufferCollection.AlbedoBuffer.Height));
            LightHelper.PackLightTextures(outputCollection.LightPositionTexture, outputCollection.LightColorTexture, onScreenLights);
            int activeLights = onScreenLights.Count;

            OutputRenderer.RenderLightingBuffer(deferredSpriteBatch, outputCollection, bufferCollection, targetCollection, activeLights);

            deferredSpriteBatch.Begin();
            deferredSpriteBatch.Draw(outputCollection.LightingBuffer, Vector2.Zero, Color.White);
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

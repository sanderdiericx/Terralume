using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terralume.Engine.Buffers;
using Terralume.Engine.Shaders;
using Terraria;

namespace Terralume.Engine.Render
{
    public static class OutputRenderer
    {
        private static readonly int MAX_LIGHTS = 32;

        public static void RenderLightingBuffer(SpriteBatch spriteBatch, OutputCollection outputCollection, BufferCollection bufferCollection, TargetCollection targetCollection, int activeLights)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            device.SetRenderTarget(outputCollection.LightingBuffer);
            device.Clear(Color.Transparent);

            Effect effect = EffectManager.GetEffect(EffectType.Lighting);

            Vector2 screenSize = new Vector2(bufferCollection.AlbedoBuffer.Width, bufferCollection.AlbedoBuffer.Height);

            effect.Parameters["ScreenSize"].SetValue(screenSize);
            effect.Parameters["DepthScale"].SetValue(100);
            effect.Parameters["CameraPos"].SetValue(new Vector3(bufferCollection.AlbedoBuffer.Width / 2f, bufferCollection.AlbedoBuffer.Height / 2f, 100f));

            effect.Parameters["ActiveLights"].SetValue(activeLights);
            effect.Parameters["LightDepth"].SetValue(90f);
            effect.Parameters["LightDataTextureSize"].SetValue(new Vector2(outputCollection.LightPositionTexture.Width, outputCollection.LightPositionTexture.Height));

            effect.Parameters["DepthBuffer"].SetValue(bufferCollection.DepthBuffer);
            effect.Parameters["NormalBuffer"].SetValue(bufferCollection.NormalBuffer);
            effect.Parameters["MaterialBuffer"].SetValue(bufferCollection.MaterialBuffer);

            effect.Parameters["LightPositionTexture"].SetValue(outputCollection.LightPositionTexture);
            effect.Parameters["LightColorTexture"].SetValue(outputCollection.LightColorTexture);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            spriteBatch.Draw(targetCollection.BackgroundTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, effect);
            spriteBatch.Draw(bufferCollection.AlbedoBuffer, Vector2.Zero, Color.White);
            spriteBatch.End();

            device.SetRenderTargets(oldTargets);
        }
    }
}

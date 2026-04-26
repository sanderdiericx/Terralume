using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terralume.Engine.Buffers;
using Terralume.Engine.Shaders;
using Terraria;

namespace Terralume.Engine.Render
{
    public static class OutputRenderer
    {
        public static void RenderLightingBuffer(SpriteBatch spriteBatch, OutputCollection outputCollection, BufferCollection bufferCollection, TargetCollection targetCollection)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            device.SetRenderTarget(outputCollection.LightingBuffer);
            device.Clear(Color.Transparent);

            Effect effect = EffectManager.GetEffect(EffectType.Lighting);

            effect.Parameters["ScreenSize"].SetValue(new Vector2(bufferCollection.AlbedoBuffer.Width, bufferCollection.AlbedoBuffer.Height));
            effect.Parameters["DepthScale"].SetValue(100);

            // Center the light and camera on the screen
            float screenCenterX = bufferCollection.AlbedoBuffer.Width / 2f;
            float screenCenterY = bufferCollection.AlbedoBuffer.Height / 2f;

            effect.Parameters["CameraPos"].SetValue(new Vector3(screenCenterX, screenCenterY, 100f));
            effect.Parameters["LightPos"].SetValue(new Vector3(screenCenterX, screenCenterY, 50f));
            effect.Parameters["LightColor"].SetValue(new Vector3(5.0f, 5.0f, 5.0f));
            effect.Parameters["LightRadius"].SetValue(0.01f);
            effect.Parameters["EmmisiveMultiplier"].SetValue(0.4f);

            effect.Parameters["DepthBuffer"].SetValue(bufferCollection.DepthBuffer);
            effect.Parameters["NormalBuffer"].SetValue(bufferCollection.NormalBuffer);
            effect.Parameters["MaterialBuffer"].SetValue(bufferCollection.MaterialBuffer);

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

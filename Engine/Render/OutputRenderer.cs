using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terralume.Engine.Buffers;
using Terralume.Engine.Buffers.Data;
using Terralume.Engine.Pipeline;
using Terralume.Engine.Shaders;
using Terraria;

namespace Terralume.Engine.Render
{
    public static class OutputRenderer
    {
        private static readonly int MAX_LIGHTS = 32;

        public static void RenderLightingBuffer(SpriteBatch spriteBatch, OutputCollection outputCollection, BufferCollection bufferCollection, TargetCollection targetCollection)
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

            List<(Vector2, LightProfile)> onScreenLights = LightCapturer.GetOnScreenLights(screenSize);

            int ActiveLights = onScreenLights.Count < MAX_LIGHTS ? onScreenLights.Count : MAX_LIGHTS;

            Vector2[] lightPositions = new Vector2[ActiveLights];
            Vector4[] lightColors = new Vector4[ActiveLights];
            float[] lightRadii = new float[ActiveLights];

            for (int i = 0; i < ActiveLights; i++)
            {
                lightPositions[i] = onScreenLights[i].Item1;
                lightColors[i] = onScreenLights[i].Item2.LightColor;
                lightRadii[i] = onScreenLights[i].Item2.LightRadius;
            }

            effect.Parameters["LightPositions"].SetValue(lightPositions);
            effect.Parameters["LightColors"].SetValue(lightColors);
            effect.Parameters["LightRadii"].SetValue(lightRadii);
            effect.Parameters["ActiveLights"].SetValue(ActiveLights);
            effect.Parameters["LightDepth"].SetValue(90f);

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

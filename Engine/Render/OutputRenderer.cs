using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Runtime.InteropServices;
using Terralume.Engine.Buffers;
using Terralume.Engine.Shaders;
using Terraria;
using Terraria.ID;

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

            Vector2 screenSize = new Vector2(bufferCollection.AlbedoBuffer.Width, bufferCollection.AlbedoBuffer.Height);

            effect.Parameters["ScreenSize"].SetValue(screenSize);
            effect.Parameters["DepthScale"].SetValue(100);
            effect.Parameters["CameraPos"].SetValue(new Vector3(bufferCollection.AlbedoBuffer.Width / 2f, bufferCollection.AlbedoBuffer.Height / 2f, 100f));

            Tuple<Vector2[], Vector4[], int> closestLights = LightCollection.GetClosestLights(6, screenSize);

            effect.Parameters["LightPositions"].SetValue(closestLights.Item1);
            effect.Parameters["LightColors"].SetValue(closestLights.Item2);
            effect.Parameters["ActiveLights"].SetValue(closestLights.Item3);

            effect.Parameters["LightRadius"].SetValue(300f);
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

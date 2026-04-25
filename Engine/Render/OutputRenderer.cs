using log4net.Filter;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;
using System.Security.Claims;
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
            
            effect.Parameters["WorldThickness"].SetValue(200);
            effect.Parameters["LightDepth"].SetValue(180);
            effect.Parameters["LightIntensity"].SetValue(5.0f);

            effect.Parameters["Constant"].SetValue(1.0f);
            effect.Parameters["Linear"].SetValue(1.5f);
            effect.Parameters["Quadratic"].SetValue(2.5f);

            effect.Parameters["Ambient"].SetValue(new Vector3(0.02f, 0.02f, 0.05f));
            effect.Parameters["Diffuse"].SetValue(new Vector3(1.0f, 0.6f, 0.3f));
            effect.Parameters["Specular"].SetValue(new Vector3(0.8f, 0.7f, 0.6f));

            effect.Parameters["DepthBuffer"].SetValue(bufferCollection.DepthBuffer);
            effect.Parameters["NormalBuffer"].SetValue(bufferCollection.NormalBuffer);

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

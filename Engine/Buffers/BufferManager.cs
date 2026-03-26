using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace Terralume.Engine.Buffers
{
    public class BufferManager : IDisposable
    {
        public RenderTarget2D BackgroundBuffer;
        public RenderTarget2D AlbedoBuffer;

        public int Width;
        public int Height;

        public BufferManager(GraphicsDevice graphicsDevice, int width, int height)
        {
            BackgroundBuffer = new RenderTarget2D(graphicsDevice, width, height);
            AlbedoBuffer = new RenderTarget2D(graphicsDevice, width, height);

            Width = width;
            Height = height;
        }


        public void RenderAlbedoBuffer(SpriteBatch deferredSpriteBatch)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            device.SetRenderTarget(AlbedoBuffer);
            device.Clear(Color.Transparent);

            // Bake underground background buffer into our albedo buffer.
            Vector2 drawPos = Main.sceneBackgroundPos - Main.screenPosition;
            deferredSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            deferredSpriteBatch.Draw(Main.instance.backgroundTarget, drawPos, Color.White);
            deferredSpriteBatch.End();

            // Copy the current screen target to our albedo buffer
            deferredSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            deferredSpriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            deferredSpriteBatch.End();

            device.SetRenderTarget(Main.instance.backgroundTarget);
            device.Clear(Color.Transparent);

            // Reset Terraria drawing state
            device.SetRenderTargets(oldTargets);
        }

        public void RenderBackgroundBuffer(SpriteBatch deferredSpriteBatch)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            // Copy the current screen target to our background buffer
            device.SetRenderTarget(BackgroundBuffer);
            device.Clear(Color.Transparent);

            deferredSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            deferredSpriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            deferredSpriteBatch.End();

            device.SetRenderTargets(oldTargets);
            device.Clear(Color.Transparent);
        }

        void IDisposable.Dispose()
        {
            BackgroundBuffer.Dispose();
            AlbedoBuffer.Dispose();
        }
    }
}

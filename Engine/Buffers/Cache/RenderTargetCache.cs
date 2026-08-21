using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Terralume.Engine.Buffers.Cache
{
    // System for handing out temporary render targets, all targets use SurfaceFormat.Vector4 by default
    internal class RenderTargetCache : ModSystem
    {
        private static List<CachedTarget> cachedRenderTargets = new List<CachedTarget>();

        public override void Load()
        {
            base.Load();
        }

        public override void Unload()
        {
            base.Unload();

            ClearCache();
        }

        // Must be called from the Main thread
        public static RenderTarget2D RequestTarget(int width, int height, SurfaceFormat surfaceFormat)
        {
            // Search for a cached target at the desired resolution
            for (int i = cachedRenderTargets.Count - 1; i >= 0; i--)
            {
                CachedTarget cachedTarget = cachedRenderTargets[i];

                if (cachedTarget.Width == width && cachedTarget.Height == height && cachedTarget.SurfaceFormat == surfaceFormat)
                {
                    cachedRenderTargets.RemoveAt(i);
                    return cachedTarget.RenderTarget;
                }
            }

            Main.NewText($"NEW TARGET; ({width} : {height})");

            // No suitable target was found, create a new one
            return new RenderTarget2D(Main.graphics.GraphicsDevice, width, height, false, surfaceFormat, DepthFormat.None);
        }


        public static void ReturnTarget(RenderTarget2D renderTarget)
        {
            cachedRenderTargets.Add(new CachedTarget(renderTarget.Width, renderTarget.Height, renderTarget.Format, renderTarget));
        }

        // Disposes all available rendertargets and clears the cached targets list
        public static void ClearCache()
        {
            Main.QueueMainThreadAction(() =>
            {
                foreach (CachedTarget cachedTarget in cachedRenderTargets)
                {
                    cachedTarget.RenderTarget.Dispose();
                }

                cachedRenderTargets.Clear();
            });
        }
    }
}

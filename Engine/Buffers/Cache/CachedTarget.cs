using Microsoft.Xna.Framework.Graphics;

namespace Terralume.Engine.Buffers.Cache
{
    internal class CachedTarget
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        public SurfaceFormat SurfaceFormat { get; private set; }
        public RenderTarget2D RenderTarget { get; private set; }

        public CachedTarget(int width, int height, SurfaceFormat surfaceFormat, RenderTarget2D renderTarget)
        {
            Width = width;
            Height = height;

            SurfaceFormat = surfaceFormat;

            RenderTarget = renderTarget;
        }
    }
}

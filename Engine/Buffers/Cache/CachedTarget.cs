using Microsoft.Xna.Framework.Graphics;

namespace Terralume.Engine.Buffers.Cache
{
    public class CachedTarget
    {
        public int Width;
        public int Height;

        public RenderTarget2D RenderTarget;

        public CachedTarget(int width, int height, RenderTarget2D renderTarget)
        {
            Width = width;
            Height = height;

            RenderTarget = renderTarget;
        }
    }
}

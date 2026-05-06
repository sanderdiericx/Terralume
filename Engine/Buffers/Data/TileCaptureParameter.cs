using Microsoft.Xna.Framework;

namespace Terralume.Engine.Buffers.Data
{
    public struct TileCaptureParameter
    {
        public Vector2 ScreenPosition;
        public Vector2 ViewSize;
        public Vector2 TileStride;
        public Vector2 MinTile;
        public Vector2 MaxTile;

        public TileCaptureParameter(Vector2 screenPosition, Vector2 viewSize, Vector2 stride, Vector2 minTile, Vector2 maxTile)
        {
            ScreenPosition = screenPosition;
            ViewSize = viewSize;
            TileStride = stride;
            MinTile = minTile;
            MaxTile = maxTile;
        }
    }
}

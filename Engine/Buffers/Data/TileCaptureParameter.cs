using Microsoft.Xna.Framework;

namespace Terralume.Engine.Buffers.Data
{
    internal struct TileCaptureParameter
    {
        public Vector2 ScreenPosition { get; private set; }
        public Vector2 ViewSize { get; private set; }
        public Vector2 TileStride { get; private set; }
        public Vector2 MinTile { get; private set; }
        public Vector2 MaxTile { get; private set; }

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

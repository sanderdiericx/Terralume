using Microsoft.Xna.Framework;
using System;
using Terralume.Engine.Buffers.Data;
using Terralume.Engine.Constants;
using Terraria;

namespace Terralume.Engine.Utils
{
    public static class TileHelper
    {
        public static TileCaptureParameter CalculateTileCaptureParameter()
        {
            Vector2 screenSize = RenderConstants.SCREEN_SIZE;

            Vector2 viewSize = screenSize;
            if (Main.GameViewMatrix != null)
            {
                viewSize /= Main.GameViewMatrix.Zoom;
            }

            Vector2 screenPosition = Main.screenPosition + (screenSize * 0.5f) - (viewSize * 0.5f);

            int strideX = (int)Math.Ceiling(viewSize.X / 16.0) + 2;
            int strideY = (int)Math.Ceiling(viewSize.Y / 16.0) + 2;

            int minTileX = (int)Math.Floor(screenPosition.X / 16.0);
            int minTileY = (int)Math.Floor(screenPosition.Y / 16.0);

            int maxTileX = minTileX + strideX - 1;
            int maxTileY = minTileY + strideY - 1;

            return new TileCaptureParameter(screenPosition, viewSize, new Vector2(strideX, strideY), new Vector2(minTileX, minTileY), new Vector2(maxTileX, maxTileY));
        }
    }
}
using Microsoft.Xna.Framework;
using System;
using Terralume.Engine.Buffers.Data;
using Terralume.Engine.Constants;
using Terraria;

namespace Terralume.Engine.Utils
{
    internal static class TileHelper
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


        // Copies the Main.tile tilemap into a 2D array, only copies values within a specified bounding box
        public static Tile[,] CopyTileMap(Vector2 startPos, Vector2 endPos)
        {
            // Ensure start and end positions are within bounds
            int startX = (int)Math.Clamp(startPos.X, 0, Main.tile.Width);
            int startY = (int)Math.Clamp(startPos.Y, 0, Main.tile.Height);

            int endX = (int)Math.Clamp(endPos.X, 0, Main.tile.Width);
            int endY = (int)Math.Clamp(endPos.Y, 0, Main.tile.Height);

            Tile[,] tiles = new Tile[endX - startX, endY - startY];

            // Copy Main.tile values into tiles
            for (int x = startX; x < endX; x++)
            {
                for (int y = startY; y < endY; y++)
                {
                    tiles[x - startX, y - startY] = Main.tile[x, y];
                }
            }

            return tiles;
        }
    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System;
using System.Collections.Generic;
using Terralume.Engine.Buffers;
using Terralume.Engine.Buffers.Data;
using Terralume.Engine.Constants;

namespace Terralume.Engine.Utils
{
    public static class LightHelper
    {
        private static HalfVector4[] lightPositionPixels = new HalfVector4[RenderConstants.LIGHT_DATA_RESOLUTION * RenderConstants.LIGHT_DATA_RESOLUTION];
        private static Color[] lightColorPixels = new Color[RenderConstants.LIGHT_DATA_RESOLUTION * RenderConstants.LIGHT_DATA_RESOLUTION];

        private static Vector4[] gridPixels;
        private static Vector4[] indexPixels;

        private static List<int>[] tileBuckets;

        public static void PackLightTextures(OutputCollection outputCollection, List<(Vector2, LightProfile)> lightProfiles, Vector2 screenSize)
        {
            Array.Clear(lightPositionPixels, 0, lightPositionPixels.Length);
            Array.Clear(lightColorPixels, 0, lightColorPixels.Length);

            if (gridPixels != null)
            {
                Array.Clear(gridPixels, 0, gridPixels.Length);
            }

            if (indexPixels != null)
            {
                Array.Clear(indexPixels, 0, indexPixels.Length);
            }

            int lightCount = Math.Min(lightProfiles.Count, RenderConstants.LIGHT_DATA_RESOLUTION * RenderConstants.LIGHT_DATA_RESOLUTION);

            // Pack position, range, intensity, color data
            for (int i = 0; i < lightCount; i++)
            {
                Vector2 position = lightProfiles[i].Item1;
                Vector4 color = lightProfiles[i].Item2.Color;

                lightPositionPixels[i] = new HalfVector4(position.X, position.Y, lightProfiles[i].Item2.Range, lightProfiles[i].Item2.Intensity);
                lightColorPixels[i] = new Color(color.X, color.Y, color.Z, color.W);
            }

            outputCollection.LightPositionTexture.SetData(lightPositionPixels);
            outputCollection.LightColorTexture.SetData(lightColorPixels);

            // Initialize grid data and index arrays based on screen size
            int gridWidth = (int)Math.Ceiling(screenSize.X / RenderConstants.LIGHT_GRID_TILE_SIZE);
            int gridHeight = (int)Math.Ceiling(screenSize.Y / RenderConstants.LIGHT_GRID_TILE_SIZE);
            int totalTiles = gridWidth * gridHeight;

            if (indexPixels == null || indexPixels.Length != outputCollection.LightIndexTexture.Width * outputCollection.LightIndexTexture.Height)
            {
                indexPixels = new Vector4[outputCollection.LightIndexTexture.Width * outputCollection.LightIndexTexture.Height];
            }

            if (gridPixels == null || gridPixels.Length != totalTiles)
            {
                gridPixels = new Vector4[totalTiles];
                tileBuckets = new List<int>[totalTiles];

                for (int i = 0; i < totalTiles; i++)
                {
                    tileBuckets[i] = new List<int>(16);
                }
            }

            // Clear old frame buckets
            for (int i = 0; i < totalTiles; i++)
            {
                tileBuckets[i].Clear();
            }

            // Bin lights into screen tiles
            for (int i = 0; i < lightCount; i++)
            {
                // Note: Must be already converted to screen space
                Vector2 screenPos = lightProfiles[i].Item1;
                float range = lightProfiles[i].Item2.Range;

                int minX = Math.Max(0, (int)((screenPos.X - range) / RenderConstants.LIGHT_GRID_TILE_SIZE));
                int maxX = Math.Min(gridWidth - 1, (int)((screenPos.X + range) / RenderConstants.LIGHT_GRID_TILE_SIZE));
                int minY = Math.Max(0, (int)((screenPos.Y - range) / RenderConstants.LIGHT_GRID_TILE_SIZE));
                int maxY = Math.Min(gridHeight - 1, (int)((screenPos.Y + range) / RenderConstants.LIGHT_GRID_TILE_SIZE));

                for (int y = minY; y <= maxY; y++)
                {
                    for (int x = minX; x <= maxX; x++)
                    {
                        tileBuckets[y * gridWidth + x].Add(i);
                    }
                }
            }

            int currentIndex = 0;
            for (int i = 0; i < totalTiles; i++)
            {
                int startIndexForTile = currentIndex;
                int writtenCount = 0;

                foreach (int lightIndex in tileBuckets[i])
                {
                    // Currently only the first lights registered in GetOnScreenLights take priority, (Ordering system?)
                    if (writtenCount >= RenderConstants.MAX_LIGHTS_PER_TILE)
                    {
                        break;
                    }

                    if (currentIndex < indexPixels.Length)
                    {
                        indexPixels[currentIndex] = new Vector4(lightIndex, 0, 0, 0);
                        currentIndex++;
                        writtenCount++;
                    }
                }

                gridPixels[i] = new Vector4(startIndexForTile, writtenCount, 0, 0);
            }

            outputCollection.LightGridTexture.SetData(gridPixels);
            outputCollection.LightIndexTexture.SetData(indexPixels);
        }
    }
}
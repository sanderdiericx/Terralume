using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System;
using System.Collections.Generic;
using Terralume.Engine.Buffers.Data;
using Terralume.Engine.Constants;
using Terraria;

namespace Terralume.Engine.Utils
{
    public static class LightHelper
    {
        private static HalfVector4[] lightPositionPixels = new HalfVector4[RenderConstants.LIGHT_DATA_RESOLUTION * RenderConstants.LIGHT_DATA_RESOLUTION];
        private static Color[] lightColorPixels = new Color[RenderConstants.LIGHT_DATA_RESOLUTION * RenderConstants.LIGHT_DATA_RESOLUTION];

        private static Vector4[] gridPixels;
        private static Vector4[] indexPixels = new Vector4[RenderConstants.LIGHT_INDEX_RESOLUTION * RenderConstants.LIGHT_INDEX_RESOLUTION];

        private static List<int>[] tileBuckets;

        public static void PackLightTextures(Texture2D lightPositionTexture, Texture2D lightColorTexture, Texture2D lightGridTexture, Texture2D lightIndexTexture, List<(Vector2, LightProfile)> lightProfiles, Vector2 screenSize)
        {
            int lightCount = Math.Min(lightProfiles.Count, RenderConstants.LIGHT_DATA_RESOLUTION * RenderConstants.LIGHT_DATA_RESOLUTION);

            // Pack position, range, intensity, color data
            for (int i = 0; i < lightCount; i++)
            {
                Vector2 position = lightProfiles[i].Item1;
                Vector4 color = lightProfiles[i].Item2.Color;

                lightPositionPixels[i] = new HalfVector4(position.X, position.Y, lightProfiles[i].Item2.Range, lightProfiles[i].Item2.Intensity);
                lightColorPixels[i] = new Color(color.X, color.Y, color.Z, color.W);
            }

            lightPositionTexture.SetData(lightPositionPixels);
            lightColorTexture.SetData(lightColorPixels);

            // Initialize grid gata based on screen size
            int gridWidth = (int)Math.Ceiling(screenSize.X / RenderConstants.LIGHT_GRID_TILE_SIZE);
            int gridHeight = (int)Math.Ceiling(screenSize.Y / RenderConstants.LIGHT_GRID_TILE_SIZE);
            int totalTiles = gridWidth * gridHeight;

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
                float intensity = lightProfiles[i].Item2.Intensity;

                float minVisibleThreshold = 0.02f;
                float distanceMultiplier = (float)Math.Sqrt(Math.Max(0, (intensity / minVisibleThreshold) - 1.0));

                float zDifference = 85f * Main.GameViewMatrix.Zoom.X;
                float true3DCullRadius = range * distanceMultiplier;

                float cullRadius2D = 0;
                if (true3DCullRadius > zDifference)
                {
                    cullRadius2D = (float)Math.Sqrt((true3DCullRadius * true3DCullRadius) - (zDifference * zDifference));
                }

                int minX = Math.Max(0, (int)((screenPos.X - cullRadius2D) / RenderConstants.LIGHT_GRID_TILE_SIZE));
                int maxX = Math.Min(gridWidth - 1, (int)((screenPos.X + cullRadius2D) / RenderConstants.LIGHT_GRID_TILE_SIZE));
                int minY = Math.Max(0, (int)((screenPos.Y - cullRadius2D) / RenderConstants.LIGHT_GRID_TILE_SIZE));
                int maxY = Math.Min(gridHeight - 1, (int)((screenPos.Y + cullRadius2D) / RenderConstants.LIGHT_GRID_TILE_SIZE));

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
                int count = tileBuckets[i].Count;

                gridPixels[i] = new Vector4(currentIndex, count, 0, 0);

                foreach (int lightIndex in tileBuckets[i])
                {
                    if (currentIndex < RenderConstants.LIGHT_INDEX_RESOLUTION * RenderConstants.LIGHT_INDEX_RESOLUTION)
                    {
                        indexPixels[currentIndex] = new Vector4(lightIndex, 0, 0, 0);
                        currentIndex++;
                    }
                }
            }

            lightGridTexture.SetData(gridPixels);
            lightIndexTexture.SetData(indexPixels);
        }
    }
}
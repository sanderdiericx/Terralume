using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terralume.Engine.Buffers.Data;
using Terralume.Engine.Constants;
using Terraria;
using Terraria.ModLoader;

namespace Terralume.Engine.Hooks.Tracking
{
    internal class SunOcclusionTracker : ModSystem
    {
        public static HashSet<int> DirtyColumns { get; private set; } = new HashSet<int>();
        public static Texture2D SunOcclusionTexture { get; private set; }

        // An array with (world width) elements, each index represents a column and stores the position of the highest occluder in tile-space.
        private static int[] occluderHeights;
        private static float[] occlusionPixels = new float[RenderConstants.SUN_OCCLUSION_DATA_RESOLUTION];

        public override void Load()
        {
            base.Load();

            GraphicsDevice device = Main.graphics.GraphicsDevice;

            Main.QueueMainThreadAction(() =>
            {
                SunOcclusionTexture = new Texture2D(device, RenderConstants.SUN_OCCLUSION_DATA_RESOLUTION, 1, false, SurfaceFormat.Single);
            });
        }

        public override void Unload()
        {
            base.Unload();

            Main.QueueMainThreadAction(() =>
            {
                SunOcclusionTexture.Dispose();
            });
        }

        public override void PostWorldLoad()
        {
            base.PostWorldLoad();

            occluderHeights = new int[Main.tile.Width];

            for (int column = 0; column < Main.tile.Width; column++)
            {
                occluderHeights[column] = ResolveColumn(column);
            }
        }

        public static void PackSunOcclusionTexture(TileCaptureParameter tileCaptureParameter)
        {
            int maxPixels = RenderConstants.SUN_OCCLUSION_DATA_RESOLUTION;

            Array.Clear(occlusionPixels, 0, maxPixels);

            int count = 0;

            for (int i = (int)tileCaptureParameter.MinTile.X; i <= tileCaptureParameter.MaxTile.X; i++)
            {
                if (i < 0 || i >= Main.maxTilesX) // Out of bounds
                {
                    occlusionPixels[count] = -1f;
                }
                else
                {
                    int height = occluderHeights[i];
                    occlusionPixels[count] = height;
                }

                count++;
            }

            SunOcclusionTexture.SetData(occlusionPixels);
        }

        public static void UpdateOccluders()
        {
            foreach (int column in DirtyColumns)
            {
                occluderHeights[column] = ResolveColumn(column);
            }

            DirtyColumns.Clear();
        }

        private static int ResolveColumn(int column)
        {
            for (int row = 0; row < Main.tile.Height; row++)
            {
                Tile tile = Main.tile[column, row];

                if (tile.HasTile && Main.tileSolid[tile.TileType])
                {
                    return row;
                }
            }

            return Main.tile.Height;
        }
    }
}

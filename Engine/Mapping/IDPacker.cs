using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terralume.Engine.Buffers.Data;
using Terralume.Engine.Constants;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace Terralume.Engine.Mapping
{
    public class IDPacker : ModSystem
    {
        public static Texture2D TileIDTexture;
        public static Texture2D WallIDTexture;

        private static ushort[] tileIDs = new ushort[8];
        private static ushort[] wallIDs = new ushort[8];

        private static Color[] shortPixels = new Color[RenderConstants.MATERIAL_DATA_RESOLUTION * RenderConstants.MATERIAL_DATA_RESOLUTION];

        public override void Load()
        {
            base.Load();

            GraphicsDevice device = Main.graphics.GraphicsDevice;

            Main.QueueMainThreadAction(() =>
            {
                TileIDTexture = new Texture2D(device, RenderConstants.MATERIAL_DATA_RESOLUTION, RenderConstants.MATERIAL_DATA_RESOLUTION);
                WallIDTexture = new Texture2D(device, RenderConstants.MATERIAL_DATA_RESOLUTION, RenderConstants.MATERIAL_DATA_RESOLUTION);
            });
        }

        public override void Unload()
        {
            base.Unload();

            Main.QueueMainThreadAction(() =>
            {
                TileIDTexture.Dispose();
                WallIDTexture.Dispose();
            });
        }

        public static void UpdateStoredIDs(TileCaptureParameter tileCaptureParameter)
        {
            GetVisibleIDs(tileCaptureParameter, out tileIDs, out wallIDs);
        }

        public static void PackTileIDTexture()
        {
            PackShortsIntoTexture(TileIDTexture, tileIDs);
        }

        public static void PackWallIDTexture()
        {
            PackShortsIntoTexture(WallIDTexture, wallIDs);
        }

        // Encodes an array of shorts into a 2D texture
        //
        // Note: Each texture index stores two shorts
        private static void PackShortsIntoTexture(Texture2D texture, ushort[] shorts)
        {
            int pixelCount = (shorts.Length + 1) / 2;

            int safePixelCount = Math.Min(pixelCount, RenderConstants.MATERIAL_DATA_RESOLUTION * RenderConstants.MATERIAL_DATA_RESOLUTION);

            for (int i = 0; i < safePixelCount; i++)
            {
                int index = i * 2;
                ushort u0 = shorts[index];
                ushort u1 = (index + 1 < shorts.Length) ? shorts[index + 1] : (ushort)65535;

                shortPixels[i] = new Color(
                    (byte)(u0 & 0xFF), // low byte of u0
                    (byte)(u0 >> 8), // high byte of u0
                    (byte)(u1 & 0xFF), // low byte of u1
                    (byte)(u1 >> 8) // high byte of u1
                );
            }

            texture.SetData(shortPixels);
        }

        // Builds a flat array with tile ids of all visible tiles, and a second flat array with ids of all visible wall tiles
        //
        // Note: Indices without tiles will recieve tile ID 65535 by default, this method supports max 65534 different tiles/walls
        private static void GetVisibleIDs(TileCaptureParameter tileCaptureParameter, out ushort[] tileIDs, out ushort[] wallIDs)
        {
            Vector2 stride = tileCaptureParameter.TileStride;
            Vector2 minTile = tileCaptureParameter.MinTile;
            Vector2 maxTile = tileCaptureParameter.MaxTile;

            tileIDs = new ushort[(int)stride.X * (int)stride.Y];
            wallIDs = new ushort[(int)stride.X * (int)stride.Y];

            int count = 0;

            for (int i = (int)minTile.Y; i <= maxTile.Y; i++)
            {
                for (int j = (int)minTile.X; j <= maxTile.X; j++)
                {
                    if (j < 0 || j >= Main.maxTilesX || i < 0 || i >= Main.maxTilesY) // Out of bounds;
                    {
                        tileIDs[count] = 65535;
                        wallIDs[count] = 65535;
                    }
                    else
                    {
                        tileIDs[count] = (Main.tile[j, i].HasTile ? Main.tile[j, i].TileType : (ushort)65535);
                        wallIDs[count] = (Main.tile[j, i].WallType != WallID.None ? Main.tile[j, i].WallType : (ushort)65535);
                    }

                    count++;
                }
            }
        }
    }
}

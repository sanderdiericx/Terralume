using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terralume.Engine.Constants;
using Terraria;
using Terraria.ModLoader;

namespace Terralume.Engine.Mapping
{
    internal class ReflectionPacker : ModSystem
    {
        public static bool TilesValid { get; private set; } = false;
        private static Dictionary<int, Vector3> tiles = new Dictionary<int, Vector3>();

        public static bool WallsValid { get; private set; } = false;
        private static Dictionary<int, Vector3> walls = new Dictionary<int, Vector3>();

        public static bool LiquidsValid { get; private set; } = false;
        private static Dictionary<int, Vector3> liquids = new Dictionary<int, Vector3>();

        public static Texture2D TileReflectionTexture { get; private set; }
        public static Texture2D WallReflectionTexture { get; private set; }
        public static Texture2D LiquidReflectionTexture { get; private set; }

        private static Color[] reflectionPixels = new Color[RenderConstants.MATERIAL_DATA_RESOLUTION * RenderConstants.MATERIAL_DATA_RESOLUTION];

        public override void Load()
        {
            base.Load();

            GraphicsDevice device = Main.graphics.GraphicsDevice;

            Main.QueueMainThreadAction(() =>
            {
                TileReflectionTexture = new Texture2D(device, RenderConstants.MATERIAL_DATA_RESOLUTION, RenderConstants.MATERIAL_DATA_RESOLUTION);
                WallReflectionTexture = new Texture2D(device, RenderConstants.MATERIAL_DATA_RESOLUTION, RenderConstants.MATERIAL_DATA_RESOLUTION);
                LiquidReflectionTexture = new Texture2D(device, RenderConstants.MATERIAL_DATA_RESOLUTION, RenderConstants.MATERIAL_DATA_RESOLUTION);
            });
        }

        public override void Unload()
        {
            base.Unload();

            Main.QueueMainThreadAction(() =>
            {
                TileReflectionTexture.Dispose();
                WallReflectionTexture.Dispose();
                LiquidReflectionTexture.Dispose();
            });
        }

        public static void PackTileReflectionTexture()
        {
            PackReflectionTexture(TileReflectionTexture, tiles);

            TilesValid = true;
        }

        public static void PackWallReflectionTexture()
        {
            PackReflectionTexture(WallReflectionTexture, walls);

            WallsValid = true;
        }

        public static void PackLiquidReflectionTexture()
        {
            PackReflectionTexture(LiquidReflectionTexture, liquids);

            LiquidsValid = true;
        }

        public static void SetTileReflections(Dictionary<int, Vector3> mappedTiles)
        {
            tiles = new Dictionary<int, Vector3>(mappedTiles);

            TilesValid = false;
        }

        public static void SetWallReflections(Dictionary<int, Vector3> mappedWalls)
        {
            walls = new Dictionary<int, Vector3>(mappedWalls);

            WallsValid = false;
        }

        public static void SetLiquidReflections(Dictionary<int, Vector3> mappedLiquids)
        {
            liquids = new Dictionary<int, Vector3>(mappedLiquids);

            LiquidsValid = false;
        }

        // Encodes a dictionary (mapping ID to material vector) into a texture, the index of a pixel inside the texture refers to its ID
        private static void PackReflectionTexture(Texture2D texture, Dictionary<int, Vector3> mappedMaterials)
        {
            int maxPixels = RenderConstants.MATERIAL_DATA_RESOLUTION * RenderConstants.MATERIAL_DATA_RESOLUTION;

            Array.Clear(reflectionPixels, 0, reflectionPixels.Length);

            foreach (var kvp in mappedMaterials)
            {
                int id = kvp.Key;

                if (id >= 0 && id < maxPixels)
                {
                    Vector3 color = kvp.Value;
                    reflectionPixels[id] = new Color(color.X, color.Y, color.Z);
                }
            }

            texture.SetData(reflectionPixels);
        }
    }
}

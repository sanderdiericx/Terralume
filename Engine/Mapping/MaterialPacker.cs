using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terralume.Engine.Constants;
using Terraria;
using Terraria.ModLoader;

namespace Terralume.Engine.Mapping
{
    public class MaterialPacker : ModSystem
    {
        public static bool TilesValid = false;
        private static Dictionary<int, Vector3> tiles = new Dictionary<int, Vector3>();

        public static bool WallsValid = false;
        private static Dictionary<int, Vector3> walls = new Dictionary<int, Vector3>();

        public static Texture2D TileMaterialTexture;
        public static Texture2D WallMaterialTexture;

        private static Color[] materialPixels = new Color[RenderConstants.MATERIAL_DATA_RESOLUTION * RenderConstants.MATERIAL_DATA_RESOLUTION];

        public override void Load()
        {
            base.Load();

            GraphicsDevice device = Main.graphics.GraphicsDevice;

            Main.QueueMainThreadAction(() =>
            {
                TileMaterialTexture = new Texture2D(device, RenderConstants.MATERIAL_DATA_RESOLUTION, RenderConstants.MATERIAL_DATA_RESOLUTION);
                WallMaterialTexture = new Texture2D(device, RenderConstants.MATERIAL_DATA_RESOLUTION, RenderConstants.MATERIAL_DATA_RESOLUTION);
            });
        }

        public override void Unload()
        {
            base.Unload();

            TileMaterialTexture.Dispose();
            WallMaterialTexture.Dispose();
        }

        public static void PackTileMaterialTexture()
        {
            int maxPixels = RenderConstants.MATERIAL_DATA_RESOLUTION * RenderConstants.MATERIAL_DATA_RESOLUTION;

            Array.Clear(materialPixels, 0, materialPixels.Length);

            foreach (var kvp in tiles)
            {
                int id = kvp.Key;

                if (id >= 0 && id < maxPixels)
                {
                    Vector3 color = kvp.Value;
                    materialPixels[id] = new Color(color.X, color.Y, color.Z);
                }
            }

            TileMaterialTexture.SetData(materialPixels);

            TilesValid = true;
        }

        public static void PackWallMaterialTexture()
        {
            int maxPixels = RenderConstants.MATERIAL_DATA_RESOLUTION * RenderConstants.MATERIAL_DATA_RESOLUTION;

            Array.Clear(materialPixels, 0, materialPixels.Length);

            foreach (var kvp in walls)
            {
                int id = kvp.Key;

                if (id >= 0 && id < maxPixels)
                {
                    Vector3 color = kvp.Value;
                    materialPixels[id] = new Color(color.X, color.Y, color.Z);
                }
            }

            WallMaterialTexture.SetData(materialPixels);

            WallsValid = true;
        }

        public static void SetTileMaterials(Dictionary<int, Vector3> mappedTiles)
        {
            tiles = new Dictionary<int, Vector3>(mappedTiles);

            TilesValid = false;
        }

        public static void SetWallMaterials(Dictionary<int, Vector3> mappedWalls)
        {
            walls = new Dictionary<int, Vector3>(mappedWalls);

            WallsValid = false;
        }
    }
}

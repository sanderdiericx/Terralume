using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terralume.Engine.Constants;
using Terraria;
using Terraria.ModLoader;

namespace Terralume.Engine.Mapping
{
    internal class MaterialPacker : ModSystem
    {
        public static bool TilesValid { get; private set; } = false;
        private static Dictionary<int, Vector3> tiles = new Dictionary<int, Vector3>();

        public static bool WallsValid { get; private set; } = false;
        private static Dictionary<int, Vector3> walls = new Dictionary<int, Vector3>();

        public static bool LiquidsValid { get; private set; } = false;
        private static Dictionary<int, Vector3> liquids = new Dictionary<int, Vector3>();

        public static Texture2D TileMaterialTexture { get; private set; }
        public static Texture2D WallMaterialTexture { get; private set; }
        public static Texture2D LiquidMaterialTexture { get; private set; }

        private static Color[] materialPixels = new Color[RenderConfig.MaterialDataResolution * RenderConfig.MaterialDataResolution];

        public override void Load()
        {
            base.Load();

            GraphicsDevice device = Main.graphics.GraphicsDevice;

            Main.QueueMainThreadAction(() =>
            {
                TileMaterialTexture = new Texture2D(device, RenderConfig.MaterialDataResolution, RenderConfig.MaterialDataResolution);
                WallMaterialTexture = new Texture2D(device, RenderConfig.MaterialDataResolution, RenderConfig.MaterialDataResolution);
                LiquidMaterialTexture = new Texture2D(device, RenderConfig.MaterialDataResolution, RenderConfig.MaterialDataResolution);
            });
        }

        public override void Unload()
        {
            base.Unload();

            Main.QueueMainThreadAction(() =>
            {
                TileMaterialTexture.Dispose();
                WallMaterialTexture.Dispose();
                LiquidMaterialTexture.Dispose();
            });
        }

        public static void PackTileMaterialTexture()
        {
            PackMaterialTexture(TileMaterialTexture, tiles);

            TilesValid = true;
        }

        public static void PackWallMaterialTexture()
        {
            PackMaterialTexture(WallMaterialTexture, walls);

            WallsValid = true;
        }

        public static void PackLiquidMaterialTexture()
        {
            PackMaterialTexture(LiquidMaterialTexture, liquids);

            LiquidsValid = true;
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

        public static void SetLiquidMaterials(Dictionary<int, Vector3> mappedLiquids)
        {
            liquids = new Dictionary<int, Vector3>(mappedLiquids);

            LiquidsValid = false;
        }

        // Encodes a dictionary (mapping ID to material vector) into a texture, the index of a pixel inside the texture refers to its ID
        private static void PackMaterialTexture(Texture2D texture, Dictionary<int, Vector3> mappedMaterials)
        {
            int maxPixels = RenderConfig.MaterialDataResolution * RenderConfig.MaterialDataResolution;

            Array.Clear(materialPixels, 0, materialPixels.Length);

            foreach (var kvp in mappedMaterials)
            {
                int id = kvp.Key;

                if (id >= 0 && id < maxPixels)
                {
                    Vector3 color = kvp.Value;
                    materialPixels[id] = new Color(color.X, color.Y, color.Z);
                }
            }

            texture.SetData(materialPixels);
        }
    }
}

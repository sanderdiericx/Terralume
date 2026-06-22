using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System;
using System.Collections.Generic;
using Terralume.Engine.Constants;
using Terraria;
using Terraria.ModLoader;

namespace Terralume.Engine.Mapping
{
    public class NoisePacker : ModSystem
    {
        public static bool TilesValid = false;
        private static Dictionary<int, Vector2> tiles = new Dictionary<int, Vector2>();

        public static bool WallsValid = false;
        private static Dictionary<int, Vector2> walls = new Dictionary<int, Vector2>();

        public static Texture2D TileNoiseTexture;
        public static Texture2D WallNoiseTexture;

        private static HalfVector2[] noisePixels = new HalfVector2[RenderConstants.MATERIAL_DATA_RESOLUTION * RenderConstants.MATERIAL_DATA_RESOLUTION];

        public override void Load()
        {
            base.Load();

            GraphicsDevice device = Main.graphics.GraphicsDevice;

            Main.QueueMainThreadAction(() =>
            {
                TileNoiseTexture = new Texture2D(device, RenderConstants.MATERIAL_DATA_RESOLUTION, RenderConstants.MATERIAL_DATA_RESOLUTION, false, SurfaceFormat.HalfVector2);
                WallNoiseTexture = new Texture2D(device, RenderConstants.MATERIAL_DATA_RESOLUTION, RenderConstants.MATERIAL_DATA_RESOLUTION, false, SurfaceFormat.HalfVector2);
            });
        }

        public override void Unload()
        {
            base.Unload();

            Main.QueueMainThreadAction(() =>
            {
                TileNoiseTexture.Dispose();
                WallNoiseTexture.Dispose();
            });
        }

        public static void PackTileNoiseTexture()
        {
            PackNoiseTexture(TileNoiseTexture, tiles);

            TilesValid = true;
        }

        public static void PackWallNoiseTexture()
        {
            PackNoiseTexture(WallNoiseTexture, walls);

            WallsValid = true;
        }

        public static void SetTileNoise(Dictionary<int, Vector2> mappedTiles)
        {
            tiles = new Dictionary<int, Vector2>(mappedTiles);

            TilesValid = false;
        }
        public static void SetWallNoise(Dictionary<int, Vector2> mappedWalls)
        {
            walls = new Dictionary<int, Vector2>(mappedWalls);

            WallsValid = false;
        }

        // Encodes a dictionary (mapping ID to noise vector) into a texture (HalfVector2)
        private static void PackNoiseTexture(Texture2D texture, Dictionary<int, Vector2> mappedNoise)
        {
            int maxPixels = RenderConstants.MATERIAL_DATA_RESOLUTION * RenderConstants.MATERIAL_DATA_RESOLUTION;

            Array.Clear(noisePixels, 0, noisePixels.Length);

            foreach (var kvp in mappedNoise)
            {
                int id = kvp.Key;

                if (id >= 0 && id < maxPixels)
                {
                    Vector2 noise = kvp.Value;
                    noisePixels[id] = new HalfVector2(noise.X, noise.Y);
                }
            }

            texture.SetData(noisePixels);
        }
    }
}

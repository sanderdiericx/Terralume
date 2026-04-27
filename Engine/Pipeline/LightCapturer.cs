using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terralume.Engine.Buffers.Data;
using Terralume.Engine.Constants;
using Terralume.Engine.Mapping;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terralume.Engine.Pipeline
{
    class LightCapturer : ModSystem
    {
        private static List<(Vector2, LightProfile)> hookedLights = new List<(Vector2, LightProfile)>();

        public override void Load()
        {
            On_Lighting.AddLight_Vector2_Vector3 += CaptureVectorLight;
            On_Lighting.AddLight_int_int_float_float_float += CaptureTileLight;
        }

        public override void Unload()
        {
            On_Lighting.AddLight_Vector2_Vector3 -= CaptureVectorLight;
            On_Lighting.AddLight_int_int_float_float_float -= CaptureTileLight;
        }

        // Make sure ores dont cast light
        // TO DO: Filter out only ores
        public override void PostSetupContent()
        {
            for (int i = 0; i < TileID.Count; i++)
            {
                if (Main.tileShine[i] > 0)
                {
                    Main.tileShine[i] = 0;
                }
            }
        }

        private void CaptureVectorLight(On_Lighting.orig_AddLight_Vector2_Vector3 orig, Vector2 position, Vector3 rgb)
        {
            orig(position, rgb);

            Vector2 screenPos = position - Main.screenPosition;

            hookedLights.Add((screenPos, new LightProfile(new Vector4(rgb, 1f), LightMapper.ProjectileLightIntensity, LightMapper.ProjectileLightRange)));
        }

        private void CaptureTileLight(On_Lighting.orig_AddLight_int_int_float_float_float orig, int i, int j, float r, float g, float b)
        {
            orig(i, j, r, g, b);
            Vector3 color = new Vector3(r, g, b);

            Vector2 screenPos = new Vector2(i * 16 + 8, j * 16 + 8) - Main.screenPosition;

            hookedLights.Add((screenPos, new LightProfile(new Vector4(r, g, b, 1f), LightMapper.ProjectileLightIntensity, LightMapper.ProjectileLightRange)));
        }

        // Captures light profiles for tiles by scanning and binding tile ID to a mapped light profile
        private static void CaptureVisibleTileLights()
        {
            int startX = Math.Clamp((int)(Main.screenPosition.X / 16f) - 2, 0, Main.maxTilesX - 1);
            int endX = Math.Clamp((int)((Main.screenPosition.X + Main.screenWidth) / 16f) + 2, 0, Main.maxTilesX - 1);
            int startY = Math.Clamp((int)(Main.screenPosition.Y / 16f) - 2, 0, Main.maxTilesY - 1);
            int endY = Math.Clamp((int)((Main.screenPosition.Y + Main.screenHeight) / 16f) + 2, 0, Main.maxTilesY - 1);

            for (int i = startX; i < endX; i++)
            {
                for (int j = startY; j < endY; j++)
                {
                    Tile tile = Main.tile[i, j];

                    if (tile.HasTile && Main.tileLighted[tile.TileType])
                    {
                        // Note: Right now it gives every torch the same light color; fix: check Tile.FrameY for torch type
                        bool tileMapped = LightMapper.TileProfiles.TryGetValue(tile.TileType, out LightProfile lightProfile);

                        if (tileMapped)
                        {
                            Vector2 screenPos = new Vector2(i * 16 + 8, j * 16 + 8) - Main.screenPosition;

                            hookedLights.Add((screenPos, lightProfile));
                        }
                    }
                }
            }
        }

        // Gets the screen space position and light profile of all lights on screen
        public static List<(Vector2, LightProfile)> GetOnScreenLights(Vector2 screenSize)
        {
            CaptureVisibleTileLights();

            List<(Vector2, LightProfile)> onScreenLights = new List<(Vector2, LightProfile)>();

            for (int i = 0; i < hookedLights.Count; i++)
            {
                Vector2 lightPos = hookedLights[i].Item1;

                if (lightPos.X >= 0 && lightPos.Y >= 0 && lightPos.X < screenSize.X && lightPos.Y < screenSize.Y)
                {
                    onScreenLights.Add(hookedLights[i]);
                }
            }

            hookedLights.Clear();
            return onScreenLights;
        }
    }
}

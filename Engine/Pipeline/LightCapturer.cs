using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terralume.Engine.Buffers.Data;
using Terralume.Engine.Constants;
using Terralume.Engine.Mapping.Registry;
using Terraria;
using Terraria.ModLoader;

namespace Terralume.Engine.Pipeline
{
    class LightCapturer : ModSystem
    {
        private static List<(Vector2, LightProfile, bool)> hookedLights = new List<(Vector2, LightProfile, bool)>();

        private static List<(Vector2, LightProfile)> mergedLights = new List<(Vector2, LightProfile)>();
        private static List<(Vector2, LightProfile)> onScreenLights = new List<(Vector2, LightProfile)>();

        public override void Load()
        {
            On_Lighting.AddLight_Vector2_Vector3 += AddLight_Vector2_Vector3;
            On_Lighting.AddLight_int_int_float_float_float += AddLight_int_int_float_float_float;
            On_Lighting.AddLight_Vector2_float_float_float += On_Lighting_AddLight_Vector2_float_float_float;
        }

        public override void Unload()
        {
            On_Lighting.AddLight_Vector2_Vector3 -= AddLight_Vector2_Vector3;
            On_Lighting.AddLight_int_int_float_float_float -= AddLight_int_int_float_float_float;
            On_Lighting.AddLight_Vector2_float_float_float -= On_Lighting_AddLight_Vector2_float_float_float;
        }

        private void AddLight_Vector2_Vector3(On_Lighting.orig_AddLight_Vector2_Vector3 orig, Vector2 position, Vector3 rgb)
        {
            orig(position, rgb);

            hookedLights.Add((position, new LightProfile(new Vector4(rgb, 1f), LightMapper.DefaultLightIntensity, LightMapper.DefaultLightRange), false));
        }

        private void AddLight_int_int_float_float_float(On_Lighting.orig_AddLight_int_int_float_float_float orig, int i, int j, float r, float g, float b)
        {
            orig(i, j, r, g, b);
            Vector4 color = new Vector4(r, g, b, 1f);

            Vector2 worldPos = new Vector2(i * 16 + 8, j * 16 + 8);
            hookedLights.Add((worldPos, new LightProfile(color, LightMapper.DefaultLightIntensity, LightMapper.DefaultLightRange), false));
        }

        private void On_Lighting_AddLight_Vector2_float_float_float(On_Lighting.orig_AddLight_Vector2_float_float_float orig, Vector2 position, float r, float g, float b)
        {
            orig(position, r, g, b);
            Vector4 color = new Vector4(r, g, b, 1f);

            LightProfile lightProfile = new LightProfile(color, LightMapper.DefaultLightIntensity, LightMapper.DefaultLightRange);
            LightMapper.DefaultLightExceptions.TryGetValue(color, out lightProfile);

            hookedLights.Add((position, lightProfile, false));
        }

        // Captures light profiles for tiles by scanning and binding tile ID to a mapped light profile
        private static void CaptureVisibleTileLights(TileCaptureParameter tileCaptureParameter)
        {
            int tilePadding = RenderConstants.LIGHT_SEARCH_TILE_PADDING;

            int minX = (int)tileCaptureParameter.MinTile.X - tilePadding;
            int maxX = (int)tileCaptureParameter.MaxTile.X + tilePadding;
            int minY = (int)tileCaptureParameter.MinTile.Y - tilePadding;
            int maxY = (int)tileCaptureParameter.MaxTile.Y + tilePadding;

            for (int i = minY; i <= maxY; i++)
            {
                for (int j = minX; j <= maxX; j++)
                {
                    if (j < 0 || i < 0 || j >= Main.maxTilesX || i >= Main.maxTilesY) continue;

                    Tile tile = Main.tile[j, i];

                    if (tile.HasTile && Main.tileLighted[tile.TileType])
                    {
                        if (LightMapper.TileProfiles.TryGetValue(tile.TileType, out LightProfile lightProfile))
                        {
                            Vector2 worldPos = new Vector2(j * 16 + 8, i * 16 + 8);
                            hookedLights.Add((worldPos, lightProfile, true));
                        }
                    }
                }
            }
        }

        // Merges lights within a given radius together, since some weapons create hundreds of lights at the same position
        private static void MergeHookedLights()
        {
            float mergeRadius = RenderConstants.LIGHT_MERGE_RADIUS;

            mergedLights.Clear();

            foreach (var light in hookedLights)
            {
                // Skip merging with flagged lights
                if (light.Item3)
                {
                    mergedLights.Add((light.Item1, light.Item2));
                    continue;
                }

                bool absorbed = false;
                for (int i = 0; i < mergedLights.Count; i++)
                {
                    if (Vector2.DistanceSquared(mergedLights[i].Item1, light.Item1) < mergeRadius * mergeRadius)
                    {
                        var existing = mergedLights[i];

                        // Average position, add intensities
                        Vector4 color = existing.Item2.Color;
                        float range = Math.Max(existing.Item2.Range, light.Item2.Range);
                        float intensity = existing.Item2.Intensity + light.Item2.Intensity;

                        mergedLights[i] = ((existing.Item1 + light.Item1) / 2f, new LightProfile(color, intensity, range));
                        absorbed = true;
                        break;
                    }
                }

                if (!absorbed)
                {
                    mergedLights.Add((light.Item1, light.Item2));
                }
            }
        }


        // Gets the screen space position and light profile of all lights on screen
        //
        // Note: Before calling this method light elements are stored in world space with their original range and intensity, this method converts the position to screen space then applies a global range and intensity modifier
        public static List<(Vector2, LightProfile)> GetOnScreenLights(TileCaptureParameter tileCaptureParameter)
        {
            Vector2 zoom = Main.GameViewMatrix.Zoom;

            // Capture tile lights into hookedLights
            CaptureVisibleTileLights(tileCaptureParameter);

            // Merge hookedLights into mergedLights
            MergeHookedLights();

            onScreenLights.Clear();
            hookedLights.Clear();

            for (int i = 0; i < mergedLights.Count; i++)
            {
                Vector2 worldPos = mergedLights[i].Item1;

                Vector2 screenPos = (worldPos - tileCaptureParameter.ScreenPosition) * zoom;

                LightProfile originalProfile = mergedLights[i].Item2;

                float range = originalProfile.Range * zoom.X;

                // Check if the light's bounding box intersects the screen
                if (screenPos.X + range >= 0 && screenPos.X - range < RenderConstants.SCREEN_SIZE.X && screenPos.Y + range >= 0 && screenPos.Y - range < RenderConstants.SCREEN_SIZE.Y)
                {
                    onScreenLights.Add((screenPos, new LightProfile(originalProfile.Color, originalProfile.Intensity, range)));
                }
            }

            return onScreenLights;
        }
    }
}

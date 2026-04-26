using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terralume.Engine.Buffers
{
    class LightCollection : ModSystem
    {
        private static List<Tuple<Vector2, Vector3>> hookedLights = new();

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

        private void CaptureVectorLight(On_Lighting.orig_AddLight_Vector2_Vector3 orig, Vector2 position, Vector3 rgb)
        {
            orig(position, rgb);
            if (rgb.LengthSquared() > 0.01f)
            {
                hookedLights.Add(new Tuple<Vector2, Vector3>(position, rgb));
            }
        }

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
                        float r = 0f, g = 0f, b = 0f;

                        if (tile.TileType == TileID.Torches)
                        {
                            // Note: Right now it gives every torch the same light color; fix: check Tile.FrameY for torch type
                            r = 1f; g = 0.95f; b = 0.8f;
                        }

                        else if (tile.TileType == TileID.Campfire)
                        {
                            r = 0.9f; g = 0.7f; b = 0.6f;
                        }

                        // TileLoader.ModifyLight(i, j, tile.TileType, ref r, ref g, ref b);

                        if (r > 0 || g > 0 || b > 0)
                        {
                            Vector2 worldPos = new Vector2(i * 16 + 8, j * 16 + 8); // Center of the 16x16 tile

                            hookedLights.Add(new Tuple<Vector2, Vector3>(worldPos, new Vector3(r, g, b)));

                        }
                    }
                }
            }
        }


        private void CaptureTileLight(On_Lighting.orig_AddLight_int_int_float_float_float orig, int i, int j, float r, float g, float b)
        {
            orig(i, j, r, g, b);
            Vector3 color = new Vector3(r, g, b);
            if (color.LengthSquared() > 0.01f)
            {
                // Convert tile grid coordinates (16x16 pixels) to world center coordinates
                Vector2 worldPos = new Vector2(i * 16 + 8, j * 16 + 8);
                hookedLights.Add(new Tuple<Vector2, Vector3>(worldPos, color));
            }
        }

        public static Tuple<Vector2[], Vector4[], int> GetClosestLights(int count, Vector2 screenSize)
        {
            CaptureVisibleTileLights();

            Vector2[] positions = new Vector2[count];
            Vector4[] colors = new Vector4[count];
            int lightCount = 0;

            Vector2 center = Main.screenPosition + new Vector2(screenSize.X / 2, screenSize.Y / 2);
            var sortedLights = hookedLights.OrderBy(l => Vector2.DistanceSquared(l.Item1, center)).Take(count).ToList();

            for (int i = 0; i < sortedLights.Count; i++)
            {
                positions[i] = sortedLights[i].Item1 - Main.screenPosition;

                colors[i] = new Vector4(sortedLights[i].Item2, 1f);
                lightCount++;
            }

            hookedLights.Clear();
            return new Tuple<Vector2[], Vector4[], int>(positions, colors, lightCount);
        }
    }
}

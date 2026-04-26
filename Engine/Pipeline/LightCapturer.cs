using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terralume.Engine.Buffers.Data;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terralume.Engine.Pipeline
{
    class LightCapturer : ModSystem
    {
        private static readonly float PROJECTILE_RADIUS = 100f;

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

        private void CaptureVectorLight(On_Lighting.orig_AddLight_Vector2_Vector3 orig, Vector2 position, Vector3 rgb)
        {
            orig(position, rgb);
            if (rgb.LengthSquared() > 0.3f)
            {
                Vector2 screenPos = position - Main.screenPosition;

                hookedLights.Add((screenPos, new LightProfile(new Vector4(rgb, 1f), PROJECTILE_RADIUS)));
            }
        }

        private void CaptureTileLight(On_Lighting.orig_AddLight_int_int_float_float_float orig, int i, int j, float r, float g, float b)
        {
            orig(i, j, r, g, b);
            Vector3 color = new Vector3(r, g, b);

            if (color.LengthSquared() > 0.3f)
            {
                Vector2 screenPos = new Vector2(i * 16 + 8, j * 16 + 8) - Main.screenPosition;

                hookedLights.Add((screenPos, new LightProfile(new Vector4(r, g, b, 1f), PROJECTILE_RADIUS)));
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
                            Vector2 screenPos = new Vector2(i * 16 + 8, j * 16 + 8) - Main.screenPosition;

                            hookedLights.Add((screenPos, new LightProfile(new Vector4(r, g, b, 1f), PROJECTILE_RADIUS)));
                        }
                    }
                }
            }
        }

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

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terralume.Engine.Buffers.Data;
using Terralume.Engine.Constants;
using Terralume.Engine.Mapping;
using Terralume.Engine.Utils;
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

            hookedLights.Add((position, new LightProfile(new Vector4(rgb, 1f), LightMapper.ProjectileLightIntensity, LightMapper.ProjectileLightRange)));
        }

        private void CaptureTileLight(On_Lighting.orig_AddLight_int_int_float_float_float orig, int i, int j, float r, float g, float b)
        {
            orig(i, j, r, g, b);
            Vector3 color = new Vector3(r, g, b);

            Vector2 worldPos = new Vector2(i * 16 + 8, j * 16 + 8);
            hookedLights.Add((worldPos, new LightProfile(new Vector4(r, g, b, 1f), LightMapper.ProjectileLightIntensity, LightMapper.ProjectileLightRange)));
        }

        // Captures light profiles for tiles by scanning and binding tile ID to a mapped light profile
        private static void CaptureVisibleTileLights(TileCaptureParameter tileCaptureParameter, short[] tileIDs)
        {
            for (int i = (int)tileCaptureParameter.MinTile.Y; i <= tileCaptureParameter.MaxTile.Y; i++)
            {
                for (int j = (int)tileCaptureParameter.MinTile.X; j <= tileCaptureParameter.MaxTile.X; j++)
                {
                    int yIndex = i - (int) tileCaptureParameter.MinTile.Y;
                    int xIndex = j - (int) tileCaptureParameter.MinTile.X;

                    int index = xIndex + (yIndex * (int) tileCaptureParameter.TileStride.X);

                    if (index < 0 || index >= tileIDs.Length) continue;

                    short tileID = tileIDs[index];

                    if (tileID != -1 && Main.tileLighted[tileID])
                    {
                        // Note: Right now it gives every torch the same light color; fix: check Tile.FrameY for torch type
                        bool tileMapped = LightMapper.TileProfiles.TryGetValue(tileID, out LightProfile lightProfile);

                        if (tileMapped)
                        {
                            Vector2 worldPos = new Vector2(j * 16 + 8, i * 16 + 8);

                            hookedLights.Add((worldPos, lightProfile));
                        }
                    }
                }
            }
        }

        // Gets the screen space position and light profile of all lights on screen
        //
        // Note: Before calling this method light positions are stored in world space, this method converts it to screen space
        public static List<(Vector2, LightProfile)> GetOnScreenLights(TileCaptureParameter tileCaptureParameter, short[] tileIDs)
        {
            CaptureVisibleTileLights(tileCaptureParameter, tileIDs);

            List<(Vector2, LightProfile)> onScreenLights = new List<(Vector2, LightProfile)>();

            Vector2 zoom = Main.GameViewMatrix.Zoom;

            for (int i = 0; i < hookedLights.Count; i++)
            {
                Vector2 worldPos = hookedLights[i].Item1;

                Vector2 screenPos = (worldPos - tileCaptureParameter.ScreenPosition) * zoom;

                if (screenPos.X >= 0 && screenPos.Y >= 0 && screenPos.X < tileCaptureParameter.ScreenSize.X && screenPos.Y < tileCaptureParameter.ScreenSize.Y)
                {
                    onScreenLights.Add((screenPos, hookedLights[i].Item2));
                }
            }

            hookedLights.Clear();
            return onScreenLights;
        }
    }
}

using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terralume.Engine.Buffers.Data;
using Terralume.Engine.Mapping;
using Terraria;
using Terraria.ModLoader;

namespace Terralume.Engine.Pipeline
{
    class LightCapturer : ModSystem
    {
        private static List<(Vector2, LightProfile)> hookedLights = new List<(Vector2, LightProfile)>();

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

            hookedLights.Add((position, new LightProfile(new Vector4(rgb, 1f), LightMapper.DefaultLightIntensity, LightMapper.DefaultLightRange)));
        }

        private void AddLight_int_int_float_float_float(On_Lighting.orig_AddLight_int_int_float_float_float orig, int i, int j, float r, float g, float b)
        {
            orig(i, j, r, g, b);
            Vector4 color = new Vector4(r, g, b, 1f);

            Vector2 worldPos = new Vector2(i * 16 + 8, j * 16 + 8);
            hookedLights.Add((worldPos, new LightProfile(color, LightMapper.DefaultLightIntensity, LightMapper.DefaultLightRange)));
        }

        private void On_Lighting_AddLight_Vector2_float_float_float(On_Lighting.orig_AddLight_Vector2_float_float_float orig, Vector2 position, float r, float g, float b)
        {
            orig(position, r, g, b);
            Vector4 color = new Vector4(r, g, b, 1f);

            hookedLights.Add((position, new LightProfile(color, LightMapper.DefaultLightIntensity, LightMapper.DefaultLightRange)));
        }

        // Captures light profiles for tiles by scanning and binding tile ID to a mapped light profile
        private static void CaptureVisibleTileLights(TileCaptureParameter tileCaptureParameter, short[] tileIDs)
        {
            for (int i = (int)tileCaptureParameter.MinTile.Y; i <= tileCaptureParameter.MaxTile.Y; i++)
            {
                for (int j = (int)tileCaptureParameter.MinTile.X; j <= tileCaptureParameter.MaxTile.X; j++)
                {
                    int yIndex = i - (int)tileCaptureParameter.MinTile.Y;
                    int xIndex = j - (int)tileCaptureParameter.MinTile.X;

                    int index = xIndex + (yIndex * (int)tileCaptureParameter.TileStride.X);

                    if (index < 0 || index >= tileIDs.Length) continue;

                    short tileID = tileIDs[index];

                    if (tileID != -1 && Main.tileLighted[tileID])
                    {
                        // Note: Right now it gives every torch the same light color; fix: check Tile.FrameY for torch type
                        bool tileMapped = LightMapper.TileProfiles.TryGetValue((ushort)tileID, out LightProfile lightProfile);

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

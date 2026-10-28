using Microsoft.Xna.Framework;
using System;
using Terralume.Engine.Buffers.Data;
using Terraria;
using Terraria.ModLoader;

namespace Terralume.Engine.Hooks.Tracking
{
    internal class AmbientLightTracker : ModSystem
    {
        private static AmbientLightProfile lastAmbientLightProfile = new AmbientLightProfile(Vector3.Zero, 0f);
        private static float t = 0f;

        public static AmbientLightProfile GetAmbientLightProfile()
        {
            // Find the ambient light profile for the current biome
            AmbientLightProfile biomeAmbient = GetBiomeAmbient();

            // If the current and previous biome ambient do not closely match, we interpolate between the last and current profiles
            float distance = Vector3.Distance(biomeAmbient.AmbientColor, lastAmbientLightProfile.AmbientColor);

            if (distance > 0.001f || Math.Abs(biomeAmbient.AmbientStrength - lastAmbientLightProfile.AmbientStrength) > 0.001f)
            {
                t = MathHelper.Clamp(t + 0.01f, 0f, 1f);

                lastAmbientLightProfile.AmbientColor = Vector3.Lerp(lastAmbientLightProfile.AmbientColor, biomeAmbient.AmbientColor, t);
                lastAmbientLightProfile.AmbientStrength = float.Lerp(lastAmbientLightProfile.AmbientStrength, biomeAmbient.AmbientStrength, t);

                return lastAmbientLightProfile;
            }

            // Reset interpolation progress
            t = 0f;
            lastAmbientLightProfile = biomeAmbient;

            return lastAmbientLightProfile;
        }

        private static AmbientLightProfile GetBiomeAmbient()
        {
            Player player = Main.player[Main.myPlayer];

            Vector3 skyLinear = ToLinear(Main.ColorOfTheSkies.ToVector3());

            Vector3 ambientColor = new Vector3(0.50f, 0.50f, 0.55f);
            float ambientStrength = 0.18f;

            bool corruptSpace = player.ZoneSkyHeight && player.ZoneCorrupt;
            bool corruptSurface = player.ZoneOverworldHeight && player.ZoneCorrupt;
            bool corruptUnderground = player.ZoneDirtLayerHeight && player.ZoneCorrupt;
            bool corruptCaverns = player.ZoneRockLayerHeight && player.ZoneCorrupt;

            bool crimsonSpace = player.ZoneSkyHeight && player.ZoneCrimson;
            bool crimsonSurface = player.ZoneOverworldHeight && player.ZoneCrimson;
            bool crimsonUnderground = player.ZoneDirtLayerHeight && player.ZoneCrimson;
            bool crimsonCaverns = player.ZoneRockLayerHeight && player.ZoneCrimson;

            bool hallowSpace = player.ZoneSkyHeight && player.ZoneHallow;
            bool hallowSurface = player.ZoneOverworldHeight && player.ZoneHallow;
            bool hallowUnderground = player.ZoneDirtLayerHeight && player.ZoneHallow;
            bool hallowCaverns = player.ZoneRockLayerHeight && player.ZoneHallow;

            bool jungleSpace = player.ZoneSkyHeight && player.ZoneJungle;
            bool jungleSurface = player.ZoneOverworldHeight && player.ZoneJungle;
            bool jungleUnderground = player.ZoneDirtLayerHeight && player.ZoneJungle;
            bool jungleCaverns = player.ZoneRockLayerHeight && player.ZoneJungle;

            bool snowSpace = player.ZoneSkyHeight && player.ZoneSnow;
            bool snowSurface = player.ZoneOverworldHeight && player.ZoneSnow;
            bool snowUnderground = player.ZoneDirtLayerHeight && player.ZoneSnow;
            bool snowCaverns = player.ZoneRockLayerHeight && player.ZoneSnow;

            // TOWERS
            if (player.ZoneTowerSolar)
            {
                ambientColor = new Vector3(1.00f, 0.45f, 0.15f);
                ambientStrength = 0.85f;
            }
            else if (player.ZoneTowerVortex)
            {
                ambientColor = new Vector3(0.30f, 0.70f, 1.00f);
                ambientStrength = 0.85f;
            }
            else if (player.ZoneTowerNebula)
            {
                ambientColor = new Vector3(0.80f, 0.40f, 1.00f);
                ambientStrength = 0.85f;
            }
            else if (player.ZoneTowerStardust)
            {
                ambientColor = new Vector3(0.25f, 0.35f, 0.90f);
                ambientStrength = 0.85f;
            }

            // MISC UNDERGROUND
            else if (player.ZoneUnderworldHeight)
            {
                ambientColor = new Vector3(0.90f, 0.25f, 0.10f);
                ambientStrength = 0.75f;
            }
            else if (player.ZoneDungeon)
            {
                ambientColor = new Vector3(0.25f, 0.30f, 0.45f);
                ambientStrength = 0.48f;
            }
            else if (player.ZoneShimmer)
            {
                ambientColor = new Vector3(0.50f, 0.90f, 0.85f);
                ambientStrength = 0.55f;
            }
            else if (player.ZoneLihzhardTemple)
            {
                ambientColor = new Vector3(0.45f, 0.50f, 0.25f);
                ambientStrength = 0.46f;
            }
            else if (player.ZoneHive)
            {
                ambientColor = new Vector3(0.90f, 0.70f, 0.20f);
                ambientStrength = 0.46f;
            }
            else if (player.ZoneGranite)
            {
                ambientColor = new Vector3(0.35f, 0.40f, 0.55f);
                ambientStrength = 0.44f;
            }
            else if (player.ZoneMarble)
            {
                ambientColor = new Vector3(0.55f, 0.60f, 0.65f);
                ambientStrength = 0.48f;
            }
            else if (player.ZoneUndergroundDesert)
            {
                ambientColor = new Vector3(0.50f, 0.40f, 0.25f);
                ambientStrength = 0.38f;
            }
            else if (player.ZoneGemCave)
            {
                ambientColor = new Vector3(0.40f, 0.55f, 0.60f);
                ambientStrength = 0.36f;
            }
            else if (player.ZoneGlowshroom)
            {
                ambientColor = new Vector3(0.30f, 0.40f, 0.60f);
                ambientStrength = 0.48f;
            }

            // CAVERNS
            else if (corruptCaverns)
            {
                ambientColor = new Vector3(0.25f, 0.10f, 0.34f);
                ambientStrength = 0.48f;
            }
            else if (crimsonCaverns)
            {
                ambientColor = new Vector3(0.36f, 0.06f, 0.08f);
                ambientStrength = 0.48f;
            }
            else if (hallowCaverns)
            {
                ambientColor = new Vector3(0.44f, 0.26f, 0.56f);
                ambientStrength = 0.42f;
            }
            else if (jungleCaverns)
            {
                ambientColor = new Vector3(0.12f, 0.22f, 0.08f);
                ambientStrength = 0.42f;
            }
            else if (snowCaverns)
            {
                ambientColor = new Vector3(0.26f, 0.32f, 0.44f);
                ambientStrength = 0.44f;
            }
            else if (player.ZoneNormalCaverns)
            {
                ambientColor = new Vector3(0.28f, 0.30f, 0.36f);
                ambientStrength = 0.44f;
            }

            // UNDERGROUND
            else if (corruptUnderground)
            {
                ambientColor = new Vector3(0.30f, 0.14f, 0.40f);
                ambientStrength = 0.40f;
            }
            else if (crimsonUnderground)
            {
                ambientColor = new Vector3(0.42f, 0.08f, 0.10f);
                ambientStrength = 0.50f;
            }
            else if (hallowUnderground)
            {
                ambientColor = new Vector3(0.55f, 0.35f, 0.68f);
                ambientStrength = 0.40f;
            }
            else if (jungleUnderground)
            {
                ambientColor = new Vector3(0.16f, 0.30f, 0.10f);
                ambientStrength = 0.44f;
            }
            else if (snowUnderground)
            {
                ambientColor = new Vector3(0.36f, 0.44f, 0.58f);
                ambientStrength = 0.42f;
            }
            else if (player.ZoneNormalUnderground)
            {
                ambientColor = new Vector3(0.35f, 0.33f, 0.30f);
                ambientStrength = 0.46f;
            }

            // GENERAL MISC
            else if (player.ZoneGraveyard)
            {
                ambientColor = new Vector3(0.35f, 0.40f, 0.35f);
                ambientStrength = 0.40f;
            }
            else if (player.ZoneOldOneArmy)
            {
                ambientColor = Vector3.Lerp(skyLinear, new Vector3(0.50f, 0.30f, 0.25f), 0.7f);
                ambientStrength = 0.80f;
            }

            // SURFACE
            else if (corruptSurface)
            {
                ambientColor = Vector3.Lerp(skyLinear, new Vector3(0.28f, 0.10f, 0.40f), 0.50f);
                ambientStrength = 0.60f;
            }
            else if (crimsonSurface)
            {
                ambientColor = Vector3.Lerp(skyLinear, new Vector3(0.55f, 0.08f, 0.12f), 0.50f);
                ambientStrength = 0.65f;
            }
            else if (hallowSurface)
            {
                ambientColor = Vector3.Lerp(skyLinear, new Vector3(0.70f, 0.50f, 0.85f), 0.40f);
                ambientStrength = 0.65f;
            }
            else if (jungleSurface)
            {
                ambientColor = Vector3.Lerp(skyLinear, new Vector3(0.18f, 0.42f, 0.12f), 0.38f);
                ambientStrength = 0.75f;
            }
            else if (snowSurface)
            {
                ambientColor = Vector3.Lerp(skyLinear, new Vector3(0.72f, 0.82f, 0.96f), 0.25f);
                ambientStrength = 0.85f;
            }
            else if (player.ZoneDesert)
            {
                ambientColor = skyLinear * new Vector3(1.15f, 0.95f, 0.75f);
                ambientStrength = 0.80f;
            }
            else if (player.ZoneBeach)
            {
                ambientColor = skyLinear * new Vector3(0.95f, 1.00f, 1.05f);
                ambientStrength = 0.85f;
            }
            else if (player.ZoneForest)
            {
                ambientColor = Vector3.Lerp(skyLinear, new Vector3(0.40f, 0.55f, 0.35f), 0.25f);
                ambientStrength = 0.80f;
            }

            // SURFACE MISC
            else if (player.ZoneSandstorm)
            {
                ambientColor = skyLinear * new Vector3(0.90f, 0.75f, 0.55f);
                ambientStrength = 0.60f;
            }
            else if (player.ZoneRain)
            {
                ambientColor = skyLinear * new Vector3(0.70f, 0.75f, 0.85f);
                ambientStrength = 0.65f;
            }
            else if (player.ZoneMeteor)
            {
                ambientColor = new Vector3(0.80f, 0.40f, 0.20f);
                ambientStrength = 0.65f;
            }

            // SPACE
            else if (corruptSpace)
            {
                ambientColor = Vector3.Lerp(skyLinear, new Vector3(0.28f, 0.10f, 0.40f), 0.18f);
                ambientStrength = 0.38f;
            }
            else if (crimsonSpace)
            {
                ambientColor = Vector3.Lerp(skyLinear, new Vector3(0.50f, 0.10f, 0.14f), 0.18f);
                ambientStrength = 0.40f;
            }
            else if (hallowSpace)
            {
                ambientColor = Vector3.Lerp(skyLinear, new Vector3(0.68f, 0.52f, 0.85f), 0.20f);
                ambientStrength = 0.42f;
            }
            else if (jungleSpace)
            {
                ambientColor = Vector3.Lerp(skyLinear, new Vector3(0.20f, 0.45f, 0.16f), 0.15f);
                ambientStrength = 0.45f;
            }
            else if (snowSpace)
            {
                ambientColor = Vector3.Lerp(skyLinear, new Vector3(0.74f, 0.84f, 0.98f), 0.15f);
                ambientStrength = 0.48f;
            }
            else if (player.ZoneNormalSpace)
            {
                ambientColor = skyLinear;
                ambientStrength = 0.45f;
            }

            return new AmbientLightProfile(ambientColor, ambientStrength);
        }

        private static Vector3 ToLinear(Vector3 srgb)
        {
            return new Vector3(MathHelper.Clamp((float)Math.Pow(srgb.X, 2.2f), 0f, 4f), MathHelper.Clamp((float)Math.Pow(srgb.Y, 2.2f), 0f, 4f), MathHelper.Clamp((float)Math.Pow(srgb.Z, 2.2f), 0f, 4f));
        }
    }
}

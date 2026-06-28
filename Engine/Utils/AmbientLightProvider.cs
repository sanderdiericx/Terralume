using Microsoft.Xna.Framework;
using System;
using Terralume.Engine.Buffers.Data;
using Terraria;
using Terraria.ModLoader;

namespace Terralume.Engine.Utils
{
    public class AmbientLightProvider : ModSystem
    {
        public static AmbientLightProfile GetAmbientLightProfile()
        {
            return GetBiomeAmbient();
        }

        private static AmbientLightProfile GetBiomeAmbient()
        {
            Player player = Main.player[Main.myPlayer];

            Vector3 skyLinear = ToLinear(Main.ColorOfTheSkies.ToVector3());

            Vector3 ambientColor = new Vector3(0.50f, 0.50f, 0.55f);
            float ambientStrength = 0.30f;

            if (player.ZoneTowerSolar)
            {
                ambientColor = new Vector3(1.00f, 0.45f, 0.15f);
                ambientStrength = 0.90f;
            }
            else if (player.ZoneTowerVortex)
            {
                ambientColor = new Vector3(0.30f, 0.70f, 1.00f);
                ambientStrength = 0.75f;
            }
            else if (player.ZoneTowerNebula)
            {
                ambientColor = new Vector3(0.80f, 0.40f, 1.00f);
                ambientStrength = 0.75f;
            }
            else if (player.ZoneTowerStardust)
            {
                ambientColor = new Vector3(0.25f, 0.35f, 0.90f);
                ambientStrength = 0.80f;
            }
            else if (player.ZoneUnderworldHeight)
            {
                ambientColor = new Vector3(0.90f, 0.25f, 0.10f);
                ambientStrength = 0.35f;
            }
            else if (player.ZoneDungeon)
            {
                ambientColor = new Vector3(0.25f, 0.30f, 0.45f);
                ambientStrength = 0.12f;
            }
            else if (player.ZoneShimmer)
            {
                ambientColor = new Vector3(0.50f, 0.90f, 0.85f);
                ambientStrength = 0.40f;
            }
            else if (player.ZoneLihzhardTemple)
            {
                ambientColor = new Vector3(0.45f, 0.50f, 0.25f);
                ambientStrength = 0.18f;
            }
            else if (player.ZoneHive)
            {
                ambientColor = new Vector3(0.90f, 0.70f, 0.20f);
                ambientStrength = 0.22f;
            }
            else if (player.ZoneGranite)
            {
                ambientColor = new Vector3(0.35f, 0.40f, 0.55f);
                ambientStrength = 0.15f;
            }
            else if (player.ZoneMarble)
            {
                ambientColor = new Vector3(0.55f, 0.60f, 0.65f);
                ambientStrength = 0.20f;
            }
            else if (player.ZoneUndergroundDesert)
            {
                ambientColor = new Vector3(0.50f, 0.40f, 0.25f);
                ambientStrength = 0.15f;
            }
            else if (player.ZoneGemCave)
            {
                ambientColor = new Vector3(0.40f, 0.55f, 0.60f);
                ambientStrength = 0.18f;
            }
            else if (player.ZoneGlowshroom)
            {
                ambientColor = new Vector3(0.30f, 0.90f, 0.60f);
                ambientStrength = 0.35f;
            }
            else if (player.ZoneJungle)
            {
                ambientColor = Vector3.Lerp(skyLinear, new Vector3(0.25f, 0.55f, 0.20f), 0.6f);
                ambientStrength = 0.40f;
            }
            else if (player.ZoneCorrupt)
            {
                ambientColor = new Vector3(0.45f, 0.25f, 0.55f);
                ambientStrength = 0.22f;
            }
            else if (player.ZoneCrimson)
            {
                ambientColor = new Vector3(0.55f, 0.15f, 0.25f);
                ambientStrength = 0.22f;
            }
            else if (player.ZoneHallow)
            {
                ambientColor = new Vector3(0.70f, 0.55f, 0.90f);
                ambientStrength = 0.45f;
            }
            else if (player.ZoneGraveyard)
            {
                ambientColor = new Vector3(0.35f, 0.40f, 0.35f);
                ambientStrength = 0.18f;
            }
            else if (player.ZoneOldOneArmy)
            {
                ambientColor = Vector3.Lerp(skyLinear, new Vector3(0.50f, 0.30f, 0.25f), 0.7f);
                ambientStrength = 0.45f;
            }
            else if (player.ZoneShadowCandle)
            {
                ambientColor = new Vector3(0.30f, 0.15f, 0.15f);
                ambientStrength = 0.15f;
            }
            else if (player.ZoneMeteor)
            {
                ambientColor = new Vector3(0.80f, 0.40f, 0.20f);
                ambientStrength = 0.50f;
            }
            else if (player.ZoneDesert)
            {
                ambientColor = skyLinear * new Vector3(1.15f, 0.95f, 0.75f);
                ambientStrength = 0.80f;
            }
            else if (player.ZoneSnow)
            {
                ambientColor = skyLinear * new Vector3(0.90f, 0.95f, 1.15f);
                ambientStrength = 0.85f;
            }
            else if (player.ZoneBeach)
            {
                ambientColor = skyLinear * new Vector3(0.95f, 1.00f, 1.05f);
                ambientStrength = 0.90f;
            }
            else if (player.ZoneSandstorm)
            {
                ambientColor = skyLinear * new Vector3(0.90f, 0.75f, 0.55f);
                ambientStrength = 0.45f;
            }
            else if (player.ZoneRain)
            {
                ambientColor = skyLinear * new Vector3(0.70f, 0.75f, 0.85f);
                ambientStrength = 0.50f;
            }
            else if (player.ZoneWaterCandle)
            {
                ambientColor = skyLinear * new Vector3(0.85f, 0.90f, 1.00f);
                ambientStrength = 0.75f;
            }
            else if (player.ZonePeaceCandle)
            {
                ambientColor = skyLinear * new Vector3(1.05f, 1.00f, 0.95f);
                ambientStrength = 0.85f;
            }
            else if (player.ZoneSkyHeight)
            {
                ambientColor = skyLinear;
                ambientStrength = 1.05f;
            }
            else if (player.ZoneOverworldHeight)
            {
                ambientColor = skyLinear;
                ambientStrength = 0.85f;
            }
            else if (player.ZoneDirtLayerHeight)
            {
                ambientColor = Vector3.Lerp(skyLinear, new Vector3(0.40f, 0.40f, 0.42f), 0.6f);
                ambientStrength = 0.35f;
            }
            else if (player.ZoneRockLayerHeight)
            {
                ambientColor = new Vector3(0.30f, 0.32f, 0.38f);
                ambientStrength = 0.15f;
            }

            else if (player.ZoneNormalCaverns)
            {
                ambientColor = new Vector3(0.28f, 0.30f, 0.36f);
                ambientStrength = 0.10f;
            }
            else if (player.ZoneNormalUnderground)
            {
                ambientColor = new Vector3(0.35f, 0.33f, 0.30f);
                ambientStrength = 0.12f;
            }
            else if (player.ZoneNormalSpace)
            {
                ambientColor = skyLinear;
                ambientStrength = 1.10f;
            }
            else if (player.ZonePurity)
            {
                ambientColor = skyLinear;
                ambientStrength = 0.80f;
            }
            else if (player.ZoneForest)
            {
                ambientColor = Vector3.Lerp(skyLinear, new Vector3(0.40f, 0.55f, 0.35f), 0.25f);
                ambientStrength = 0.80f;
            }

            return new AmbientLightProfile(ambientColor, ambientStrength);
        }

        private static Vector3 ToLinear(Vector3 srgb)
        {
            return new Vector3(MathHelper.Clamp((float)Math.Pow(srgb.X, 2.2f), 0f, 4f), MathHelper.Clamp((float)Math.Pow(srgb.Y, 2.2f), 0f, 4f), MathHelper.Clamp((float)Math.Pow(srgb.Z, 2.2f), 0f, 4f));
        }
    }
}

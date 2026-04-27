using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terralume.Engine.Buffers.Data;
using Terraria.ID;

namespace Terralume.Engine.Mapping
{
    internal static class LightMapper
    {
        public static float ProjectileLightIntensity = 20f;
        public static float ProjectileLightRange = 15f;

        public static Dictionary<int, LightProfile> TileProfiles = new Dictionary<int, LightProfile>()
        {
            { TileID.Torches, new LightProfile(new Vector4(1f, 0.6f, 0.3f, 1f), 25f, 20f) }
        };
    }
}

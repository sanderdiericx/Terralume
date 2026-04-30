using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terralume.Engine.Buffers.Data;
using Terraria.ID;

namespace Terralume.Engine.Mapping
{
    internal static class LightMapper
    {
        public static float DefaultLightIntensity = 20f;
        public static float DefaultLightRange = 15f;

        public static Dictionary<int, LightProfile> TileProfiles = new Dictionary<int, LightProfile>()
        {
            { TileID.Torches, new LightProfile(new Vector4(1f, 0.95f, 0.8f, 1f), 25f, 20f) }
        };
    }
}

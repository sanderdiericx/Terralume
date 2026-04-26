using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terralume.Engine.Buffers.Data;

namespace Terralume.Engine.Mapping
{
    internal static class LightMapper
    {
        public static LightProfile DefaultProfile = new LightProfile(new Vector4(1f, 0.95f, 0.8f, 1f), 200);

        public static Dictionary<int, LightProfile> TileProfiles = new Dictionary<int, LightProfile>()
        {

        };
    }
}

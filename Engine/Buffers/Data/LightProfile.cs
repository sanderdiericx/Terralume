using Microsoft.Xna.Framework;

namespace Terralume.Engine.Buffers.Data
{
    public struct LightProfile
    {
        public Vector4 LightColor;
        public float LightRadius;

        public LightProfile(Vector4 lightColor, float lightRadius)
        {
            LightColor = lightColor;
            LightRadius = lightRadius;
        }
    }
}

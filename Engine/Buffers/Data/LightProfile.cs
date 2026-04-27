using Microsoft.Xna.Framework;

namespace Terralume.Engine.Buffers.Data
{
    public struct LightProfile
    {
        public Vector4 Color;
        public float Intensity;
        public float Range;

        public LightProfile(Vector4 color, float intensity, float range)
        {
            Color = color;
            Intensity = intensity;
            Range = range;
        }
    }
}

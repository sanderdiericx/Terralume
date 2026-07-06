using Microsoft.Xna.Framework;

namespace Terralume.Engine.Buffers.Data
{
    internal struct LightProfile
    {
        public Vector4 Color { get; private set; }
        public float Intensity { get; private set; }
        public float Range { get; private set; }

        public LightProfile(Vector4 color, float intensity, float range)
        {
            Color = color;
            Intensity = intensity;
            Range = range;
        }
    }
}

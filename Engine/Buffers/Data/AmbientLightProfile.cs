using Microsoft.Xna.Framework;

namespace Terralume.Engine.Buffers.Data
{
    internal struct AmbientLightProfile
    {
        public Vector3 AmbientColor { get; set; }
        public float AmbientStrength { get; set; }

        public AmbientLightProfile(Vector3 ambientColor, float ambientStrength)
        {
            AmbientColor = ambientColor;
            AmbientStrength = ambientStrength;
        }
    }
}

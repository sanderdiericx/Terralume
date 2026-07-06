using Microsoft.Xna.Framework;

namespace Terralume.Engine.Buffers.Data
{
    internal struct AmbientLightProfile
    {
        public Vector3 AmbientColor { get; private set; }
        public float AmbientStrength { get; private set; }

        public AmbientLightProfile(Vector3 ambientColor, float ambientStrength)
        {
            AmbientColor = ambientColor;
            AmbientStrength = ambientStrength;
        }
    }
}

using Microsoft.Xna.Framework;

namespace Terralume.Engine.Buffers.Data
{
    public struct AmbientLightProfile
    {
        public Vector3 AmbientColor;
        public float AmbientStrength;

        public AmbientLightProfile(Vector3 ambientColor, float ambientStrength)
        {
            AmbientColor = ambientColor;
            AmbientStrength = ambientStrength;
        }
    }
}

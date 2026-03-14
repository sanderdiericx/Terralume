namespace Terralume.Engine.Materials
{
    public class MaterialData
    {
        public readonly float Metallic;
        public readonly float Roughness;
        public readonly float Emissive;

        public MaterialData(float metallic, float roughness, float emissive)
        {
            Metallic = metallic;
            Roughness = roughness;
            Emissive = emissive;
        }
    }
}

using Terralume.Engine.Buffers.Data;

namespace Terralume.Engine.Constants
{
    public static class RenderConstants
    {
        public static readonly int LIGHT_DATA_RESOLUTION = 32;
        public static readonly int MATERIAL_DATA_RESOLUTION = 256;

        public static readonly LayerParameter LAYER_PARAMETER = new LayerParameter(0.0f, 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f);
        public static readonly DepthParameter DEPTH_PARAMETER = new DepthParameter(0.1f, 0.7f, 0.5f, 0.3f, 0.5f, 0.5f, 0.5f, 0.5f);
    }
}

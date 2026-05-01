using Terralume.Engine.Buffers.Data;

namespace Terralume.Engine.Constants
{
    public static class RenderConstants
    {
        public static readonly int LIGHT_DATA_RESOLUTION = 64;
        public static readonly int LIGHT_SEARCH_TILE_PADDING = 16;

        public static readonly int LIGHT_GRID_TILE_SIZE = 16;
        public static readonly int MAX_LIGHTS_PER_TILE = 255;

        public static readonly float LIGHT_MERGE_RADIUS = 8f;

        public static readonly int MATERIAL_DATA_RESOLUTION = 256;

        public static float GLOBAL_RANGE_MODIFIER = 0f;
        public static float GLOBAL_INTENSITY_MODIFIER = 0f;

        public static readonly LayerParameter LAYER_PARAMETER = new LayerParameter(0.0f, 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f);
        public static readonly DepthParameter DEPTH_PARAMETER = new DepthParameter(0.1f, 0.7f, 0.5f, 0.3f, 0.5f, 0.5f, 0.5f, 0.5f);
    }
}

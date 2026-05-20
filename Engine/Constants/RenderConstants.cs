using Microsoft.Xna.Framework;
using Terralume.Engine.Buffers.Data;
using Terraria;

namespace Terralume.Engine.Constants
{
    public static class RenderConstants
    {
        public static readonly Vector2 SCREEN_SIZE = new Vector2(Main.graphics.GraphicsDevice.PresentationParameters.BackBufferWidth, Main.graphics.GraphicsDevice.PresentationParameters.BackBufferHeight);

        public static readonly Vector3 AMBIENT = new Vector3(0.01f, 0.01f, 0.0125f);

        public static readonly int LIGHT_DATA_RESOLUTION = 64;
        public static readonly int MATERIAL_DATA_RESOLUTION = 256;

        public static readonly int LIGHT_GRID_TILE_SIZE = 16;
        public static readonly int MAX_LIGHTS_PER_TILE = 255;

        public static readonly int LIGHT_SEARCH_TILE_PADDING = 16;
        public static readonly float LIGHT_MERGE_RADIUS = 2f;

        public static readonly float LIGHT_DEPTH = 0.85f;

        public static readonly float DEPTH_SCALE = 100f;

        public static readonly float ALBEDO_VARIATION_FACTOR = 0.15f;
        public static readonly float MATERIAL_VARIATION_FACTOR = 0.3f;
        public static readonly float HEIGHT_VARIATION_FACTOR = 0.1f;

        public static readonly float NORMAL_STRENGTH = 1.0f;

        public static readonly int SSAO_SAMPLE_COUNT = 16;
        public static readonly float SSAO_RADIUS = 16f;
        public static readonly float SSAO_BIAS = 0.025f;
        public static readonly float SSAO_DEPTH_FALLOFF = 10f;

        public static readonly float GLOBAL_RANGE_MODIFIER = 0f;
        public static readonly float GLOBAL_INTENSITY_MODIFIER = 0f;

        public static readonly float TONE_MAPPING_EXPOSURE = 0.8f;

        public static readonly LayerParameter LAYER_PARAMETER = new LayerParameter(0.0f, 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f);
        public static readonly DepthParameter DEPTH_PARAMETER = new DepthParameter(0.1f, 0.7f, 0.5f, 0.3f, 0.5f, 0.5f, 0.5f, 0.5f);
    }
}

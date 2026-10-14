using Microsoft.Xna.Framework;
using Terralume.Engine.Buffers.Data;
using Terralume.Engine.Hooks.Tracking;
using Terraria;

namespace Terralume.Engine.Constants
{
    // Note: Many constants here feed directly into the shading models, the values selected were tuned by eye, therefore changing them can result in a broken look
    internal static class RenderConstants
    {
        public static readonly Vector2 SCREEN_SIZE = new Vector2(Main.graphics.GraphicsDevice.PresentationParameters.BackBufferWidth, Main.graphics.GraphicsDevice.PresentationParameters.BackBufferHeight);

        // TEXTURE PACKING
        public static readonly int LIGHT_DATA_RESOLUTION = 64;
        public static readonly int MATERIAL_DATA_RESOLUTION = 256;
        public static readonly int SUN_OCCLUSION_DATA_RESOLUTION = 256;

        // TILED LIGHTING
        public static readonly int LIGHT_GRID_TILE_SIZE = 16;
        public static readonly int MAX_LIGHTS_PER_TILE = 255;

        // LIGHT PACKING
        public static readonly int LIGHT_SEARCH_TILE_PADDING = 16;
        public static readonly float LIGHT_MERGE_RADIUS = 2f;

        // RADIANCE DEPTH
        public static readonly float LIGHT_DEPTH = 0.85f;
        public static readonly float DEPTH_SCALE = 100f;

        // BACKGROUND BLUR
        public static readonly float BACKGROUND_BLUR_START_DISTANCE = 40f;
        public static readonly float BACKGROUND_BLUR_END_DISTANCE = 150f;

        // NOISE FREQUENCY/AMPLITUDE
        public static readonly Vector2 DEFAULT_NOISE = new Vector2(0.5f, 0.3f);

        // NORMAL MAPPING
        public static readonly float NORMAL_STRENGTH = 0.6f;
        public static readonly float NORMAL_BLUR_DEVIATIONS = 0.5f;

        // TONE MAPPING/GAMMA CORRECTION
        public static readonly float TONE_MAPPING_EXPOSURE = 0.8f;
        public static readonly float GAMMA = 1.9f;

        // BLOOM
        public static readonly float BLOOM_INTENSITY = 1.1f;

        // AMBIENT OCCLUSION
        public static readonly float AO_RADIUS = 40f;
        public static readonly float AO_STRENGTH = 0.001f;


        // TILE OCCLUSION
        public static readonly float TILE_OCCLUSION_FACTOR = 0.08f;


        // POINT LIGHT VOLUMETRIC
        public static readonly float PL_VOLUMETRIC_STRENGTH = 0.04f;
        public static readonly float PL_VOLUMETRIC_RANGE = 0.1f;
        public static readonly float PL_VOLUMETRIC_NOISE_CONTRAST = 0.6f;

        // AMBIENT LIGHTING
        public static readonly float AMBIENT_DIFFUSE_WEIGHT = 0.2f;

        // DIRECTIONAL LIGHT VOLUMETRIC
        public static readonly int DL_MAX_STEP_COUNT = 128;
        public static readonly float DL_VOLUMETRIC_INTENSITY = 1f;
        public static readonly float DL_VOLUMETRIC_ATTENUATION_STRENGTH = 0.5f;

        // LAYER CONFIGURATION
        public static readonly LayerParameter LAYER_PARAMETER = new LayerParameter(0.0f, 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f);
        public static readonly DepthParameter DEPTH_PARAMETER = new DepthParameter(0.1f, 0.7f, 0.5f, 0.3f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f);
    }
}

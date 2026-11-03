using Microsoft.Xna.Framework;
using Terralume.Engine.Buffers.Data;
using Terraria;

namespace Terralume.Engine.Constants
{
    // Note: Many constants here feed directly into the shading models, the values selected were tuned by eye, therefore changing them can result in a broken look
    internal static class RenderConfig
    {
        public static Vector2 ScreenSize => new Vector2(Main.graphics.GraphicsDevice.PresentationParameters.BackBufferWidth, Main.graphics.GraphicsDevice.PresentationParameters.BackBufferHeight);

        // TEXTURE PACKING
        public static int LightDataResolution = 64;
        public static int MaterialDataResolution = 256;
        public static int SunOcclusionDataResolution = 256;

        // TILED LIGHTING
        public static int LightGridTileSize = 16;
        public static int MaxLightsPerTile = 255;

        // LIGHT PACKING
        public static int LightSearchTilePadding = 16;
        public static float LightMergeRadius = 2f;

        // RADIANCE DEPTH
        public static float LightDepth = 0.85f;
        public static float DepthScale = 100f;

        // BACKGROUND BLUR
        public static float BackgroundBlurStartDistance = 40f;
        public static float BackgroundBlurEndDistance = 150f;

        // NOISE FREQUENCY/AMPLITUDE
        public static Vector2 DefaultNoise = new Vector2(0.5f, 0.3f);

        // NORMAL MAPPING
        public static float NormalStrength = 0.6f;
        public static float NormalBlurDeviations = 0.5f;

        // TONE MAPPING/GAMMA CORRECTION
        public static float ToneMappingExposure = 0.8f;
        public static float Gamma = 1.9f;

        // BLOOM
        public static float BloomIntensity = 1.2f;

        // AMBIENT OCCLUSION
        public static float AORadius = 40f;
        public static float AOStrength = 0.001f;
        public static float PlayerAORadius = 8f;
        public static float PlayerAOStrength = 1f;

        // AMBIENT LIGHT FILTER
        public static float AmbientFilterMinLightInfluence = 1000f;
        public static int AmbientFilterMaxTilePassCount = 5;

        // TILE OCCLUSION
        public static float TileOcclusionFactor = 0.08f;

        // ARMOR MATERIALS
        public static bool RenderArmorMaterials = true;

        // POINT LIGHT VOLUMETRIC
        public static float PointLightVolumetricStrength = 0.04f;
        public static float PointLightVolumetricRange = 0.1f;
        public static float PointLightVolumetricNoiseContrast = 0.6f;

        // AMBIENT LIGHTING
        public static float AmbientDiffuseWeight = 0.2f;
        public static Vector3 AmbientFillDirection = new Vector3(-0.4f, -0.6f, 1f);
        public static float AmbientFillStrength = 1f;
        public static float AmbientFillRoughness = 0.55f;

        // SUN OCCLUSION
        public static int SunOcclusionMaxRaySteps = 300;
        public static int SunOcclusionMaxTilePasses = 20;
        public static Vector2 SunOcclusionTilemapPadding = new Vector2(100, 100);


        // ABOVE GROUND BACKGROUND
        public static float AbovegroundBackgroundBlurDeviations = 0.6f;

        // LAYER CONFIGURATION
        public static LayerParameter LayerParameter = new LayerParameter(0.0f, 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f);
        public static DepthParameter DepthParameter = new DepthParameter(0.1f, 0.7f, 0.5f, 0.3f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f);
    }
}

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
        public static readonly int LightDataResolution = 64;
        public static readonly int MaterialDataResolution = 256;
        public static readonly int SunOcclusionDataResolution = 256;

        // TILED LIGHTING
        public static readonly int LightGridTileSize = 16;
        public static readonly int MaxLightsPerTile = 255;

        // LIGHT PACKING
        public static readonly int LightSearchTilePadding = 16;
        public static readonly float LightMergeRadius = 2f;

        // RADIANCE DEPTH
        public static readonly float LightDepth = 0.85f;
        public static readonly float DepthScale = 100f;

        // BACKGROUND BLUR
        public static readonly float BackgroundBlurStartDistance = 40f;
        public static readonly float BackgroundBlurEndDistance = 150f;

        // NOISE FREQUENCY/AMPLITUDE
        public static readonly Vector2 DefaultNoise = new Vector2(0.5f, 0.3f);

        // NORMAL MAPPING
        public static readonly float NormalStrength = 0.6f;
        public static readonly float NormalBlurDeviations = 0.5f;

        // TONE MAPPING/GAMMA CORRECTION
        public static readonly float ToneMappingExposure = 0.8f;
        public static readonly float Gamma = 1.9f;

        // BLOOM
        public static readonly float BloomIntensity = 1.2f;

        // AMBIENT OCCLUSION
        public static readonly float AORadius = 40f;
        public static readonly float AOStrength = 0.001f;
        public static readonly float PlayerAORadius = 8f;
        public static readonly float PlayerAOStrength = 1f;

        // AMBIENT LIGHT FILTER
        public static readonly float AmbientFilterMinLightInfluence = 1000f;
        public static readonly int AmbientFilterMaxTilePassCount = 5;

        // TILE OCCLUSION
        public static readonly float TileOcclusionFactor = 0.08f;


        // POINT LIGHT VOLUMETRIC
        public static readonly float PointLightVolumetricStrength = 0.04f;
        public static readonly float PointLightVolumetricRange = 0.1f;
        public static readonly float PointLightVolumetricNoiseContrast = 0.6f;

        // AMBIENT LIGHTING
        public static readonly float AmbientDiffuseWeight = 0.2f;
        public static readonly Vector3 AmbientFillDirection = new Vector3(-0.4f, -0.6f, 1f);
        public static readonly float AmbientFillStrength = 1f;
        public static readonly float AmbientFillRoughness = 0.55f;

        // SUN OCCLUSION
        public static readonly int SunOcclusionMaxRaySteps = 300;
        public static readonly int SunOcclusionMaxTilePasses = 20;
        public static readonly Vector2 SunOcclusionTilemapPadding = new Vector2(100, 100);


        // ABOVE GROUND BACKGROUND
        public static readonly float AbovegroundBackgroundBlurDeviations = 0.6f;

        // LAYER CONFIGURATION
        public static readonly LayerParameter LayerParameter = new LayerParameter(0.0f, 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f);
        public static readonly DepthParameter DepthParameter = new DepthParameter(0.1f, 0.7f, 0.5f, 0.3f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f);
    }
}

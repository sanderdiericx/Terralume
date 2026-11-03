using System;
using System.ComponentModel;
using Terralume.Engine.Constants;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace Terralume.Configuration
{
    // TO DO: Expose more configuration parameters
    internal class Config : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Range(0.0f, 1.2f), DefaultValue(0.6f)] public float NormalStrength = 0.6f;
        [Range(0.0f, 2.0f), DefaultValue(1.0f)] public float AmbientStrength = 1f;
        [Range(0.0f, 2.0f), DefaultValue(1.2f)] public float BloomIntensity = 1.2f;

        [Range(0.2f, 2.0f), DefaultValue(0.8f)] public float Exposure = 0.8f;
        [Range(1.5f, 2.5f), DefaultValue(1.9f)] public float Gamma = 1.9f;

        [DefaultValue(true)] public bool RenderArmorMaterials = true;

        [DefaultValue(DebugView.Output)] public DebugView DebugView = DebugView.Output;

        public override void OnChanged()
        {
            base.OnChanged();

            UpdateRenderConfig();
        }

        public static void UpdateRenderConfig()
        {
            Config config = ModContent.GetInstance<Config>();

            if (config == null)
            {
                return;
            }

            RenderConfig.NormalStrength = config.NormalStrength;
            RenderConfig.AmbientFillStrength = config.AmbientStrength;
            RenderConfig.BloomIntensity = config.BloomIntensity;
            RenderConfig.ToneMappingExposure = config.Exposure;
            RenderConfig.Gamma = config.Gamma;
            RenderConfig.RenderArmorMaterials = config.RenderArmorMaterials;
            RenderConfig.DebugView = config.DebugView;
        }
    }
}

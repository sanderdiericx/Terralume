using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Terralume.Engine.Shaders
{
    public class EffectManager
    {
        private static readonly string effectPath = "Terralume/Engine/Shaders/Effects/";

        private static readonly Dictionary<EffectType, string> mappedEffects = new Dictionary<EffectType, string>()
        {
            { EffectType.Layer, "layer" },
            { EffectType.Depth, "depth" },
            { EffectType.Material, "material" },
            { EffectType.Normal, "normal" },
            { EffectType.Radiance, "radiance" },
            { EffectType.Output, "output" },
            { EffectType.Noise, "noise" },
            { EffectType.Height, "height" }
        };

        private static Dictionary<EffectType, Asset<Effect>> loadedEffects = new Dictionary<EffectType, Asset<Effect>>();

        public static void LoadEffect(EffectType effectType)
        {
            if (!loadedEffects.ContainsKey(effectType))
            {
                Asset<Effect> asset = ModContent.Request<Effect>($"{effectPath}{mappedEffects[effectType]}", AssetRequestMode.ImmediateLoad);
                loadedEffects.Add(effectType, asset);
            }
        }

        public static Effect GetEffect(EffectType effectType)
        {
            if (loadedEffects.TryGetValue(effectType, out Asset<Effect> asset))
            {
                return asset.Value;
            }

            LoadEffect(effectType);
            return loadedEffects[effectType].Value;
        }
    }
}

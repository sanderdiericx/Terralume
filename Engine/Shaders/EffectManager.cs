using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Terralume.Engine.Shaders
{
    public class EffectManager
    {
        private static readonly string effectPath = "Terralume/Engine/Shaders/Effects/";

        private static readonly Dictionary<EffectType, string> mappedEffects = new Dictionary<EffectType, string>()
        {
            { EffectType.Depth, "depth" },
            { EffectType.Normal, "normal" },
            { EffectType.Material, "material" }
        };

        private static Dictionary<EffectType, Effect> loadedEffects = new Dictionary<EffectType, Effect>();

        public static void LoadEffect(EffectType effectType)
        {
            Asset<Effect> asset = null;

            Main.QueueMainThreadAction(() =>
            {
                asset = ModContent.Request<Effect>($"{effectPath}{mappedEffects[effectType]}");

                if (!loadedEffects.TryAdd(effectType, asset.Value))
                {
                    loadedEffects[effectType] = asset.Value;
                }
            });
        }

        public static Effect GetEffect(EffectType effectType)
        {
            if (!loadedEffects.TryGetValue(effectType, out Effect effect))
            {
                LoadEffect(effectType);

                effect = GetEffect(effectType);
            }

            return effect;
        }
    }
}

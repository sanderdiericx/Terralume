using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terralume.Engine.Materials
{
    public static class MaterialProfile
    {
        public static readonly MaterialData Default = new MaterialData(metallic: 0f, roughness: 0.8f, emissive: 0f);

        // === Natural Ores ===
        public static readonly MaterialData CopperOre = new MaterialData(1f, 0.7f, 0f);
        public static readonly MaterialData IronOre = new MaterialData(1f, 0.65f, 0f);
        public static readonly MaterialData SilverOre = new MaterialData(1f, 0.6f, 0f);
        public static readonly MaterialData GoldOre = new MaterialData(1f, 0.4f, 0f);
        public static readonly MaterialData DemoniteOre = new MaterialData(1f, 0.5f, 0f);
        public static readonly MaterialData CobaltOre = new MaterialData(1f, 0.35f, 0f);
        public static readonly MaterialData MythrilOre = new MaterialData(1f, 0.3f, 0f);
        public static readonly MaterialData AdamantiteOre = new MaterialData(1f, 0.25f, 0f);
        public static readonly MaterialData TitaniumOre = new MaterialData(1f, 0.25f, 0f);
        public static readonly MaterialData ChlorophyteOre = new MaterialData(1f, 0.3f, 0f);
        public static readonly MaterialData PlatinumOre = new MaterialData(1f, 0.3f, 0f);
        public static readonly MaterialData TungstenOre = new MaterialData(1f, 0.7f, 0f);
        public static readonly MaterialData MeteoriteOre = new MaterialData(1f, 0.9f, 0f);

        // === Natural Blocks / Soils ===
        public static readonly MaterialData Dirt = new MaterialData(0f, 0.95f, 0f);
        public static readonly MaterialData Grass = new MaterialData(0f, 0.9f, 0f);
        public static readonly MaterialData Stone = new MaterialData(0f, 0.85f, 0f);
        public static readonly MaterialData Sand = new MaterialData(0f, 0.8f, 0f);
        public static readonly MaterialData Clay = new MaterialData(0f, 0.85f, 0f);
        public static readonly MaterialData Mud = new MaterialData(0f, 0.9f, 0f);
        public static readonly MaterialData Snow = new MaterialData(0f, 0.9f, 0f);
        public static readonly MaterialData Ice = new MaterialData(0f, 0.2f, 0f);

        // === Special Biome Blocks ===
        public static readonly MaterialData Ebonstone = new MaterialData(0f, 0.8f, 0f);
        public static readonly MaterialData Crimstone = new MaterialData(0f, 0.8f, 0f);
        public static readonly MaterialData Pearlstone = new MaterialData(0f, 0.8f, 0f);
        public static readonly MaterialData Ebonsand = new MaterialData(0f, 0.85f, 0f);
        public static readonly MaterialData Crimsand = new MaterialData(0f, 0.85f, 0f);
        public static readonly MaterialData Pearlsand = new MaterialData(0f, 0.85f, 0f);

        // === Wood Types (Basic) ===
        public static readonly MaterialData Wood = new MaterialData(0f, 0.6f, 0f);
        public static readonly MaterialData BorealWood = new MaterialData(0f, 0.6f, 0f);
        public static readonly MaterialData RichMahogany = new MaterialData(0f, 0.6f, 0f);
        public static readonly MaterialData PalmWood = new MaterialData(0f, 0.6f, 0f);
        public static readonly MaterialData Ebonwood = new MaterialData(0f, 0.6f, 0f);
        public static readonly MaterialData Shadewood = new MaterialData(0f, 0.6f, 0f);
        public static readonly MaterialData Pearlwood = new MaterialData(0f, 0.6f, 0f);

        // === Gems & Precious Stones ===
        public static readonly MaterialData Amethyst = new MaterialData(0f, 0.2f, 0f);
        public static readonly MaterialData Topaz = new MaterialData(0f, 0.2f, 0f);
        public static readonly MaterialData Sapphire = new MaterialData(0f, 0.2f, 0f);
        public static readonly MaterialData Emerald = new MaterialData(0f, 0.2f, 0f);
        public static readonly MaterialData Ruby = new MaterialData(0f, 0.2f, 0f);
        public static readonly MaterialData Diamond = new MaterialData(0f, 0.05f, 0f);
        public static readonly MaterialData Amber = new MaterialData(0f, 0.3f, 0f);

        // === Bricks & Crafted Blocks ===
        public static readonly MaterialData CopperBrick = new MaterialData(1f, 0.6f, 0f);
        public static readonly MaterialData SilverBrick = new MaterialData(1f, 0.55f, 0f);
        public static readonly MaterialData GoldBrick = new MaterialData(1f, 0.35f, 0f);
        public static readonly MaterialData ObsidianBrick = new MaterialData(0f, 0.7f, 0f);
        public static readonly MaterialData PearlstoneBrick = new MaterialData(0f, 0.8f, 0f);

        // === Liquids & Emissive ===
        public static readonly MaterialData Water = new MaterialData(0f, 0.05f, 0f);
        public static readonly MaterialData Lava = new MaterialData(0f, 0.1f, 1f);
    }
}

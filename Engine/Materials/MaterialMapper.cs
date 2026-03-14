using System.Collections.Generic;
using Terraria.ID;

namespace Terralume.Engine.Materials
{
    public static class MaterialMapper
    {
        // Maps Terraria tile IDs to their respective MaterialData
        public static Dictionary<int, MaterialData> Tiles = new Dictionary<int, MaterialData>()
        {
            // Natural soils & stone
            { TileID.Dirt,             MaterialProfile.Dirt },
            { TileID.Grass,            MaterialProfile.Grass },
            { TileID.Stone,            MaterialProfile.Stone },
            { TileID.Sand,             MaterialProfile.Sand },
            { TileID.ClayBlock,        MaterialProfile.Clay },
            { TileID.Mud,              MaterialProfile.Mud },
            { TileID.SnowBlock,        MaterialProfile.Snow },
            { TileID.IceBlock,         MaterialProfile.Ice },

            // Ores
            { TileID.Copper,           MaterialProfile.CopperOre },
            { TileID.Iron,             MaterialProfile.IronOre },
            { TileID.Silver,           MaterialProfile.SilverOre },
            { TileID.Gold,             MaterialProfile.GoldOre },
            { TileID.Demonite,         MaterialProfile.DemoniteOre },
            { TileID.Meteorite,        MaterialProfile.MeteoriteOre },

            { TileID.Cobalt,           MaterialProfile.CobaltOre },
            { TileID.Mythril,          MaterialProfile.MythrilOre },
            { TileID.Adamantite,       MaterialProfile.AdamantiteOre },
            { TileID.Platinum,         MaterialProfile.PlatinumOre },
            { TileID.Titanium,         MaterialProfile.TitaniumOre },
            { TileID.Chlorophyte,      MaterialProfile.ChlorophyteOre },

            // Wood / organic
            { TileID.WoodBlock,        MaterialProfile.Wood },
            { TileID.BorealWood,       MaterialProfile.BorealWood },
            { TileID.PalmWood,         MaterialProfile.PalmWood },
            { TileID.Ebonwood,         MaterialProfile.Wood },
            { TileID.Pearlwood,        MaterialProfile.Pearlwood },

            // Gems
            { TileID.Amethyst,         MaterialProfile.Amethyst },
            { TileID.Topaz,            MaterialProfile.Topaz },
            { TileID.Sapphire,         MaterialProfile.Sapphire },
            { TileID.Emerald,          MaterialProfile.Emerald },
            { TileID.Ruby,             MaterialProfile.Ruby },
            { TileID.Diamond,          MaterialProfile.Diamond },
            { TileID.AmberStoneBlock,  MaterialProfile.Amber },

            // Bricks & building blocks
            { TileID.CopperBrick,      MaterialProfile.CopperBrick },
            { TileID.SilverBrick,      MaterialProfile.SilverBrick },
            { TileID.GoldBrick,        MaterialProfile.GoldBrick },
            { TileID.ObsidianBrick,    MaterialProfile.ObsidianBrick },
            { TileID.PearlstoneBrick,  MaterialProfile.PearlstoneBrick },

            // Biome stone variants
            { TileID.Ebonstone,        MaterialProfile.Ebonstone },
            { TileID.Crimstone,        MaterialProfile.Crimstone },
            { TileID.Pearlstone,       MaterialProfile.Pearlstone },
        };


        // Maps Terraria liquid IDs to their respective MaterialData
        public static Dictionary<int, MaterialData> Liquids = new Dictionary<int, MaterialData>()
        {
            { LiquidID.Water,          MaterialProfile.Water },
            { LiquidID.Lava,           MaterialProfile.Lava }
        };

    }
}
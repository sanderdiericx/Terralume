using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terralume.Engine.Mapping.Registry;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Terralume.Engine.Hooks.Tracking
{
    internal class PlayerMaterialTracker : ModSystem
    {
        public static Dictionary<Vector3, List<DrawData>> DrawData { get; private set; } = new Dictionary<Vector3, List<DrawData>>();

        public override void Load()
        {
            base.Load();

            On_PlayerDrawLayers.DrawPlayer_21_Head += On_PlayerDrawLayers_DrawPlayer_21_Head;
            On_PlayerDrawLayers.DrawPlayer_17_Torso += On_PlayerDrawLayers_DrawPlayer_17_Torso;
            On_PlayerDrawLayers.DrawPlayer_13_Leggings += On_PlayerDrawLayers_DrawPlayer_13_Leggings;
            On_PlayerDrawLayers.DrawPlayer_28_ArmOverItem += On_PlayerDrawLayers_DrawPlayer_28_ArmOverItem;
            On_PlayerDrawLayers.DrawPlayer_12_SkinComposite_BackArmShirt += On_PlayerDrawLayers_DrawPlayer_12_SkinComposite_BackArmShirt;
        }

        public override void Unload()
        {
            base.Unload();

            On_PlayerDrawLayers.DrawPlayer_21_Head -= On_PlayerDrawLayers_DrawPlayer_21_Head;
            On_PlayerDrawLayers.DrawPlayer_17_Torso -= On_PlayerDrawLayers_DrawPlayer_17_Torso;
            On_PlayerDrawLayers.DrawPlayer_13_Leggings -= On_PlayerDrawLayers_DrawPlayer_13_Leggings;
            On_PlayerDrawLayers.DrawPlayer_28_ArmOverItem -= On_PlayerDrawLayers_DrawPlayer_28_ArmOverItem;
            On_PlayerDrawLayers.DrawPlayer_12_SkinComposite_BackArmShirt += On_PlayerDrawLayers_DrawPlayer_12_SkinComposite_BackArmShirt;
        }

        public override void PreUpdatePlayers()
        {
            base.PreUpdatePlayers();

            DrawData.Clear();
        }

        private void On_PlayerDrawLayers_DrawPlayer_12_SkinComposite_BackArmShirt(On_PlayerDrawLayers.orig_DrawPlayer_12_SkinComposite_BackArmShirt orig, ref PlayerDrawSet drawinfo)
        {
            int countBefore = drawinfo.DrawDataCache.Count;

            orig(ref drawinfo);

            if (drawinfo.shadow != 0f)
            {
                return;
            }

            int itemType = drawinfo.drawPlayer.armor[1].type;
            if (!PlayerMaterialMapper.MappedMaterials.TryGetValue(itemType, out Vector3 material))
            {
                return;
            }

            SaveDrawItems(ref drawinfo, material, countBefore);
        }

        private void On_PlayerDrawLayers_DrawPlayer_21_Head(On_PlayerDrawLayers.orig_DrawPlayer_21_Head orig, ref PlayerDrawSet drawinfo)
        {
            int countBefore = drawinfo.DrawDataCache.Count;

            orig(ref drawinfo);

            if (drawinfo.shadow != 0f)
            {
                return;
            }

            int itemType = drawinfo.drawPlayer.armor[0].type;
            if (!PlayerMaterialMapper.MappedMaterials.TryGetValue(itemType, out Vector3 material))
            {
                return;
            }

            int headSlot = drawinfo.drawPlayer.head;
            if (headSlot < 0 || headSlot >= TextureAssets.ArmorHead.Length)
            {
                return;
            }

            Texture2D expectedTexture = TextureAssets.ArmorHead[headSlot].Value;

            SaveDrawItems(ref drawinfo, material, countBefore, expectedTexture);
        }

        private void On_PlayerDrawLayers_DrawPlayer_17_Torso(On_PlayerDrawLayers.orig_DrawPlayer_17_Torso orig, ref PlayerDrawSet drawinfo)
        {
            int countBefore = drawinfo.DrawDataCache.Count;

            orig(ref drawinfo);

            if (drawinfo.shadow != 0f)
            {
                return;
            }

            int itemType = drawinfo.drawPlayer.armor[1].type;
            if (!PlayerMaterialMapper.MappedMaterials.TryGetValue(itemType, out Vector3 material))
            {
                return;
            }

            SaveDrawItems(ref drawinfo, material, countBefore);
        }

        private void On_PlayerDrawLayers_DrawPlayer_13_Leggings(On_PlayerDrawLayers.orig_DrawPlayer_13_Leggings orig, ref PlayerDrawSet drawinfo)
        {
            int countBefore = drawinfo.DrawDataCache.Count;

            orig(ref drawinfo);

            if (drawinfo.shadow != 0f)
            {
                return;
            }

            int itemType = drawinfo.drawPlayer.armor[2].type;
            if (!PlayerMaterialMapper.MappedMaterials.TryGetValue(itemType, out Vector3 material))
            {
                return;
            }

            int legSlot = drawinfo.drawPlayer.legs;
            if (legSlot < 0 || legSlot >= TextureAssets.ArmorLeg.Length)
            {
                return;
            }

            Texture2D expectedTexture = TextureAssets.ArmorLeg[legSlot].Value;

            SaveDrawItems(ref drawinfo, material, countBefore, expectedTexture);
        }


        private void On_PlayerDrawLayers_DrawPlayer_28_ArmOverItem(On_PlayerDrawLayers.orig_DrawPlayer_28_ArmOverItem orig, ref PlayerDrawSet drawinfo)
        {
            int countBefore = drawinfo.DrawDataCache.Count;

            orig(ref drawinfo);

            if (drawinfo.shadow != 0f)
            {
                return;
            }

            int itemType = drawinfo.drawPlayer.armor[1].type;
            if (!PlayerMaterialMapper.MappedMaterials.TryGetValue(itemType, out Vector3 material))
            {
                return;
            }

            SaveDrawItems(ref drawinfo, material, countBefore);
        }

        private void SaveDrawItems(ref PlayerDrawSet drawinfo, Vector3 material, int countBefore, Texture2D expectedTexture)
        {
            List<DrawData> drawItems = new List<DrawData>();

            for (int i = countBefore; i < drawinfo.DrawDataCache.Count; i++)
            {
                DrawData drawItem = drawinfo.DrawDataCache[i];

                if (drawItem.texture != expectedTexture)
                {
                    continue;
                }

                drawItems.Add(drawItem);
            }

            if (drawItems.Count == 0)
            {
                return;
            }

            if (!DrawData.TryAdd(material, drawItems))
            {
                DrawData[material].AddRange(drawItems);
            }
        }

        private void SaveDrawItems(ref PlayerDrawSet drawinfo, Vector3 material, int countBefore)
        {
            List<DrawData> drawItems = new List<DrawData>();

            for (int i = countBefore; i < drawinfo.DrawDataCache.Count; i++)
            {
                DrawData drawItem = drawinfo.DrawDataCache[i];

                drawItems.Add(drawItem);
            }

            if (drawItems.Count == 0)
            {
                return;
            }

            if (!DrawData.TryAdd(material, drawItems))
            {
                DrawData[material].AddRange(drawItems);
            }
        }
    }
}
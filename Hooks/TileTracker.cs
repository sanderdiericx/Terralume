using Terralume.Engine.Mapping;
using Terraria;
using Terraria.ModLoader;

namespace Terralume.Hooks
{
    class TileTracker : GlobalTile
    {
        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            base.KillTile(i, j, type, ref fail, ref effectOnly, ref noItem);

            if (fail || effectOnly)
            {
                return;
            }

            SunOcclusionTracker.DirtyColumns.Add(i);
        }

        public override void PlaceInWorld(int i, int j, int type, Item item)
        {
            base.PlaceInWorld(i, j, type, item);

            SunOcclusionTracker.DirtyColumns.Add(i);
        }
    }
}

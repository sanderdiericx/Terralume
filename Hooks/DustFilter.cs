using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terralume.Hooks
{
    public class DustFilter : ModSystem
    {
        // Make sure ores dont create light casting dust
        public override void PostSetupContent()
        {
            for (int i = 0; i < TileID.Count; i++)
            {
                if (TileID.Sets.Ore[i] && Main.tileShine[i] > 0)
                {
                    Main.tileShine[i] = 0;
                }
            }
        }
    }
}

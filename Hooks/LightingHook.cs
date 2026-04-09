using Terraria.ModLoader;

namespace Terralume.Hooks
{
    public class LightingHook : GlobalWall
    {
        // Cancel out all lighting influence
        public override void ModifyLight(int i, int j, int type, ref float r, ref float g, ref float b)
        {
            r = 1.0f;
            g = 1.0f;
            b = 1.0f;
        }
    }
}

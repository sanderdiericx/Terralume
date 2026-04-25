using Microsoft.Xna.Framework.Graphics;

namespace Terralume.Engine.Buffers.Data
{
    public class DepthParameter
    {
        public float BackgroundDepth;
        public float TileDepth;
        public float NonSolidTileDepth;
        public float WallDepth;
        public float EntityDepth;
        public float PlayerDepth;
        public float ProjectileDepth;
        public float OtherDepth;

        public DepthParameter(float backgroundDepth, float tileDepth, float nonSolidTileDepth, float wallDepth, float entityDepth, float playerDepth, float projectileDepth, float otherDepth)
        {
            BackgroundDepth = backgroundDepth;
            TileDepth = tileDepth;
            NonSolidTileDepth = nonSolidTileDepth;
            WallDepth = wallDepth;
            EntityDepth = entityDepth;
            PlayerDepth = playerDepth;
            ProjectileDepth = projectileDepth;
            OtherDepth = otherDepth;
        }

        public void SetUniforms(Effect effect)
        {
            effect.Parameters["DepthParameter_BackgroundDepth"].SetValue(BackgroundDepth);
            effect.Parameters["DepthParameter_TileDepth"].SetValue(TileDepth);
            effect.Parameters["DepthParameter_NonSolidTileDepth"].SetValue(NonSolidTileDepth);
            effect.Parameters["DepthParameter_WallDepth"].SetValue(WallDepth);
            effect.Parameters["DepthParameter_EntityDepth"].SetValue(EntityDepth);
            effect.Parameters["DepthParameter_PlayerDepth"].SetValue(PlayerDepth);
            effect.Parameters["DepthParameter_ProjectileDepth"].SetValue(ProjectileDepth);
            effect.Parameters["DepthParameter_OtherDepth"].SetValue(OtherDepth);
        }
    }
}

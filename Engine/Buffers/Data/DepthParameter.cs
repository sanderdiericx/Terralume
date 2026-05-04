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
        public float ProjectileDepth;
        public float PlayerDepth;
        public float OtherDepth;

        public DepthParameter(float backgroundDepth, float tileDepth, float nonSolidTileDepth, float wallDepth, float entityDepth, float projectileDepth, float playerDepth, float otherDepth)
        {
            BackgroundDepth = backgroundDepth;
            TileDepth = tileDepth;
            NonSolidTileDepth = nonSolidTileDepth;
            WallDepth = wallDepth;
            EntityDepth = entityDepth;
            ProjectileDepth = projectileDepth;
            PlayerDepth = playerDepth;
            OtherDepth = otherDepth;
        }

        public void SetUniforms(Effect effect)
        {
            effect.Parameters["BackgroundDepth"].SetValue(BackgroundDepth);
            effect.Parameters["TileDepth"].SetValue(TileDepth);
            effect.Parameters["NonSolidTileDepth"].SetValue(NonSolidTileDepth);
            effect.Parameters["WallDepth"].SetValue(WallDepth);
            effect.Parameters["EntityDepth"].SetValue(EntityDepth);
            effect.Parameters["ProjectileDepth"].SetValue(ProjectileDepth);
            effect.Parameters["PlayerDepth"].SetValue(PlayerDepth);
            effect.Parameters["OtherDepth"].SetValue(OtherDepth);
        }
    }
}

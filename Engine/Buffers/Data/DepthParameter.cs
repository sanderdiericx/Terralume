using Microsoft.Xna.Framework.Graphics;

namespace Terralume.Engine.Buffers.Data
{
    internal class DepthParameter
    {
        public float BackgroundDepth { get; private set; }
        public float TileDepth { get; private set; }
        public float NonSolidTileDepth { get; private set; }
        public float WallDepth { get; private set; }
        public float EntityDepth { get; private set; }
        public float ProjectileDepth { get; private set; }
        public float PlayerDepth { get; private set; }
        public float LiquidDepth { get; private set; }
        public float OtherDepth { get; private set; }

        public DepthParameter(float backgroundDepth, float tileDepth, float nonSolidTileDepth, float wallDepth, float entityDepth, float projectileDepth, float playerDepth, float liquidDepth, float otherDepth)
        {
            BackgroundDepth = backgroundDepth;
            TileDepth = tileDepth;
            NonSolidTileDepth = nonSolidTileDepth;
            WallDepth = wallDepth;
            EntityDepth = entityDepth;
            ProjectileDepth = projectileDepth;
            PlayerDepth = playerDepth;
            LiquidDepth = liquidDepth;
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
            effect.Parameters["LiquidDepth"].SetValue(LiquidDepth);
            effect.Parameters["OtherDepth"].SetValue(OtherDepth);
        }
    }
}

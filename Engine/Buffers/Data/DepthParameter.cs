namespace Terralume.Engine.Buffers.Data
{
    public struct DepthParameter
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
    }
}

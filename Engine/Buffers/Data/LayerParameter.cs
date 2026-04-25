namespace Terralume.Engine.Buffers.Data
{
    public struct LayerParameter
    {
        public float Background;
        public float Tile;
        public float NonSolidTile;
        public float Wall;
        public float Entity;
        public float Player;
        public float Projectile;
        public float Other;

        public LayerParameter(float background, float tile, float nonSolidTile, float wall, float entity, float player, float projectile, float other)
        {
            Background = background;
            Tile = tile;
            NonSolidTile = nonSolidTile;
            Wall = wall;
            Entity = entity;
            Player = player;
            Projectile = projectile;
            Other = other;
        }
    }
}

using Microsoft.Xna.Framework.Graphics;

namespace Terralume.Engine.Buffers.Data
{
    internal class LayerParameter
    {
        public float Background { get; private set; }
        public float Tile { get; private set; }
        public float NonSolidTile { get; private set; }
        public float Wall { get; private set; }
        public float Entity { get; private set; }
        public float Projectile { get; private set; }
        public float Player { get; private set; }
        public float Other { get; private set; }

        public LayerParameter(float background, float tile, float nonSolidTile, float wall, float entity, float projectile, float player, float other)
        {
            Background = background;
            Tile = tile;
            NonSolidTile = nonSolidTile;
            Wall = wall;
            Entity = entity;
            Projectile = projectile;
            Player = player;
            Other = other;
        }

        public void SetUniforms(Effect effect)
        {
            effect.Parameters["BackgroundLayer"].SetValue(Background);
            effect.Parameters["TileLayer"].SetValue(Tile);
            effect.Parameters["NonSolidTileLayer"].SetValue(NonSolidTile);
            effect.Parameters["WallLayer"].SetValue(Wall);
            effect.Parameters["EntityLayer"].SetValue(Entity);
            effect.Parameters["ProjectileLayer"].SetValue(Projectile);
            effect.Parameters["PlayerLayer"].SetValue(Player);
            effect.Parameters["OtherLayer"].SetValue(Other);
        }
    }
}

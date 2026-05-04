using Microsoft.Xna.Framework.Graphics;

namespace Terralume.Engine.Buffers.Data
{
    public class LayerParameter
    {
        public float Background;
        public float Tile;
        public float NonSolidTile;
        public float Wall;
        public float Entity;
        public float Projectile;
        public float Player;
        public float Other;

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

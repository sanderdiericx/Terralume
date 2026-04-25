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

        public void SetUniforms(Effect effect)
        {
            effect.Parameters["LayerParameter_Background"].SetValue(Background);
            effect.Parameters["LayerParameter_Tile"].SetValue(Tile);
            effect.Parameters["LayerParameter_NonSolidTile"].SetValue(NonSolidTile);
            effect.Parameters["LayerParameter_Wall"].SetValue(Wall);
            effect.Parameters["LayerParameter_Entity"].SetValue(Entity);
            effect.Parameters["LayerParameter_Player"].SetValue(Player);
            effect.Parameters["LayerParameter_Projectile"].SetValue(Projectile);
            effect.Parameters["LayerParameter_Other"].SetValue(Other);
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terralume.Engine.Buffers;
using Terraria;
using Terraria.GameContent;
using static System.Net.Mime.MediaTypeNames;

namespace Terralume.Engine.Render
{
    internal static class TextRenderer
    {
        public static void RenderCombatText(SpriteBatch spriteBatch, OutputCollection outputCollection)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RenderTargetBinding[] oldTargets = device.GetRenderTargets();

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, Main.GameViewMatrix.ZoomMatrix);

            foreach (var text in Main.combatText)
            {
                if (!text.active)
                {
                    continue;
                }

                var font = FontAssets.CombatText[text.crit ? 1 : 0].Value;
                var origin = font.MeasureString(text.text) * 0.5f;
                var position = text.position - Main.screenPosition + origin;

                spriteBatch.DrawString(font, text.text, position, text.color * text.alpha, text.rotation, origin, text.scale, SpriteEffects.None, 0f);
            }

            spriteBatch.End();


            device.SetRenderTargets(oldTargets);
        }
    }
}

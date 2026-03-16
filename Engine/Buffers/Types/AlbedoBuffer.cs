using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;

namespace Terralume.Engine.Buffers.Types
{
    public class AlbedoBuffer
    {
        public RenderTarget2D RenderTarget;
        
        private GraphicsDevice graphicsDevice;
        private SpriteBatch spriteBatch;

        public AlbedoBuffer(GraphicsDevice graphicsDevice, int width, int height)
        {
            RenderTarget = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.None);
        
            this.graphicsDevice = graphicsDevice;
            spriteBatch = new SpriteBatch(graphicsDevice);
        }

        
        public void Render()
        {
            // Set custom RenderTarget
            graphicsDevice.SetRenderTarget(RenderTarget);
            graphicsDevice.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            RedrawFrame(spriteBatch);

            spriteBatch.End();

            // Reset back to main RenderTarget
            graphicsDevice.SetRenderTargets(Main.screenTarget);
        }


        // Draws the world (tiles, walls, liquids, NPCs, players) into the albedo buffer, ignoring lighting.
        private void RedrawFrame(SpriteBatch spriteBatch)
        {
            // Screen bounds in tiles
            int screenTilesX = Main.screenWidth / 16 + 2;
            int screenTilesY = Main.screenHeight / 16 + 2;
            int startX = (int)(Main.screenPosition.X / 16f) - 1;
            int startY = (int)(Main.screenPosition.Y / 16f) - 1;

            startX = Utils.Clamp(startX, 0, Main.maxTilesX - 1);
            startY = Utils.Clamp(startY, 0, Main.maxTilesY - 1);

            // Draw tiles and walls
            for (int x = startX; x < startX + screenTilesX; x++)
            {
                for (int y = startY; y < startY + screenTilesY; y++)
                {
                    Tile tile = Main.tile[x, y];

                    // Draw tils
                    if (tile.HasTile)
                    {
                        Texture2D tex = TextureAssets.Tile[tile.TileType].Value;
                        Vector2 pos = new Vector2(x * 16 - Main.screenPosition.X, y * 16 - Main.screenPosition.Y);
                        spriteBatch.Draw(tex, pos, Color.White);
                    }

                    // Draw walls
                    if (tile.WallType > 0)
                    {
                        Texture2D wallTex = TextureAssets.Wall[tile.WallType].Value;
                        Vector2 wallPos = new Vector2(x * 16 - Main.screenPosition.X, y * 16 - Main.screenPosition.Y);
                        spriteBatch.Draw(wallTex, wallPos, Color.White);
                    }

                    // Draw liquids
                    if (tile.LiquidAmount > 0)
                    {
                        Texture2D liquidTex = TextureAssets.Liquid[tile.LiquidType].Value;
                        Vector2 liquidPos = new Vector2(x * 16 - Main.screenPosition.X, y * 16 - Main.screenPosition.Y);
                        Rectangle sourceRect = new Rectangle(0, 0, 16, 16);
                        spriteBatch.Draw(liquidTex, liquidPos, sourceRect, Color.White);
                    }
                }
            }

            // Draw NPCs
            foreach (NPC npc in Main.npc)
            {
                if (!npc.active)
                {
                    continue;
                }

                Texture2D npcTexture = (Texture2D)TextureAssets.Npc[npc.type];

                int frameHeight = npcTexture.Height / Main.npcFrameCount[npc.type];

                Rectangle sourceRect = new Rectangle(0, frameHeight * npc.frame.Y, npcTexture.Width, frameHeight);

                spriteBatch.Draw(npcTexture, sourceRect, Color.White);
            }

            // Draw players
            foreach (Player player in Main.player)
            {
                if (!player.active)
                {
                    continue;
                }

                Vector2 drawPos = player.Center - Main.screenPosition;

                int skinVar = player.skinVariant;

                // Legs
                Texture2D legsTex = TextureAssets.Players[skinVar, 0].Value;
                Rectangle legsRect = player.legFrame;
                spriteBatch.Draw(
                    legsTex,
                    drawPos,
                    legsRect,
                    Color.White,
                    player.legRotation,
                    legsRect.Size() / 2f,
                    1f,
                    SpriteEffects.None,
                    0f
                );

                // Body
                Texture2D bodyTex = TextureAssets.Players[skinVar, 1].Value;
                Rectangle bodyRect = player.bodyFrame;
                spriteBatch.Draw(
                    bodyTex,
                    drawPos,
                    bodyRect,
                    Color.White,
                    player.bodyRotation,
                    bodyRect.Size() / 2f,
                    1f,
                    SpriteEffects.None,
                    0f
                );

                // Head
                Texture2D headTex = TextureAssets.Players[skinVar, 0].Value;
                Rectangle headRect = player.headFrame;
                spriteBatch.Draw(
                    headTex,
                    drawPos,
                    headRect,
                    Color.White,
                    player.headRotation,
                    headRect.Size() / 2f,
                    1f,
                    SpriteEffects.None,
                    0f
                );

                // Hair
                Texture2D hairTex = TextureAssets.PlayerHair[player.hair].Value;
                Rectangle hairRect = player.headFrame;
                spriteBatch.Draw(
                    hairTex,
                    drawPos,
                    hairRect,
                    Color.White,
                    player.headRotation,
                    hairRect.Size() / 2f,
                    1f,
                    SpriteEffects.None,
                    0f
                );
            }
        }
    }
}
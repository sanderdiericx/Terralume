using Humanizer;
using Terralume.Engine.Buffers;
using Terralume.Engine.Buffers.Types;
using Terraria;
using Terraria.ModLoader;

namespace Terralume.RenderHooks
{
    public class RenderHook : ModSystem
    {
        private BufferManager bufferManager;

        public override void OnModLoad()
        {
            base.OnModLoad();

            bufferManager = new BufferManager(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
        }


        public override void PostDrawTiles()
        {
            base.PostDrawTiles();

            AlbedoBuffer albedoBuffer = bufferManager.AlbedoBuffer;

            albedoBuffer.Render(Main.spriteBatch, Main.DefaultSamplerState, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}

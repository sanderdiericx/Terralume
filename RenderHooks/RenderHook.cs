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

            // Graphics related tasks must be performed on the main thread
            Main.QueueMainThreadAction(() =>
            {
                bufferManager = new BufferManager(Main.graphics.GraphicsDevice, Main.graphics.GraphicsDevice.PresentationParameters.BackBufferWidth, Main.graphics.GraphicsDevice.PresentationParameters.BackBufferHeight);
            });
        }


        public override void PostDrawTiles()
        {
            base.PostDrawTiles();

            // TO DO, CHECK FOR RESIZE, ON RESIZE BUILD NEW RENDERTARGETS

            AlbedoBuffer albedoBuffer = bufferManager.AlbedoBuffer;

            albedoBuffer.Render();
        }
    }
}

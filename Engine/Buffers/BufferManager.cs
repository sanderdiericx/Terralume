using Microsoft.Xna.Framework.Graphics;
using Terralume.Engine.Buffers.Types;

namespace Terralume.Engine.Buffers
{
    public class BufferManager
    {
        public AlbedoBuffer AlbedoBuffer;

        public BufferManager(GraphicsDevice graphicsDevice, int width, int height)
        {
            AlbedoBuffer = new AlbedoBuffer(graphicsDevice, width, height);
        }
    }
}

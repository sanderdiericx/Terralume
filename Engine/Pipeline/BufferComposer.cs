using Microsoft.Xna.Framework.Graphics;
using Terralume.Engine.Buffers;
using Terralume.Engine.Buffers.Data;
using Terralume.Engine.Materials;
using Terralume.Engine.Render;
using Terralume.Engine.Shaders;
using Terralume.Engine.Utils;
using Terraria;
using Terraria.ModLoader;

namespace Terralume.Engine.Pipeline
{
    public class BufferComposer : ModSystem
    {
        public static BufferCollection BufferCollection;
        private static SpriteBatch deferredSpriteBatch;

        private static readonly LayerParameter layerParameter = new LayerParameter(0.0f, 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f);
        private static readonly DepthParameter depthParameter = new DepthParameter(0.9f, 0.3f, 0.5f, 0.7f, 0.5f, 0.5f, 0.5f, 0.5f);

        public override void Load()
        {
            Main.QueueMainThreadAction(() =>
            {
                BufferCollection = new BufferCollection(Main.graphics.GraphicsDevice, Main.graphics.GraphicsDevice.PresentationParameters.BackBufferWidth, Main.graphics.GraphicsDevice.PresentationParameters.BackBufferHeight);
                deferredSpriteBatch = new SpriteBatch(Main.graphics.GraphicsDevice);

                EffectManager.LoadEffect(EffectType.Layer);
                EffectManager.LoadEffect(EffectType.Depth);
                EffectManager.LoadEffect(EffectType.Material);
                EffectManager.LoadEffect(EffectType.Normal);
            });
        }

        public override void Unload()
        {
            base.Unload();

            Main.QueueMainThreadAction(() =>
            {
                BufferCollection.Dispose();
            });
        }

        // Must be called at On_Main.DrawInterface!
        public static void ComposeGBuffer()
        {
            UpdateBuffer();

            TargetCapturer.CaptureInterfaceTargets();
            TargetCollection targetCollection = TargetCapturer.TargetCollection;

            short[] tileIDs = TileHelper.GetVisibleTiles();

            TileHelper.PackTileIDTexture(BufferCollection.TileIDTexture, tileIDs);
            TileHelper.PackMaterialTexture(BufferCollection.MaterialTexture, MaterialMapper.MappedTiles);

            BufferRenderer.RenderLayerBuffer(deferredSpriteBatch, BufferCollection, targetCollection, layerParameter);

            BufferRenderer.RenderDepthBuffer(deferredSpriteBatch, BufferCollection, layerParameter, depthParameter);
            BufferRenderer.RenderMaterialBuffer(deferredSpriteBatch, BufferCollection, layerParameter);
            BufferRenderer.RenderNormalBuffer(deferredSpriteBatch, BufferCollection);
            BufferRenderer.RenderAlbedoBuffer(deferredSpriteBatch, BufferCollection, targetCollection);
        }

        // Make sure all buffers match screen size
        private static void UpdateBuffer()
        {
            if (!BufferCollection.IsValid())
            {
                BufferCollection.Dispose();
                BufferCollection = new BufferCollection(Main.graphics.GraphicsDevice, Main.graphics.GraphicsDevice.PresentationParameters.BackBufferWidth, Main.graphics.GraphicsDevice.PresentationParameters.BackBufferHeight);
            }
        }
    }
}
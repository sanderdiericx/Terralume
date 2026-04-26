using Microsoft.Xna.Framework.Graphics;
using Terralume.Engine.Buffers;
using Terralume.Engine.Buffers.Data;
using Terralume.Engine.Constants;
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

        public override void Load()
        {
            Main.QueueMainThreadAction(() =>
            {
                BufferCollection = new BufferCollection(Main.graphics.GraphicsDevice, Main.graphics.GraphicsDevice.PresentationParameters.BackBufferWidth, Main.graphics.GraphicsDevice.PresentationParameters.BackBufferHeight);

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
        public static void ComposeGBuffer(SpriteBatch spriteBatch)
        {
            UpdateBuffer();

            TargetCapturer.CaptureInterfaceTargets();
            TargetCollection targetCollection = TargetCapturer.TargetCollection;

            LayerParameter layerParameter = RenderConstants.LAYER_PARAMETER;
            DepthParameter depthParameter = RenderConstants.DEPTH_PARAMETER;

            short[] tileIDs = TileHelper.GetVisibleTiles();

            TileHelper.PackTileIDTexture(BufferCollection.TileIDTexture, tileIDs);
            TileHelper.PackMaterialTexture(BufferCollection.MaterialTexture, MaterialMapper.MappedTiles);

            BufferRenderer.RenderLayerBuffer(spriteBatch, BufferCollection, targetCollection, layerParameter);

            BufferRenderer.RenderDepthBuffer(spriteBatch, BufferCollection, layerParameter, depthParameter);
            BufferRenderer.RenderMaterialBuffer(spriteBatch, BufferCollection, layerParameter);
            BufferRenderer.RenderNormalBuffer(spriteBatch, BufferCollection);
            BufferRenderer.RenderAlbedoBuffer(spriteBatch, BufferCollection, targetCollection);
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
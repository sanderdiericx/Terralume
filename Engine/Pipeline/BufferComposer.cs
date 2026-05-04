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
                EffectManager.LoadEffect(EffectType.Noise);
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

        // Must be called at On_Main.DrawInterface to not break :(
        public static void ComposeGBuffer(SpriteBatch spriteBatch, TileCaptureParameter tileCaptureParameter, ushort[] tileIDs, ushort[] wallIDs)
        {
            UpdateBuffer();

            TargetCollection targetCollection = TargetCapturer.TargetCollection;

            LayerParameter layerParameter = RenderConstants.LAYER_PARAMETER;
            DepthParameter depthParameter = RenderConstants.DEPTH_PARAMETER;

            TileHelper.PackShortsIntoTexture(BufferCollection.TileIDTexture, tileIDs);
            TileHelper.PackShortsIntoTexture(BufferCollection.WallIDTexture, wallIDs);

            TileHelper.PackMaterialTexture(BufferCollection.TileMaterialTexture, MaterialMapper.MappedTiles);
            TileHelper.PackMaterialTexture(BufferCollection.WallMaterialTexture, MaterialMapper.MappedWalls);

            BufferRenderer.RenderLayerBuffer(spriteBatch, BufferCollection, targetCollection, layerParameter);
            BufferRenderer.RenderNoiseBuffer(spriteBatch, BufferCollection, layerParameter, tileCaptureParameter);

            BufferRenderer.RenderDepthBuffer(spriteBatch, BufferCollection, layerParameter, depthParameter);
            BufferRenderer.RenderMaterialBuffer(spriteBatch, BufferCollection, layerParameter, tileCaptureParameter);
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
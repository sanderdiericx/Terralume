using Microsoft.Xna.Framework.Graphics;
using Terralume.Engine.Buffers;
using Terralume.Engine.Buffers.Data;
using Terralume.Engine.Constants;
using Terralume.Engine.Mapping;
using Terralume.Engine.Mapping.Registry;
using Terralume.Engine.Render;
using Terralume.Engine.Shaders;
using Terraria;
using Terraria.ModLoader;

namespace Terralume.Engine.Pipeline
{
    internal class BufferComposer : ModSystem
    {
        private static BufferCollection bufferCollection;

        public override void Load()
        {
            Main.QueueMainThreadAction(() =>
            {
                bufferCollection = new BufferCollection(Main.graphics.GraphicsDevice, (int)RenderConstants.SCREEN_SIZE.X, (int)RenderConstants.SCREEN_SIZE.Y);

                EffectManager.LoadEffect(EffectType.Layer);
                EffectManager.LoadEffect(EffectType.Depth);
                EffectManager.LoadEffect(EffectType.Material);
                EffectManager.LoadEffect(EffectType.Height);
                EffectManager.LoadEffect(EffectType.Normal);
                EffectManager.LoadEffect(EffectType.Noise);

                EffectManager.LoadEffect(EffectType.JFAInit);
                EffectManager.LoadEffect(EffectType.JFA);
                EffectManager.LoadEffect(EffectType.SDFCombine);

                EffectManager.LoadEffect(EffectType.JFAReflectionInit);
                EffectManager.LoadEffect(EffectType.JFAReflection);
                EffectManager.LoadEffect(EffectType.SDFCombineReflection);

                EffectManager.LoadEffect(EffectType.BackgroundBlur);

                EffectManager.LoadEffect(EffectType.ReflectiveMaterial);
            });

            MaterialPacker.SetTileMaterials(MaterialMapper.MappedTiles);
            MaterialPacker.SetWallMaterials(MaterialMapper.MappedWalls);
            MaterialPacker.SetLiquidMaterials(MaterialMapper.MappedLiquids);

            ReflectionPacker.SetTileReflections(ReflectionMapper.MappedTiles);
            ReflectionPacker.SetLiquidReflections(ReflectionMapper.MappedLiquids);
        }

        public override void Unload()
        {
            base.Unload();

            Main.QueueMainThreadAction(() =>
            {
                bufferCollection.Dispose();
            });
        }

        // Must be called at On_Main.DrawInterface to not break
        public static BufferCollection GetBufferCollection(SpriteBatch spriteBatch, TileCaptureParameter tileCaptureParameter)
        {
            UpdateBuffer();

            TargetCollection targetCollection = TargetCapturer.GetTargetCollection();

            LayerParameter layerParameter = RenderConstants.LAYER_PARAMETER;
            DepthParameter depthParameter = RenderConstants.DEPTH_PARAMETER;

            IDPacker.UpdateStoredIDs(tileCaptureParameter);
            IDPacker.PackTileIDTexture();
            IDPacker.PackWallIDTexture();
            IDPacker.PackLiquidIDTexture();

            SunOcclusionPacker.UpdateSunOcclusionValues(tileCaptureParameter);
            SunOcclusionPacker.PackSunOcclusionTexture();

            if (!MaterialPacker.TilesValid) MaterialPacker.PackTileMaterialTexture();
            if (!MaterialPacker.WallsValid) MaterialPacker.PackWallMaterialTexture();
            if (!MaterialPacker.LiquidsValid) MaterialPacker.PackLiquidMaterialTexture();

            if (!ReflectionPacker.TilesValid) ReflectionPacker.PackTileReflectionTexture();
            if (!ReflectionPacker.LiquidsValid) ReflectionPacker.PackLiquidReflectionTexture();

            BufferRenderer.RenderLayerBuffer(spriteBatch, bufferCollection, targetCollection, layerParameter);
            BufferRenderer.RenderNoiseBuffer(spriteBatch, bufferCollection, layerParameter, tileCaptureParameter);

            BufferRenderer.RenderSDFBuffer(spriteBatch, bufferCollection, layerParameter);

            BufferRenderer.BlurUndergroundBackgroundBuffer(spriteBatch, bufferCollection, targetCollection);

            BufferRenderer.RenderHeightBuffer(spriteBatch, bufferCollection);
            BufferRenderer.RenderDepthBuffer(spriteBatch, bufferCollection, layerParameter, depthParameter);

            BufferRenderer.RenderMaterialBuffer(spriteBatch, bufferCollection, layerParameter, tileCaptureParameter);

            BufferRenderer.RenderReflectiveMaterialBuffer(spriteBatch, bufferCollection, layerParameter, tileCaptureParameter);
            BufferRenderer.RenderSDFBufferReflection(spriteBatch, bufferCollection);

            BufferRenderer.RenderNormalBuffer(spriteBatch, bufferCollection);
            BufferRenderer.RenderAlbedoBuffer(spriteBatch, bufferCollection);

            BufferRenderer.RenderSunOcclusionBuffer(spriteBatch, bufferCollection, tileCaptureParameter);

            return bufferCollection;
        }

        // Make sure all buffers match screen size
        private static void UpdateBuffer()
        {
            if (!bufferCollection.IsValid())
            {
                bufferCollection.Dispose();
                bufferCollection = new BufferCollection(Main.graphics.GraphicsDevice, (int)RenderConstants.SCREEN_SIZE.X, (int)RenderConstants.SCREEN_SIZE.Y);
            }
        }
    }
}
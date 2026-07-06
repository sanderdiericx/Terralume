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
        public static BufferCollection BufferCollection { get; private set; }

        public override void Load()
        {
            Main.QueueMainThreadAction(() =>
            {
                BufferCollection = new BufferCollection(Main.graphics.GraphicsDevice, (int)RenderConstants.SCREEN_SIZE.X, (int)RenderConstants.SCREEN_SIZE.Y);

                EffectManager.LoadEffect(EffectType.Layer);
                EffectManager.LoadEffect(EffectType.Depth);
                EffectManager.LoadEffect(EffectType.Material);
                EffectManager.LoadEffect(EffectType.Height);
                EffectManager.LoadEffect(EffectType.Normal);
                EffectManager.LoadEffect(EffectType.Noise);

                EffectManager.LoadEffect(EffectType.JFAInit);
                EffectManager.LoadEffect(EffectType.JFA);
                EffectManager.LoadEffect(EffectType.SDFCombine);

                EffectManager.LoadEffect(EffectType.BackgroundBlur);
            });

            MaterialPacker.SetTileMaterials(MaterialMapper.MappedTiles);
            MaterialPacker.SetWallMaterials(MaterialMapper.MappedWalls);

            NoisePacker.SetTileNoise(NoiseMapper.MappedTiles);
            NoisePacker.SetWallNoise(NoiseMapper.MappedWalls);
        }

        public override void Unload()
        {
            base.Unload();

            Main.QueueMainThreadAction(() =>
            {
                BufferCollection.Dispose();
            });
        }

        // Must be called at On_Main.DrawInterface to not break
        public static void ComposeGBuffer(SpriteBatch spriteBatch, TileCaptureParameter tileCaptureParameter)
        {
            UpdateBuffer();

            TargetCollection targetCollection = TargetCapturer.TargetCollection;

            LayerParameter layerParameter = RenderConstants.LAYER_PARAMETER;
            DepthParameter depthParameter = RenderConstants.DEPTH_PARAMETER;

            IDPacker.UpdateStoredIDs(tileCaptureParameter);
            IDPacker.PackTileIDTexture();
            IDPacker.PackWallIDTexture();

            if (!MaterialPacker.TilesValid) MaterialPacker.PackTileMaterialTexture();
            if (!MaterialPacker.WallsValid) MaterialPacker.PackWallMaterialTexture();


            if (!NoisePacker.TilesValid) NoisePacker.PackTileNoiseTexture();
            if (!NoisePacker.WallsValid) NoisePacker.PackWallNoiseTexture();

            BufferRenderer.RenderLayerBuffer(spriteBatch, BufferCollection, targetCollection, layerParameter);
            BufferRenderer.RenderNoiseBuffer(spriteBatch, BufferCollection, layerParameter, tileCaptureParameter);

            BufferRenderer.RenderSDFBuffer(spriteBatch, BufferCollection, layerParameter);
            BufferRenderer.BlurUndergroundBackgroundBuffer(spriteBatch, BufferCollection, targetCollection);

            BufferRenderer.RenderHeightBuffer(spriteBatch, BufferCollection);
            BufferRenderer.RenderDepthBuffer(spriteBatch, BufferCollection, layerParameter, depthParameter);
            BufferRenderer.RenderMaterialBuffer(spriteBatch, BufferCollection, layerParameter, tileCaptureParameter);
            BufferRenderer.RenderNormalBuffer(spriteBatch, BufferCollection);
            BufferRenderer.RenderAlbedoBuffer(spriteBatch, BufferCollection);
        }

        // Make sure all buffers match screen size
        private static void UpdateBuffer()
        {
            if (!BufferCollection.IsValid())
            {
                BufferCollection.Dispose();
                BufferCollection = new BufferCollection(Main.graphics.GraphicsDevice, (int)RenderConstants.SCREEN_SIZE.X, (int)RenderConstants.SCREEN_SIZE.Y);
            }
        }
    }
}
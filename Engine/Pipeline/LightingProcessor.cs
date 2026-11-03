using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Terralume.Configuration;
using Terralume.Engine.Buffers;
using Terralume.Engine.Buffers.Data;
using Terralume.Engine.Constants;
using Terralume.Engine.Hooks.Tracking;
using Terralume.Engine.Mapping;
using Terralume.Engine.Render;
using Terralume.Engine.Shaders;
using Terralume.Engine.Utils;
using Terraria;
using Terraria.ModLoader;

namespace Terralume.Engine.Pipeline
{
    internal class LightingProcessor : ModSystem
    {
        private OutputCollection outputCollection;
        private SpriteBatch deferredSpriteBatch;

        private List<(Vector2, LightProfile)> onScreenLights;

        public override void Load()
        {
            Main.QueueMainThreadAction(() =>
            {
                outputCollection = new OutputCollection(Main.graphics.GraphicsDevice, (int)RenderConfig.ScreenSize.X, (int)RenderConfig.ScreenSize.Y);
                deferredSpriteBatch = new SpriteBatch(Main.graphics.GraphicsDevice);

                EffectManager.LoadEffect(EffectType.Downsample);
                EffectManager.LoadEffect(EffectType.Radiance);
                EffectManager.LoadEffect(EffectType.Output);
            });

            onScreenLights = new List<(Vector2, LightProfile)>();

            On_Main.DrawInterface += On_Main_DrawInterface;
        }

        public override void Unload()
        {
            base.Unload();

            Main.QueueMainThreadAction(() =>
            {
                outputCollection.Dispose();
            });

            On_Main.DrawInterface -= On_Main_DrawInterface;
        }

        private void On_Main_DrawInterface(On_Main.orig_DrawInterface orig, Main self, GameTime gameTime)
        {
            UpdateBuffer();

            AmbientLightTracker.GetAmbientLightProfile();

            LayerParameter layerParameter = RenderConfig.LayerParameter;

            TileCaptureParameter tileCaptureParameter = TileHelper.CalculateTileCaptureParameter();

            TargetCollection targetCollection = TargetCapturer.GetTargetCollection();
            BufferCollection bufferCollection = BufferComposer.GetBufferCollection(deferredSpriteBatch, tileCaptureParameter);

            onScreenLights = LightTracker.GetOnScreenLights(tileCaptureParameter);
            LightPacker.PackLightTextures(onScreenLights, RenderConfig.ScreenSize);

            AmbientFilterPacker.CalculateReachableTiles(tileCaptureParameter, onScreenLights);
            AmbientFilterPacker.PackAmbientFilterTexture(tileCaptureParameter);

            OutputRenderer.RenderAmbientFilterBuffer(deferredSpriteBatch, outputCollection, bufferCollection, tileCaptureParameter);
            OutputRenderer.RenderRadianceAndBloomBuffer(deferredSpriteBatch, outputCollection, bufferCollection, layerParameter, tileCaptureParameter);
            OutputRenderer.RenderBlurredBloomBuffer(deferredSpriteBatch, outputCollection, 2);
            OutputRenderer.RenderReflectionBuffer(deferredSpriteBatch, outputCollection, bufferCollection, targetCollection);

            OutputRenderer.RenderOutputBuffer(deferredSpriteBatch, outputCollection, bufferCollection, targetCollection);

            RenderTarget2D currentRenderTarget = GetCurrentRenderTarget(outputCollection, bufferCollection);

            deferredSpriteBatch.Begin();
            deferredSpriteBatch.Draw(currentRenderTarget, Vector2.Zero, Color.White);
            deferredSpriteBatch.End();

            TextRenderer.RenderCombatText(deferredSpriteBatch, outputCollection);
            TextRenderer.RenderPopupText(deferredSpriteBatch, outputCollection);

            // Draw UI normally
            orig(self, gameTime);
        }

        private static RenderTarget2D GetCurrentRenderTarget(OutputCollection outputCollection, BufferCollection bufferCollection)
        {
            DebugView debugView = RenderConfig.DebugView;

            switch (debugView)
            {
                case DebugView.Output:
                    return outputCollection.OutputBuffer;
                case DebugView.Albedo:
                    return bufferCollection.AlbedoBuffer;
                case DebugView.Normal:
                    return bufferCollection.NormalBuffer;
                case DebugView.Depth:
                    return bufferCollection.DepthBuffer;
                case DebugView.Material:
                    return bufferCollection.MaterialBuffer;
                case DebugView.Layer:
                    return bufferCollection.LayerBuffer;
                case DebugView.Noise:
                    return bufferCollection.NoiseBuffer;
                case DebugView.ReflectiveMaterial:
                    return bufferCollection.ReflectiveMaterialBuffer;
                case DebugView.SunOcclusion:
                    return bufferCollection.SunOcclusionBuffer;
                case DebugView.AmbientFilter:
                    return outputCollection.AmbientFilterBuffer;
                default:
                    return outputCollection.OutputBuffer;
            }
        }

        // Make sure all buffers match screen size
        private void UpdateBuffer()
        {
            if (!outputCollection.IsValid())
            {
                outputCollection.Dispose();
                outputCollection = new OutputCollection(Main.graphics.GraphicsDevice, (int)RenderConfig.ScreenSize.X, (int)RenderConfig.ScreenSize.Y);
            }
        }
    }
}

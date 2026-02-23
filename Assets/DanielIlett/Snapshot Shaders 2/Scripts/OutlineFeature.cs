using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

namespace DanielIlett.SnapshotShaders2.URP
{
    [DisallowMultipleRendererFeature("Snapshot Shaders 2/Outline")]
    public class OutlineFeature : ScriptableRendererFeature
    {
        OutlineRenderPass pass;

        public override void Create()
        {
            pass = new OutlineRenderPass();
            name = "Outline";
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            var settings = VolumeManager.instance.stack.GetComponent<OutlineSettings>();

            if (settings != null && settings.IsActive())
            {
                pass.ConfigureInput(ScriptableRenderPassInput.Depth);
                pass.ConfigureInput(ScriptableRenderPassInput.Normal);
                renderer.EnqueuePass(pass);
            }
        }
    }

    sealed class OutlineRenderPass : SnapshotRenderPass
    {
        protected override string ShaderName
        {
            get { return "Hidden/SnapshotShaders2/Outline"; }
        }

        public OutlineRenderPass()
        {
            profilingSampler = new ProfilingSampler("SS2 - Outline");
            requiresIntermediateTexture = true;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var settings = VolumeManager.instance.stack.GetComponent<OutlineSettings>();
            renderPassEvent = settings.renderPassEvent.value;

            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();

            var colorCopyDescriptor = GetCopyPassDescriptor(cameraData.cameraTargetDescriptor);
            TextureHandle copiedColor = TextureHandle.nullHandle;

            // Draw a local mask if required, which sets appropriate keywords on the PassMaterial.
            var maskHandleSettings = DrawMaskIfRequired<OutlineFeature>(renderGraph, frameData, settings, "SS2 Outline Local Mask");
            bool useGlobalMask = maskHandleSettings.useGlobalMask;
            bool useLocalMask = maskHandleSettings.useLocalMask;
            TextureHandle localMaskTextureHandle = maskHandleSettings.localMaskTextureHandle;

            // Perform the intermediate copy pass (source -> temp).
            copiedColor = UniversalRenderer.CreateRenderGraphTexture(renderGraph, colorCopyDescriptor, "_ColorCopy", false);

            using (var builder = renderGraph.AddRasterRenderPass<CopyPassData>("SS2 Outline Copy Color", out var passData, profilingSampler))
            {
                passData.inputTexture = resourceData.activeColorTexture;
                passData.bilinear = true;

                builder.UseTexture(resourceData.activeColorTexture, AccessFlags.Read);
                builder.SetRenderAttachment(copiedColor, 0, AccessFlags.Write);
                builder.SetRenderFunc(static (CopyPassData data, RasterGraphContext context) => ExecuteCopyPass(context.cmd, data.inputTexture, data.bilinear));
            }

            // Perform main pass (temp -> source).
            using (var builder = renderGraph.AddRasterRenderPass<BasicPassData>("SS2 Outline Main Pass", out var passData, profilingSampler))
            {
                passData.material = PassMaterial;
                passData.inputTexture = copiedColor;

                builder.UseTexture(copiedColor, AccessFlags.Read);

                if (useGlobalMask && frameData.Contains<GlobalMaskData>())
                {
                    var globalMaskData = frameData.Get<GlobalMaskData>();
                    builder.UseTexture(globalMaskData.globalMaskedObjects, AccessFlags.Read);
                    passData.maskTexture = globalMaskData.globalMaskedObjects;
                }
                else if (useLocalMask)
                {
                    builder.UseTexture(localMaskTextureHandle, AccessFlags.Read);
                    passData.maskTexture = localMaskTextureHandle;
                }

                builder.SetRenderAttachment(resourceData.activeColorTexture, 0, AccessFlags.Write);
                builder.SetRenderFunc(static (BasicPassData data, RasterGraphContext context) =>
                {
                    // Set Outline effect properties.
                    var settings = VolumeManager.instance.stack.GetComponent<OutlineSettings>();
                    data.material.SetColor("_OutlineColor", settings.outlineColor.value);
                    data.material.SetFloat("_ColorThreshold", settings.colorThreshold.value);
                    data.material.SetFloat("_ColorStrength", settings.colorStrength.value);
                    data.material.SetFloat("_DepthThreshold", settings.depthThreshold.value);
                    data.material.SetFloat("_DepthStrength", settings.depthStrength.value);
                    data.material.SetFloat("_NormalThreshold", settings.normalThreshold.value);
                    data.material.SetFloat("_NormalStrength", settings.normalStrength.value);
                    data.material.SetFloat("_SkyboxDepthCutoff", settings.skyboxDepthCutoff.value);

                    if(settings.drawingMode.value == OutlineDrawingMode.OutlinesOnly ||
                        settings.drawingMode.value == OutlineDrawingMode.NeonOnly)
                    {
                        data.material.SetColor("_BackgroundColor", settings.backgroundColor.value);
                        data.material.DisableKeyword("_USE_SCENE_TEXTURE");
                    }
                    else
                    {
                        data.material.EnableKeyword("_USE_SCENE_TEXTURE");
                    }

                    if (settings.drawingMode.value == OutlineDrawingMode.NeonOverlay ||
                        settings.drawingMode.value == OutlineDrawingMode.NeonOnly)
                    {
                        data.material.SetFloat("_SaturationFloor", settings.neonSaturationFloor.value);
                        data.material.SetFloat("_LightnessFloor", settings.neonLightnessFloor.value);
                        data.material.EnableKeyword("_USE_NEON_COLOR");
                    }
                    else
                    {
                        data.material.DisableKeyword("_USE_NEON_COLOR");
                    }

                    if ((RTHandle)data.maskTexture != null)
                    {
                        data.material.SetTexture(maskHandleName, data.maskTexture);
                    }

                    Blitter.BlitTexture(context.cmd, data.inputTexture, new Vector4(1, 1, 0, 0), data.material, 0);
                });
            }
        }
    }
}

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

namespace DanielIlett.SnapshotShaders2.URP
{
    [DisallowMultipleRendererFeature("Snapshot Shaders 2/Colorblind")]
    public class ColorblindFeature : ScriptableRendererFeature
    {
        ColorblindRenderPass pass;

        public override void Create()
        {
            pass = new ColorblindRenderPass();
            name = "Colorblind";
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            var settings = VolumeManager.instance.stack.GetComponent<ColorblindSettings>();

            if (settings != null && settings.IsActive())
            {
                renderer.EnqueuePass(pass);
            }
        }
    }

    sealed class ColorblindRenderPass : SnapshotRenderPass
    {
        // Values based on: https://web.archive.org/web/20081014161121/http://www.colorjack.com/labs/colormatrix/
        private static readonly Matrix4x4 protanopiaFilter =    new Matrix4x4(new Vector4(0.56667f, 0.43333f, 0.00000f), new Vector4(0.55833f, 0.44167f, 0.00000f), new Vector4(0.00000f, 0.24167f, 0.75833f), new Vector4(0.0f, 0.0f, 0.0f, 1.0f));
        private static readonly Matrix4x4 protanomalyFilter =   new Matrix4x4(new Vector4(0.81667f, 0.18333f, 0.00000f), new Vector4(0.33333f, 0.66667f, 0.00000f), new Vector4(0.00000f, 0.12500f, 0.87500f), new Vector4(0.0f, 0.0f, 0.0f, 1.0f));
        private static readonly Matrix4x4 deuteranopiaFilter =  new Matrix4x4(new Vector4(0.62500f, 0.37500f, 0.00000f), new Vector4(0.70000f, 0.30000f, 0.00000f), new Vector4(0.00000f, 0.30000f, 0.70000f), new Vector4(0.0f, 0.0f, 0.0f, 1.0f));
        private static readonly Matrix4x4 deuteranomalyFilter = new Matrix4x4(new Vector4(0.80000f, 0.20000f, 0.00000f), new Vector4(0.25833f, 0.74167f, 0.00000f), new Vector4(0.00000f, 0.14167f, 0.85833f), new Vector4(0.0f, 0.0f, 0.0f, 1.0f));
        private static readonly Matrix4x4 tritanopiaFilter =    new Matrix4x4(new Vector4(0.95000f, 0.05000f, 0.00000f), new Vector4(0.00000f, 0.43333f, 0.56667f), new Vector4(0.00000f, 0.47500f, 0.52500f), new Vector4(0.0f, 0.0f, 0.0f, 1.0f));
        private static readonly Matrix4x4 tritanomalyFilter =   new Matrix4x4(new Vector4(0.96667f, 0.03333f, 0.00000f), new Vector4(0.00000f, 0.73333f, 0.26667f), new Vector4(0.00000f, 0.18333f, 0.81667f), new Vector4(0.0f, 0.0f, 0.0f, 1.0f));
        private static readonly Matrix4x4 achromatopsiaFilter = new Matrix4x4(new Vector4(0.29900f, 0.58700f, 0.11400f), new Vector4(0.29900f, 0.58700f, 0.11400f), new Vector4(0.29900f, 0.58700f, 0.11400f), new Vector4(0.0f, 0.0f, 0.0f, 1.0f));
        private static readonly Matrix4x4 achromatomalyFilter = new Matrix4x4(new Vector4(0.61800f, 0.32000f, 0.06200f), new Vector4(0.16300f, 0.77500f, 0.06200f), new Vector4(0.16300f, 0.32000f, 0.51600f), new Vector4(0.0f, 0.0f, 0.0f, 1.0f));

        protected override string ShaderName
        {
            get { return "Hidden/SnapshotShaders2/Colorblind"; }
        }

        public ColorblindRenderPass()
        {
            profilingSampler = new ProfilingSampler("SS2 - Colorblind");
            requiresIntermediateTexture = true;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var settings = VolumeManager.instance.stack.GetComponent<ColorblindSettings>();
            renderPassEvent = settings.renderPassEvent.value;

            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();

            var colorCopyDescriptor = GetCopyPassDescriptor(cameraData.cameraTargetDescriptor);
            TextureHandle copiedColor = TextureHandle.nullHandle;

            // Draw a local mask if required, which sets appropriate keywords on the PassMaterial.
            var maskHandleSettings = DrawMaskIfRequired<ColorblindFeature>(renderGraph, frameData, settings, "SS2 Colorblind Local Mask");
            bool useGlobalMask = maskHandleSettings.useGlobalMask;
            bool useLocalMask = maskHandleSettings.useLocalMask;
            TextureHandle localMaskTextureHandle = maskHandleSettings.localMaskTextureHandle;

            // Perform the intermediate copy pass (source -> temp).
            copiedColor = UniversalRenderer.CreateRenderGraphTexture(renderGraph, colorCopyDescriptor, "_ColorCopy", false);

            using (var builder = renderGraph.AddRasterRenderPass<CopyPassData>("SS2 Colorblind Copy Color", out var passData, profilingSampler))
            {
                passData.inputTexture = resourceData.activeColorTexture;
                passData.bilinear = true;

                builder.UseTexture(resourceData.activeColorTexture, AccessFlags.Read);
                builder.SetRenderAttachment(copiedColor, 0, AccessFlags.Write);
                builder.SetRenderFunc(static (CopyPassData data, RasterGraphContext context) => ExecuteCopyPass(context.cmd, data.inputTexture, data.bilinear));
            }

            // Perform main pass (temp -> source).
            using (var builder = renderGraph.AddRasterRenderPass<BasicPassData>("SS2 Colorblind Main Pass", out var passData, profilingSampler))
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
                    // Set Colorblind effect properties.
                    var settings = VolumeManager.instance.stack.GetComponent<ColorblindSettings>();
                    
                    switch(settings.colorblindMode.value)
                    {
                        case ColorblindMode.Protanopia:
                            data.material.SetMatrix("_ColorblindFilter", protanopiaFilter);
                            break;
                        case ColorblindMode.Protanomaly:
                            data.material.SetMatrix("_ColorblindFilter", protanomalyFilter);
                            break;
                        case ColorblindMode.Deuteranopia:
                            data.material.SetMatrix("_ColorblindFilter", deuteranopiaFilter);
                            break;
                        case ColorblindMode.Deuteranomaly:
                            data.material.SetMatrix("_ColorblindFilter", deuteranomalyFilter);
                            break;
                        case ColorblindMode.Tritanopia:
                            data.material.SetMatrix("_ColorblindFilter", tritanopiaFilter);
                            break;
                        case ColorblindMode.Tritanomaly:
                            data.material.SetMatrix("_ColorblindFilter", tritanomalyFilter);
                            break;
                        case ColorblindMode.Achromatopsia:
                            data.material.SetMatrix("_ColorblindFilter", achromatopsiaFilter);
                            break;
                        case ColorblindMode.Achromatomaly:
                            data.material.SetMatrix("_ColorblindFilter", achromatomalyFilter);
                            break;
                    }

                    data.material.SetFloat("_Strength", settings.strength.value);

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

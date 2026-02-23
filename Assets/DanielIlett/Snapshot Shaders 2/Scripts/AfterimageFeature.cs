using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

namespace DanielIlett.SnapshotShaders2.URP
{
    [DisallowMultipleRendererFeature("Snapshot Shaders 2/Afterimage")]
    public class AfterimageFeature : ScriptableRendererFeature
    {
        AfterimageRenderPass pass;

        public override void Create()
        {
            pass = new AfterimageRenderPass();
            name = "Afterimage";
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            var settings = VolumeManager.instance.stack.GetComponent<AfterimageSettings>();

            if (settings != null && settings.IsActive())
            {
                renderer.EnqueuePass(pass);
            }
        }

        protected override void Dispose(bool disposing)
        {
            pass.Dispose();
            base.Dispose(disposing);
        }
    }

    sealed class AfterimageRenderPass : SnapshotRenderPass
    {
        private RTHandle afterimageHandle;
        private bool hasCopiedAfterimage = false;

        protected override string ShaderName
        {
            get { return "Hidden/SnapshotShaders2/Afterimage"; }
        }

        public AfterimageRenderPass()
        {
            profilingSampler = new ProfilingSampler("SS2 - Afterimage");
            requiresIntermediateTexture = true;

            //var descriptor = new RenderTextureDescriptor(Screen.width, Screen.height, RenderTextureFormat.ARGBInt);

            //RenderingUtils.ReAllocateHandleIfNeeded(ref afterimageHandle, descriptor, name: "_AfterimageTexture");
        }

        private class AfterimagePassData
        {
            public Material material;
            public TextureHandle inputTexture;
            public TextureHandle maskTexture;
            public TextureHandle afterimageTexture;
        }

        private void CopyAfterimage(RenderGraph renderGraph, UniversalResourceData resourceData, TextureHandle afterimageTexture)
        {
            // Perform the afterimage copy pass (source -> afterimage).
            using (var builder = renderGraph.AddRasterRenderPass<CopyPassData>("SS2 Afterimage Copy Afterimage", out var passData, profilingSampler))
            {
                passData.inputTexture = resourceData.activeColorTexture;
                passData.bilinear = true;

                builder.UseTexture(resourceData.activeColorTexture, AccessFlags.Read);
                builder.SetRenderAttachment(afterimageTexture, 0, AccessFlags.Write);
                builder.SetRenderFunc(static (CopyPassData data, RasterGraphContext context) => ExecuteCopyPass(context.cmd, data.inputTexture, data.bilinear));
            }
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var settings = VolumeManager.instance.stack.GetComponent<AfterimageSettings>();
            renderPassEvent = settings.renderPassEvent.value;

            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();

            var colorCopyDescriptor = GetCopyPassDescriptor(cameraData.cameraTargetDescriptor);
            TextureHandle copiedColor = TextureHandle.nullHandle;
            TextureHandle afterimageTexture = TextureHandle.nullHandle;

            RenderingUtils.ReAllocateHandleIfNeeded(ref afterimageHandle, colorCopyDescriptor, name: "_AfterimageTexture");

            if (afterimageHandle != null)
            {
                afterimageTexture = renderGraph.ImportTexture(afterimageHandle);
            }

            // Draw a local mask if required, which sets appropriate keywords on the PassMaterial.
            var maskHandleSettings = DrawMaskIfRequired<AfterimageFeature>(renderGraph, frameData, settings, "SS2 Afterimage Local Mask");
            bool useGlobalMask = maskHandleSettings.useGlobalMask;
            bool useLocalMask = maskHandleSettings.useLocalMask;
            TextureHandle localMaskTextureHandle = maskHandleSettings.localMaskTextureHandle;

            if(!hasCopiedAfterimage)
            {
                CopyAfterimage(renderGraph, resourceData, afterimageTexture);
                hasCopiedAfterimage = true;
            }

            // Perform the intermediate copy pass (source -> temp).
            copiedColor = UniversalRenderer.CreateRenderGraphTexture(renderGraph, colorCopyDescriptor, "_ColorCopy", false);

            using (var builder = renderGraph.AddRasterRenderPass<CopyPassData>("SS2 Afterimage Copy Color", out var passData, profilingSampler))
            {
                passData.inputTexture = resourceData.activeColorTexture;
                passData.bilinear = true;

                builder.UseTexture(resourceData.activeColorTexture, AccessFlags.Read);
                builder.SetRenderAttachment(copiedColor, 0, AccessFlags.Write);
                builder.SetRenderFunc(static (CopyPassData data, RasterGraphContext context) => ExecuteCopyPass(context.cmd, data.inputTexture, data.bilinear));
            }

            // Perform main pass (temp -> source).
            using (var builder = renderGraph.AddRasterRenderPass<AfterimagePassData>("SS2 Afterimage Main Pass", out var passData, profilingSampler))
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

                builder.UseTexture(afterimageTexture, AccessFlags.Read);
                passData.afterimageTexture = afterimageTexture;

                builder.SetRenderAttachment(resourceData.activeColorTexture, 0, AccessFlags.Write);
                builder.SetRenderFunc(static (AfterimagePassData data, RasterGraphContext context) =>
                {
                    // Set Afterimage effect properties.
                    var settings = VolumeManager.instance.stack.GetComponent<AfterimageSettings>();
                    data.material.SetFloat("_Persistence", settings.persistence.value);

                    if(settings.afterimageMode.value == AfterimageMode.Masked)
                    {
                        data.material.DisableKeyword("_AFTERIMAGE_OFF");
                        data.material.EnableKeyword("_AFTERIMAGE_MASKED");
                        data.material.DisableKeyword("_AFTERIMAGE_ALL");
                    }
                    else if(settings.afterimageMode.value == AfterimageMode.Everywhere)
                    {
                        data.material.DisableKeyword("_AFTERIMAGE_OFF");
                        data.material.DisableKeyword("_AFTERIMAGE_MASKED");
                        data.material.EnableKeyword("_AFTERIMAGE_ALL");
                    }
                    else
                    {
                        data.material.EnableKeyword("_AFTERIMAGE_OFF");
                        data.material.DisableKeyword("_AFTERIMAGE_MASKED");
                        data.material.DisableKeyword("_AFTERIMAGE_ALL");
                    }

                    if ((RTHandle)data.maskTexture != null)
                    {
                        data.material.SetTexture(maskHandleName, data.maskTexture);
                    }

                    data.material.SetTexture("_AfterimageTexture", data.afterimageTexture);

                    Blitter.BlitTexture(context.cmd, data.inputTexture, new Vector4(1, 1, 0, 0), data.material, 0);
                });
            }

            CopyAfterimage(renderGraph, resourceData, afterimageTexture);
        }

        public void Dispose()
        {
            afterimageHandle?.Release();
        }
    }

    /*
    public sealed class AfterimageAfterimageData : ContextItem
    {
        public TextureHandle afterimageHandle;

        public override void Reset()
        {
            afterimageHandle = TextureHandle.nullHandle;
        }
    }
    */
}

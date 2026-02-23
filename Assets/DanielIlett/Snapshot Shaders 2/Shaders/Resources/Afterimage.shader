Shader "Hidden/SnapshotShaders2/Afterimage"
{
	SubShader
    {
		Tags
		{
			"RenderPipeline" = "UniversalPipeline"
		}

        Pass
        {
            ZTest Always
            Cull Off
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include_with_pragmas "SnapshotHelper.hlsl"

            #pragma multi_compile_local_fragment _AFTERIMAGE_OFF _AFTERIMAGE_MASKED _AFTERIMAGE_ALL

            // Post process volume settings.
			float _Persistence;

            TEXTURE2D_X(_AfterimageTexture);

            float4 frag (Varyings i) : SV_TARGET
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

				float4 col = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, i.texcoord);
                float4 afterimage = SAMPLE_TEXTURE2D_X(_AfterimageTexture, sampler_LinearClamp, i.texcoord);

                float mask = SampleMask(i.texcoord);

#if defined(_AFTERIMAGE_MASKED)
                col.rgb = lerp(col.rgb, lerp(col.rgb, afterimage.rgb, _Persistence), mask);
#else
                col.rgb = lerp(col.rgb, afterimage.rgb, _Persistence);
#endif

                return col;
            }
            ENDHLSL
        }
    }
}

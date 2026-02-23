Shader "Hidden/SnapshotShaders2/ColorSeparation"
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

            // Post process volume settings.
			float4 _SeparationOffset;

            float4 frag (Varyings i) : SV_TARGET
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

				float4 col = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, i.texcoord);
                float r = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, i.texcoord + _SeparationOffset).r;
                float b = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, i.texcoord - _SeparationOffset).b;

                float mask = SampleMask(i.texcoord);

				col.rgb = lerp(col.rgb, float3(r, col.g, b), mask);
                return col;
            }
            ENDHLSL
        }
    }
}

Shader "Hidden/SnapshotShaders2/Colorblind"
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
			float _Strength;
            float4x4 _ColorblindFilter;

            float4 frag (Varyings i) : SV_TARGET
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

                float4 col = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, i.texcoord);

                float mask = SampleMask(i.texcoord);

                col.rgb = lerp(col.rgb, saturate(mul(_ColorblindFilter, col.rgb)), _Strength * mask);

                return col;
            }
            ENDHLSL
        }
    }
}

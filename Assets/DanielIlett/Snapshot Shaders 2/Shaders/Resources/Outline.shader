Shader "Hidden/SnapshotShaders2/Outline"
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
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl"
			#include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include_with_pragmas "SnapshotHelper.hlsl"

			#pragma multi_compile_local_fragment _ _USE_SCENE_TEXTURE
			#pragma multi_compile_local_fragment _ _USE_NEON_COLOR

            // Post process volume settings.
			float4 _OutlineColor;
			float _ColorThreshold;
			float _ColorStrength;
			float _DepthThreshold;
			float _DepthStrength;
			float _NormalThreshold;
			float _NormalStrength;
			float4 _BackgroundColor;
			float _SkyboxDepthCutoff;
			float _SaturationFloor;
			float _LightnessFloor;

			// Credit to https://alexanderameye.github.io/outlineshader.html:
			float3 DecodeNormal(float4 enc)
			{
				float kScale = 1.7777;
				float3 nn = enc.xyz*float3(2 * kScale, 2 * kScale, 0) + float3(-kScale, -kScale, 1);
				float g = 2.0 / dot(nn.xyz, nn.xyz);
				float3 n;
				n.xy = g * nn.xy;
				n.z = g - 1;
				return n;
			}

			// Credit to http://lolengine.net/blog/2013/07/27/rgb-to-hsv-in-glsl
			float3 rgb2hsv(float3 c)
			{
				float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
				float4 p = c.g < c.b ? float4(c.bg, K.wz) : float4(c.gb, K.xy);
				float4 q = c.r < p.x ? float4(p.xyw, c.r) : float4(c.r, p.yzx);

				float d = q.x - min(q.w, q.y);
				float e = 1.0e-10;
				return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
			}

			// Credit to http://lolengine.net/blog/2013/07/27/rgb-to-hsv-in-glsl
			float3 hsv2rgb(float3 c)
			{
				float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
				float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
				return c.z * lerp(K.xxx, saturate(p - K.xxx), c.y);
			}

            float4 frag (Varyings i) : SV_TARGET
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

				float mask = SampleMask(i.texcoord);

				float4 col = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, i.texcoord);

				float2 leftUV = i.texcoord + float2(1.0f / -_ScreenParams.x, 0.0f);
				float2 rightUV = i.texcoord + float2(1.0f / _ScreenParams.x, 0.0f);
				float2 bottomUV = i.texcoord + float2(0.0f, 1.0f / -_ScreenParams.y);
				float2 topUV = i.texcoord + float2(0.0f, 1.0f / _ScreenParams.y);

				float3 col0 = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, leftUV).rgb * mask;
				float3 col1 = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, rightUV).rgb * mask;
				float3 col2 = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, bottomUV).rgb * mask;
				float3 col3 = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, topUV).rgb * mask;

				float3 c0 = col1 - col0;
				float3 c1 = col3 - col2;

				float edgeCol = sqrt(dot(c0, c0) + dot(c1, c1));
				edgeCol = edgeCol > _ColorThreshold ? _ColorStrength : 0;

#if UNITY_REVERSED_Z
				float depth0 = SampleSceneDepth(leftUV) * mask;
				float depth1 = SampleSceneDepth(rightUV) * mask;
				float depth2 = SampleSceneDepth(bottomUV) * mask;
				float depth3 = SampleSceneDepth(topUV) * mask;
#else
				float depth0 = lerp(UNITY_NEAR_CLIP_VALUE, 1, SampleSceneDepth(leftUV) * mask);
				float depth1 = lerp(UNITY_NEAR_CLIP_VALUE, 1, SampleSceneDepth(rightUV) * mask);
				float depth2 = lerp(UNITY_NEAR_CLIP_VALUE, 1, SampleSceneDepth(bottomUV) * mask);
				float depth3 = lerp(UNITY_NEAR_CLIP_VALUE, 1, SampleSceneDepth(topUV) * mask);
#endif

				depth0 = Linear01Depth(depth0, _ZBufferParams);
				depth1 = Linear01Depth(depth1, _ZBufferParams);
				depth2 = Linear01Depth(depth2, _ZBufferParams);
				depth3 = Linear01Depth(depth3, _ZBufferParams);

				float d0 = depth1 - depth0;
				float d1 = depth3 - depth2;

				float edgeDepth = sqrt(d0 * d0 + d1 * d1);
				edgeDepth = edgeDepth > _DepthThreshold ? _DepthStrength : 0;

				float3 normal0 = DecodeNormal(SAMPLE_TEXTURE2D_X(_CameraNormalsTexture, sampler_LinearClamp, leftUV) * mask);
				float3 normal1 = DecodeNormal(SAMPLE_TEXTURE2D_X(_CameraNormalsTexture, sampler_LinearClamp, rightUV) * mask);
				float3 normal2 = DecodeNormal(SAMPLE_TEXTURE2D_X(_CameraNormalsTexture, sampler_LinearClamp, bottomUV) * mask);
				float3 normal3 = DecodeNormal(SAMPLE_TEXTURE2D_X(_CameraNormalsTexture, sampler_LinearClamp, topUV) * mask);

				float n0 = normal1 - normal0;
				float3 n1 = normal3 - normal2;

				float edgeNormal = sqrt(dot(n0, n0) + dot(n1, n1));
				edgeNormal = edgeNormal > _NormalThreshold ? _NormalStrength : 0;

				float edge = max(max(edgeCol, edgeDepth), edgeNormal);
				edge *= _OutlineColor.a;

#if UNITY_REVERSED_Z
				float depth = SampleSceneDepth(i.texcoord);
#else
				float depth = lerp(UNITY_NEAR_CLIP_VALUE, 1, SampleSceneDepth(i.texcoord));
#endif
				depth = Linear01Depth(depth, _ZBufferParams);
				edge = depth > _SkyboxDepthCutoff ? 0.0f : edge;

#if _USE_SCENE_TEXTURE
				float4 background = col;
#else
				float4 background = _BackgroundColor;
#endif

#if defined(_USE_NEON_COLOR)
				float3 hsvTex = rgb2hsv(col.rgb);
				hsvTex.y = max(hsvTex.y, _SaturationFloor);
				hsvTex.z = max(hsvTex.z, _LightnessFloor);
				float3 neonCol = hsv2rgb(hsvTex);

				return background + float4(neonCol * edge * _OutlineColor, 1.0f);
#else
				return lerp(background, _OutlineColor, edge);
#endif
            }
            ENDHLSL
        }
    }
}

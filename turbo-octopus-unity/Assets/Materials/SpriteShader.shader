Shader "Custom/SpriteShader"
{
    Properties
    {
        _MainTex ("Diffuse Texture", 2D) = "white" {}

		_SpecColor ("Specular Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_Shininess ("Shininess", Float) = 10

        _LightCutoff ("Light Cutoff", Range(0.001, 1.0)) = 0.005
        _LightBandSize ("Light Band Size", Float) = 0.005
    }
    SubShader
    {
        AlphaTest NotEqual 0.0
        Pass
        {
            Tags { "LightMode" = "ForwardBase" }
 
            CGPROGRAM
 
            #pragma vertex vert
            #pragma fragment frag
 
            #include "UnityCG.cginc"
 
            // user-specified properties
            uniform sampler2D _MainTex;

            // unity-specific properties
            uniform float4 _LightColor0;
 
            struct VertexInput
            {
                float4 vertex : POSITION;
                float4 uv : TEXCOORD0;
            };
 
            struct VertexOutput
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
 
            VertexOutput vert(VertexInput v) 
            {
                VertexOutput o;

                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.uv = float2(v.uv);

                return o;
            }
 
            float4 frag(VertexOutput i) : COLOR
            {
                float3 normalDirection = float3(0.0f, 0.0f, -1.0f);

				// texture maps
                float4 tex = tex2D(_MainTex, i.uv);
                float3 ambient = (0.0f, 0.0f, 0.0f);

                float3 lightFinal = ambient;

                if (_WorldSpaceLightPos0.w == 0.0) {
                    float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                    float atten = 1.0;

                    float surfaceLightDotNormal = saturate(dot(normalDirection, lightDirection));
                    float3 diffuse = atten * _LightColor0.rgb * surfaceLightDotNormal;

                    lightFinal = ambient + diffuse;
                }

                return float4(tex.xyz * lightFinal, tex.a);
            }
 
            ENDCG
        }
        Pass
        {
            Tags { "LightMode" = "ForwardAdd" }
        	Blend One One
 
            CGPROGRAM
 
            #pragma vertex vert
            #pragma fragment frag
 
            #include "UnityCG.cginc"
 
            // user-specified properties
            uniform sampler2D _MainTex;

			uniform float4 _SpecColor;
			uniform float _Shininess;

            uniform float _LightCutoff;
            uniform float _LightBandSize;

            // unity-specific properties
            uniform float4 _LightColor0;
            uniform sampler2D _LightTexture0;
            uniform float4x4 _LightMatrix0;

            struct VertexInput
            {
                float4 vertex : POSITION;
                float4 uv : TEXCOORD0;
            };
 
            struct VertexOutput
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 lightCoord : TEXCOORD2;
            };
 
            VertexOutput vert(VertexInput v) 
            {
                VertexOutput o;

                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.posWorld = mul(_Object2World, v.vertex);
                o.lightCoord = mul(_LightMatrix0, o.posWorld).xyz;
                o.uv = float2(v.uv);

                return o;
            }
 
            float4 frag(VertexOutput i) : COLOR
            {
				float3 normalDirection = float3(0.0f, 0.0f, -1.0f);
				float3 viewDirection = float3(0.0f, 0.0f, -1.0f);

				float3 lightDirection;
				float atten;

				// directional light or point light
				if (_WorldSpaceLightPos0.w == 0.0) {
					lightDirection = normalize(_WorldSpaceLightPos0.xyz);
					atten = 1.0;
				} else {
					float3 fragToLight = _WorldSpaceLightPos0.xyz - i.posWorld.xyz;

					//float distance = i.posLight.z;
					atten = tex2D(_LightTexture0, dot(i.lightCoord, i.lightCoord).rr).UNITY_ATTEN_CHANNEL;
					lightDirection = normalize(fragToLight);
				}

				// hard cutoff
				atten = step(_LightCutoff, atten) * atten;
				// banded lighting
				atten = atten - fmod(atten, _LightBandSize);

				float surfaceLightDotNormal = saturate(dot(normalDirection, lightDirection));
				float3 diffuse = atten * _LightColor0.rgb * surfaceLightDotNormal;
				float3 specular = diffuse * _SpecColor.rgb * pow(saturate(dot(reflect(-lightDirection, normalDirection), viewDirection)), _Shininess);

				float3 lightFinal = diffuse + specular;

				// texture maps
                float4 tex = tex2D(_MainTex, i.uv);

                return float4(tex.rgb * lightFinal, tex.a);
            }
 
            ENDCG
        }
    }
}
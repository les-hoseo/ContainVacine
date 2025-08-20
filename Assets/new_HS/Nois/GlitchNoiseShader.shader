Shader "Unlit/GlitchNoiseShader"
{
    Properties
    {
        _NoiseColor ("Noise Color", Color) = (1,1,1,0.1)
        _NoiseScale ("Noise Scale", Float) = 200
        _NoiseSpeed ("Noise Speed", Float) = 10
        _GlitchAmount ("Glitch Amount", Range(0, 1)) = 0.1
        _GlitchSpeed ("Glitch Speed", Float) = 5
        _MasterAlpha ("Master Alpha", Range(0, 1)) = 1.0 // ✨전체 투명도 조절 변수 추가
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            fixed4 _NoiseColor;
            float _NoiseScale;
            float _NoiseSpeed;
            float _GlitchAmount;
            float _GlitchSpeed;
            float _MasterAlpha; // ✨변수 선언

            v2f vert (appdata_base v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            float random(float2 st) {
                return frac(sin(dot(st.xy, float2(12.9898, 78.233))) * 43758.5453123);
            }

            fixed4 frag (v2f i) : SV_Target {
                float glitchTime = floor(_Time.y * _GlitchSpeed);
                float randomOffset = random(float2(glitchTime, glitchTime)) * _GlitchAmount;

                float2 glitchUV = i.uv;
                glitchUV.x += randomOffset;

                if (random(float2(glitchTime, 0)) > 0.95) {
                    glitchUV.y = frac(i.uv.y * 1.05);
                }

                float2 animatedUV = glitchUV + _Time.y * _NoiseSpeed * 0.01;
                float2 screenUV = animatedUV * _ScreenParams.xy;
                float2 scaledUV = screenUV / _NoiseScale;
                float noise = random(floor(scaledUV));
                fixed4 finalColor = _NoiseColor * noise;

                finalColor.a *= _MasterAlpha; // ✨최종 알파값에 _MasterAlpha를 곱해줌

                return finalColor;
            }
            ENDCG
        }
    }
}
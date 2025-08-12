Shader "Unlit/GridShader"
{
    Properties
    {
        _GridColor1 ("Grid Color 1", Color) = (1,1,1,0.5)
        _Spacing1 ("Spacing 1", Float) = 80
        _Speed1 ("Speed 1", Float) = 10

        _GridColor2 ("Grid Color 2", Color) = (1,1,1,0.5)
        _Spacing2 ("Spacing 2", Float) = 80
        _Speed2 ("Speed 2", Float) = 15
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _GridColor1;
            float _Spacing1;
            float _Speed1;
            fixed4 _GridColor2;
            float _Spacing2;
            float _Speed2;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv; // Use raw UVs for UI
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // Grid 1 calculation
                float2 uv1 = i.uv;
                uv1.y += _Time.y * _Speed1 * 0.01; // Move up
                uv1.x -= _Time.y * _Speed1 * 0.01; // Move left
                float grid1 = (1-step(0.95, frac(uv1.x * 10))) * (1-step(0.95, frac(uv1.y * 10)));
                fixed4 col1 = (1-grid1) * _GridColor1;
                col1.a *= (1-grid1);

                // Grid 2 calculation
                float2 uv2 = i.uv;
                uv2.y += _Time.y * _Speed2 * 0.01; // Move up
                uv2.x -= _Time.y * _Speed2 * 0.01; // Move left
                uv2 += 1.5 / _Spacing2; // Offset
                float grid2 = (1-step(0.95, frac(uv2.x * 10))) * (1-step(0.95, frac(uv2.y * 10)));
                fixed4 col2 = (1-grid2) * _GridColor2;
                col2.a *= (1-grid2);

                return col1 + col2;
            }
            ENDCG
        }
    }
}
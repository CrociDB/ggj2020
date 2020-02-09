Shader "Game/EnergyShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color", Color) = (0,0,0,0)
    }
    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 100

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // sample the texture
                fixed4 t = tex2D(_MainTex, i.uv + fixed2(.2 * (-sin(i.uv.x * 1.4 + _Time.x * 4.2)), 0.0));
                fixed4 tc = tex2D(_MainTex, i.uv + fixed2(t.r * .8, t.g * .2 + _Time.x * 2.5));
                fixed4 tc2 = tc * .8f + 1.0f;

                float edge = clamp(0, 1, pow(1.0 - abs(i.uv.x * 1.9 - 1.9) * 0.5, 0.67));
                fixed4 col = _Color * tc2;
                col += smoothstep(0.36, 0.5, tc) * .2;
                col.a = _Color.a * edge;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}

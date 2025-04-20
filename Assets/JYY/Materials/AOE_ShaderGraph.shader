Shader "Unlit/AOE_ShaderGraph"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Radius ("Radius", Range(0,1)) = 0.5
        _Feather ("Feather", Range(0,1)) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

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
            float _Radius;
            float _Feather;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 중심에서의 거리 계산
                float2 centeredUV = i.uv - 0.5;
                float dist = length(centeredUV);

                // 알파 조절: 중심에서 점점 투명 → 가장자리는 불투명
                float alpha = smoothstep(_Radius, _Radius - _Feather, dist);

                // 텍스처 적용 (선택)
                fixed4 texCol = tex2D(_MainTex, i.uv);

                // 최종 컬러 (컬러 * 텍스처 * 알파)
                fixed4 col = _Color * texCol;
                col.a *= alpha;
                return col;
            }
            ENDCG
        }
    }
}

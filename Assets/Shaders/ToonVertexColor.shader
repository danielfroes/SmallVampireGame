Shader "Kweer/Unlit/ToonVertexColor"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "gray" {}
        _ColorR ("Color R", color) = (1,1,1,1)
        _ColorG ("Color G", color) = (1,1,1,1)
        _ColorB ("Color B", color) = (1,1,1,1)
        _ColorA ("Tint", color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct MeshData
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                half4 color : COLOR;
            };

            struct Interpolators
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                half4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            half4 _ColorR;
            half4 _ColorG;
            half4 _ColorB;
            half4 _ColorA;

            Interpolators vert (MeshData v)
            {
                Interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                // o.color = v.color;        

                o.color = float4( (_ColorR * v.color.r) + (_ColorG * v.color.g) + (_ColorB * v.color.b) + (_ColorA * v.color.a)  );

                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (Interpolators i) : SV_Target
            {
                // sample the texture
                fixed4 tex = tex2D(_MainTex, i.uv);
                fixed4 col = (tex * (i.color * 4));

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);

                return col;
            }
            ENDCG
        }
    }
}

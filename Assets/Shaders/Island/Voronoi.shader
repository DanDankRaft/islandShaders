Shader "Image/Voronoi"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            uniform float2 _Points[200];
            uniform int _Length;

            sampler2D _MainTex;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 screen = tex2D(_MainTex, i.uv);
                //_Points[0] = float2(0,0);
                //_Points[1] = _ScreenParams.xy;
                float minDist = 10000; // (Infinity)
                int minI = 0;
                float2 currPosition = i.vertex.xy;
                for (int i = 0; i < _Length; i++) {
                    float dist = distance(currPosition, _Points[i]);
                    if (dist < minDist) {
                        minDist = dist;
                        minI = i;
                    }
                }
                return fixed4(screen.r, float(minI) / float(_Length-1), screen.b, 1);
            }
            ENDCG
        }
    }
}

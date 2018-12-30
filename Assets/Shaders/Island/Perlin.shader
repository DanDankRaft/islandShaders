Shader "Island/Perlin"
{
    Properties
    {
        _Seed ("Seed", Int) = 1
        _Multiplier ("Multiplier", Int) = 1
        _Increment ("Increment", Int) = 1
    }
    SubShader
    {
        // No culling or depth
        //Cull Off ZWrite Off ZTest Always

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
            uniform float2 _Vectors[200];
            uniform float _Length;

            uniform float _Seed;
            uniform int _Increment;
            uniform int _Multiplier;
            float random(float2 p)
            {
                //_Seed ^= (_Seed << 13);
                //_Seed ^= (_Seed >> 17);
                //_Seed ^= (_Seed << 5);
                _Seed += 0.1;
                return float(frac(sin(dot(p.xy,float2(_Seed,65.115)))*2773.8856));
            }

            uniform fixed2 _GradientValues[200];
            uniform int _GradientsLength;
            uniform fixed thing = 0;

            fixed2 lowerLeft;
            fixed2 lowerLeftDistance;
            fixed2 lowerRight;
            fixed2 lowerRightDistance;
            fixed2 upperLeft;
            fixed2 upperLeftDistance;
            fixed2 upperRight;
            fixed2 upperRightDistance;
            fixed4 frag (v2f i) : SV_Target
            {
                fixed returnValue = random(i.vertex.xy);
                return fixed4(returnValue, returnValue, returnValue, 0);
            }
            ENDCG
        }
    }
}

// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Unlit/Effect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SecTex("Texture", 2D) = "white" {}
        _Alpha("Alpha" , Range(0,1)) = 1
         _Alpha1("Alpha" , Range(0,1)) = 1
         _Speed("Speed", float) = 5
        _SpeedY("Speed", float) = 5
         _WaveHeight("Wave height", float) = 0.1
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
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
                float4 normal :NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _SecTex;
            float4 _SecTex_ST;
            fixed _Alpha;
            fixed _Alpha1;
            float _Speed;
            float _SpeedY;
            float _WaveHeight;
            v2f vert (appdata v)
            {
                v2f o;
                
                v.vertex = v.vertex + v.normal * _WaveHeight * abs(sin((v.vertex.x + v.vertex.z)*100 + 2* _Time));
                o.vertex = UnityObjectToClipPos(v.vertex);
               
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv1 = TRANSFORM_TEX(v.uv, _SecTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed2 scrolledUV = i.uv;

                fixed xScrollValue = _SpeedY * _Time;
                fixed yScrollValue = _Speed * _Time;

                //Apply offset
                scrolledUV -= fixed2(xScrollValue, yScrollValue);

                // sample the texture
                fixed4 col = float4((float3(tex2D(_MainTex, scrolledUV).xyz * _Alpha) +float3(tex2D(_SecTex, scrolledUV).xyz * _Alpha1)), (_Alpha + _Alpha1)/2);
               
                //fixed4 col = tex2D(_MainTex, scrolledUV) + tex2D(_SecTex, scrolledUV);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                /*return fixed4(col.xyz ,_Alpha);*/
                return col;
            }
            ENDCG
        }
    }
}

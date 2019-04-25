Shader "Unlit/GemWarp"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Ramp ("ColorRamp", 2D) = "transparent" {}
		_WormholePos("Clip space wormhole position", Vector) = (0,0,0,0)
		_WormholeRadius("Wormhole radius", Float) = 0
		_EffectScale ("Effect Scale", Float) = 5
    }
    SubShader
    {
		Tags{ "RenderType" = "Transparent" "Queue" = "Transparent+50" }
		Blend SrcAlpha OneMinusSrcAlpha
		LOD 100
		ZWrite Off
			
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
				float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				float4 color : COLOR;
				float  wormEffect: TEXCOORD1;
            };

            sampler2D _MainTex, _Ramp;
            float4 _MainTex_ST;
			float3 _WormholePos;
			fixed _WormholeRadius, _EffectScale;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				float d = (length(o.vertex.xy - _WormholePos.xy) - _WormholeRadius)/ _EffectScale;
				o.vertex.xy = lerp(_WormholePos.xy, o.vertex.xy, saturate(d));
				o.wormEffect = 1 - saturate(d);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				//transfer vertex color
				o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;
				fixed4 wcol = tex2D(_Ramp, float2(i.wormEffect,0.5));

				fixed4 outC = col;
				col.rgb = lerp(col.rgb, wcol.rgb, wcol.a * i.wormEffect);

				return col;
            }
            ENDCG
        }

    }
}

Shader "Unlit/GemGleam"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Ramp ("ColorRamp", 2D) = "transparent" {}
		_EffectStrength ("Effect Strength", Float) = 1
		_EffectMult ("Effect Mult", Float) = 1
		_EffectScale ("Effect Scale", Float) = 1
		_EffectMax("Effect Max", Float) = 1
		_EffectMin("Effect Min", Float) = 1
		_EffectSpeed("Effect Speed", Float) = 1
		[Toggle]
		_EffectToggle("Effect On", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
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
				float3 vpos: TEXCOORD1;
            };

            sampler2D _MainTex, _Ramp;
            float4 _MainTex_ST;
			fixed _EffectStrength, _EffectMax, _EffectMin, _EffectSpeed, _EffectScale, _EffectMult, _EffectToggle;

            v2f vert (appdata v)
            {
                v2f o;
				o.vpos = v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				//transfer vertex color
				o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;
				float _EffectPos = (_Time[0] * _EffectSpeed) % (_EffectMax - _EffectMin) + _EffectMin;
				float eff = i.vpos.x * 1.414 + i.vpos.y * 1.414 + _EffectPos;
				fixed4 wcol = tex2D(_Ramp, float2(eff*_EffectScale,0.5));

				fixed4 outC = col;
				col.rgb = col.rgb + _EffectToggle*(col.rgb * wcol.rgb * wcol.a * _EffectMult + wcol.rgb * wcol.a * _EffectStrength);

				return col;
            }
            ENDCG
        }
    }
}

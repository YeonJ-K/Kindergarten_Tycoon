Shader "Custom/SpriteRadialFill"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _FillAmount ("Fill Amount", Range(0,1)) = 1
        [MaterialToggle] _Clockwise ("Clockwise", Float) = 1
    }

    SubShader
    {
        Tags
        {
            "Queue"            = "Transparent"
            "IgnoreProjector"  = "True"
            "RenderType"       = "Transparent"
            "PreviewType"      = "Plane"
            "CanUseSpriteAtlas"= "True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha   // 스프라이트 기본(프리멀티플라이드 알파)

        Pass
        {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            #define TAU 6.28318530718

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            sampler2D _MainTex;
            fixed4    _Color;
            float     _FillAmount;
            float     _Clockwise;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex   = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color    = IN.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;

                // 1) UV를 중심(0.5, 0.5) 기준으로 이동 → -0.5 ~ +0.5
                float2 p = IN.texcoord - 0.5;

                // 2) 각도 계산: atan2(x, y)는 위쪽이 0, 시계방향으로 증가
                //    반시계 방향을 원하면 x를 뒤집는다
                float x = lerp(-p.x, p.x, _Clockwise);
                float angle = atan2(x, p.y);

                // 3) -PI~PI → 0~1 로 정규화
                angle /= TAU;
                if (angle < 0) angle += 1;

                // 4) 채우기 범위 밖의 픽셀은 버린다
                //    (_FillAmount - angle)이 음수면 clip
                clip(_FillAmount - angle);

                // 스프라이트 기본 블렌드용 프리멀티플라이
                c.rgb *= c.a;
                return c;
            }
        ENDCG
        }
    }
}

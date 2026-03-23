Shader "Custom/UI_Glow"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint (vertex * material)", Color) = (1,1,1,1)

        _Tint ("Global Tint", Color) = (1,1,1,1)              // <-- sẽ tint sprite trắng
        _GlowColor ("Glow Color", Color) = (1,1,1,1)
        _GlowIntensity ("Glow Intensity", Range(0,10)) = 2

        // ==== UI / Mask required properties ====
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        // Blend additive-friendly: keeps alpha while letting emission add brightness
        Blend One OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            fixed4 _Color;      // material tint * vertex color
            fixed4 _Tint;       // global tint controlled from script
            fixed4 _GlowColor;
            float _GlowIntensity;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                float4 color : COLOR;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                o.color = v.color; // pass vertex color (Image.color)
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample sprite
                fixed4 tex = tex2D(_MainTex, i.texcoord);

                // Apply vertex color and material tint and global tint.
                // If your sprite is white, _Tint will determine visible color.
                tex *= i.color;    // Image.color (vertex)
                tex *= _Color;     // material's _Color property
                tex.rgb *= _Tint.rgb; // global tint from script

                // Create emission based on glow color * intensity and alpha area of sprite
                float3 emission = _GlowColor.rgb * _GlowIntensity * tex.a;

                fixed4 finalCol;
                finalCol.rgb = tex.rgb + emission; // additive emission (HDR-capable)
                finalCol.a = tex.a;

                return finalCol;
            }
            ENDCG
        }
    }
}

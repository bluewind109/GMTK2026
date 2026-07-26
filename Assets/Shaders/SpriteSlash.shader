Shader "Custom/Sprites/SlashEffect"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _SlashAngle ("Slash Angle (degrees)", Range(-180, 180)) = -45
        _SlashOffset ("Slash Position Offset", Range(-0.5, 0.5)) = 0
        _SlashProgress ("Slash Progress", Range(0, 1)) = 0
        _SeparationAmount ("Separation Amount", Range(0, 0.5)) = 0.15
        _FadeAmount ("Fade on Separation", Range(0, 1)) = 0.7

        // Set automatically by SlashEffect.cs. Stores the sprite's UV sub-rect
        // within the atlas texture: x=minU, y=minV, z=maxU, w=maxV.
        // Defaults to (0,0,1,1) for standalone (non-atlas) sprites.
        [HideInInspector] _UVRect ("Atlas UV Rect", Vector) = (0, 0, 1, 1)
    }

    SubShader
    {
        Tags
        {
            "Queue"             = "Transparent"
            "RenderType"        = "Transparent"
            "RenderPipeline"    = "UniversalPipeline"
            "IgnoreProjector"   = "True"
            "CanUseSpriteAtlas" = "True"
        }

        Blend One OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            Name "SpriteSlash"
            Tags { "LightMode" = "Universal2D" }

            HLSLPROGRAM
            #pragma target 2.0
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4  _Color;
                float  _SlashAngle;
                float  _SlashOffset;
                float  _SlashProgress;
                float  _SeparationAmount;
                float  _FadeAmount;
                // (minU, minV, maxU, maxV) in atlas texture space.
                float4 _UVRect;
            CBUFFER_END

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            struct Attributes
            {
                float4 positionOS : POSITION;
                half4  color      : COLOR;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                half4  color       : COLOR;
                float2 uv          : TEXCOORD0;
            };

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv    = v.uv;
                o.color = v.color;
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                // Convert from atlas UV to sprite-local [0,1] space so the
                // slash math always treats (0.5, 0.5) as the sprite centre,
                // regardless of where the sprite sits in the atlas.
                float2 uvSize   = _UVRect.zw - _UVRect.xy;
                float2 spriteUV = (i.uv - _UVRect.xy) / uvSize;

                // --- Slash line ---
                float  angle       = _SlashAngle * PI / 180.0;
                float2 slashNormal = float2(-sin(angle), cos(angle));
                float  separation  = _SlashProgress * _SeparationAmount;

                // Which side of the slash line is this pixel on?
                float dist = dot(spriteUV - 0.5, slashNormal) - _SlashOffset;
                float side = dist >= 0.0 ? 1.0 : -1.0;

                // Reverse-map to where this pixel was before the halves moved.
                float2 origSpriteUV = spriteUV - side * slashNormal * separation;

                // Gap rejection: discard pixels whose origin crossed the slash.
                float origDist = dot(origSpriteUV - 0.5, slashNormal) - _SlashOffset;
                float origSide = origDist >= 0.0 ? 1.0 : -1.0;
                clip(side * origSide - 0.5);

                // Edge rejection: discard pixels that slid outside the sprite.
                clip(origSpriteUV.x);
                clip(1.0 - origSpriteUV.x);
                clip(origSpriteUV.y);
                clip(1.0 - origSpriteUV.y);

                // Remap back to atlas UV space for the texture sample.
                float2 origAtlasUV = origSpriteUV * uvSize + _UVRect.xy;

                half4 c = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, origAtlasUV) * i.color * _Color;
                c *= 1.0 - _SlashProgress * _FadeAmount;
                return c;
            }
            ENDHLSL
        }
    }

    Fallback "Sprites/Default"
}

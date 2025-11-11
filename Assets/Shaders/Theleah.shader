// Stencil Injection by ShaderGraphStencilInjector

Shader "Stencil Shader Graph/Theleah"
{
Properties
{
    _Thickness("Thickness", Float) = 1
    _FaceColor("FaceColor", Color) = (1, 1, 1, 1)
    _OutlineColor("OutlineColor", Color) = (0, 0, 0, 1)
    [ToggleUI]_RainbowOutline("RainbowOutline", Float) = 0
    _RainbowSpeed("RainbowSpeed", Float) = 1
    [NoScaleOffset]_Texture2DAsset_142ed57588bb4411b5721e51d840a96c_Out_0_Texture2D("Texture2D", 2D) = "white" {}

    // Stencil Properties
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
"RenderPipeline"="UniversalPipeline"
"RenderType"="Transparent"
"UniversalMaterialType" = "Unlit"
"Queue"="Transparent"
// DisableBatching: <None>
"ShaderGraphShader"="true"
"ShaderGraphTargetId"="UniversalSpriteUnlitSubTarget"
"IgnoreProjector"="True"
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

Pass
{
    Name "Sprite Unlit"
    Tags
    {
        "LightMode" = "Universal2D"
    }

// Render State
Cull Off
Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
ZTest LEqual
ZWrite Off

        // Stencil Buffer Setup
        Stencil
        {
            Ref [_StencilRef]
			 Ref [_Stencil]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
            Comp [_StencilComp]
            Pass [_StencilPass]
            Fail [_StencilFail]
            ZFail [_StencilZFail]
        }

// Debug
// <None>

// --------------------------------------------------
// Pass

HLSLPROGRAM

// Pragmas
#pragma target 2.0
#pragma exclude_renderers d3d11_9x
#pragma multi_compile_instancing
#pragma vertex vert
#pragma fragment frag

// Keywords
#pragma multi_compile_fragment _ DEBUG_DISPLAY
#pragma multi_compile_vertex _ SKINNED_SPRITE
// GraphKeywords: <None>

// Defines

#define ATTRIBUTES_NEED_NORMAL
#define ATTRIBUTES_NEED_TANGENT
#define ATTRIBUTES_NEED_TEXCOORD0
#define ATTRIBUTES_NEED_COLOR
#define FEATURES_GRAPH_VERTEX_NORMAL_OUTPUT
#define FEATURES_GRAPH_VERTEX_TANGENT_OUTPUT
#define VARYINGS_NEED_POSITION_WS
#define VARYINGS_NEED_TEXCOORD0
#define VARYINGS_NEED_COLOR
#define FEATURES_GRAPH_VERTEX
/* WARNING: $splice Could not find named fragment 'PassInstancing' */
#define SHADERPASS SHADERPASS_SPRITEUNLIT


// custom interpolator pre-include
/* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

// Includes
#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Fog.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Core2D.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

// --------------------------------------------------
// Structs and Packing

// custom interpolators pre packing
/* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

struct Attributes
{
 float3 positionOS : POSITION;
 float3 normalOS : NORMAL;
 float4 tangentOS : TANGENT;
 float4 uv0 : TEXCOORD0;
 float4 color : COLOR;
#if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
 uint instanceID : INSTANCEID_SEMANTIC;
#endif
};
struct Varyings
{
 float4 positionCS : SV_POSITION;
 float3 positionWS;
 float4 texCoord0;
 float4 color;
#if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
 uint instanceID : CUSTOM_INSTANCE_ID;
#endif
#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
 uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
#endif
#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
 uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
#endif
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
 FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
#endif
};
struct SurfaceDescriptionInputs
{
 float4 uv0;
 float3 TimeParameters;
};
struct VertexDescriptionInputs
{
 float3 ObjectSpaceNormal;
 float3 ObjectSpaceTangent;
 float3 ObjectSpacePosition;
};
struct PackedVaryings
{
 float4 positionCS : SV_POSITION;
 float4 texCoord0 : INTERP0;
 float4 color : INTERP1;
 float3 positionWS : INTERP2;
#if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
 uint instanceID : CUSTOM_INSTANCE_ID;
#endif
#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
 uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
#endif
#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
 uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
#endif
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
 FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
#endif
};

PackedVaryings PackVaryings (Varyings input)
{
PackedVaryings output;
ZERO_INITIALIZE(PackedVaryings, output);
output.positionCS = input.positionCS;
output.texCoord0.xyzw = input.texCoord0;
output.color.xyzw = input.color;
output.positionWS.xyz = input.positionWS;
#if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
output.instanceID = input.instanceID;
#endif
#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
#endif
#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
#endif
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
output.cullFace = input.cullFace;
#endif
return output;
}

Varyings UnpackVaryings (PackedVaryings input)
{
Varyings output;
output.positionCS = input.positionCS;
output.texCoord0 = input.texCoord0.xyzw;
output.color = input.color.xyzw;
output.positionWS = input.positionWS.xyz;
#if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
output.instanceID = input.instanceID;
#endif
#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
#endif
#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
#endif
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
output.cullFace = input.cullFace;
#endif
return output;
}


// --------------------------------------------------
// Graph

// Graph Properties
CBUFFER_START(UnityPerMaterial)
float4 _Texture2DAsset_142ed57588bb4411b5721e51d840a96c_Out_0_Texture2D_TexelSize;
float _Thickness;
float4 _OutlineColor;
float _RainbowOutline;
float _RainbowSpeed;
float4 _FaceColor;
UNITY_TEXTURE_STREAMING_DEBUG_VARS;
CBUFFER_END

#if defined(DOTS_INSTANCING_ON)
// DOTS instancing definitions
UNITY_DOTS_INSTANCING_START(MaterialPropertyMetadata)
    UNITY_DOTS_INSTANCED_PROP_OVERRIDE_SUPPORTED(float4, _OutlineColor)
    UNITY_DOTS_INSTANCED_PROP_OVERRIDE_SUPPORTED(float, _RainbowOutline)
UNITY_DOTS_INSTANCING_END(MaterialPropertyMetadata)
// DOTS instancing usage macros
#define UNITY_ACCESS_HYBRID_INSTANCED_PROP(var, type) UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(type, var)
#elif defined(UNITY_INSTANCING_ENABLED)
// Unity instancing definitions
UNITY_INSTANCING_BUFFER_START(SGPerInstanceData)
    UNITY_DEFINE_INSTANCED_PROP(float4, _OutlineColor)
    UNITY_DEFINE_INSTANCED_PROP(float, _RainbowOutline)
UNITY_INSTANCING_BUFFER_END(SGPerInstanceData)
// Unity instancing usage macros
#define UNITY_ACCESS_HYBRID_INSTANCED_PROP(var, type) UNITY_ACCESS_INSTANCED_PROP(SGPerInstanceData, var)
#else
#define UNITY_ACCESS_HYBRID_INSTANCED_PROP(var, type) var
#endif

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
TEXTURE2D(_Texture2DAsset_142ed57588bb4411b5721e51d840a96c_Out_0_Texture2D);
SAMPLER(sampler_Texture2DAsset_142ed57588bb4411b5721e51d840a96c_Out_0_Texture2D);

// Graph Includes
// GraphIncludes: <None>

// -- Property used by ScenePickingPass
#ifdef SCENEPICKINGPASS
float4 _SelectionID;
#endif

// -- Properties used by SceneSelectionPass
#ifdef SCENESELECTIONPASS
int _ObjectId;
int _PassValue;
#endif

// Graph Functions

void Unity_Comparison_Greater_float(float A, float B, out float Out)
{
    Out = A > B ? 1 : 0;
}

void Unity_Multiply_float_float(float A, float B, out float Out)
{
Out = A * B;
}

void Unity_Fraction_float(float In, out float Out)
{
    Out = frac(In);
}

void Unity_SampleGradientV1_float(Gradient Gradient, float Time, out float4 Out)
{
    // convert to OkLab if we need perceptual color space.
    float3 color = lerp(Gradient.colors[0].rgb, LinearToOklab(Gradient.colors[0].rgb), Gradient.type == 2);

    [unroll]
    for (int c = 1; c < Gradient.colorsLength; c++)
    {
        float colorPos = saturate((Time - Gradient.colors[c - 1].w) / (Gradient.colors[c].w - Gradient.colors[c - 1].w)) * step(c, Gradient.colorsLength - 1);
        float3 color2 = lerp(Gradient.colors[c].rgb, LinearToOklab(Gradient.colors[c].rgb), Gradient.type == 2);
        color = lerp(color, color2, lerp(colorPos, step(0.01, colorPos), Gradient.type % 2)); // grad.type == 1 is fixed, 0 and 2 are blends.
    }
    color = lerp(color, OklabToLinear(color), Gradient.type == 2);

#ifdef UNITY_COLORSPACE_GAMMA
    color = LinearToSRGB(color);
#endif

    float alpha = Gradient.alphas[0].x;
    [unroll]
    for (int a = 1; a < Gradient.alphasLength; a++)
    {
        float alphaPos = saturate((Time - Gradient.alphas[a - 1].y) / (Gradient.alphas[a].y - Gradient.alphas[a - 1].y)) * step(a, Gradient.alphasLength - 1);
        alpha = lerp(alpha, Gradient.alphas[a].x, lerp(alphaPos, step(0.01, alphaPos), Gradient.type % 2));
    }

    Out = float4(color, alpha);
}

void Unity_Branch_float4(float Predicate, float4 True, float4 False, out float4 Out)
{
    Out = Predicate ? True : False;
}

void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
{
Out = A * B;
}

void Unity_Add_float2(float2 A, float2 B, out float2 Out)
{
    Out = A + B;
}

struct Bindings_OneDirectionOutline_98b26c7076faacf4995ee8dca0ee037e_float
{
half4 uv0;
};

void SG_OneDirectionOutline_98b26c7076faacf4995ee8dca0ee037e_float(UnityTexture2D _MainTex, float2 _OutlineDirection, float _OutlineThickness, float2 _TexelSize, Bindings_OneDirectionOutline_98b26c7076faacf4995ee8dca0ee037e_float IN, out float OutAlpha_2)
{
UnityTexture2D _Property_1306d87535f5441fad3e3846f3d376a0_Out_0_Texture2D = _MainTex;
float4 _UV_8174ac5fed2a41779b432a46cf1666eb_Out_0_Vector4 = IN.uv0;
float2 _Property_fcb39e0ed69643318c35efd34122563c_Out_0_Vector2 = _TexelSize;
float2 _Property_a7a7ece6e5e54d48975408b7da9c316f_Out_0_Vector2 = _OutlineDirection;
float2 _Multiply_722a058362cc496d941a3976df7722dc_Out_2_Vector2;
Unity_Multiply_float2_float2(_Property_fcb39e0ed69643318c35efd34122563c_Out_0_Vector2, _Property_a7a7ece6e5e54d48975408b7da9c316f_Out_0_Vector2, _Multiply_722a058362cc496d941a3976df7722dc_Out_2_Vector2);
float _Property_b31903c0d6de4426a7c2fa26ae94835f_Out_0_Float = _OutlineThickness;
float2 _Multiply_05a80bcacb77489d9365a13428ff6336_Out_2_Vector2;
Unity_Multiply_float2_float2(_Multiply_722a058362cc496d941a3976df7722dc_Out_2_Vector2, (_Property_b31903c0d6de4426a7c2fa26ae94835f_Out_0_Float.xx), _Multiply_05a80bcacb77489d9365a13428ff6336_Out_2_Vector2);
float2 _Add_1956f2a04d2c4bcdb557dbf121377204_Out_2_Vector2;
Unity_Add_float2((_UV_8174ac5fed2a41779b432a46cf1666eb_Out_0_Vector4.xy), _Multiply_05a80bcacb77489d9365a13428ff6336_Out_2_Vector2, _Add_1956f2a04d2c4bcdb557dbf121377204_Out_2_Vector2);
float4 _SampleTexture2D_6a3c479c42814ec8bfd30e4d312c27b1_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_1306d87535f5441fad3e3846f3d376a0_Out_0_Texture2D.tex, _Property_1306d87535f5441fad3e3846f3d376a0_Out_0_Texture2D.samplerstate, _Property_1306d87535f5441fad3e3846f3d376a0_Out_0_Texture2D.GetTransformedUV(_Add_1956f2a04d2c4bcdb557dbf121377204_Out_2_Vector2) );
float _SampleTexture2D_6a3c479c42814ec8bfd30e4d312c27b1_R_4_Float = _SampleTexture2D_6a3c479c42814ec8bfd30e4d312c27b1_RGBA_0_Vector4.r;
float _SampleTexture2D_6a3c479c42814ec8bfd30e4d312c27b1_G_5_Float = _SampleTexture2D_6a3c479c42814ec8bfd30e4d312c27b1_RGBA_0_Vector4.g;
float _SampleTexture2D_6a3c479c42814ec8bfd30e4d312c27b1_B_6_Float = _SampleTexture2D_6a3c479c42814ec8bfd30e4d312c27b1_RGBA_0_Vector4.b;
float _SampleTexture2D_6a3c479c42814ec8bfd30e4d312c27b1_A_7_Float = _SampleTexture2D_6a3c479c42814ec8bfd30e4d312c27b1_RGBA_0_Vector4.a;
OutAlpha_2 = _SampleTexture2D_6a3c479c42814ec8bfd30e4d312c27b1_A_7_Float;
}

void Unity_Maximum_float(float A, float B, out float Out)
{
    Out = max(A, B);
}

void Unity_Clamp_float(float In, float Min, float Max, out float Out)
{
    Out = clamp(In, Min, Max);
}

struct Bindings_IntercardinalOutline_c32de0ee88394f1449f11c8588dae173_float
{
half4 uv0;
};

void SG_IntercardinalOutline_c32de0ee88394f1449f11c8588dae173_float(UnityTexture2D _MainTex, float _Alpha, float _OutlineThickness, float2 _TexelSize, Bindings_IntercardinalOutline_c32de0ee88394f1449f11c8588dae173_float IN, out float OutAlpha_2)
{
UnityTexture2D _Property_f2d2f0c6a91146f987bff81c4df4b8d0_Out_0_Texture2D = _MainTex;
float2 _Vector2_0ee9ea2042ff4234aae63d8be2c913a1_Out_0_Vector2 = float2(float(1), float(-1));
float _Property_3ea8e41f4cb14cb08c5a6300bd7a891d_Out_0_Float = _OutlineThickness;
float2 _Property_4cd485a1743443e5a3a83c81658381ce_Out_0_Vector2 = _TexelSize;
Bindings_OneDirectionOutline_98b26c7076faacf4995ee8dca0ee037e_float _OneDirectionOutline_24c147574fef4ae5b3b283568dfcacd8;
_OneDirectionOutline_24c147574fef4ae5b3b283568dfcacd8.uv0 = IN.uv0;
half _OneDirectionOutline_24c147574fef4ae5b3b283568dfcacd8_OutAlpha_2_Float;
SG_OneDirectionOutline_98b26c7076faacf4995ee8dca0ee037e_float(_Property_f2d2f0c6a91146f987bff81c4df4b8d0_Out_0_Texture2D, _Vector2_0ee9ea2042ff4234aae63d8be2c913a1_Out_0_Vector2, _Property_3ea8e41f4cb14cb08c5a6300bd7a891d_Out_0_Float, _Property_4cd485a1743443e5a3a83c81658381ce_Out_0_Vector2, _OneDirectionOutline_24c147574fef4ae5b3b283568dfcacd8, _OneDirectionOutline_24c147574fef4ae5b3b283568dfcacd8_OutAlpha_2_Float);
float2 _Vector2_74d7b11c5f4d453499442e0a15881810_Out_0_Vector2 = float2(float(-1), float(-1));
Bindings_OneDirectionOutline_98b26c7076faacf4995ee8dca0ee037e_float _OneDirectionOutline_b682e7916a9d40a8bed2268ea41d86f5;
_OneDirectionOutline_b682e7916a9d40a8bed2268ea41d86f5.uv0 = IN.uv0;
half _OneDirectionOutline_b682e7916a9d40a8bed2268ea41d86f5_OutAlpha_2_Float;
SG_OneDirectionOutline_98b26c7076faacf4995ee8dca0ee037e_float(_Property_f2d2f0c6a91146f987bff81c4df4b8d0_Out_0_Texture2D, _Vector2_74d7b11c5f4d453499442e0a15881810_Out_0_Vector2, _Property_3ea8e41f4cb14cb08c5a6300bd7a891d_Out_0_Float, _Property_4cd485a1743443e5a3a83c81658381ce_Out_0_Vector2, _OneDirectionOutline_b682e7916a9d40a8bed2268ea41d86f5, _OneDirectionOutline_b682e7916a9d40a8bed2268ea41d86f5_OutAlpha_2_Float);
float _Maximum_234adf467c3c470ebbe8cdb01dd41866_Out_2_Float;
Unity_Maximum_float(_OneDirectionOutline_24c147574fef4ae5b3b283568dfcacd8_OutAlpha_2_Float, _OneDirectionOutline_b682e7916a9d40a8bed2268ea41d86f5_OutAlpha_2_Float, _Maximum_234adf467c3c470ebbe8cdb01dd41866_Out_2_Float);
float2 _Vector2_5b9d8c2ec98b4094b25e2b9e29c64d83_Out_0_Vector2 = float2(float(1), float(1));
Bindings_OneDirectionOutline_98b26c7076faacf4995ee8dca0ee037e_float _OneDirectionOutline_75748be69bf84cbdb7777f268a50c28d;
_OneDirectionOutline_75748be69bf84cbdb7777f268a50c28d.uv0 = IN.uv0;
half _OneDirectionOutline_75748be69bf84cbdb7777f268a50c28d_OutAlpha_2_Float;
SG_OneDirectionOutline_98b26c7076faacf4995ee8dca0ee037e_float(_Property_f2d2f0c6a91146f987bff81c4df4b8d0_Out_0_Texture2D, _Vector2_5b9d8c2ec98b4094b25e2b9e29c64d83_Out_0_Vector2, _Property_3ea8e41f4cb14cb08c5a6300bd7a891d_Out_0_Float, _Property_4cd485a1743443e5a3a83c81658381ce_Out_0_Vector2, _OneDirectionOutline_75748be69bf84cbdb7777f268a50c28d, _OneDirectionOutline_75748be69bf84cbdb7777f268a50c28d_OutAlpha_2_Float);
float2 _Vector2_6740ec37432f4574b09041bad486c76b_Out_0_Vector2 = float2(float(-1), float(1));
Bindings_OneDirectionOutline_98b26c7076faacf4995ee8dca0ee037e_float _OneDirectionOutline_42928c1253cb4c0b9effaa7902148870;
_OneDirectionOutline_42928c1253cb4c0b9effaa7902148870.uv0 = IN.uv0;
half _OneDirectionOutline_42928c1253cb4c0b9effaa7902148870_OutAlpha_2_Float;
SG_OneDirectionOutline_98b26c7076faacf4995ee8dca0ee037e_float(_Property_f2d2f0c6a91146f987bff81c4df4b8d0_Out_0_Texture2D, _Vector2_6740ec37432f4574b09041bad486c76b_Out_0_Vector2, _Property_3ea8e41f4cb14cb08c5a6300bd7a891d_Out_0_Float, _Property_4cd485a1743443e5a3a83c81658381ce_Out_0_Vector2, _OneDirectionOutline_42928c1253cb4c0b9effaa7902148870, _OneDirectionOutline_42928c1253cb4c0b9effaa7902148870_OutAlpha_2_Float);
float _Maximum_4eaa9bb17e104daa8f6b620650d41210_Out_2_Float;
Unity_Maximum_float(_OneDirectionOutline_75748be69bf84cbdb7777f268a50c28d_OutAlpha_2_Float, _OneDirectionOutline_42928c1253cb4c0b9effaa7902148870_OutAlpha_2_Float, _Maximum_4eaa9bb17e104daa8f6b620650d41210_Out_2_Float);
float _Maximum_e683a7fcd27d46d3b64529c5801ee036_Out_2_Float;
Unity_Maximum_float(_Maximum_234adf467c3c470ebbe8cdb01dd41866_Out_2_Float, _Maximum_4eaa9bb17e104daa8f6b620650d41210_Out_2_Float, _Maximum_e683a7fcd27d46d3b64529c5801ee036_Out_2_Float);
float _Clamp_614a1da6ce0e467b84f0eb0f381aa0ec_Out_3_Float;
Unity_Clamp_float(_Maximum_e683a7fcd27d46d3b64529c5801ee036_Out_2_Float, float(0), float(1), _Clamp_614a1da6ce0e467b84f0eb0f381aa0ec_Out_3_Float);
OutAlpha_2 = _Clamp_614a1da6ce0e467b84f0eb0f381aa0ec_Out_3_Float;
}

// Custom interpolators pre vertex
/* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

// Graph Vertex
struct VertexDescription
{
float3 Position;
float3 Normal;
float3 Tangent;
};

VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
{
VertexDescription description = (VertexDescription)0;
description.Position = IN.ObjectSpacePosition;
description.Normal = IN.ObjectSpaceNormal;
description.Tangent = IN.ObjectSpaceTangent;
return description;
}

// Custom interpolators, pre surface
#ifdef FEATURES_GRAPH_VERTEX
Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
{
return output;
}
#define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
#endif

// Graph Pixel
struct SurfaceDescription
{
float3 BaseColor;
float Alpha;
};

SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
{
SurfaceDescription surface = (SurfaceDescription)0;
float4 _SampleTexture2D_bf4de36812474d71befc37c267335de0_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(UnityBuildTexture2DStructNoScale(_Texture2DAsset_142ed57588bb4411b5721e51d840a96c_Out_0_Texture2D).tex, UnityBuildTexture2DStructNoScale(_Texture2DAsset_142ed57588bb4411b5721e51d840a96c_Out_0_Texture2D).samplerstate, UnityBuildTexture2DStructNoScale(_Texture2DAsset_142ed57588bb4411b5721e51d840a96c_Out_0_Texture2D).GetTransformedUV(IN.uv0.xy) );
float _SampleTexture2D_bf4de36812474d71befc37c267335de0_R_4_Float = _SampleTexture2D_bf4de36812474d71befc37c267335de0_RGBA_0_Vector4.r;
float _SampleTexture2D_bf4de36812474d71befc37c267335de0_G_5_Float = _SampleTexture2D_bf4de36812474d71befc37c267335de0_RGBA_0_Vector4.g;
float _SampleTexture2D_bf4de36812474d71befc37c267335de0_B_6_Float = _SampleTexture2D_bf4de36812474d71befc37c267335de0_RGBA_0_Vector4.b;
float _SampleTexture2D_bf4de36812474d71befc37c267335de0_A_7_Float = _SampleTexture2D_bf4de36812474d71befc37c267335de0_RGBA_0_Vector4.a;
float _Comparison_31199d7bae99412594c107a4945966b8_Out_2_Boolean;
Unity_Comparison_Greater_float(_SampleTexture2D_bf4de36812474d71befc37c267335de0_A_7_Float, float(0), _Comparison_31199d7bae99412594c107a4945966b8_Out_2_Boolean);
float4 _Property_72496aa6468947d3b81342440c0b70a1_Out_0_Vector4 = _FaceColor;
float _Property_1f9c3c33d2434ea4967c84f0d79beb20_Out_0_Boolean = _RainbowOutline;
float _Property_076f2dccfc3c4b4a850c7922fc03e893_Out_0_Float = _RainbowSpeed;
float _Multiply_8480769ee1a34167a5460550fea112a2_Out_2_Float;
Unity_Multiply_float_float(_Property_076f2dccfc3c4b4a850c7922fc03e893_Out_0_Float, IN.TimeParameters.x, _Multiply_8480769ee1a34167a5460550fea112a2_Out_2_Float);
float _Fraction_89c08226a54b405793b6af61ddbdbff1_Out_1_Float;
Unity_Fraction_float(_Multiply_8480769ee1a34167a5460550fea112a2_Out_2_Float, _Fraction_89c08226a54b405793b6af61ddbdbff1_Out_1_Float);
float4 _SampleGradient_4d4fbe219f544ee59043a9c2920b4c8f_Out_2_Vector4;
Unity_SampleGradientV1_float(NewGradient(0, 4, 2, float4(1, 0, 0, 0),float4(0, 1, 0, 0.3329976),float4(0, 0, 1, 0.6659952),float4(1, 0, 0, 1),float4(0, 0, 0, 0),float4(0, 0, 0, 0),float4(0, 0, 0, 0),float4(0, 0, 0, 0), float2(1, 0),float2(1, 1),float2(0, 0),float2(0, 0),float2(0, 0),float2(0, 0),float2(0, 0),float2(0, 0)), _Fraction_89c08226a54b405793b6af61ddbdbff1_Out_1_Float, _SampleGradient_4d4fbe219f544ee59043a9c2920b4c8f_Out_2_Vector4);
float4 _Property_e594d7c2e6694502accc6e1f49aebae1_Out_0_Vector4 = UNITY_ACCESS_HYBRID_INSTANCED_PROP(_OutlineColor, float4);
float4 _Branch_896999a999054319b363139c48b649ff_Out_3_Vector4;
Unity_Branch_float4(_Property_1f9c3c33d2434ea4967c84f0d79beb20_Out_0_Boolean, _SampleGradient_4d4fbe219f544ee59043a9c2920b4c8f_Out_2_Vector4, _Property_e594d7c2e6694502accc6e1f49aebae1_Out_0_Vector4, _Branch_896999a999054319b363139c48b649ff_Out_3_Vector4);
float _Split_bc9b5447aba2448ebb2992adce02e5e6_R_1_Float = _Branch_896999a999054319b363139c48b649ff_Out_3_Vector4[0];
float _Split_bc9b5447aba2448ebb2992adce02e5e6_G_2_Float = _Branch_896999a999054319b363139c48b649ff_Out_3_Vector4[1];
float _Split_bc9b5447aba2448ebb2992adce02e5e6_B_3_Float = _Branch_896999a999054319b363139c48b649ff_Out_3_Vector4[2];
float _Split_bc9b5447aba2448ebb2992adce02e5e6_A_4_Float = _Branch_896999a999054319b363139c48b649ff_Out_3_Vector4[3];
float _Property_5abae425deae40e9a1d17f580edcb6e9_Out_0_Float = _Thickness;
float _TextureSize_aeb360e19fa241e9885e448ed47c7f26_Width_0_Float = UnityBuildTexture2DStructNoScale(_Texture2DAsset_142ed57588bb4411b5721e51d840a96c_Out_0_Texture2D).texelSize.z;
float _TextureSize_aeb360e19fa241e9885e448ed47c7f26_Height_2_Float = UnityBuildTexture2DStructNoScale(_Texture2DAsset_142ed57588bb4411b5721e51d840a96c_Out_0_Texture2D).texelSize.w;
float _TextureSize_aeb360e19fa241e9885e448ed47c7f26_TexelWidth_3_Float = UnityBuildTexture2DStructNoScale(_Texture2DAsset_142ed57588bb4411b5721e51d840a96c_Out_0_Texture2D).texelSize.x;
float _TextureSize_aeb360e19fa241e9885e448ed47c7f26_TexelHeight_4_Float = UnityBuildTexture2DStructNoScale(_Texture2DAsset_142ed57588bb4411b5721e51d840a96c_Out_0_Texture2D).texelSize.y;
float2 _Vector2_3619b913caad41e9b0e2215448ac4bd0_Out_0_Vector2 = float2(_TextureSize_aeb360e19fa241e9885e448ed47c7f26_TexelWidth_3_Float, _TextureSize_aeb360e19fa241e9885e448ed47c7f26_TexelHeight_4_Float);
Bindings_IntercardinalOutline_c32de0ee88394f1449f11c8588dae173_float _IntercardinalOutline_c2bf7ef5a7a841a1a013df375e6efa75;
_IntercardinalOutline_c2bf7ef5a7a841a1a013df375e6efa75.uv0 = IN.uv0;
float _IntercardinalOutline_c2bf7ef5a7a841a1a013df375e6efa75_OutAlpha_2_Float;
SG_IntercardinalOutline_c32de0ee88394f1449f11c8588dae173_float(UnityBuildTexture2DStructNoScale(_Texture2DAsset_142ed57588bb4411b5721e51d840a96c_Out_0_Texture2D), float(0), _Property_5abae425deae40e9a1d17f580edcb6e9_Out_0_Float, _Vector2_3619b913caad41e9b0e2215448ac4bd0_Out_0_Vector2, _IntercardinalOutline_c2bf7ef5a7a841a1a013df375e6efa75, _IntercardinalOutline_c2bf7ef5a7a841a1a013df375e6efa75_OutAlpha_2_Float);
float4 _Vector4_c084949a85b140d1ae367e4d5fb4692d_Out_0_Vector4 = float4(_Split_bc9b5447aba2448ebb2992adce02e5e6_R_1_Float, _Split_bc9b5447aba2448ebb2992adce02e5e6_G_2_Float, _Split_bc9b5447aba2448ebb2992adce02e5e6_B_3_Float, _IntercardinalOutline_c2bf7ef5a7a841a1a013df375e6efa75_OutAlpha_2_Float);
float4 _Branch_26b8e9dd9a3147c5bacdda6ffffb067b_Out_3_Vector4;
Unity_Branch_float4(_Comparison_31199d7bae99412594c107a4945966b8_Out_2_Boolean, _Property_72496aa6468947d3b81342440c0b70a1_Out_0_Vector4, _Vector4_c084949a85b140d1ae367e4d5fb4692d_Out_0_Vector4, _Branch_26b8e9dd9a3147c5bacdda6ffffb067b_Out_3_Vector4);
float _Maximum_c3b1f3ee2ae7430c9ca6bb3fb47b45d7_Out_2_Float;
Unity_Maximum_float(_SampleTexture2D_bf4de36812474d71befc37c267335de0_A_7_Float, _IntercardinalOutline_c2bf7ef5a7a841a1a013df375e6efa75_OutAlpha_2_Float, _Maximum_c3b1f3ee2ae7430c9ca6bb3fb47b45d7_Out_2_Float);
surface.BaseColor = (_Branch_26b8e9dd9a3147c5bacdda6ffffb067b_Out_3_Vector4.xyz);
surface.Alpha = _Maximum_c3b1f3ee2ae7430c9ca6bb3fb47b45d7_Out_2_Float;
return surface;
}

// --------------------------------------------------
// Build Graph Inputs
#ifdef HAVE_VFX_MODIFICATION
#define VFX_SRP_ATTRIBUTES Attributes
#define VFX_SRP_VARYINGS Varyings
#define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
#endif
VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
{
    VertexDescriptionInputs output;
    ZERO_INITIALIZE(VertexDescriptionInputs, output);

    output.ObjectSpaceNormal =                          input.normalOS;
    output.ObjectSpaceTangent =                         input.tangentOS.xyz;
    output.ObjectSpacePosition =                        input.positionOS;
#if UNITY_ANY_INSTANCING_ENABLED
#else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
#endif

    return output;
}
SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
{
    SurfaceDescriptionInputs output;
    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

#ifdef HAVE_VFX_MODIFICATION
#if VFX_USE_GRAPH_VALUES
    uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
    /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
#endif
    /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

#endif

    






    #if UNITY_UV_STARTS_AT_TOP
    #else
    #endif


    output.uv0 = input.texCoord0;
#if UNITY_ANY_INSTANCING_ENABLED
#else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
#endif
    output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
#else
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
#endif
#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

        return output;
}

// --------------------------------------------------
// Main

#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteUnlitPass.hlsl"

// --------------------------------------------------
// Visual Effect Vertex Invocations
#ifdef HAVE_VFX_MODIFICATION
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
#endif

ENDHLSL
}
Pass
{
    Name "SceneSelectionPass"
    Tags
    {
        "LightMode" = "SceneSelectionPass"
    }

// Render State
Cull Off

// Debug
// <None>

// --------------------------------------------------
// Pass

HLSLPROGRAM

// Pragmas
#pragma target 2.0
#pragma exclude_renderers d3d11_9x
#pragma multi_compile_instancing
#pragma vertex vert
#pragma fragment frag

// Keywords
// PassKeywords: <None>
// GraphKeywords: <None>

// Defines

#define ATTRIBUTES_NEED_NORMAL
#define ATTRIBUTES_NEED_TANGENT
#define ATTRIBUTES_NEED_TEXCOORD0
#define FEATURES_GRAPH_VERTEX_NORMAL_OUTPUT
#define FEATURES_GRAPH_VERTEX_TANGENT_OUTPUT
#define VARYINGS_NEED_TEXCOORD0
#define FEATURES_GRAPH_VERTEX
/* WARNING: $splice Could not find named fragment 'PassInstancing' */
#define SHADERPASS SHADERPASS_DEPTHONLY
#define SCENESELECTIONPASS 1



// custom interpolator pre-include
/* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

// Includes
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

// --------------------------------------------------
// Structs and Packing

// custom interpolators pre packing
/* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

struct Attributes
{
 float3 positionOS : POSITION;
 float3 normalOS : NORMAL;
 float4 tangentOS : TANGENT;
 float4 uv0 : TEXCOORD0;
#if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
 uint instanceID : INSTANCEID_SEMANTIC;
#endif
};
struct Varyings
{
 float4 positionCS : SV_POSITION;
 float4 texCoord0;
#if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
 uint instanceID : CUSTOM_INSTANCE_ID;
#endif
#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
 uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
#endif
#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
 uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
#endif
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
 FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
#endif
};
struct SurfaceDescriptionInputs
{
 float4 uv0;
};
struct VertexDescriptionInputs
{
 float3 ObjectSpaceNormal;
 float3 ObjectSpaceTangent;
 float3 ObjectSpacePosition;
};
struct PackedVaryings
{
 float4 positionCS : SV_POSITION;
 float4 texCoord0 : INTERP0;
#if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
 uint instanceID : CUSTOM_INSTANCE_ID;
#endif
#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
 uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
#endif
#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
 uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
#endif
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
 FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
#endif
};

PackedVaryings PackVaryings (Varyings input)
{
PackedVaryings output;
ZERO_INITIALIZE(PackedVaryings, output);
output.positionCS = input.positionCS;
output.texCoord0.xyzw = input.texCoord0;
#if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
output.instanceID = input.instanceID;
#endif
#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
#endif
#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
#endif
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
output.cullFace = input.cullFace;
#endif
return output;
}

Varyings UnpackVaryings (PackedVaryings input)
{
Varyings output;
output.positionCS = input.positionCS;
output.texCoord0 = input.texCoord0.xyzw;
#if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
output.instanceID = input.instanceID;
#endif
#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
#endif
#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
#endif
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
output.cullFace = input.cullFace;
#endif
return output;
}


// --------------------------------------------------
// Graph

// Graph Properties
CBUFFER_START(UnityPerMaterial)
float4 _Texture2DAsset_142ed57588bb4411b5721e51d840a96c_Out_0_Texture2D_TexelSize;
float _Thickness;
float4 _OutlineColor;
float _RainbowOutline;
float _RainbowSpeed;
float4 _FaceColor;
UNITY_TEXTURE_STREAMING_DEBUG_VARS;
CBUFFER_END

#if defined(DOTS_INSTANCING_ON)
// DOTS instancing definitions
UNITY_DOTS_INSTANCING_START(MaterialPropertyMetadata)
    UNITY_DOTS_INSTANCED_PROP_OVERRIDE_SUPPORTED(float4, _OutlineColor)
    UNITY_DOTS_INSTANCED_PROP_OVERRIDE_SUPPORTED(float, _RainbowOutline)
UNITY_DOTS_INSTANCING_END(MaterialPropertyMetadata)
// DOTS instancing usage macros
#define UNITY_ACCESS_HYBRID_INSTANCED_PROP(var, type) UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(type, var)
#elif defined(UNITY_INSTANCING_ENABLED)
// Unity instancing definitions
UNITY_INSTANCING_BUFFER_START(SGPerInstanceData)
    UNITY_DEFINE_INSTANCED_PROP(float4, _OutlineColor)
    UNITY_DEFINE_INSTANCED_PROP(float, _RainbowOutline)
UNITY_INSTANCING_BUFFER_END(SGPerInstanceData)
// Unity instancing usage macros
#define UNITY_ACCESS_HYBRID_INSTANCED_PROP(var, type) UNITY_ACCESS_INSTANCED_PROP(SGPerInstanceData, var)
#else
#define UNITY_ACCESS_HYBRID_INSTANCED_PROP(var, type) var
#endif

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
TEXTURE2D(_Texture2DAsset_142ed57588bb4411b5721e51d840a96c_Out_0_Texture2D);
SAMPLER(sampler_Texture2DAsset_142ed57588bb4411b5721e51d840a96c_Out_0_Texture2D);

// Graph Includes
// GraphIncludes: <None>

// -- Property used by ScenePickingPass
#ifdef SCENEPICKINGPASS
float4 _SelectionID;
#endif

// -- Properties used by SceneSelectionPass
#ifdef SCENESELECTIONPASS
int _ObjectId;
int _PassValue;
#endif

// Graph Functions

void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
{
Out = A * B;
}

void Unity_Add_float2(float2 A, float2 B, out float2 Out)
{
    Out = A + B;
}

struct Bindings_OneDirectionOutline_98b26c7076faacf4995ee8dca0ee037e_float
{
half4 uv0;
};

void SG_OneDirectionOutline_98b26c7076faacf4995ee8dca0ee037e_float(UnityTexture2D _MainTex, float2 _OutlineDirection, float _OutlineThickness, float2 _TexelSize, Bindings_OneDirectionOutline_98b26c7076faacf4995ee8dca0ee037e_float IN, out float OutAlpha_2)
{
UnityTexture2D _Property_1306d87535f5441fad3e3846f3d376a0_Out_0_Texture2D = _MainTex;
float4 _UV_8174ac5fed2a41779b432a46cf1666eb_Out_0_Vector4 = IN.uv0;
float2 _Property_fcb39e0ed69643318c35efd34122563c_Out_0_Vector2 = _TexelSize;
float2 _Property_a7a7ece6e5e54d48975408b7da9c316f_Out_0_Vector2 = _OutlineDirection;
float2 _Multiply_722a058362cc496d941a3976df7722dc_Out_2_Vector2;
Unity_Multiply_float2_float2(_Property_fcb39e0ed69643318c35efd34122563c_Out_0_Vector2, _Property_a7a7ece6e5e54d48975408b7da9c316f_Out_0_Vector2, _Multiply_722a058362cc496d941a3976df7722dc_Out_2_Vector2);
float _Property_b31903c0d6de4426a7c2fa26ae94835f_Out_0_Float = _OutlineThickness;
float2 _Multiply_05a80bcacb77489d9365a13428ff6336_Out_2_Vector2;
Unity_Multiply_float2_float2(_Multiply_722a058362cc496d941a3976df7722dc_Out_2_Vector2, (_Property_b31903c0d6de4426a7c2fa26ae94835f_Out_0_Float.xx), _Multiply_05a80bcacb77489d9365a13428ff6336_Out_2_Vector2);
float2 _Add_1956f2a04d2c4bcdb557dbf121377204_Out_2_Vector2;
Unity_Add_float2((_UV_8174ac5fed2a41779b432a46cf1666eb_Out_0_Vector4.xy), _Multiply_05a80bcacb77489d9365a13428ff6336_Out_2_Vector2, _Add_1956f2a04d2c4bcdb557dbf121377204_Out_2_Vector2);
float4 _SampleTexture2D_6a3c479c42814ec8bfd30e4d312c27b1_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_1306d87535f5441fad3e3846f3d376a0_Out_0_Texture2D.tex, _Property_1306d87535f5441fad3e3846f3d376a0_Out_0_Texture2D.samplerstate, _Property_1306d87535f5441fad3e3846f3d376a0_Out_0_Texture2D.GetTransformedUV(_Add_1956f2a04d2c4bcdb557dbf121377204_Out_2_Vector2) );
float _SampleTexture2D_6a3c479c42814ec8bfd30e4d312c27b1_R_4_Float = _SampleTexture2D_6a3c479c42814ec8bfd30e4d312c27b1_RGBA_0_Vector4.r;
float _SampleTexture2D_6a3c479c42814ec8bfd30e4d312c27b1_G_5_Float = _SampleTexture2D_6a3c479c42814ec8bfd30e4d312c27b1_RGBA_0_Vector4.g;
float _SampleTexture2D_6a3c479c42814ec8bfd30e4d312c27b1_B_6_Float = _SampleTexture2D_6a3c479c42814ec8bfd30e4d312c27b1_RGBA_0_Vector4.b;
float _SampleTexture2D_6a3c479c42814ec8bfd30e4d312c27b1_A_7_Float = _SampleTexture2D_6a3c479c42814ec8bfd30e4d312c27b1_RGBA_0_Vector4.a;
OutAlpha_2 = _SampleTexture2D_6a3c479c42814ec8bfd30e4d312c27b1_A_7_Float;
}

void Unity_Maximum_float(float A, float B, out float Out)
{
    Out = max(A, B);
}

void Unity_Clamp_float(float In, float Min, float Max, out float Out)
{
    Out = clamp(In, Min, Max);
}

struct Bindings_IntercardinalOutline_c32de0ee88394f1449f11c8588dae173_float
{
half4 uv0;
};

void SG_IntercardinalOutline_c32de0ee88394f1449f11c8588dae173_float(UnityTexture2D _MainTex, float _Alpha, float _OutlineThickness, float2 _TexelSize, Bindings_IntercardinalOutline_c32de0ee88394f1449f11c8588dae173_float IN, out float OutAlpha_2)
{
UnityTexture2D _Property_f2d2f0c6a91146f987bff81c4df4b8d0_Out_0_Texture2D = _MainTex;
float2 _Vector2_0ee9ea2042ff4234aae63d8be2c913a1_Out_0_Vector2 = float2(float(1), float(-1));
float _Property_3ea8e41f4cb14cb08c5a6300bd7a891d_Out_0_Float = _OutlineThickness;
float2 _Property_4cd485a1743443e5a3a83c81658381ce_Out_0_Vector2 = _TexelSize;
Bindings_OneDirectionOutline_98b26c7076faacf4995ee8dca0ee037e_float _OneDirectionOutline_24c147574fef4ae5b3b283568dfcacd8;
_OneDirectionOutline_24c147574fef4ae5b3b283568dfcacd8.uv0 = IN.uv0;
half _OneDirectionOutline_24c147574fef4ae5b3b283568dfcacd8_OutAlpha_2_Float;
SG_OneDirectionOutline_98b26c7076faacf4995ee8dca0ee037e_float(_Property_f2d2f0c6a91146f987bff81c4df4b8d0_Out_0_Texture2D, _Vector2_0ee9ea2042ff4234aae63d8be2c913a1_Out_0_Vector2, _Property_3ea8e41f4cb14cb08c5a6300bd7a891d_Out_0_Float, _Property_4cd485a1743443e5a3a83c81658381ce_Out_0_Vector2, _OneDirectionOutline_24c147574fef4ae5b3b283568dfcacd8, _OneDirectionOutline_24c147574fef4ae5b3b283568dfcacd8_OutAlpha_2_Float);
float2 _Vector2_74d7b11c5f4d453499442e0a15881810_Out_0_Vector2 = float2(float(-1), float(-1));
Bindings_OneDirectionOutline_98b26c7076faacf4995ee8dca0ee037e_float _OneDirectionOutline_b682e7916a9d40a8bed2268ea41d86f5;
_OneDirectionOutline_b682e7916a9d40a8bed2268ea41d86f5.uv0 = IN.uv0;
half _OneDirectionOutline_b682e7916a9d40a8bed2268ea41d86f5_OutAlpha_2_Float;
SG_OneDirectionOutline_98b26c7076faacf4995ee8dca0ee037e_float(_Property_f2d2f0c6a91146f987bff81c4df4b8d0_Out_0_Texture2D, _Vector2_74d7b11c5f4d453499442e0a15881810_Out_0_Vector2, _Property_3ea8e41f4cb14cb08c5a6300bd7a891d_Out_0_Float, _Property_4cd485a1743443e5a3a83c81658381ce_Out_0_Vector2, _OneDirectionOutline_b682e7916a9d40a8bed2268ea41d86f5, _OneDirectionOutline_b682e7916a9d40a8bed2268ea41d86f5_OutAlpha_2_Float);
float _Maximum_234adf467c3c470ebbe8cdb01dd41866_Out_2_Float;
Unity_Maximum_float(_OneDirectionOutline_24c147574fef4ae5b3b283568dfcacd8_OutAlpha_2_Float, _OneDirectionOutline_b682e7916a9d40a8bed2268ea41d86f5_OutAlpha_2_Float, _Maximum_234adf467c3c470ebbe8cdb01dd41866_Out_2_Float);
float2 _Vector2_5b9d8c2ec98b4094b25e2b9e29c64d83_Out_0_Vector2 = float2(float(1), float(1));
Bindings_OneDirectionOutline_98b26c7076faacf4995ee8dca0ee037e_float _OneDirectionOutline_75748be69bf84cbdb7777f268a50c28d;
_OneDirectionOutline_75748be69bf84cbdb7777f268a50c28d.uv0 = IN.uv0;
half _OneDirectionOutline_75748be69bf84cbdb7777f268a50c28d_OutAlpha_2_Float;
SG_OneDirectionOutline_98b26c7076faacf4995ee8dca0ee037e_float(_Property_f2d2f0c6a91146f987bff81c4df4b8d0_Out_0_Texture2D, _Vector2_5b9d8c2ec98b4094b25e2b9e29c64d83_Out_0_Vector2, _Property_3ea8e41f4cb14cb08c5a6300bd7a891d_Out_0_Float, _Property_4cd485a1743443e5a3a83c81658381ce_Out_0_Vector2, _OneDirectionOutline_75748be69bf84cbdb7777f268a50c28d, _OneDirectionOutline_75748be69bf84cbdb7777f268a50c28d_OutAlpha_2_Float);
float2 _Vector2_6740ec37432f4574b09041bad486c76b_Out_0_Vector2 = float2(float(-1), float(1));
Bindings_OneDirectionOutline_98b26c7076faacf4995ee8dca0ee037e_float _OneDirectionOutline_42928c1253cb4c0b9effaa7902148870;
_OneDirectionOutline_42928c1253cb4c0b9effaa7902148870.uv0 = IN.uv0;
half _OneDirectionOutline_42928c1253cb4c0b9effaa7902148870_OutAlpha_2_Float;
SG_OneDirectionOutline_98b26c7076faacf4995ee8dca0ee037e_float(_Property_f2d2f0c6a91146f987bff81c4df4b8d0_Out_0_Texture2D, _Vector2_6740ec37432f4574b09041bad486c76b_Out_0_Vector2, _Property_3ea8e41f4cb14cb08c5a6300bd7a891d_Out_0_Float, _Property_4cd485a1743443e5a3a83c81658381ce_Out_0_Vector2, _OneDirectionOutline_42928c1253cb4c0b9effaa7902148870, _OneDirectionOutline_42928c1253cb4c0b9effaa7902148870_OutAlpha_2_Float);
float _Maximum_4eaa9bb17e104daa8f6b620650d41210_Out_2_Float;
Unity_Maximum_float(_OneDirectionOutline_75748be69bf84cbdb7777f268a50c28d_OutAlpha_2_Float, _OneDirectionOutline_42928c1253cb4c0b9effaa7902148870_OutAlpha_2_Float, _Maximum_4eaa9bb17e104daa8f6b620650d41210_Out_2_Float);
float _Maximum_e683a7fcd27d46d3b64529c5801ee036_Out_2_Float;
Unity_Maximum_float(_Maximum_234adf467c3c470ebbe8cdb01dd41866_Out_2_Float, _Maximum_4eaa9bb17e104daa8f6b620650d41210_Out_2_Float, _Maximum_e683a7fcd27d46d3b64529c5801ee036_Out_2_Float);
float _Clamp_614a1da6ce0e467b84f0eb0f381aa0ec_Out_3_Float;
Unity_Clamp_float(_Maximum_e683a7fcd27d46d3b64529c5801ee036_Out_2_Float, float(0), float(1), _Clamp_614a1da6ce0e467b84f0eb0f381aa0ec_Out_3_Float);
OutAlpha_2 = _Clamp_614a1da6ce0e467b84f0eb0f381aa0ec_Out_3_Float;
}

// Custom interpolators pre vertex
/* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

// Graph Vertex
struct VertexDescription
{
float3 Position;
float3 Normal;
float3 Tangent;
};

VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
{
VertexDescription description = (VertexDescription)0;
description.Position = IN.ObjectSpacePosition;
description.Normal = IN.ObjectSpaceNormal;
description.Tangent = IN.ObjectSpaceTangent;
return description;
}

// Custom interpolators, pre surface
#ifdef FEATURES_GRAPH_VERTEX
Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
{
return output;
}
#define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
#endif

// Graph Pixel
struct SurfaceDescription
{
float Alpha;
};

SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
{
SurfaceDescription surface = (SurfaceDescription)0;
float4 _SampleTexture2D_bf4de36812474d71befc37c267335de0_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(UnityBuildTexture2DStructNoScale(_Texture2DAsset_142ed57588bb4411b5721e51d840a96c_Out_0_Texture2D).tex, UnityBuildTexture2DStructNoScale(_Texture2DAsset_142ed57588bb4411b5721e51d840a96c_Out_0_Texture2D).samplerstate, UnityBuildTexture2DStructNoScale(_Texture2DAsset_142ed57588bb4411b5721e51d840a96c_Out_0_Texture2D).GetTransformedUV(IN.uv0.xy) );
float _SampleTexture2D_bf4de36812474d71befc37c267335de0_R_4_Float = _SampleTexture2D_bf4de36812474d71befc37c267335de0_RGBA_0_Vector4.r;
float _SampleTexture2D_bf4de36812474d71befc37c267335de0_G_5_Float = _SampleTexture2D_bf4de36812474d71befc37c267335de0_RGBA_0_Vector4.g;
float _SampleTexture2D_bf4de36812474d71befc37c267335de0_B_6_Float = _SampleTexture2D_bf4de36812474d71befc37c267335de0_RGBA_0_Vector4.b;
float _SampleTexture2D_bf4de36812474d71befc37c267335de0_A_7_Float = _SampleTexture2D_bf4de36812474d71befc37c267335de0_RGBA_0_Vector4.a;
float _Property_5abae425deae40e9a1d17f580edcb6e9_Out_0_Float = _Thickness;
float _TextureSize_aeb360e19fa241e9885e448ed47c7f26_Width_0_Float = UnityBuildTexture2DStructNoScale(_Texture2DAsset_142ed57588bb4411b5721e51d840a96c_Out_0_Texture2D).texelSize.z;
float _TextureSize_aeb360e19fa241e9885e448ed47c7f26_Height_2_Float = UnityBuildTexture2DStructNoScale(_Texture2DAsset_142ed57588bb4411b5721e51d840a96c_Out_0_Texture2D).texelSize.w;
float _TextureSize_aeb360e19fa241e9885e448ed47c7f26_TexelWidth_3_Float = UnityBuildTexture2DStructNoScale(_Texture2DAsset_142ed57588bb4411b5721e51d840a96c_Out_0_Texture2D).texelSize.x;
float _TextureSize_aeb360e19fa241e9885e448ed47c7f26_TexelHeight_4_Float = UnityBuildTexture2DStructNoScale(_Texture2DAsset_142ed57588bb4411b5721e51d840a96c_Out_0_Texture2D).texelSize.y;
float2 _Vector2_3619b913caad41e9b0e2215448ac4bd0_Out_0_Vector2 = float2(_TextureSize_aeb360e19fa241e9885e448ed47c7f26_TexelWidth_3_Float, _TextureSize_aeb360e19fa241e9885e448ed47c7f26_TexelHeight_4_Float);
Bindings_IntercardinalOutline_c32de0ee88394f1449f11c8588dae173_float _IntercardinalOutline_c2bf7ef5a7a841a1a013df375e6efa75;
_IntercardinalOutline_c2bf7ef5a7a841a1a013df375e6efa75.uv0 = IN.uv0;
float _IntercardinalOutline_c2bf7ef5a7a841a1a013df375e6efa75_OutAlpha_2_Float;
SG_IntercardinalOutline_c32de0ee88394f1449f11c8588dae173_float(UnityBuildTexture2DStructNoScale(_Texture2DAsset_142ed57588bb4411b5721e51d840a96c_Out_0_Texture2D), float(0), _Property_5abae425deae40e9a1d17f580edcb6e9_Out_0_Float, _Vector2_3619b913caad41e9b0e2215448ac4bd0_Out_0_Vector2, _IntercardinalOutline_c2bf7ef5a7a841a1a013df375e6efa75, _IntercardinalOutline_c2bf7ef5a7a841a1a013df375e6efa75_OutAlpha_2_Float);
float _Maximum_c3b1f3ee2ae7430c9ca6bb3fb47b45d7_Out_2_Float;
Unity_Maximum_float(_SampleTexture2D_bf4de36812474d71befc37c267335de0_A_7_Float, _IntercardinalOutline_c2bf7ef5a7a841a1a013df375e6efa75_OutAlpha_2_Float, _Maximum_c3b1f3ee2ae7430c9ca6bb3fb47b45d7_Out_2_Float);
surface.Alpha = _Maximum_c3b1f3ee2ae7430c9ca6bb3fb47b45d7_Out_2_Float;
return surface;
}

// --------------------------------------------------
// Build Graph Inputs
#ifdef HAVE_VFX_MODIFICATION
#define VFX_SRP_ATTRIBUTES Attributes
#define VFX_SRP_VARYINGS Varyings
#define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
#endif
VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
{
    VertexDescriptionInputs output;
    ZERO_INITIALIZE(VertexDescriptionInputs, output);

    output.ObjectSpaceNormal =                          input.normalOS;
    output.ObjectSpaceTangent =                         input.tangentOS.xyz;
    output.ObjectSpacePosition =                        input.positionOS;
#if UNITY_ANY_INSTANCING_ENABLED
#else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
#endif

    return output;
}
SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
{
    SurfaceDescriptionInputs output;
    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

#ifdef HAVE_VFX_MODIFICATION
#if VFX_USE_GRAPH_VALUES
    uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
    /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
#endif
    /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

#endif

    






    #if UNITY_UV_STARTS_AT_TOP
    #else
    #endif


    output.uv0 = input.texCoord0;
#if UNITY_ANY_INSTANCING_ENABLED
#else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
#endif
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
#else
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
#endif
#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

        return output;
}

// --------------------------------------------------
// Main

#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"

// --------------------------------------------------
// Visual Effect Vertex Invocations
#ifdef HAVE_VFX_MODIFICATION
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
#endif

ENDHLSL
}
Pass
{
    Name "ScenePickingPass"
    Tags
    {
        "LightMode" = "Picking"
    }

// Render State
Cull Back

// Debug
// <None>

// --------------------------------------------------
// Pass

HLSLPROGRAM

// Pragmas
#pragma target 2.0
#pragma exclude_renderers d3d11_9x
#pragma multi_compile_instancing
#pragma vertex vert
#pragma fragment frag

// Keywords
// PassKeywords: <None>
// GraphKeywords: <None>

// Defines

#define ATTRIBUTES_NEED_NORMAL
#define ATTRIBUTES_NEED_TANGENT
#define ATTRIBUTES_NEED_TEXCOORD0
#define FEATURES_GRAPH_VERTEX_NORMAL_OUTPUT
#define FEATURES_GRAPH_VERTEX_TANGENT_OUTPUT
#define VARYINGS_NEED_TEXCOORD0
#define FEATURES_GRAPH_VERTEX
/* WARNING: $splice Could not find named fragment 'PassInstancing' */
#define SHADERPASS SHADERPASS_DEPTHONLY
#define SCENEPICKINGPASS 1



// custom interpolator pre-include
/* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

// Includes
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

// --------------------------------------------------
// Structs and Packing

// custom interpolators pre packing
/* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

struct Attributes
{
 float3 positionOS : POSITION;
 float3 normalOS : NORMAL;
 float4 tangentOS : TANGENT;
 float4 uv0 : TEXCOORD0;
#if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
 uint instanceID : INSTANCEID_SEMANTIC;
#endif
};
struct Varyings
{
 float4 positionCS : SV_POSITION;
 float4 texCoord0;
#if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
 uint instanceID : CUSTOM_INSTANCE_ID;
#endif
#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
 uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
#endif
#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
 uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
#endif
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
 FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
#endif
};
struct SurfaceDescriptionInputs
{
 float4 uv0;
};
struct VertexDescriptionInputs
{
 float3 ObjectSpaceNormal;
 float3 ObjectSpaceTangent;
 float3 ObjectSpacePosition;
};
struct PackedVaryings
{
 float4 positionCS : SV_POSITION;
 float4 texCoord0 : INTERP0;
#if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
 uint instanceID : CUSTOM_INSTANCE_ID;
#endif
#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
 uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
#endif
#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
 uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
#endif
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
 FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
#endif
};

PackedVaryings PackVaryings (Varyings input)
{
PackedVaryings output;
ZERO_INITIALIZE(PackedVaryings, output);
output.positionCS = input.positionCS;
output.texCoord0.xyzw = input.texCoord0;
#if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
output.instanceID = input.instanceID;
#endif
#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
#endif
#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
#endif
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
output.cullFace = input.cullFace;
#endif
return output;
}

Varyings UnpackVaryings (PackedVaryings input)
{
Varyings output;
output.positionCS = input.positionCS;
output.texCoord0 = input.texCoord0.xyzw;
#if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
output.instanceID = input.instanceID;
#endif
#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
#endif
#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
#endif
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
output.cullFace = input.cullFace;
#endif
return output;
}


// --------------------------------------------------
// Graph

// Graph Properties
CBUFFER_START(UnityPerMaterial)
float4 _Texture2DAsset_142ed57588bb4411b5721e51d840a96c_Out_0_Texture2D_TexelSize;
float _Thickness;
float4 _OutlineColor;
float _RainbowOutline;
float _RainbowSpeed;
float4 _FaceColor;
UNITY_TEXTURE_STREAMING_DEBUG_VARS;
CBUFFER_END

#if defined(DOTS_INSTANCING_ON)
// DOTS instancing definitions
UNITY_DOTS_INSTANCING_START(MaterialPropertyMetadata)
    UNITY_DOTS_INSTANCED_PROP_OVERRIDE_SUPPORTED(float4, _OutlineColor)
    UNITY_DOTS_INSTANCED_PROP_OVERRIDE_SUPPORTED(float, _RainbowOutline)
UNITY_DOTS_INSTANCING_END(MaterialPropertyMetadata)
// DOTS instancing usage macros
#define UNITY_ACCESS_HYBRID_INSTANCED_PROP(var, type) UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(type, var)
#elif defined(UNITY_INSTANCING_ENABLED)
// Unity instancing definitions
UNITY_INSTANCING_BUFFER_START(SGPerInstanceData)
    UNITY_DEFINE_INSTANCED_PROP(float4, _OutlineColor)
    UNITY_DEFINE_INSTANCED_PROP(float, _RainbowOutline)
UNITY_INSTANCING_BUFFER_END(SGPerInstanceData)
// Unity instancing usage macros
#define UNITY_ACCESS_HYBRID_INSTANCED_PROP(var, type) UNITY_ACCESS_INSTANCED_PROP(SGPerInstanceData, var)
#else
#define UNITY_ACCESS_HYBRID_INSTANCED_PROP(var, type) var
#endif

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
TEXTURE2D(_Texture2DAsset_142ed57588bb4411b5721e51d840a96c_Out_0_Texture2D);
SAMPLER(sampler_Texture2DAsset_142ed57588bb4411b5721e51d840a96c_Out_0_Texture2D);

// Graph Includes
// GraphIncludes: <None>

// -- Property used by ScenePickingPass
#ifdef SCENEPICKINGPASS
float4 _SelectionID;
#endif

// -- Properties used by SceneSelectionPass
#ifdef SCENESELECTIONPASS
int _ObjectId;
int _PassValue;
#endif

// Graph Functions

void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
{
Out = A * B;
}

void Unity_Add_float2(float2 A, float2 B, out float2 Out)
{
    Out = A + B;
}

struct Bindings_OneDirectionOutline_98b26c7076faacf4995ee8dca0ee037e_float
{
half4 uv0;
};

void SG_OneDirectionOutline_98b26c7076faacf4995ee8dca0ee037e_float(UnityTexture2D _MainTex, float2 _OutlineDirection, float _OutlineThickness, float2 _TexelSize, Bindings_OneDirectionOutline_98b26c7076faacf4995ee8dca0ee037e_float IN, out float OutAlpha_2)
{
UnityTexture2D _Property_1306d87535f5441fad3e3846f3d376a0_Out_0_Texture2D = _MainTex;
float4 _UV_8174ac5fed2a41779b432a46cf1666eb_Out_0_Vector4 = IN.uv0;
float2 _Property_fcb39e0ed69643318c35efd34122563c_Out_0_Vector2 = _TexelSize;
float2 _Property_a7a7ece6e5e54d48975408b7da9c316f_Out_0_Vector2 = _OutlineDirection;
float2 _Multiply_722a058362cc496d941a3976df7722dc_Out_2_Vector2;
Unity_Multiply_float2_float2(_Property_fcb39e0ed69643318c35efd34122563c_Out_0_Vector2, _Property_a7a7ece6e5e54d48975408b7da9c316f_Out_0_Vector2, _Multiply_722a058362cc496d941a3976df7722dc_Out_2_Vector2);
float _Property_b31903c0d6de4426a7c2fa26ae94835f_Out_0_Float = _OutlineThickness;
float2 _Multiply_05a80bcacb77489d9365a13428ff6336_Out_2_Vector2;
Unity_Multiply_float2_float2(_Multiply_722a058362cc496d941a3976df7722dc_Out_2_Vector2, (_Property_b31903c0d6de4426a7c2fa26ae94835f_Out_0_Float.xx), _Multiply_05a80bcacb77489d9365a13428ff6336_Out_2_Vector2);
float2 _Add_1956f2a04d2c4bcdb557dbf121377204_Out_2_Vector2;
Unity_Add_float2((_UV_8174ac5fed2a41779b432a46cf1666eb_Out_0_Vector4.xy), _Multiply_05a80bcacb77489d9365a13428ff6336_Out_2_Vector2, _Add_1956f2a04d2c4bcdb557dbf121377204_Out_2_Vector2);
float4 _SampleTexture2D_6a3c479c42814ec8bfd30e4d312c27b1_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_1306d87535f5441fad3e3846f3d376a0_Out_0_Texture2D.tex, _Property_1306d87535f5441fad3e3846f3d376a0_Out_0_Texture2D.samplerstate, _Property_1306d87535f5441fad3e3846f3d376a0_Out_0_Texture2D.GetTransformedUV(_Add_1956f2a04d2c4bcdb557dbf121377204_Out_2_Vector2) );
float _SampleTexture2D_6a3c479c42814ec8bfd30e4d312c27b1_R_4_Float = _SampleTexture2D_6a3c479c42814ec8bfd30e4d312c27b1_RGBA_0_Vector4.r;
float _SampleTexture2D_6a3c479c42814ec8bfd30e4d312c27b1_G_5_Float = _SampleTexture2D_6a3c479c42814ec8bfd30e4d312c27b1_RGBA_0_Vector4.g;
float _SampleTexture2D_6a3c479c42814ec8bfd30e4d312c27b1_B_6_Float = _SampleTexture2D_6a3c479c42814ec8bfd30e4d312c27b1_RGBA_0_Vector4.b;
float _SampleTexture2D_6a3c479c42814ec8bfd30e4d312c27b1_A_7_Float = _SampleTexture2D_6a3c479c42814ec8bfd30e4d312c27b1_RGBA_0_Vector4.a;
OutAlpha_2 = _SampleTexture2D_6a3c479c42814ec8bfd30e4d312c27b1_A_7_Float;
}

void Unity_Maximum_float(float A, float B, out float Out)
{
    Out = max(A, B);
}

void Unity_Clamp_float(float In, float Min, float Max, out float Out)
{
    Out = clamp(In, Min, Max);
}

struct Bindings_IntercardinalOutline_c32de0ee88394f1449f11c8588dae173_float
{
half4 uv0;
};

void SG_IntercardinalOutline_c32de0ee88394f1449f11c8588dae173_float(UnityTexture2D _MainTex, float _Alpha, float _OutlineThickness, float2 _TexelSize, Bindings_IntercardinalOutline_c32de0ee88394f1449f11c8588dae173_float IN, out float OutAlpha_2)
{
UnityTexture2D _Property_f2d2f0c6a91146f987bff81c4df4b8d0_Out_0_Texture2D = _MainTex;
float2 _Vector2_0ee9ea2042ff4234aae63d8be2c913a1_Out_0_Vector2 = float2(float(1), float(-1));
float _Property_3ea8e41f4cb14cb08c5a6300bd7a891d_Out_0_Float = _OutlineThickness;
float2 _Property_4cd485a1743443e5a3a83c81658381ce_Out_0_Vector2 = _TexelSize;
Bindings_OneDirectionOutline_98b26c7076faacf4995ee8dca0ee037e_float _OneDirectionOutline_24c147574fef4ae5b3b283568dfcacd8;
_OneDirectionOutline_24c147574fef4ae5b3b283568dfcacd8.uv0 = IN.uv0;
half _OneDirectionOutline_24c147574fef4ae5b3b283568dfcacd8_OutAlpha_2_Float;
SG_OneDirectionOutline_98b26c7076faacf4995ee8dca0ee037e_float(_Property_f2d2f0c6a91146f987bff81c4df4b8d0_Out_0_Texture2D, _Vector2_0ee9ea2042ff4234aae63d8be2c913a1_Out_0_Vector2, _Property_3ea8e41f4cb14cb08c5a6300bd7a891d_Out_0_Float, _Property_4cd485a1743443e5a3a83c81658381ce_Out_0_Vector2, _OneDirectionOutline_24c147574fef4ae5b3b283568dfcacd8, _OneDirectionOutline_24c147574fef4ae5b3b283568dfcacd8_OutAlpha_2_Float);
float2 _Vector2_74d7b11c5f4d453499442e0a15881810_Out_0_Vector2 = float2(float(-1), float(-1));
Bindings_OneDirectionOutline_98b26c7076faacf4995ee8dca0ee037e_float _OneDirectionOutline_b682e7916a9d40a8bed2268ea41d86f5;
_OneDirectionOutline_b682e7916a9d40a8bed2268ea41d86f5.uv0 = IN.uv0;
half _OneDirectionOutline_b682e7916a9d40a8bed2268ea41d86f5_OutAlpha_2_Float;
SG_OneDirectionOutline_98b26c7076faacf4995ee8dca0ee037e_float(_Property_f2d2f0c6a91146f987bff81c4df4b8d0_Out_0_Texture2D, _Vector2_74d7b11c5f4d453499442e0a15881810_Out_0_Vector2, _Property_3ea8e41f4cb14cb08c5a6300bd7a891d_Out_0_Float, _Property_4cd485a1743443e5a3a83c81658381ce_Out_0_Vector2, _OneDirectionOutline_b682e7916a9d40a8bed2268ea41d86f5, _OneDirectionOutline_b682e7916a9d40a8bed2268ea41d86f5_OutAlpha_2_Float);
float _Maximum_234adf467c3c470ebbe8cdb01dd41866_Out_2_Float;
Unity_Maximum_float(_OneDirectionOutline_24c147574fef4ae5b3b283568dfcacd8_OutAlpha_2_Float, _OneDirectionOutline_b682e7916a9d40a8bed2268ea41d86f5_OutAlpha_2_Float, _Maximum_234adf467c3c470ebbe8cdb01dd41866_Out_2_Float);
float2 _Vector2_5b9d8c2ec98b4094b25e2b9e29c64d83_Out_0_Vector2 = float2(float(1), float(1));
Bindings_OneDirectionOutline_98b26c7076faacf4995ee8dca0ee037e_float _OneDirectionOutline_75748be69bf84cbdb7777f268a50c28d;
_OneDirectionOutline_75748be69bf84cbdb7777f268a50c28d.uv0 = IN.uv0;
half _OneDirectionOutline_75748be69bf84cbdb7777f268a50c28d_OutAlpha_2_Float;
SG_OneDirectionOutline_98b26c7076faacf4995ee8dca0ee037e_float(_Property_f2d2f0c6a91146f987bff81c4df4b8d0_Out_0_Texture2D, _Vector2_5b9d8c2ec98b4094b25e2b9e29c64d83_Out_0_Vector2, _Property_3ea8e41f4cb14cb08c5a6300bd7a891d_Out_0_Float, _Property_4cd485a1743443e5a3a83c81658381ce_Out_0_Vector2, _OneDirectionOutline_75748be69bf84cbdb7777f268a50c28d, _OneDirectionOutline_75748be69bf84cbdb7777f268a50c28d_OutAlpha_2_Float);
float2 _Vector2_6740ec37432f4574b09041bad486c76b_Out_0_Vector2 = float2(float(-1), float(1));
Bindings_OneDirectionOutline_98b26c7076faacf4995ee8dca0ee037e_float _OneDirectionOutline_42928c1253cb4c0b9effaa7902148870;
_OneDirectionOutline_42928c1253cb4c0b9effaa7902148870.uv0 = IN.uv0;
half _OneDirectionOutline_42928c1253cb4c0b9effaa7902148870_OutAlpha_2_Float;
SG_OneDirectionOutline_98b26c7076faacf4995ee8dca0ee037e_float(_Property_f2d2f0c6a91146f987bff81c4df4b8d0_Out_0_Texture2D, _Vector2_6740ec37432f4574b09041bad486c76b_Out_0_Vector2, _Property_3ea8e41f4cb14cb08c5a6300bd7a891d_Out_0_Float, _Property_4cd485a1743443e5a3a83c81658381ce_Out_0_Vector2, _OneDirectionOutline_42928c1253cb4c0b9effaa7902148870, _OneDirectionOutline_42928c1253cb4c0b9effaa7902148870_OutAlpha_2_Float);
float _Maximum_4eaa9bb17e104daa8f6b620650d41210_Out_2_Float;
Unity_Maximum_float(_OneDirectionOutline_75748be69bf84cbdb7777f268a50c28d_OutAlpha_2_Float, _OneDirectionOutline_42928c1253cb4c0b9effaa7902148870_OutAlpha_2_Float, _Maximum_4eaa9bb17e104daa8f6b620650d41210_Out_2_Float);
float _Maximum_e683a7fcd27d46d3b64529c5801ee036_Out_2_Float;
Unity_Maximum_float(_Maximum_234adf467c3c470ebbe8cdb01dd41866_Out_2_Float, _Maximum_4eaa9bb17e104daa8f6b620650d41210_Out_2_Float, _Maximum_e683a7fcd27d46d3b64529c5801ee036_Out_2_Float);
float _Clamp_614a1da6ce0e467b84f0eb0f381aa0ec_Out_3_Float;
Unity_Clamp_float(_Maximum_e683a7fcd27d46d3b64529c5801ee036_Out_2_Float, float(0), float(1), _Clamp_614a1da6ce0e467b84f0eb0f381aa0ec_Out_3_Float);
OutAlpha_2 = _Clamp_614a1da6ce0e467b84f0eb0f381aa0ec_Out_3_Float;
}

// Custom interpolators pre vertex
/* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

// Graph Vertex
struct VertexDescription
{
float3 Position;
float3 Normal;
float3 Tangent;
};

VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
{
VertexDescription description = (VertexDescription)0;
description.Position = IN.ObjectSpacePosition;
description.Normal = IN.ObjectSpaceNormal;
description.Tangent = IN.ObjectSpaceTangent;
return description;
}

// Custom interpolators, pre surface
#ifdef FEATURES_GRAPH_VERTEX
Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
{
return output;
}
#define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
#endif

// Graph Pixel
struct SurfaceDescription
{
float Alpha;
};

SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
{
SurfaceDescription surface = (SurfaceDescription)0;
float4 _SampleTexture2D_bf4de36812474d71befc37c267335de0_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(UnityBuildTexture2DStructNoScale(_Texture2DAsset_142ed57588bb4411b5721e51d840a96c_Out_0_Texture2D).tex, UnityBuildTexture2DStructNoScale(_Texture2DAsset_142ed57588bb4411b5721e51d840a96c_Out_0_Texture2D).samplerstate, UnityBuildTexture2DStructNoScale(_Texture2DAsset_142ed57588bb4411b5721e51d840a96c_Out_0_Texture2D).GetTransformedUV(IN.uv0.xy) );
float _SampleTexture2D_bf4de36812474d71befc37c267335de0_R_4_Float = _SampleTexture2D_bf4de36812474d71befc37c267335de0_RGBA_0_Vector4.r;
float _SampleTexture2D_bf4de36812474d71befc37c267335de0_G_5_Float = _SampleTexture2D_bf4de36812474d71befc37c267335de0_RGBA_0_Vector4.g;
float _SampleTexture2D_bf4de36812474d71befc37c267335de0_B_6_Float = _SampleTexture2D_bf4de36812474d71befc37c267335de0_RGBA_0_Vector4.b;
float _SampleTexture2D_bf4de36812474d71befc37c267335de0_A_7_Float = _SampleTexture2D_bf4de36812474d71befc37c267335de0_RGBA_0_Vector4.a;
float _Property_5abae425deae40e9a1d17f580edcb6e9_Out_0_Float = _Thickness;
float _TextureSize_aeb360e19fa241e9885e448ed47c7f26_Width_0_Float = UnityBuildTexture2DStructNoScale(_Texture2DAsset_142ed57588bb4411b5721e51d840a96c_Out_0_Texture2D).texelSize.z;
float _TextureSize_aeb360e19fa241e9885e448ed47c7f26_Height_2_Float = UnityBuildTexture2DStructNoScale(_Texture2DAsset_142ed57588bb4411b5721e51d840a96c_Out_0_Texture2D).texelSize.w;
float _TextureSize_aeb360e19fa241e9885e448ed47c7f26_TexelWidth_3_Float = UnityBuildTexture2DStructNoScale(_Texture2DAsset_142ed57588bb4411b5721e51d840a96c_Out_0_Texture2D).texelSize.x;
float _TextureSize_aeb360e19fa241e9885e448ed47c7f26_TexelHeight_4_Float = UnityBuildTexture2DStructNoScale(_Texture2DAsset_142ed57588bb4411b5721e51d840a96c_Out_0_Texture2D).texelSize.y;
float2 _Vector2_3619b913caad41e9b0e2215448ac4bd0_Out_0_Vector2 = float2(_TextureSize_aeb360e19fa241e9885e448ed47c7f26_TexelWidth_3_Float, _TextureSize_aeb360e19fa241e9885e448ed47c7f26_TexelHeight_4_Float);
Bindings_IntercardinalOutline_c32de0ee88394f1449f11c8588dae173_float _IntercardinalOutline_c2bf7ef5a7a841a1a013df375e6efa75;
_IntercardinalOutline_c2bf7ef5a7a841a1a013df375e6efa75.uv0 = IN.uv0;
float _IntercardinalOutline_c2bf7ef5a7a841a1a013df375e6efa75_OutAlpha_2_Float;
SG_IntercardinalOutline_c32de0ee88394f1449f11c8588dae173_float(UnityBuildTexture2DStructNoScale(_Texture2DAsset_142ed57588bb4411b5721e51d840a96c_Out_0_Texture2D), float(0), _Property_5abae425deae40e9a1d17f580edcb6e9_Out_0_Float, _Vector2_3619b913caad41e9b0e2215448ac4bd0_Out_0_Vector2, _IntercardinalOutline_c2bf7ef5a7a841a1a013df375e6efa75, _IntercardinalOutline_c2bf7ef5a7a841a1a013df375e6efa75_OutAlpha_2_Float);
float _Maximum_c3b1f3ee2ae7430c9ca6bb3fb47b45d7_Out_2_Float;
Unity_Maximum_float(_SampleTexture2D_bf4de36812474d71befc37c267335de0_A_7_Float, _IntercardinalOutline_c2bf7ef5a7a841a1a013df375e6efa75_OutAlpha_2_Float, _Maximum_c3b1f3ee2ae7430c9ca6bb3fb47b45d7_Out_2_Float);
surface.Alpha = _Maximum_c3b1f3ee2ae7430c9ca6bb3fb47b45d7_Out_2_Float;
return surface;
}

// --------------------------------------------------
// Build Graph Inputs
#ifdef HAVE_VFX_MODIFICATION
#define VFX_SRP_ATTRIBUTES Attributes
#define VFX_SRP_VARYINGS Varyings
#define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
#endif
VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
{
    VertexDescriptionInputs output;
    ZERO_INITIALIZE(VertexDescriptionInputs, output);

    output.ObjectSpaceNormal =                          input.normalOS;
    output.ObjectSpaceTangent =                         input.tangentOS.xyz;
    output.ObjectSpacePosition =                        input.positionOS;
#if UNITY_ANY_INSTANCING_ENABLED
#else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
#endif

    return output;
}
SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
{
    SurfaceDescriptionInputs output;
    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

#ifdef HAVE_VFX_MODIFICATION
#if VFX_USE_GRAPH_VALUES
    uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
    /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
#endif
    /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

#endif

    






    #if UNITY_UV_STARTS_AT_TOP
    #else
    #endif


    output.uv0 = input.texCoord0;
#if UNITY_ANY_INSTANCING_ENABLED
#else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
#endif
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
#else
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
#endif
#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

        return output;
}

// --------------------------------------------------
// Main

#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"

// --------------------------------------------------
// Visual Effect Vertex Invocations
#ifdef HAVE_VFX_MODIFICATION
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
#endif

ENDHLSL
}
Pass
{
    Name "Sprite Unlit"
    Tags
    {
        "LightMode" = "UniversalForward"
    }

// Render State
Cull Off
Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
ZTest LEqual
ZWrite Off

// Debug
// <None>

// --------------------------------------------------
// Pass

HLSLPROGRAM

// Pragmas
#pragma target 2.0
#pragma exclude_renderers d3d11_9x
#pragma multi_compile_instancing
#pragma vertex vert
#pragma fragment frag

// Keywords
#pragma multi_compile_fragment _ DEBUG_DISPLAY
#pragma multi_compile_vertex _ SKINNED_SPRITE
// GraphKeywords: <None>

// Defines

#define ATTRIBUTES_NEED_NORMAL
#define ATTRIBUTES_NEED_TANGENT
#define ATTRIBUTES_NEED_TEXCOORD0
#define ATTRIBUTES_NEED_COLOR
#define FEATURES_GRAPH_VERTEX_NORMAL_OUTPUT
#define FEATURES_GRAPH_VERTEX_TANGENT_OUTPUT
#define VARYINGS_NEED_POSITION_WS
#define VARYINGS_NEED_TEXCOORD0
#define VARYINGS_NEED_COLOR
#define FEATURES_GRAPH_VERTEX
/* WARNING: $splice Could not find named fragment 'PassInstancing' */
#define SHADERPASS SHADERPASS_SPRITEFORWARD


// custom interpolator pre-include
/* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

// Includes
#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Fog.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Core2D.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

// --------------------------------------------------
// Structs and Packing

// custom interpolators pre packing
/* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

struct Attributes
{
 float3 positionOS : POSITION;
 float3 normalOS : NORMAL;
 float4 tangentOS : TANGENT;
 float4 uv0 : TEXCOORD0;
 float4 color : COLOR;
#if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
 uint instanceID : INSTANCEID_SEMANTIC;
#endif
};
struct Varyings
{
 float4 positionCS : SV_POSITION;
 float3 positionWS;
 float4 texCoord0;
 float4 color;
#if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
 uint instanceID : CUSTOM_INSTANCE_ID;
#endif
#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
 uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
#endif
#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
 uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
#endif
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
 FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
#endif
};
struct SurfaceDescriptionInputs
{
 float4 uv0;
 float3 TimeParameters;
};
struct VertexDescriptionInputs
{
 float3 ObjectSpaceNormal;
 float3 ObjectSpaceTangent;
 float3 ObjectSpacePosition;
};
struct PackedVaryings
{
 float4 positionCS : SV_POSITION;
 float4 texCoord0 : INTERP0;
 float4 color : INTERP1;
 float3 positionWS : INTERP2;
#if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
 uint instanceID : CUSTOM_INSTANCE_ID;
#endif
#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
 uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
#endif
#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
 uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
#endif
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
 FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
#endif
};

PackedVaryings PackVaryings (Varyings input)
{
PackedVaryings output;
ZERO_INITIALIZE(PackedVaryings, output);
output.positionCS = input.positionCS;
output.texCoord0.xyzw = input.texCoord0;
output.color.xyzw = input.color;
output.positionWS.xyz = input.positionWS;
#if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
output.instanceID = input.instanceID;
#endif
#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
#endif
#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
#endif
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
output.cullFace = input.cullFace;
#endif
return output;
}

Varyings UnpackVaryings (PackedVaryings input)
{
Varyings output;
output.positionCS = input.positionCS;
output.texCoord0 = input.texCoord0.xyzw;
output.color = input.color.xyzw;
output.positionWS = input.positionWS.xyz;
#if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
output.instanceID = input.instanceID;
#endif
#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
#endif
#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
#endif
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
output.cullFace = input.cullFace;
#endif
return output;
}


// --------------------------------------------------
// Graph

// Graph Properties
CBUFFER_START(UnityPerMaterial)
float4 _Texture2DAsset_142ed57588bb4411b5721e51d840a96c_Out_0_Texture2D_TexelSize;
float _Thickness;
float4 _OutlineColor;
float _RainbowOutline;
float _RainbowSpeed;
float4 _FaceColor;
UNITY_TEXTURE_STREAMING_DEBUG_VARS;
CBUFFER_END

#if defined(DOTS_INSTANCING_ON)
// DOTS instancing definitions
UNITY_DOTS_INSTANCING_START(MaterialPropertyMetadata)
    UNITY_DOTS_INSTANCED_PROP_OVERRIDE_SUPPORTED(float4, _OutlineColor)
    UNITY_DOTS_INSTANCED_PROP_OVERRIDE_SUPPORTED(float, _RainbowOutline)
UNITY_DOTS_INSTANCING_END(MaterialPropertyMetadata)
// DOTS instancing usage macros
#define UNITY_ACCESS_HYBRID_INSTANCED_PROP(var, type) UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(type, var)
#elif defined(UNITY_INSTANCING_ENABLED)
// Unity instancing definitions
UNITY_INSTANCING_BUFFER_START(SGPerInstanceData)
    UNITY_DEFINE_INSTANCED_PROP(float4, _OutlineColor)
    UNITY_DEFINE_INSTANCED_PROP(float, _RainbowOutline)
UNITY_INSTANCING_BUFFER_END(SGPerInstanceData)
// Unity instancing usage macros
#define UNITY_ACCESS_HYBRID_INSTANCED_PROP(var, type) UNITY_ACCESS_INSTANCED_PROP(SGPerInstanceData, var)
#else
#define UNITY_ACCESS_HYBRID_INSTANCED_PROP(var, type) var
#endif

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
TEXTURE2D(_Texture2DAsset_142ed57588bb4411b5721e51d840a96c_Out_0_Texture2D);
SAMPLER(sampler_Texture2DAsset_142ed57588bb4411b5721e51d840a96c_Out_0_Texture2D);

// Graph Includes
// GraphIncludes: <None>

// -- Property used by ScenePickingPass
#ifdef SCENEPICKINGPASS
float4 _SelectionID;
#endif

// -- Properties used by SceneSelectionPass
#ifdef SCENESELECTIONPASS
int _ObjectId;
int _PassValue;
#endif

// Graph Functions

void Unity_Comparison_Greater_float(float A, float B, out float Out)
{
    Out = A > B ? 1 : 0;
}

void Unity_Multiply_float_float(float A, float B, out float Out)
{
Out = A * B;
}

void Unity_Fraction_float(float In, out float Out)
{
    Out = frac(In);
}

void Unity_SampleGradientV1_float(Gradient Gradient, float Time, out float4 Out)
{
    // convert to OkLab if we need perceptual color space.
    float3 color = lerp(Gradient.colors[0].rgb, LinearToOklab(Gradient.colors[0].rgb), Gradient.type == 2);

    [unroll]
    for (int c = 1; c < Gradient.colorsLength; c++)
    {
        float colorPos = saturate((Time - Gradient.colors[c - 1].w) / (Gradient.colors[c].w - Gradient.colors[c - 1].w)) * step(c, Gradient.colorsLength - 1);
        float3 color2 = lerp(Gradient.colors[c].rgb, LinearToOklab(Gradient.colors[c].rgb), Gradient.type == 2);
        color = lerp(color, color2, lerp(colorPos, step(0.01, colorPos), Gradient.type % 2)); // grad.type == 1 is fixed, 0 and 2 are blends.
    }
    color = lerp(color, OklabToLinear(color), Gradient.type == 2);

#ifdef UNITY_COLORSPACE_GAMMA
    color = LinearToSRGB(color);
#endif

    float alpha = Gradient.alphas[0].x;
    [unroll]
    for (int a = 1; a < Gradient.alphasLength; a++)
    {
        float alphaPos = saturate((Time - Gradient.alphas[a - 1].y) / (Gradient.alphas[a].y - Gradient.alphas[a - 1].y)) * step(a, Gradient.alphasLength - 1);
        alpha = lerp(alpha, Gradient.alphas[a].x, lerp(alphaPos, step(0.01, alphaPos), Gradient.type % 2));
    }

    Out = float4(color, alpha);
}

void Unity_Branch_float4(float Predicate, float4 True, float4 False, out float4 Out)
{
    Out = Predicate ? True : False;
}

void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
{
Out = A * B;
}

void Unity_Add_float2(float2 A, float2 B, out float2 Out)
{
    Out = A + B;
}

struct Bindings_OneDirectionOutline_98b26c7076faacf4995ee8dca0ee037e_float
{
half4 uv0;
};

void SG_OneDirectionOutline_98b26c7076faacf4995ee8dca0ee037e_float(UnityTexture2D _MainTex, float2 _OutlineDirection, float _OutlineThickness, float2 _TexelSize, Bindings_OneDirectionOutline_98b26c7076faacf4995ee8dca0ee037e_float IN, out float OutAlpha_2)
{
UnityTexture2D _Property_1306d87535f5441fad3e3846f3d376a0_Out_0_Texture2D = _MainTex;
float4 _UV_8174ac5fed2a41779b432a46cf1666eb_Out_0_Vector4 = IN.uv0;
float2 _Property_fcb39e0ed69643318c35efd34122563c_Out_0_Vector2 = _TexelSize;
float2 _Property_a7a7ece6e5e54d48975408b7da9c316f_Out_0_Vector2 = _OutlineDirection;
float2 _Multiply_722a058362cc496d941a3976df7722dc_Out_2_Vector2;
Unity_Multiply_float2_float2(_Property_fcb39e0ed69643318c35efd34122563c_Out_0_Vector2, _Property_a7a7ece6e5e54d48975408b7da9c316f_Out_0_Vector2, _Multiply_722a058362cc496d941a3976df7722dc_Out_2_Vector2);
float _Property_b31903c0d6de4426a7c2fa26ae94835f_Out_0_Float = _OutlineThickness;
float2 _Multiply_05a80bcacb77489d9365a13428ff6336_Out_2_Vector2;
Unity_Multiply_float2_float2(_Multiply_722a058362cc496d941a3976df7722dc_Out_2_Vector2, (_Property_b31903c0d6de4426a7c2fa26ae94835f_Out_0_Float.xx), _Multiply_05a80bcacb77489d9365a13428ff6336_Out_2_Vector2);
float2 _Add_1956f2a04d2c4bcdb557dbf121377204_Out_2_Vector2;
Unity_Add_float2((_UV_8174ac5fed2a41779b432a46cf1666eb_Out_0_Vector4.xy), _Multiply_05a80bcacb77489d9365a13428ff6336_Out_2_Vector2, _Add_1956f2a04d2c4bcdb557dbf121377204_Out_2_Vector2);
float4 _SampleTexture2D_6a3c479c42814ec8bfd30e4d312c27b1_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_1306d87535f5441fad3e3846f3d376a0_Out_0_Texture2D.tex, _Property_1306d87535f5441fad3e3846f3d376a0_Out_0_Texture2D.samplerstate, _Property_1306d87535f5441fad3e3846f3d376a0_Out_0_Texture2D.GetTransformedUV(_Add_1956f2a04d2c4bcdb557dbf121377204_Out_2_Vector2) );
float _SampleTexture2D_6a3c479c42814ec8bfd30e4d312c27b1_R_4_Float = _SampleTexture2D_6a3c479c42814ec8bfd30e4d312c27b1_RGBA_0_Vector4.r;
float _SampleTexture2D_6a3c479c42814ec8bfd30e4d312c27b1_G_5_Float = _SampleTexture2D_6a3c479c42814ec8bfd30e4d312c27b1_RGBA_0_Vector4.g;
float _SampleTexture2D_6a3c479c42814ec8bfd30e4d312c27b1_B_6_Float = _SampleTexture2D_6a3c479c42814ec8bfd30e4d312c27b1_RGBA_0_Vector4.b;
float _SampleTexture2D_6a3c479c42814ec8bfd30e4d312c27b1_A_7_Float = _SampleTexture2D_6a3c479c42814ec8bfd30e4d312c27b1_RGBA_0_Vector4.a;
OutAlpha_2 = _SampleTexture2D_6a3c479c42814ec8bfd30e4d312c27b1_A_7_Float;
}

void Unity_Maximum_float(float A, float B, out float Out)
{
    Out = max(A, B);
}

void Unity_Clamp_float(float In, float Min, float Max, out float Out)
{
    Out = clamp(In, Min, Max);
}

struct Bindings_IntercardinalOutline_c32de0ee88394f1449f11c8588dae173_float
{
half4 uv0;
};

void SG_IntercardinalOutline_c32de0ee88394f1449f11c8588dae173_float(UnityTexture2D _MainTex, float _Alpha, float _OutlineThickness, float2 _TexelSize, Bindings_IntercardinalOutline_c32de0ee88394f1449f11c8588dae173_float IN, out float OutAlpha_2)
{
UnityTexture2D _Property_f2d2f0c6a91146f987bff81c4df4b8d0_Out_0_Texture2D = _MainTex;
float2 _Vector2_0ee9ea2042ff4234aae63d8be2c913a1_Out_0_Vector2 = float2(float(1), float(-1));
float _Property_3ea8e41f4cb14cb08c5a6300bd7a891d_Out_0_Float = _OutlineThickness;
float2 _Property_4cd485a1743443e5a3a83c81658381ce_Out_0_Vector2 = _TexelSize;
Bindings_OneDirectionOutline_98b26c7076faacf4995ee8dca0ee037e_float _OneDirectionOutline_24c147574fef4ae5b3b283568dfcacd8;
_OneDirectionOutline_24c147574fef4ae5b3b283568dfcacd8.uv0 = IN.uv0;
half _OneDirectionOutline_24c147574fef4ae5b3b283568dfcacd8_OutAlpha_2_Float;
SG_OneDirectionOutline_98b26c7076faacf4995ee8dca0ee037e_float(_Property_f2d2f0c6a91146f987bff81c4df4b8d0_Out_0_Texture2D, _Vector2_0ee9ea2042ff4234aae63d8be2c913a1_Out_0_Vector2, _Property_3ea8e41f4cb14cb08c5a6300bd7a891d_Out_0_Float, _Property_4cd485a1743443e5a3a83c81658381ce_Out_0_Vector2, _OneDirectionOutline_24c147574fef4ae5b3b283568dfcacd8, _OneDirectionOutline_24c147574fef4ae5b3b283568dfcacd8_OutAlpha_2_Float);
float2 _Vector2_74d7b11c5f4d453499442e0a15881810_Out_0_Vector2 = float2(float(-1), float(-1));
Bindings_OneDirectionOutline_98b26c7076faacf4995ee8dca0ee037e_float _OneDirectionOutline_b682e7916a9d40a8bed2268ea41d86f5;
_OneDirectionOutline_b682e7916a9d40a8bed2268ea41d86f5.uv0 = IN.uv0;
half _OneDirectionOutline_b682e7916a9d40a8bed2268ea41d86f5_OutAlpha_2_Float;
SG_OneDirectionOutline_98b26c7076faacf4995ee8dca0ee037e_float(_Property_f2d2f0c6a91146f987bff81c4df4b8d0_Out_0_Texture2D, _Vector2_74d7b11c5f4d453499442e0a15881810_Out_0_Vector2, _Property_3ea8e41f4cb14cb08c5a6300bd7a891d_Out_0_Float, _Property_4cd485a1743443e5a3a83c81658381ce_Out_0_Vector2, _OneDirectionOutline_b682e7916a9d40a8bed2268ea41d86f5, _OneDirectionOutline_b682e7916a9d40a8bed2268ea41d86f5_OutAlpha_2_Float);
float _Maximum_234adf467c3c470ebbe8cdb01dd41866_Out_2_Float;
Unity_Maximum_float(_OneDirectionOutline_24c147574fef4ae5b3b283568dfcacd8_OutAlpha_2_Float, _OneDirectionOutline_b682e7916a9d40a8bed2268ea41d86f5_OutAlpha_2_Float, _Maximum_234adf467c3c470ebbe8cdb01dd41866_Out_2_Float);
float2 _Vector2_5b9d8c2ec98b4094b25e2b9e29c64d83_Out_0_Vector2 = float2(float(1), float(1));
Bindings_OneDirectionOutline_98b26c7076faacf4995ee8dca0ee037e_float _OneDirectionOutline_75748be69bf84cbdb7777f268a50c28d;
_OneDirectionOutline_75748be69bf84cbdb7777f268a50c28d.uv0 = IN.uv0;
half _OneDirectionOutline_75748be69bf84cbdb7777f268a50c28d_OutAlpha_2_Float;
SG_OneDirectionOutline_98b26c7076faacf4995ee8dca0ee037e_float(_Property_f2d2f0c6a91146f987bff81c4df4b8d0_Out_0_Texture2D, _Vector2_5b9d8c2ec98b4094b25e2b9e29c64d83_Out_0_Vector2, _Property_3ea8e41f4cb14cb08c5a6300bd7a891d_Out_0_Float, _Property_4cd485a1743443e5a3a83c81658381ce_Out_0_Vector2, _OneDirectionOutline_75748be69bf84cbdb7777f268a50c28d, _OneDirectionOutline_75748be69bf84cbdb7777f268a50c28d_OutAlpha_2_Float);
float2 _Vector2_6740ec37432f4574b09041bad486c76b_Out_0_Vector2 = float2(float(-1), float(1));
Bindings_OneDirectionOutline_98b26c7076faacf4995ee8dca0ee037e_float _OneDirectionOutline_42928c1253cb4c0b9effaa7902148870;
_OneDirectionOutline_42928c1253cb4c0b9effaa7902148870.uv0 = IN.uv0;
half _OneDirectionOutline_42928c1253cb4c0b9effaa7902148870_OutAlpha_2_Float;
SG_OneDirectionOutline_98b26c7076faacf4995ee8dca0ee037e_float(_Property_f2d2f0c6a91146f987bff81c4df4b8d0_Out_0_Texture2D, _Vector2_6740ec37432f4574b09041bad486c76b_Out_0_Vector2, _Property_3ea8e41f4cb14cb08c5a6300bd7a891d_Out_0_Float, _Property_4cd485a1743443e5a3a83c81658381ce_Out_0_Vector2, _OneDirectionOutline_42928c1253cb4c0b9effaa7902148870, _OneDirectionOutline_42928c1253cb4c0b9effaa7902148870_OutAlpha_2_Float);
float _Maximum_4eaa9bb17e104daa8f6b620650d41210_Out_2_Float;
Unity_Maximum_float(_OneDirectionOutline_75748be69bf84cbdb7777f268a50c28d_OutAlpha_2_Float, _OneDirectionOutline_42928c1253cb4c0b9effaa7902148870_OutAlpha_2_Float, _Maximum_4eaa9bb17e104daa8f6b620650d41210_Out_2_Float);
float _Maximum_e683a7fcd27d46d3b64529c5801ee036_Out_2_Float;
Unity_Maximum_float(_Maximum_234adf467c3c470ebbe8cdb01dd41866_Out_2_Float, _Maximum_4eaa9bb17e104daa8f6b620650d41210_Out_2_Float, _Maximum_e683a7fcd27d46d3b64529c5801ee036_Out_2_Float);
float _Clamp_614a1da6ce0e467b84f0eb0f381aa0ec_Out_3_Float;
Unity_Clamp_float(_Maximum_e683a7fcd27d46d3b64529c5801ee036_Out_2_Float, float(0), float(1), _Clamp_614a1da6ce0e467b84f0eb0f381aa0ec_Out_3_Float);
OutAlpha_2 = _Clamp_614a1da6ce0e467b84f0eb0f381aa0ec_Out_3_Float;
}

// Custom interpolators pre vertex
/* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

// Graph Vertex
struct VertexDescription
{
float3 Position;
float3 Normal;
float3 Tangent;
};

VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
{
VertexDescription description = (VertexDescription)0;
description.Position = IN.ObjectSpacePosition;
description.Normal = IN.ObjectSpaceNormal;
description.Tangent = IN.ObjectSpaceTangent;
return description;
}

// Custom interpolators, pre surface
#ifdef FEATURES_GRAPH_VERTEX
Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
{
return output;
}
#define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
#endif

// Graph Pixel
struct SurfaceDescription
{
float3 BaseColor;
float Alpha;
};

SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
{
SurfaceDescription surface = (SurfaceDescription)0;
float4 _SampleTexture2D_bf4de36812474d71befc37c267335de0_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(UnityBuildTexture2DStructNoScale(_Texture2DAsset_142ed57588bb4411b5721e51d840a96c_Out_0_Texture2D).tex, UnityBuildTexture2DStructNoScale(_Texture2DAsset_142ed57588bb4411b5721e51d840a96c_Out_0_Texture2D).samplerstate, UnityBuildTexture2DStructNoScale(_Texture2DAsset_142ed57588bb4411b5721e51d840a96c_Out_0_Texture2D).GetTransformedUV(IN.uv0.xy) );
float _SampleTexture2D_bf4de36812474d71befc37c267335de0_R_4_Float = _SampleTexture2D_bf4de36812474d71befc37c267335de0_RGBA_0_Vector4.r;
float _SampleTexture2D_bf4de36812474d71befc37c267335de0_G_5_Float = _SampleTexture2D_bf4de36812474d71befc37c267335de0_RGBA_0_Vector4.g;
float _SampleTexture2D_bf4de36812474d71befc37c267335de0_B_6_Float = _SampleTexture2D_bf4de36812474d71befc37c267335de0_RGBA_0_Vector4.b;
float _SampleTexture2D_bf4de36812474d71befc37c267335de0_A_7_Float = _SampleTexture2D_bf4de36812474d71befc37c267335de0_RGBA_0_Vector4.a;
float _Comparison_31199d7bae99412594c107a4945966b8_Out_2_Boolean;
Unity_Comparison_Greater_float(_SampleTexture2D_bf4de36812474d71befc37c267335de0_A_7_Float, float(0), _Comparison_31199d7bae99412594c107a4945966b8_Out_2_Boolean);
float4 _Property_72496aa6468947d3b81342440c0b70a1_Out_0_Vector4 = _FaceColor;
float _Property_1f9c3c33d2434ea4967c84f0d79beb20_Out_0_Boolean = _RainbowOutline;
float _Property_076f2dccfc3c4b4a850c7922fc03e893_Out_0_Float = _RainbowSpeed;
float _Multiply_8480769ee1a34167a5460550fea112a2_Out_2_Float;
Unity_Multiply_float_float(_Property_076f2dccfc3c4b4a850c7922fc03e893_Out_0_Float, IN.TimeParameters.x, _Multiply_8480769ee1a34167a5460550fea112a2_Out_2_Float);
float _Fraction_89c08226a54b405793b6af61ddbdbff1_Out_1_Float;
Unity_Fraction_float(_Multiply_8480769ee1a34167a5460550fea112a2_Out_2_Float, _Fraction_89c08226a54b405793b6af61ddbdbff1_Out_1_Float);
float4 _SampleGradient_4d4fbe219f544ee59043a9c2920b4c8f_Out_2_Vector4;
Unity_SampleGradientV1_float(NewGradient(0, 4, 2, float4(1, 0, 0, 0),float4(0, 1, 0, 0.3329976),float4(0, 0, 1, 0.6659952),float4(1, 0, 0, 1),float4(0, 0, 0, 0),float4(0, 0, 0, 0),float4(0, 0, 0, 0),float4(0, 0, 0, 0), float2(1, 0),float2(1, 1),float2(0, 0),float2(0, 0),float2(0, 0),float2(0, 0),float2(0, 0),float2(0, 0)), _Fraction_89c08226a54b405793b6af61ddbdbff1_Out_1_Float, _SampleGradient_4d4fbe219f544ee59043a9c2920b4c8f_Out_2_Vector4);
float4 _Property_e594d7c2e6694502accc6e1f49aebae1_Out_0_Vector4 = UNITY_ACCESS_HYBRID_INSTANCED_PROP(_OutlineColor, float4);
float4 _Branch_896999a999054319b363139c48b649ff_Out_3_Vector4;
Unity_Branch_float4(_Property_1f9c3c33d2434ea4967c84f0d79beb20_Out_0_Boolean, _SampleGradient_4d4fbe219f544ee59043a9c2920b4c8f_Out_2_Vector4, _Property_e594d7c2e6694502accc6e1f49aebae1_Out_0_Vector4, _Branch_896999a999054319b363139c48b649ff_Out_3_Vector4);
float _Split_bc9b5447aba2448ebb2992adce02e5e6_R_1_Float = _Branch_896999a999054319b363139c48b649ff_Out_3_Vector4[0];
float _Split_bc9b5447aba2448ebb2992adce02e5e6_G_2_Float = _Branch_896999a999054319b363139c48b649ff_Out_3_Vector4[1];
float _Split_bc9b5447aba2448ebb2992adce02e5e6_B_3_Float = _Branch_896999a999054319b363139c48b649ff_Out_3_Vector4[2];
float _Split_bc9b5447aba2448ebb2992adce02e5e6_A_4_Float = _Branch_896999a999054319b363139c48b649ff_Out_3_Vector4[3];
float _Property_5abae425deae40e9a1d17f580edcb6e9_Out_0_Float = _Thickness;
float _TextureSize_aeb360e19fa241e9885e448ed47c7f26_Width_0_Float = UnityBuildTexture2DStructNoScale(_Texture2DAsset_142ed57588bb4411b5721e51d840a96c_Out_0_Texture2D).texelSize.z;
float _TextureSize_aeb360e19fa241e9885e448ed47c7f26_Height_2_Float = UnityBuildTexture2DStructNoScale(_Texture2DAsset_142ed57588bb4411b5721e51d840a96c_Out_0_Texture2D).texelSize.w;
float _TextureSize_aeb360e19fa241e9885e448ed47c7f26_TexelWidth_3_Float = UnityBuildTexture2DStructNoScale(_Texture2DAsset_142ed57588bb4411b5721e51d840a96c_Out_0_Texture2D).texelSize.x;
float _TextureSize_aeb360e19fa241e9885e448ed47c7f26_TexelHeight_4_Float = UnityBuildTexture2DStructNoScale(_Texture2DAsset_142ed57588bb4411b5721e51d840a96c_Out_0_Texture2D).texelSize.y;
float2 _Vector2_3619b913caad41e9b0e2215448ac4bd0_Out_0_Vector2 = float2(_TextureSize_aeb360e19fa241e9885e448ed47c7f26_TexelWidth_3_Float, _TextureSize_aeb360e19fa241e9885e448ed47c7f26_TexelHeight_4_Float);
Bindings_IntercardinalOutline_c32de0ee88394f1449f11c8588dae173_float _IntercardinalOutline_c2bf7ef5a7a841a1a013df375e6efa75;
_IntercardinalOutline_c2bf7ef5a7a841a1a013df375e6efa75.uv0 = IN.uv0;
float _IntercardinalOutline_c2bf7ef5a7a841a1a013df375e6efa75_OutAlpha_2_Float;
SG_IntercardinalOutline_c32de0ee88394f1449f11c8588dae173_float(UnityBuildTexture2DStructNoScale(_Texture2DAsset_142ed57588bb4411b5721e51d840a96c_Out_0_Texture2D), float(0), _Property_5abae425deae40e9a1d17f580edcb6e9_Out_0_Float, _Vector2_3619b913caad41e9b0e2215448ac4bd0_Out_0_Vector2, _IntercardinalOutline_c2bf7ef5a7a841a1a013df375e6efa75, _IntercardinalOutline_c2bf7ef5a7a841a1a013df375e6efa75_OutAlpha_2_Float);
float4 _Vector4_c084949a85b140d1ae367e4d5fb4692d_Out_0_Vector4 = float4(_Split_bc9b5447aba2448ebb2992adce02e5e6_R_1_Float, _Split_bc9b5447aba2448ebb2992adce02e5e6_G_2_Float, _Split_bc9b5447aba2448ebb2992adce02e5e6_B_3_Float, _IntercardinalOutline_c2bf7ef5a7a841a1a013df375e6efa75_OutAlpha_2_Float);
float4 _Branch_26b8e9dd9a3147c5bacdda6ffffb067b_Out_3_Vector4;
Unity_Branch_float4(_Comparison_31199d7bae99412594c107a4945966b8_Out_2_Boolean, _Property_72496aa6468947d3b81342440c0b70a1_Out_0_Vector4, _Vector4_c084949a85b140d1ae367e4d5fb4692d_Out_0_Vector4, _Branch_26b8e9dd9a3147c5bacdda6ffffb067b_Out_3_Vector4);
float _Maximum_c3b1f3ee2ae7430c9ca6bb3fb47b45d7_Out_2_Float;
Unity_Maximum_float(_SampleTexture2D_bf4de36812474d71befc37c267335de0_A_7_Float, _IntercardinalOutline_c2bf7ef5a7a841a1a013df375e6efa75_OutAlpha_2_Float, _Maximum_c3b1f3ee2ae7430c9ca6bb3fb47b45d7_Out_2_Float);
surface.BaseColor = (_Branch_26b8e9dd9a3147c5bacdda6ffffb067b_Out_3_Vector4.xyz);
surface.Alpha = _Maximum_c3b1f3ee2ae7430c9ca6bb3fb47b45d7_Out_2_Float;
return surface;
}

// --------------------------------------------------
// Build Graph Inputs
#ifdef HAVE_VFX_MODIFICATION
#define VFX_SRP_ATTRIBUTES Attributes
#define VFX_SRP_VARYINGS Varyings
#define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
#endif
VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
{
    VertexDescriptionInputs output;
    ZERO_INITIALIZE(VertexDescriptionInputs, output);

    output.ObjectSpaceNormal =                          input.normalOS;
    output.ObjectSpaceTangent =                         input.tangentOS.xyz;
    output.ObjectSpacePosition =                        input.positionOS;
#if UNITY_ANY_INSTANCING_ENABLED
#else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
#endif

    return output;
}
SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
{
    SurfaceDescriptionInputs output;
    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

#ifdef HAVE_VFX_MODIFICATION
#if VFX_USE_GRAPH_VALUES
    uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
    /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
#endif
    /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

#endif

    






    #if UNITY_UV_STARTS_AT_TOP
    #else
    #endif


    output.uv0 = input.texCoord0;
#if UNITY_ANY_INSTANCING_ENABLED
#else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
#endif
    output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
#else
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
#endif
#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

        return output;
}

// --------------------------------------------------
// Main

#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteUnlitPass.hlsl"

// --------------------------------------------------
// Visual Effect Vertex Invocations
#ifdef HAVE_VFX_MODIFICATION
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
#endif

ENDHLSL
}
}
CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
CustomEditorForRenderPipeline "UnityEditor.ShaderGraphSpriteGUI" "UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset"
FallBack "Hidden/Shader Graph/FallbackError"
}
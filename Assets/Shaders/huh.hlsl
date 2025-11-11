// Declare a texture + sampler
TEXTURE2D(_MyTex);
SAMPLER(sampler_MyTex);

struct Attributes {
    float4 positionOS : POSITION;
    float2 uv : TEXCOORD0;
};

struct Varyings {
    float4 positionCS : SV_POSITION;
    float2 uv : TEXCOORD0;
};

// Vertex shader
Varyings vert(Attributes IN)
{
    Varyings OUT;
    OUT.positionCS = UnityObjectToClipPos(IN.positionOS);
    OUT.uv = IN.uv;
    return OUT;
}

// Fragment shader
float4 frag(Varyings IN) : SV_Target
{
    // Sample the texture normally
    float4 col = SAMPLE_TEXTURE2D(_MyTex, sampler_MyTex, IN.uv);

    // Get texture size in pixels (int2), then convert to float2
    uint width, height;
    _MyTex.GetDimensions(width, height);

    float2 texSize = float2(width, height);
    float2 texelSize = 1.0 / texSize; // reciprocal, like Unity's _TexelSize

    // Example: offset UV by one texel to the right
    float2 offsetUV = IN.uv + float2(texelSize.x, 0.0);
    float4 colOffset = SAMPLE_TEXTURE2D(_MyTex, sampler_MyTex, offsetUV);

    return lerp(col, colOffset, 0.5); // blend original + offset
}

sampler2D AlbedoSampler : register(s0);

static const float3x3 SRGB_TO_AP1 = float3x3
(
    0.59719, 0.35458, 0.04823,
    0.07600, 0.90834, 0.01566,
    0.02840, 0.13383, 0.83777
);

static const float3x3 AP1_TO_SRGB = float3x3
(
    1.60475, -0.53108, -0.07367,
    -0.10208, 1.10813, -0.00605,
    -0.00327, -0.07276, 1.17603
);

float3 Ambient;
float Exposure;

Texture2D RadianceBuffer;

sampler RadianceSampler = sampler_state
{
    Texture = (RadianceBuffer);
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

float3 ToneMappingACES(float3 color);
float3 RRTAndODTFit(float3 aces);

float3 ToLinear(float3 sRGB);
float3 ToGamma(float3 linearRGB);

float4 PixelShaderFunction(float2 uv : TEXCOORD0) : COLOR0
{
    float4 albedoOriginal = tex2D(AlbedoSampler, uv);
    if (albedoOriginal.a < 0.01)
    {
        discard;
    }
    
    float3 albedo = ToLinear(albedoOriginal.rgb);
    
    float3 ambient = Ambient * albedo;
    
    float4 direct = tex2D(RadianceSampler, uv);
    
    float3 color = direct.rgb + ambient;
    
    color *= Exposure;
    color = ToneMappingACES(color);
    
    color = ToGamma(color);
    
    return float4(color.r, color.g, color.b, albedoOriginal.a);
}

float3 ToneMappingACES(float3 color)
{
    return mul(AP1_TO_SRGB, RRTAndODTFit(mul(SRGB_TO_AP1, color)));
}

float3 RRTAndODTFit(float3 aces)
{
    float3 a = aces * (aces + 0.0245786) - 0.000090537;
    float3 b = aces * (0.983729 * aces + 0.4329510) + 0.238081;
    return a / b;
}

float3 ToLinear(float3 sRGB)
{
    return pow(sRGB, 2.2);
}

float3 ToGamma(float3 linearRGB)
{
    return pow(linearRGB, 1.0 / 2.2);
}

technique Output
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
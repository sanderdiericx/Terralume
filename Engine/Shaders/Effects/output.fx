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

float2 ScreenSize;

float AORadius;
float AOStrength;

float Exposure;
float Gamma;

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

Texture2D SDFBuffer;

sampler SDFSampler = sampler_state
{
    Texture = (SDFBuffer);
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

float SDFAO(float2 uv, float2 texelSize);

float3 ToneMappingACES(float3 color);
float3 RRTAndODTFit(float3 aces);

float3 ToLinear(float3 sRGB);
float3 ToGamma(float3 linearRGB);

float4 PixelShaderFunction(float2 uv : TEXCOORD0) : COLOR0
{
    float4 albedoOriginal = tex2D(AlbedoSampler, uv);    
    float3 albedo = ToLinear(albedoOriginal.rgb);
    
    float ao = SDFAO(uv, 1.0 / ScreenSize);
    float4 direct = tex2D(RadianceSampler, uv);
    
    float3 color = direct.rgb * ao;
    
    color *= Exposure;
    color = ToneMappingACES(color);
    
    color = ToGamma(color);
    
    return float4(color.r, color.g, color.b, albedoOriginal.a);
}

float SDFAO(float2 uv, float2 texelSize)
{
    float2 sdf = tex2Dlod(SDFSampler, float4(uv, 0, 0)).rg;
    
    if (sdf.r < 0.0)
    {
        return 1.0;
    }

    float ao = 0.0;
    int numDirections = 8;
    int numSteps = 4;
    float stepSize = AORadius / numSteps;

    for (int d = 0; d < numDirections; d++)
    {
        float angle = (d / (float) numDirections) * 6.28318;
        float2 dir = float2(cos(angle), sin(angle));

        float dirOcclusion = 0.0;
        float weight = 1.0;
        float totalWeight = 0.0;

        for (int s = 1; s <= numSteps; s++)
        {
            float2 sampleUV = uv + dir * texelSize * stepSize * s;
            float sampledDist = tex2Dlod(SDFSampler, float4(sampleUV, 0, 0)).r;

            float occ = 1.0 - saturate(sampledDist / (stepSize * s));
            dirOcclusion += occ * weight;
            totalWeight += weight;
            weight *= 0.6;
        }

        ao += dirOcclusion / totalWeight;
    }

    ao /= numDirections;
    return lerp(AOStrength, 1.0, 1.0 - ao);
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
    return pow(linearRGB, 1.0 / Gamma);
}

technique Output
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
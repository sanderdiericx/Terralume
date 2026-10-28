sampler2D InputSampler : register(s0);

float2 ScreenSize;
float2 Radius;
float Strength;
bool Horizontal;

Texture2D PlayerTarget;

sampler PlayerSampler = sampler_state
{
    Texture = (PlayerTarget);
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

float PlayerMask(float4 player)
{
    return player.a > 0.01 || dot(player.rgb, float3(1.0, 1.0, 1.0)) > 0.01 ? 1.0 : 0.0;
}

float4 PixelShaderFunction(float2 uv : TEXCOORD0) : COLOR0
{
    if (!Horizontal && PlayerMask(tex2D(PlayerSampler, uv)) > 0.0)
    {
        return float4(0.0, 0.0, 0.0, 1.0);
    }

    float2 direction = Horizontal ? float2(1.0, 0.0) : float2(0.0, 1.0);
    float result = 0.0;
    float totalWeight = 0.0;

    for (int i = -8; i <= 8; i++)
    {
        float t = i / 8.0;
        float weight = exp(-4.5 * t * t);
        float2 sampleUV = uv + direction * Radius * t / ScreenSize;

        if (all(sampleUV >= 0.0) && all(sampleUV <= 1.0))
        {
            float4 sample = tex2Dlod(InputSampler, float4(sampleUV, 0.0, 0.0));
            result += (Horizontal ? PlayerMask(sample) : sample.r) * weight;
        }

        totalWeight += weight;
    }

    result /= totalWeight;

    if (!Horizontal)
    {
        result = saturate(result * Strength);
    }

    return float4(result, 0.0, 0.0, 1.0);
}

technique PlayerAmbientOcclusion
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}

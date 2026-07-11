sampler2D InputSampler : register(s0);

float2 ScreenSize;

float3 KarisAverage(float3 c)
{
    float luma = dot(c, float3(0.2126, 0.7152, 0.0722));
    return c / (1.0 + luma); // weight down bright outliers so they don't alias
}

float4 PixelShaderFunction(float2 uv : TEXCOORD0) : COLOR0
{
    float2 texel = 1.0 / ScreenSize;

    float3 c0 = tex2D(InputSampler, uv + texel * float2(-0.5, -0.5)).rgb;
    float3 c1 = tex2D(InputSampler, uv + texel * float2(0.5, -0.5)).rgb;
    float3 c2 = tex2D(InputSampler, uv + texel * float2(-0.5, 0.5)).rgb;
    float3 c3 = tex2D(InputSampler, uv + texel * float2(0.5, 0.5)).rgb;

    float3 result = (KarisAverage(c0) + KarisAverage(c1) + KarisAverage(c2) + KarisAverage(c3)) * 0.25;
    return float4(result, 1.0);
}

technique Downsample
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
sampler2D InputSampler : register(s0);

float2 Direction;
float2 ScreenSize;
float U_Sigma;

float4 PixelShaderFunction(float2 uv : TEXCOORD0) : COLOR0
{
    float2 texelSize = float2(1.0, 1.0) / ScreenSize;

    float4 result = float4(0.0, 0.0, 0.0, 0.0);
    float totalWeight = 0.0;

    for (int i = -4; i <= 4; i++)
    {
        float2 offset = Direction * texelSize * float(i);
        float4 color = tex2Dlod(InputSampler, float4(uv.x + offset.x, uv.y + offset.y, 0.0, 0.0));

        float x = float(i);
        float weight = exp(-(x * x) / (2.0 * U_Sigma * U_Sigma));

        result += color * weight;
        totalWeight += weight;
    }

    return result / totalWeight;
}

technique Gaussian
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
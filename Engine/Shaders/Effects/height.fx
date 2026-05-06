sampler2D ScreenSampler : register(s0);

static const float3 LUMINANCE = float3(0.299, 0.587, 0.114);

float4 PixelShaderFunction(float2 uv : TEXCOORD0) : COLOR0
{    
    return float4(dot(tex2D(ScreenSampler, uv).rgb, LUMINANCE), 0.0, 0.0, 1.0);
}

technique Height
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
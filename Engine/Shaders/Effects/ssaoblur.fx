sampler2D DepthSampler : register(s0);

float2 ScreenSize;
float DepthFalloff;

Texture2D SSAOBuffer;

sampler SSAOSampler = sampler_state
{
    Texture = (SSAOBuffer);
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

float4 PixelShaderFunction(float2 uv : TEXCOORD0) : COLOR0
{
    float2 texelSize = 1.0 / (ScreenSize / 2.0);
    
    float centerDepth = tex2D(DepthSampler, uv).r;
    float result = 0.0;
    float totalWeight = 0.0;
    
    // 4x4 blur kernel
    for (int x = -2; x <= 2; x++)
    {
        for (int y = -2; y <= 2; y++)
        {
            float2 offset = float2(x, y) * texelSize;
            float2 sampleUV = uv + offset;
            
            float sampleDepth = tex2D(DepthSampler, sampleUV).r;
            float sampleAO = tex2D(SSAOSampler, sampleUV).r;
            
            float depthDiff = abs(centerDepth - sampleDepth);
            float weight = exp(-depthDiff * DepthFalloff);
            
            result += sampleAO * weight;
            totalWeight += weight;
        }
    }
    
    return float4(result / totalWeight, 0.0, 0.0, 1.0);
}

technique SSAOBlur
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
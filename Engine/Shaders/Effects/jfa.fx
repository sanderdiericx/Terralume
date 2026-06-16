sampler2D SDFSampler : register(s0);

float2 ScreenSize;
float2 JumpSizeUV;

float4 PixelShaderFunction(float2 uv : TEXCOORD0) : COLOR0
{
    float2 currentSeed = tex2Dlod(SDFSampler, float4(uv.x, uv.y, 0.0, 0.0));
    float currentDistance = length((uv * ScreenSize) - currentSeed);
    
    for (int i = -1; i <= 1; i++)
    {
        for (int j = -1; j <= 1; j++)
        {
            if (j == 0 && i == 0)
            {
                continue;
            }
            
            float2 sampleUV = float2(uv.x + JumpSizeUV.x * i, uv.y + JumpSizeUV.y * j);
            
            float2 sampledSeed = tex2Dlod(SDFSampler, float4(sampleUV.x, sampleUV.y, 0.0, 0.0));
            float sampledDistance = length((uv * ScreenSize) - sampledSeed);
            
            if (sampledSeed.x < 0.0)
            {
                continue;
            }
            
            if (sampledDistance < currentDistance)
            {
                currentSeed = sampledSeed;
                currentDistance = sampledDistance;
            }
        }
    }
    
    return float4(currentSeed.x, currentSeed.y, 0.0, 0.0);
}

technique JFA
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
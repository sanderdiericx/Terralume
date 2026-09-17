sampler2D JFASampler : register(s0);

float2 ScreenSize;
float2 JumpSizeUV;

float4 PixelShaderFunction(float2 uv : TEXCOORD0) : COLOR0
{    
    float2 currentSeed = tex2Dlod(JFASampler, float4(uv.x, uv.y, 0.0, 0.0));
    float currentDistance = (currentSeed.x >= 0.0) ? length((uv * ScreenSize) - currentSeed) : 1e9;
    
    float2 sampleUV = float2(uv.x, uv.y - JumpSizeUV.y);
   
    float2 sampledSeed = tex2Dlod(JFASampler, float4(sampleUV.x, sampleUV.y, 0.0, 0.0));
    
    float sampledDistance = (sampledSeed.x >= 0.0) ? length((uv * ScreenSize) - sampledSeed) : 1e9;
   
    if (sampledDistance < currentDistance)
    {
        currentSeed = sampledSeed;
    }
    
    return float4(currentSeed.x, currentSeed.y, 0.0, 0.0); 
}

technique JFAReflection
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
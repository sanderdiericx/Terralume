sampler2D ReflectiveMaterialSampler : register(s0);

float2 ScreenSize;

float4 PixelShaderFunction(float2 uv : TEXCOORD0) : COLOR0
{
    float4 reflectiveMaterial = tex2Dlod(ReflectiveMaterialSampler, float4(uv.x, uv.y, 0.0, 0.0));
    
    float4 seed = float4(-1.0, -1.0, 0.0, 0.0);
    
    if (reflectiveMaterial.r > 0.0)
    {
        float2 jumpSize = float2(1.0, 1.0) / ScreenSize;

        float2 sampleUV = uv + (jumpSize * float2(0.0, -1.0));
        float4 verticalMaterial = tex2Dlod(ReflectiveMaterialSampler, float4(sampleUV.x, sampleUV.y, 0.0, 0.0));
        
        if (verticalMaterial.r < 0.01)
        {
            float2 val = uv * ScreenSize;
            
            seed = float4(val.x, val.y, 0.0, 0.0);                      
        }
    }
    
    return seed;
}

technique JFAReflectionInit
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
sampler2D ReflectiveMaterialSampler : register(s0);

float2 ScreenSize;

Texture2D JFAReflectionBuffer;

sampler JFAReflectionSampler = sampler_state
{
    Texture = (JFAReflectionBuffer);
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

float4 PixelShaderFunction(float2 uv : TEXCOORD0) : COLOR0
{
    float4 output = float4(0.0, 0.0, 0.0, 0.0);
    
    float4 reflectiveMaterial = tex2Dlod(ReflectiveMaterialSampler, float4(uv.x, uv.y, 0.0, 0.0));
    
    if (reflectiveMaterial.r > 0.0)
    {
        float2 pixelPos = uv * ScreenSize;
    
        float2 reflectionSeed = tex2Dlod(JFAReflectionSampler, float4(uv.x, uv.y, 0.0, 0.0));
    
        float distToSurface = (reflectionSeed.x >= 0.0) ? length(pixelPos - reflectionSeed) : 1e9;
    
        output = float4(distToSurface / ScreenSize.y, 0.0, 0.0, 1.0);
    }
    
    return output;
}

technique SDFCombineReflection
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
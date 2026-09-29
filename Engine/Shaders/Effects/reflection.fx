sampler2D RadianceSampler : register(s0);

float2 ScreenSize;

Texture2D SDFReflectionBuffer;

sampler SDFReflectionSampler= sampler_state
{
    Texture = (SDFReflectionBuffer);
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

Texture2D ReflectiveMaterialBuffer;

sampler ReflectiveMaterialSampler = sampler_state
{
    Texture = (ReflectiveMaterialBuffer);
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

float4 PixelShaderFunction(float2 uv : TEXCOORD0) : COLOR0
{
    float4 reflectiveMaterial = tex2Dlod(ReflectiveMaterialSampler, float4(uv.x, uv.y, 0.0, 0.0));

    float reflectionStrength = reflectiveMaterial.r;
    
    float4 reflection = float4(0.0, 0.0, 0.0, 0.0);
    
    if (reflectionStrength > 0.01)
    {
        float distanceUV = tex2Dlod(SDFReflectionSampler, float4(uv.x, uv.y, 0.0, 0.0)) / ScreenSize.y;
        
        float4 albedo = tex2Dlod(RadianceSampler, float4(uv.x, uv.y - distanceUV * 2.0, 0.0, 0.0));
        
        float blendStrength = (1.0 - smoothstep(0.0, 0.1, distanceUV)) * reflectionStrength;
        
        reflection = albedo * blendStrength;
    }
    
    return reflection;
}

technique Reflection
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
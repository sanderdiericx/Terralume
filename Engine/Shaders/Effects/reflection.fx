sampler2D RadianceSampler : register(s0);

float2 ScreenSize;
float BackgroundScrollFactor;

Texture2D BackgroundBuffer;

sampler BackgroundSampler = sampler_state
{
    Texture = (BackgroundBuffer);
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

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
        
        float2 reflectionUV = float2(uv.x, uv.y - distanceUV * 2.0);
        float4 radiance = tex2Dlod(RadianceSampler, float4(reflectionUV, 0.0, 0.0));

        float surfaceY = uv.y - distanceUV;
        float backgroundCorrection = (1.0 - BackgroundScrollFactor) * (0.5 - surfaceY);
        float2 backgroundUV = float2(reflectionUV.x, reflectionUV.y + backgroundCorrection);
        backgroundUV = clamp(backgroundUV, 0.5 / ScreenSize, 1.0 - 0.5 / ScreenSize);
        float4 background = tex2Dlod(BackgroundSampler, float4(backgroundUV, 0.0, 0.0));
        
        float4 albedo = radiance + background * (1.0 - radiance.a);
        
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

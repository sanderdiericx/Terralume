sampler2D LayerSampler : register(s0);

int MaxStepCount;
float2 StepSize;

float2 SunDirection;

float3 SunColor;

float VolumetricIntensity;
float VolumetricAttenuationStrength;

float BackgroundLayer;

Texture2D SunOcclusionBuffer;

sampler SunOcclusionSampler = sampler_state
{
    Texture = (SunOcclusionBuffer);
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};


float4 PixelShaderFunction(float2 uv : TEXCOORD0) : COLOR0
{
    float3 volumetric = float3(0.0, 0.0, 0.0);
    
    if (tex2Dlod(SunOcclusionSampler, float4(uv.x, uv.y, 0.0, 0.0)).r > 0.01)
    {
        float illuminationDecay = 1.0;
        
        for (int i = 0; i < MaxStepCount; i++)
        {
            float2 sampleUV = uv + float2(SunDirection.x, -SunDirection.y) * StepSize * i;

            if (any(sampleUV > 1.0) || any(sampleUV < 0.0))
            {
                break;
            }

            float sampledLayer = tex2Dlod(LayerSampler, float4(sampleUV.x, sampleUV.y, 0.0, 0.0));

            if (abs(sampledLayer - BackgroundLayer) > 0.01)
            {
                break;   
            }
            
            float distance = length(sampleUV - uv);

            float attenuation = 1 / (1 + (VolumetricAttenuationStrength * distance) + (VolumetricAttenuationStrength * (distance * distance)));

            volumetric += SunColor * VolumetricIntensity * attenuation;
        }

        volumetric /= MaxStepCount;
    }

    return float4(volumetric.r, volumetric.g, volumetric.b, 1.0);
}

technique VolumetricLight
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
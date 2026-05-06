sampler2D DepthSampler : register(s0);

static const int SAMPLE_COUNT = 16;

float2 ScreenSize;
float2 HalfScreenSize;
float2 NoiseScale;

float Radius;
float Bias;
float DepthScale;

float BackgroundLayer;

float3 Samples[SAMPLE_COUNT];

Texture2D LayerBuffer;

sampler LayerSampler = sampler_state
{
    Texture = (LayerBuffer);
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

Texture2D NormalBuffer;

sampler NormalSampler = sampler_state
{
    Texture = (NormalBuffer);
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

Texture2D NoiseTexture;

sampler NoiseSampler = sampler_state
{
    Texture = (NoiseTexture);
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Wrap;
    AddressV = Wrap;
};

float4 PixelShaderFunction(float2 uv : TEXCOORD0) : COLOR0
{
    float layer = tex2D(LayerSampler, uv).r;
    
    if (abs(layer - BackgroundLayer) < 0.01)
    {
        return float4(1.0, 0.0, 0.0, 1.0);
    }
    
    float depth = tex2D(DepthSampler, uv).r;
    float pixelZ = depth * DepthScale;
    float3 pixelPos = float3(uv.x * ScreenSize.x, uv.y * ScreenSize.y, pixelZ);
    
    float3 normal = normalize(tex2D(NormalSampler, uv).rgb * 2.0 - 1.0);
    
    float2 noiseUV = uv * NoiseScale;
    float3 randomVec = normalize(tex2D(NoiseSampler, noiseUV).rgb * 2.0 - 1.0);
    
    // Build TBN matrix aligned to surface normal
    float3 tangent = normalize(randomVec - normal * dot(randomVec, normal));
    float3 bitangent = cross(normal, tangent);
    float3x3 TBN = float3x3(tangent, bitangent, normal);
    
    float occlusion = 0.0;
    
    [loop]
    for (int i = 0; i < SAMPLE_COUNT; i++)
    {
        float3 sampleDir = mul(Samples[i], TBN);
        float3 samplePos = pixelPos + sampleDir * Radius;
        
        float2 sampleUV = float2(samplePos.x / ScreenSize.x, samplePos.y / ScreenSize.y);
        sampleUV = clamp(sampleUV, 0.0, 1.0);

        float sampleDepth = tex2Dlod(DepthSampler, float4(sampleUV.x, sampleUV.y, 0.0, 0.0)).r * DepthScale;

        float rangeCheck = smoothstep(0.0, 1.0, Radius / abs(pixelZ - sampleDepth));

        occlusion += (sampleDepth >= samplePos.z + Bias) ? rangeCheck : 0.0;
    }
    
    occlusion = 1.0 - (occlusion / float(SAMPLE_COUNT));
    
    return float4(occlusion, 0.0, 0.0, 1.0);
}

technique SSAO
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
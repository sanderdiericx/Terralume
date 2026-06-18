sampler2D LayerSampler : register(s0);

float2 TileStride;
float2 MinTilePosition;

float2 ScreenPosition;
float2 ViewSize;

float2 DefaultNoise;
float2 BackgroundNoise;

float2 NoiseTextureSize;
float2 IDTextureSize;

float TileLayer;
float WallLayer;
float BackgroundLayer;

Texture2D TileIDTexture;

sampler TileIDSampler = sampler_state
{
    Texture = (TileIDTexture);
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

Texture2D WallIDTexture;

sampler WallIDSampler = sampler_state
{
    Texture = (WallIDTexture);
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

Texture2D TileNoiseTexture;

sampler TileNoiseSampler = sampler_state
{
    Texture = (TileNoiseTexture);
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};


Texture2D WallNoiseTexture;

sampler WallNoiseSampler = sampler_state
{
    Texture = (WallNoiseTexture);
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

float GetID(float2 uv, sampler IDSampler);
float2 GetNoise(float ID, sampler noiseSampler);

float Random(float2 st);
float FBM(float2 uv, float amplitude, float frequency);
float ValueNoise(float2 uv);

float4 PixelShaderFunction(float2 uv : TEXCOORD0) : COLOR0
{
    float layer = tex2D(LayerSampler, uv);
    float epsilon = 0.01;
    
    float2 worldPos = ScreenPosition + uv * ViewSize;
    float2 pos = worldPos / 16.0;

    float value = 1.0;
    
    if (abs(layer - TileLayer) < epsilon)
    {
        float ID = GetID(uv, TileIDSampler);
        float2 noise = GetNoise(ID, TileNoiseSampler);
        
        value = FBM(pos, noise.x, noise.y);
    }
    else if (abs(layer - WallLayer) < epsilon)
    {
        float ID = GetID(uv, WallIDSampler);
        float2 noise = GetNoise(ID, WallNoiseSampler);
        
        value = FBM(pos, noise.x, noise.y);
    }
    else
    {
        value = FBM(pos, BackgroundNoise.x, BackgroundNoise.y);
    }
    
    return float4(value, 0.0, 0.0, 1.0);
}

float GetID(float2 uv, sampler IDSampler)
{
    float2 worldPosition = ScreenPosition + (uv * ViewSize);
    int worldTileX = (int) floor(worldPosition.x / 16.0);
    int worldTileY = (int) floor(worldPosition.y / 16.0);

    int tileX = (int) round((float) worldTileX - MinTilePosition.x);
    int tileY = (int) round((float) worldTileY - MinTilePosition.y);

    tileX = clamp(tileX, 0, TileStride.x - 1);
    tileY = clamp(tileY, 0, TileStride.y - 1);

    int tileIndex = tileY * TileStride.x + tileX;

    int pixelIndex = (int) floor((float) tileIndex / 2.0);
    int channelIndex = tileIndex - pixelIndex * 2;

    float tileIDPixelX = fmod((float) pixelIndex, IDTextureSize.x);
    float tileIDPixelY = floor((float) pixelIndex / IDTextureSize.x);

    float2 tileUV = (float2(tileIDPixelX, tileIDPixelY) + 0.5) / IDTextureSize;
	
    float4 sampledTile = tex2Dlod(IDSampler, float4(tileUV.x, tileUV.y, 0, 0));
    float4 channels = sampledTile;
    
    int u0;
    int u1;
	
    if (channelIndex == 0)
    {
        u0 = (int) (channels.r * 255 + 0.5);
        u1 = (int) (channels.g * 255 + 0.5);
    }
    else
    {
        u0 = (int) (channels.b * 255 + 0.5);
        u1 = (int) (channels.a * 255 + 0.5);
    }
    
    float ID = (float) u0 + ((float) u1 * 256.0);
    
    return ID;
}

float2 GetNoise(float ID, sampler noiseSampler)
{
    if (ID == 65535)
    {
        return DefaultNoise;
    }
    
    float noisePixelX = fmod((float) ID, NoiseTextureSize.x);
    float noisePixelY = floor((float) ID / NoiseTextureSize.x);

    float2 noiseUV = (float2(noisePixelX, noisePixelY) + 0.5) / NoiseTextureSize;
    
    float4 sampledNoise = tex2Dlod(noiseSampler, float4(noiseUV.x, noiseUV.y, 0, 0));
    
    return sampledNoise;
}

float Random(float2 st)
{
    return frac(sin(dot(st.xy, float2(12.9898, 78.233))) * 43758.5453123);
}

float FBM(float2 uv, float amplitude, float frequency)
{
    float value = 0.0;
    
    [loop]
    for (int i = 0; i < 5; i++)
    {
        value += amplitude * ValueNoise(uv * frequency);
        frequency *= 2.0;
        amplitude *= 0.5;
    }
    
    return value;
}

float ValueNoise(float2 uv)
{
    float2 i = floor(uv);
    float2 f = frac(uv);
    
    float a = Random(i);
    float b = Random(i + float2(1.0, 0.0));
    float c = Random(i + float2(0.0, 1.0));
    float d = Random(i + float2(1.0, 1.0));
    
    float2 u = f * f * f * (f * (f * 6.0 - 15.0) + 10.0);
    
    return lerp(a, b, u.x) + (c - a) * u.y * (1.0 - u.x) + (d - b) * u.x * u.y;
}


technique Noise
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
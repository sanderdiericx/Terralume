sampler2D LayerSampler : register(s0);

float2 TileStride;
float2 MinTilePosition;

float2 ScreenPosition;
float2 ViewSize;

float3 DefaultReflection;

float2 ReflectionTextureSize;
float2 TileIDTextureSize;

float NonSolidTileLayer;
float TileLayer;
float LiquidLayer;

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

Texture2D LiquidIDTexture;

sampler LiquidIDSampler = sampler_state
{
    Texture = (LiquidIDTexture);
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

Texture2D TileReflectionTexture;

sampler TileReflectionSampler = sampler_state
{
    Texture = (TileReflectionTexture);
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

Texture2D LiquidReflectionTexture;

sampler LiquidReflectionSampler = sampler_state
{
    Texture = (LiquidReflectionTexture);
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

float GetID(sampler IDSampler, float2 uv);
float3 GetReflection(float ID, sampler reflectionSampler, float2 uv);

float4 PixelShaderFunction(float2 uv : TEXCOORD0) : COLOR0
{
    float layer = tex2D(LayerSampler, uv).r;
    float epsilon = 0.0001;
    
    float4 output = float4(DefaultReflection.r, DefaultReflection.g, DefaultReflection.b, -1.0);
    
    if (abs(layer - LiquidLayer) < epsilon) // Liquid
    {
        float ID = GetID(LiquidIDSampler, uv);
        float3 reflection = GetReflection(ID, LiquidReflectionSampler, uv);
        
        output = float4(reflection.r, reflection.g, reflection.b, ID);
    }
    else if (abs(layer - NonSolidTileLayer) < epsilon || abs(layer - TileLayer) < epsilon) // Non Solid Tiles, Tiles
    {
        float ID = GetID(TileIDSampler, uv);
        float3 reflection = GetReflection(ID, TileReflectionSampler, uv);
    
        output = float4(reflection.r, reflection.g, reflection.b, -1.0);
    }
    
    return output;
}

float GetID(sampler IDSampler, float2 uv)
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

    float tileIDPixelX = fmod((float) pixelIndex, TileIDTextureSize.x);
    float tileIDPixelY = floor((float) pixelIndex / TileIDTextureSize.x);

    float2 tileUV = (float2(tileIDPixelX, tileIDPixelY) + 0.5) / TileIDTextureSize;
	
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
    
    float tileID = (float) u0 + ((float) u1 * 256.0);

    return tileID;
}

float3 GetReflection(float ID, sampler reflectionSampler, float2 uv)
{    
    if (ID == 65535)
    {
        return float3(DefaultReflection.x, DefaultReflection.y, DefaultReflection.z);
    }
    
    float materialPixelX = fmod((float) ID, ReflectionTextureSize.x);
    float materialPixelY = floor((float) ID / ReflectionTextureSize.x);

    float2 materialUV = (float2(materialPixelX, materialPixelY) + 0.5) / ReflectionTextureSize;
    
    float4 sampledReflection = tex2Dlod(reflectionSampler, float4(materialUV.x, materialUV.y, 0, 0));
    
    // ID was never given a reflection vector
    if (sampledReflection.a < 0.01)
    {
        return float3(DefaultReflection.x, DefaultReflection.y, DefaultReflection.z);
    }
    
    return sampledReflection.rgb;
}

technique ReflectiveMaterial
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
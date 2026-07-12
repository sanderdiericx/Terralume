sampler2D LayerSampler : register(s0);

float2 TileStride;
float2 MinTilePosition;

float2 ScreenPosition;
float2 ViewSize;

float3 DefaultMaterial;
float MaterialVariationFactor;

float3 DefaultProjectileMaterial;

float2 MaterialTextureSize;
float2 TileIDTextureSize;

float BackgroundLayer;
float TileLayer;
float NonSolidTileLayer;
float WallLayer;
float EntityLayer;
float ProjectileLayer;
float PlayerLayer;
float LiquidLayer;
float OtherLayer;

Texture2D NoiseBuffer;

sampler NoiseSampler = sampler_state
{
    Texture = (NoiseBuffer);
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

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

Texture2D TileMaterialTexture;

sampler TileMaterialSampler = sampler_state
{
    Texture = (TileMaterialTexture);
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

Texture2D WallMaterialTexture;

sampler WallMaterialSampler = sampler_state
{
    Texture = (WallMaterialTexture);
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

Texture2D LiquidMaterialTexture;

sampler LiquidMaterialSampler = sampler_state
{
    Texture = (LiquidMaterialTexture);
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

float GetID(sampler IDSampler, float2 uv);
float4 GetMaterial(float ID, sampler materialSampler, float2 uv);

float4 PixelShaderFunction(float2 uv : TEXCOORD0) : COLOR0
{
    float layer = tex2D(LayerSampler, uv).r;
    float epsilon = 0.0001;
    
    float4 material = float4(DefaultMaterial.r, DefaultMaterial.g, DefaultMaterial.b, 1.0);
    
    if (abs(layer - BackgroundLayer) < epsilon) // Background 
    {
        material = float4(0.0, 1.0, 0.0, 1.0);
    }
    else if (abs(layer - ProjectileLayer) < epsilon)
    {
        material = float4(DefaultProjectileMaterial.r, DefaultProjectileMaterial.g, DefaultProjectileMaterial.b, 1.0);
    }
    else if (abs(layer - PlayerLayer) < epsilon || abs(layer - EntityLayer) < epsilon) // Player, Entity, Projectile
    {
        material = float4(DefaultMaterial.r, DefaultMaterial.g, DefaultMaterial.b, 1.0);
    }
    else if (abs(layer - LiquidLayer) < epsilon)
    {
        float ID = GetID(LiquidIDSampler, uv);
        material = GetMaterial(ID, LiquidMaterialSampler, uv);
    }
    else if (abs(layer - NonSolidTileLayer) < epsilon || abs(layer - TileLayer) < epsilon) // Non Solid Tiles, Tiles
    {
        float ID = GetID(TileIDSampler, uv);
        material = GetMaterial(ID, TileMaterialSampler, uv);
    }
    else if (abs(layer - WallLayer) < epsilon) // Walls
    {
        float ID = GetID(WallIDSampler, uv);
        material = GetMaterial(ID, WallMaterialSampler, uv);
    }
    
    return material;
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

float4 GetMaterial(float ID, sampler materialSampler, float2 uv)
{
    float roughnessVariation = (tex2D(NoiseSampler, uv).r - 0.5) * MaterialVariationFactor;
    float4 variation = float4(0.0, roughnessVariation, 0.0, 0.0);
    
    if (ID == 65535)
    {
        return float4(DefaultMaterial.x, DefaultMaterial.y, DefaultMaterial.z, 1.0) + variation;
    }
    
    float materialPixelX = fmod((float) ID, MaterialTextureSize.x);
    float materialPixelY = floor((float) ID / MaterialTextureSize.x);

    float2 materialUV = (float2(materialPixelX, materialPixelY) + 0.5) / MaterialTextureSize;
    
    float4 sampledMaterial = tex2Dlod(materialSampler, float4(materialUV.x, materialUV.y, 0, 0));
    
    // ID was never given a material vector
    if (sampledMaterial.a < 0.01)
    {
        return float4(DefaultMaterial.x, DefaultMaterial.y, DefaultMaterial.z, 1.0) + variation;
    }
    
    return sampledMaterial + variation;
}

technique Material
{
	pass Pass1
	{
		PixelShader = compile ps_3_0 PixelShaderFunction();
	}
}
sampler2D ScreenSampler : register(s0);

float2 TileStride;
float2 MinTilePosition;

float2 ScreenPosition;
float2 ViewSize;

float3 DefaultMaterial;
float MaterialVariationFactor;

float2 MaterialTextureSize;
float2 TileIDTextureSize;

float BackgroundLayer;
float TileLayer;
float NonSolidTileLayer;
float WallLayer;
float EntityLayer;
float ProjectileLayer;
float PlayerLayer;
float OtherLayer;

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

float4 GetMaterial(sampler IDSampler, sampler materialSampler, float2 uv, float4 defaultMaterial);

// TO DO: Add player, entity, wall materials. 
float4 PixelShaderFunction(float2 uv : TEXCOORD0) : COLOR0
{
    float layer = tex2D(LayerSampler, uv).r;
    float epsilon = 0.0001;
    
    float4 defaultMaterial = float4(DefaultMaterial.r, DefaultMaterial.g, DefaultMaterial.b, 1.0);
    float4 material = defaultMaterial;
    
    if (abs(layer - BackgroundLayer) < epsilon) // Background 
    {
        material = float4(0.0, 1.0, 0.0, 1.0);
    }
    else if (abs(layer - PlayerLayer) < epsilon || abs(layer - EntityLayer) < epsilon || abs(layer - ProjectileLayer) < epsilon) // Player, Entity, Projectile
    {
        material = defaultMaterial;
    }
    else if (abs(layer - NonSolidTileLayer) < epsilon || abs(layer - TileLayer) < epsilon) // Non Solid Tiles, Tiles
    {
        material = GetMaterial(TileIDSampler, TileMaterialSampler, uv, defaultMaterial);
    }
    else if (abs(layer - WallLayer) < epsilon) // Walls
    {
        material = GetMaterial(WallIDSampler, WallMaterialSampler, uv, defaultMaterial);
    }
    
    return material;
}

float4 GetMaterial(sampler IDSampler, sampler materialSampler, float2 uv, float4 defaultMaterial)
{
    float roughnessVariation = (tex2D(NoiseSampler, uv) - 0.5) * MaterialVariationFactor;
    float4 variation = float4(0.0, roughnessVariation, 0.0, 0.0);
    
    // Convert TexCoord to tile space
    float2 worldPosition = ScreenPosition + (uv * ViewSize);
    int worldTileX = (int) floor(worldPosition.x / 16.0);
    int worldTileY = (int) floor(worldPosition.y / 16.0);

    int tileX = (int) round((float) worldTileX - MinTilePosition.x);
    int tileY = (int) round((float) worldTileY - MinTilePosition.y);

    tileX = clamp(tileX, 0, TileStride.x - 1);
    tileY = clamp(tileY, 0, TileStride.y - 1);

	// Convert tile space coordinate to tile ID texture index
    int tileIndex = tileY * TileStride.x + tileX;

    int pixelIndex = (int) floor((float) tileIndex / 2.0);
    int channelIndex = tileIndex - pixelIndex * 2;

    float tileIDPixelX = fmod((float) pixelIndex, TileIDTextureSize.x);
    float tileIDPixelY = floor((float) pixelIndex / TileIDTextureSize.x);

    float2 tileUV = (float2(tileIDPixelX, tileIDPixelY) + 0.5) / TileIDTextureSize;
	
	// Sample from sourced index
    float4 sampledTile = tex2Dlod(IDSampler, float4(tileUV.x, tileUV.y, 0, 0));
    float4 channels = sampledTile;
    
    // Convert into integer
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
    
    // Check for no tile
    if (tileID == 65535)
    {
        return defaultMaterial + variation;
    }
    
    float materialPixelX = fmod((float) tileID, MaterialTextureSize.x);
    float materialPixelY = floor((float) tileID / MaterialTextureSize.x);

    float2 materialUV = (float2(materialPixelX, materialPixelY) + 0.5) / MaterialTextureSize;
    
    float4 sampledMaterial = tex2Dlod(materialSampler, float4(materialUV.x, materialUV.y, 0, 0));
    
    // Tile ID was never given a material vector
    if (sampledMaterial.a < 0.01)
    {
        return defaultMaterial + variation;
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
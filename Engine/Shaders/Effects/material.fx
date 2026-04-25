sampler2D ScreenSampler : register(s0);

float2 MaterialParameter_TileStride;
float2 MaterialParameter_ScreenPosition;
float2 MaterialParameter_MinTilePosition;
float2 MaterialParameter_ViewSize;
float2 MaterialParameter_TileIDTextureSize;
float2 MaterialParameter_MaterialTextureSize;
float3 MaterialParameter_DefaultMaterial;

float LayerParameter_Background;
float LayerParameter_Tile;
float LayerParameter_NonSolidTile;
float LayerParameter_Wall;
float LayerParameter_Entity;
float LayerParameter_Player;
float LayerParameter_Projectile;
float LayerParameter_Other;

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

Texture2D MaterialTexture;

sampler MaterialSampler = sampler_state
{
    Texture = (MaterialTexture);
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

struct PixelShaderInput
{
	float4 Position : SV_POSITION;
	float2 TexCoord : TEXCOORD0;
	float4 Color : COLOR0;
};

// TO DO: Add player, entity, wall materials. 
float4 PixelShaderFunction(PixelShaderInput input) : COLOR0
{
    float layer = tex2D(LayerSampler, input.TexCoord).r;
    float4 defaultMaterial = float4(MaterialParameter_DefaultMaterial.r, MaterialParameter_DefaultMaterial.g, MaterialParameter_DefaultMaterial.b, 1.0);
    
	// Background
    if (abs(layer - LayerParameter_Background) < 0.0001)
    {
        return float4(0.0, 0.0, 0.0, 0.0);
    }
    
    if (abs(layer - LayerParameter_Player) < 0.0001 || abs(layer - LayerParameter_Entity) < 0.0001 || abs(layer - LayerParameter_Projectile) < 0.0001)
    {
        return defaultMaterial;
    }
    
    if (abs(layer - LayerParameter_NonSolidTile) < 0.0001 || abs(layer - LayerParameter_Tile) < 0.0001)
    {
        // Convert TexCoord to tile space
        float2 worldPosition = MaterialParameter_ScreenPosition + (input.TexCoord * MaterialParameter_ViewSize);
        int worldTileX = (int) floor(worldPosition.x / 16.0);
        int worldTileY = (int) floor(worldPosition.y / 16.0);

        int tileX = (int) round((float) worldTileX - MaterialParameter_MinTilePosition.x);
        int tileY = (int) round((float) worldTileY - MaterialParameter_MinTilePosition.y);

        tileX = clamp(tileX, 0, MaterialParameter_TileStride.x - 1);
        tileY = clamp(tileY, 0, MaterialParameter_TileStride.y - 1);

	    // Convert tile space coordinate to tile ID texture index
        int tileIndex = tileY * MaterialParameter_TileStride.x + tileX;

        int pixelIndex = (int) floor((float) tileIndex / 2.0);
        int channelIndex = tileIndex - pixelIndex * 2;

        float tileIDPixelX = fmod((float) pixelIndex, MaterialParameter_TileIDTextureSize.x);
        float tileIDPixelY = floor((float) pixelIndex / MaterialParameter_TileIDTextureSize.x);

        float2 tileUV = (float2(tileIDPixelX, tileIDPixelY) + 0.5) / MaterialParameter_TileIDTextureSize;
	
	    // Sample from sourced index
        float4 sampledTile = tex2D(TileIDSampler, tileUV);
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
            return defaultMaterial;
        }
    
        float materialPixelX = fmod((float) tileID, MaterialParameter_MaterialTextureSize.x);
        float materialPixelY = floor((float) tileID / MaterialParameter_MaterialTextureSize.x);
    
        float2 materialUV = (float2(materialPixelX, materialPixelY) + 0.5) / MaterialParameter_MaterialTextureSize;
    
        return tex2D(MaterialSampler, float2(materialUV.x, materialUV.y));
    }
    
    // Walls for now
    return defaultMaterial;
}

technique Material
{
	pass Pass1
	{
		PixelShader = compile ps_3_0 PixelShaderFunction();
	}
}
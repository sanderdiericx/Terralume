sampler2D AlbedoTexture : register(s0);

float2 TileStride;
float2 TileIDTextureSize;

float2 ScreenPosition;
float2 ViewSize;
float2 MinTilePosition;

float2 MaterialTextureSize;

Texture2D TileTarget;

sampler TileSampler = sampler_state
{
    Texture = (TileTarget);
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

float4 PixelShaderFunction(PixelShaderInput input) : COLOR0
{
	// Background
    if (tex2D(AlbedoTexture, input.TexCoord).a < 0.01)
    {
        return float4(0.0, 0.0, 0.0, 0.0);
    }
	
	// Not a tile
    if (tex2D(TileSampler, input.TexCoord).a < 0.01)
    {
        return float4(0.0, 0.8, 0.0, 1.0);
    }

	// Convert TexCoord to tile space
    float2 worldPosition = ScreenPosition + (input.TexCoord * ViewSize) + 0.5;
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
    float4 sampledTile = tex2D(TileIDSampler, tileUV);
    float4 channels = sampledTile;
    
    // Convert into integer
    int u0;
    int u1;
	
    if (channelIndex == 0)
    {
        u0 = (int)(channels.r * 255 + 0.5);
        u1 = (int)(channels.g * 255 + 0.5);
    }
    else
    {
        u0 = (int) (channels.b * 255 + 0.5);
        u1 = (int) (channels.a * 255 + 0.5);
    }
    
    float tileID = (float) u0 + ((float) u1 * 256.0);
    
    float materialPixelX = fmod((float) tileID, MaterialTextureSize.x);
    float materialPixelY = floor((float) tileID / MaterialTextureSize.x);
    
    float2 materialUV = (float2(materialPixelX, materialPixelY) + 0.5) / MaterialTextureSize;
    
    return tex2D(MaterialSampler, float2(materialUV.x, materialUV.y));
}

technique Material
{
	pass Pass1
	{
		PixelShader = compile ps_3_0 PixelShaderFunction();
	}
}
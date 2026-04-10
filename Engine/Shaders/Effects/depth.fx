sampler2D AlbedoTexture : register(s0);

float2 TileStride;
float2 TileTextureSize;
float2 ScreenPosition;
float2 ViewSize;
float2 MinTilePosition;

Texture2D TileTexture;

sampler TileSampler = sampler_state
{
    Texture = (TileTexture);
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
	if (tex2D(AlbedoTexture, input.TexCoord).a < 0.01)
	{
		return float4(0.9, 0.9, 0.9, 1.0);
	}

	float2 worldPosition = ScreenPosition + (input.TexCoord * ViewSize) + 0.5;
	int worldTileX = (int)floor(worldPosition.x / 16.0);
	int worldTileY = (int)floor(worldPosition.y / 16.0);

	int tileX = (int)round((float)worldTileX - MinTilePosition.x);
	int tileY = (int)round((float)worldTileY - MinTilePosition.y);

	tileX = clamp(tileX, 0, TileStride.x - 1);
	tileY = clamp(tileY, 0, TileStride.y - 1);

	int tileIndex = tileY * TileStride.x + tileX;

	int pixelIndex  = (int)floor((float)tileIndex / 4.0);
	int channelIndex = tileIndex - pixelIndex * 4;

	float tw = TileTextureSize.x;
	float pixelX = fmod((float)pixelIndex, tw);
	float pixelY = floor((float)pixelIndex / tw);

	float2 tileUV = (float2(pixelX, pixelY) + 0.5) / TileTextureSize;
	float4 sampledTile = tex2D(TileSampler, tileUV);

	float4 channels = sampledTile;
	float tileVal;
	if (channelIndex == 0) tileVal = channels.r;
	else if (channelIndex == 1) tileVal = channels.g;
	else if (channelIndex == 2) tileVal = channels.b;
	else tileVal = channels.a;

	bool tile = tileVal > 0.5;

	if (tile)
	{
		return float4(0.3, 0.3, 0.3, 1.0);
	}
	else
	{
		return float4(0.6, 0.6, 0.6, 1.0);
	}
}

technique Depth
{
	pass Pass1
	{
		PixelShader = compile ps_3_0 PixelShaderFunction();
	}
}
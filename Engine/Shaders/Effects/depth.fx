sampler2D AlbedoTexture : register(s0);
sampler2D TileTexture : register(s1);

int TileStride;
float2 TileTextureSize;
float2 ScreenOffset;

struct PixelShaderInput
{
	float4 Position : SV_POSITION;
	float2 TexCoord : TEXCOORD0;
	float4 Color : COLOR0;
};

float4 PixelShaderFunction(PixelShaderInput input) : COLOR0
{
	float4 textureColor = tex2D(AlbedoTexture, input.TexCoord);
	
	if (textureColor.a < 0.01)
	{
		return float4(0.9, 0.9, 0.9, 1.0);
	}

	// Find the tile at this position
	int tileX = (int)((input.Position.x - ScreenOffset.x) / 16.0);
	int tileY = (int)((input.Position.y - ScreenOffset.y) / 16.0);
	int tileIndex = tileY * TileStride + tileX;
	
	// Decode from tile map
	int pixelIndex = tileIndex / 4;
	int channelIndex = tileIndex % 4;

	float pixelX = (float)(pixelIndex % (int)TileTextureSize.x);
    float pixelY = (float)(pixelIndex / (int)TileTextureSize.x);
    
    // Normalize and offset by 0.5 to hit pixel centers
    float2 tileUV = (float2(pixelX, pixelY) + 0.5) / TileTextureSize;
    float4 sampledTile = tex2D(TileTexture, tileUV);

	float mask[4] = { sampledTile.r, sampledTile.g, sampledTile.b, sampledTile.a };
	bool tile = mask[channelIndex] > 0.5;

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
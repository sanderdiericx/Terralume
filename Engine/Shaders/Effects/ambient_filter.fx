sampler2D LayerSampler : register(s0);

float2 ScreenPosition;
float2 ViewSize;
float2 MinTilePosition;
float2 TileStride;

float2 AmbientFilterTextureSize;

Texture2D AmbientFilterTexture;

sampler AmbientFilterSampler = sampler_state
{
	Texture = (AmbientFilterTexture);
	MinFilter = Point;
	MagFilter = Point;
	MipFilter = Point;
	AddressU = Clamp;
	AddressV = Clamp;
};

float4 PixelShaderFunction(float2 uv : TEXCOORD0) : COLOR0
{
	float2 worldPosition = ScreenPosition + (uv * ViewSize);
	int worldTileX = (int) floor(worldPosition.x / 16.0);
	int worldTileY = (int) floor(worldPosition.y / 16.0);

	int tileX = (int) round((float) worldTileX - MinTilePosition.x);
	int tileY = (int) round((float) worldTileY - MinTilePosition.y);

	tileX = clamp(tileX, 0, TileStride.x - 1);
	tileY = clamp(tileY, 0, TileStride.y - 1);

	int tileIndex = tileY * TileStride.x + tileX;

	float tileIDPixelX = fmod((float) tileIndex, AmbientFilterTextureSize.x);
	float tileIDPixelY = floor((float) tileIndex / AmbientFilterTextureSize.x);

	float2 tileUV = (float2(tileIDPixelX, tileIDPixelY) + 0.5) / AmbientFilterTextureSize;
	
	float4 sampledTile = tex2Dlod(AmbientFilterSampler, float4(tileUV.x, tileUV.y, 0, 0));
    
	return float4(sampledTile.r, 0.0, 0.0, 1.0);
}

technique AmbientFilter
{
	pass Pass1
	{
		PixelShader = compile ps_3_0 PixelShaderFunction();
	}
}
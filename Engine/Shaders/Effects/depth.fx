sampler2D AlbedoTexture : register(s0);

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

Texture2D WallTarget;

sampler WallSampler = sampler_state
{
    Texture = (WallTarget);
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
	float4 albedo = tex2D(AlbedoTexture, input.TexCoord);
	float4 tile = tex2D(TileSampler, input.TexCoord);
	float4 wall = tex2D(WallSampler, input.TexCoord);

	// Background
	if (albedo.a < 0.01)
	{
		return float4(0.9, 0.9, 0.9, 1.0);
	}

	// Tile
	if (tile.a > 0.01)
	{
		return float4(0.3, 0.3, 0.3, 1.0);
	}

	// Pixel is not a wall
	if (wall.a < 0.01)
	{
		return float4(0.5, 0.5, 0.5, 1.0);
	}
	
	// Pixel could be a wall
	if (wall.a > 0.01)
	{
		float dist = distance(albedo.xyz, wall.xyz);

		// Albedo buffer and wall buffer contain ~ the same pixel (probably a wall)
		// NOTE: Might cause artifacts, improve in the future
		if (dist < 0.0001)
		{
			return float4(0.7, 0.7, 0.7, 1.0);
		}
	}

	return float4(0.5, 0.5, 0.5, 1.0);
}

technique Depth
{
	pass Pass1
	{
		PixelShader = compile ps_3_0 PixelShaderFunction();
	}
}
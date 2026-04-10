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

Texture2D UndergroundBackgroundTarget;

sampler BackgroundSampler = sampler_state
{
    Texture = (UndergroundBackgroundTarget);
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
	// Check if this fragment is a tile
	if (tex2D(TileSampler, input.TexCoord).a > 0.01)
	{
		return float4(0.3, 0.3, 0.3, 1.0);
	}

	float4 albedoColor = tex2D(AlbedoTexture, input.TexCoord);
	float4 backgroundColor = tex2D(BackgroundSampler, input.TexCoord);

	bool isBackground = abs(backgroundColor.r - albedoColor.r) < 0.00001 && abs(backgroundColor.g - albedoColor.g) < 0.00001 && abs(backgroundColor.b - albedoColor.b) < 0.00001 && backgroundColor.a > 0.01;

	if (isBackground)
	{
		return float4(0.9, 0.9, 0.9, 1.0);
	}

	// Check albedo buffer for non tile and non transparent
	if (albedoColor.a > 0.01)
	{
		return float4(0.6, 0.6, 0.6, 1.0);
	}

	return float4(0.9, 0.9, 0.9, 1.0);
}

technique Depth
{
	pass Pass1
	{
		PixelShader = compile ps_3_0 PixelShaderFunction();
	}
}
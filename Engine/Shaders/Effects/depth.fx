sampler2D ScreenSampler : register(s0);

float DepthParameter_BackgroundDepth;
float DepthParameter_TileDepth;
float DepthParameter_NonSolidTileDepth;
float DepthParameter_WallDepth;
float DepthParameter_EntityDepth;
float DepthParameter_PlayerDepth;
float DepthParameter_ProjectileDepth;
float DepthParameter_OtherDepth;

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

Texture2D NonSolidTileTarget;

sampler NonSolidTileSampler = sampler_state
{
    Texture = (NonSolidTileTarget);
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

Texture2D EntityTarget;

sampler EntitySampler = sampler_state
{
    Texture = (EntityTarget);
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

Texture2D PlayerTarget;

sampler PlayerSampler = sampler_state
{
    Texture = (PlayerTarget);
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

Texture2D ProjectileTarget;

sampler ProjectileSampler = sampler_state
{
    Texture = (ProjectileTarget);
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
    if (tex2D(ScreenSampler, input.TexCoord).a < 0.01)
    {
        return float4(DepthParameter_BackgroundDepth, DepthParameter_BackgroundDepth, DepthParameter_BackgroundDepth, 1.0);
    }

    // Player
    if (tex2D(PlayerSampler, input.TexCoord).a > 0.01)
    {
        return float4(DepthParameter_PlayerDepth, DepthParameter_PlayerDepth, DepthParameter_PlayerDepth, 1.0);
    }
    
    // Entity
    if (tex2D(EntitySampler, input.TexCoord).a > 0.01)
    {
        return float4(DepthParameter_EntityDepth, DepthParameter_EntityDepth, DepthParameter_EntityDepth, 1.0);
    }
    
    // Projectile
    if (tex2D(ProjectileSampler, input.TexCoord).a > 0.01)
    {
        return float4(DepthParameter_ProjectileDepth, DepthParameter_ProjectileDepth, DepthParameter_ProjectileDepth, 1.0);
    }
    
    // Non Solid Tiles
    if (tex2D(NonSolidTileSampler, input.TexCoord).a > 0.01)
    {
        return float4(DepthParameter_NonSolidTileDepth, DepthParameter_NonSolidTileDepth, DepthParameter_NonSolidTileDepth, 1.0);
    }
    
    // Tiles
    if (tex2D(TileSampler, input.TexCoord).a > 0.01)
    {
        return float4(DepthParameter_TileDepth, DepthParameter_TileDepth, DepthParameter_TileDepth, 1.0);
    }
    
    // Walls
    if (tex2D(WallSampler, input.TexCoord).a > 0.01)
    {
        return float4(DepthParameter_WallDepth, DepthParameter_WallDepth, DepthParameter_WallDepth, 1.0);
    }
    
    return float4(DepthParameter_OtherDepth, DepthParameter_OtherDepth, DepthParameter_OtherDepth, 1.0);
}

technique Depth
{
	pass Pass1
	{
		PixelShader = compile ps_3_0 PixelShaderFunction();
	}
}
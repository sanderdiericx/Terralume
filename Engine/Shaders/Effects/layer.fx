sampler2D ScreenSampler : register(s0);

static const float3 LUMINANCE = float3(1.0, 1.0, 1.0);

float BackgroundLayer;
float TileLayer;
float NonSolidTileLayer;
float WallLayer;
float EntityLayer;
float ProjectileLayer;
float PlayerLayer;
float LiquidLayer;
float OtherLayer;

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

Texture2D LiquidTarget;

sampler LiquidSampler = sampler_state
{
    Texture = (LiquidTarget);
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

float4 PixelShaderFunction(float2 uv : TEXCOORD0) : COLOR0
{
    float4 layer = float4(OtherLayer, 0.0, 0.0, 1.0);
    float epsilon = 0.01;
    
    float4 projectile = tex2D(ProjectileSampler, uv);
    float4 player = tex2D(PlayerSampler, uv);
    
    // Note: sample order is fragile and modifying it can result in artifacts
    if (tex2D(LiquidSampler, uv).a > epsilon) // Liquids
    {
        layer = float4(LiquidLayer, 0.0, 0.0, 1.0);
    }
	else if (player.a > epsilon || dot(player.rgb, LUMINANCE) > 0.01) // Player
	{
		layer = float4(PlayerLayer, 0.0, 0.0, 1.0);
	}
    else if (tex2D(TileSampler, uv).a > epsilon) // Tiles
    {
        layer = float4(TileLayer, 0.0, 0.0, 1.0);
    }
    else if (projectile.a > epsilon || dot(projectile.rgb, LUMINANCE) > 0.01) // Projectile
    {
        layer = float4(ProjectileLayer, 0.0, 0.0, 1.0);
    }
    else if (tex2D(EntitySampler, uv).a > epsilon) // Entity
    {
        layer = float4(EntityLayer, 0.0, 0.0, 1.0);
    }
    else if (tex2D(NonSolidTileSampler, uv).a > epsilon) // Non Solid Tiles
    {
        layer = float4(NonSolidTileLayer, 0.0, 0.0, 1.0);
    }
    else if (tex2D(WallSampler, uv).a > epsilon) // Wall
    {
        layer = float4(WallLayer, 0.0, 0.0, 1.0);
    }
    else if (tex2D(ScreenSampler, uv).a < epsilon) // Background
    {
        layer = float4(BackgroundLayer, 0.0, 0.0, 1.0);
    }
    
    return layer;
}

technique Layer
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
sampler2D ScreenSampler : register(s0);

float LayerParameter_Background;
float LayerParameter_Tile;
float LayerParameter_NonSolidTile;
float LayerParameter_Wall;
float LayerParameter_Entity;
float LayerParameter_Player;
float LayerParameter_Projectile;
float LayerParameter_Other;

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
        return float4(LayerParameter_Background, LayerParameter_Background, LayerParameter_Background, 1.0);
    }

    // Player
    if (tex2D(PlayerSampler, input.TexCoord).a > 0.01)
    {
        return float4(LayerParameter_Player, LayerParameter_Player, LayerParameter_Player, 1.0);
    }
    
    // Entity
    if (tex2D(EntitySampler, input.TexCoord).a > 0.01)
    {
        return float4(LayerParameter_Entity, LayerParameter_Entity, LayerParameter_Entity, 1.0);
    }
    
    // Projectile
    if (tex2D(ProjectileSampler, input.TexCoord).a > 0.01)
    {
        return float4(LayerParameter_Projectile, LayerParameter_Projectile, LayerParameter_Projectile, 1.0);
    }
    
    // Non Solid Tiles
    if (tex2D(NonSolidTileSampler, input.TexCoord).a > 0.01)
    {
        return float4(LayerParameter_NonSolidTile, LayerParameter_NonSolidTile, LayerParameter_NonSolidTile, 1.0);
    }
    
    // Tiles
    if (tex2D(TileSampler, input.TexCoord).a > 0.01)
    {
        return float4(LayerParameter_Tile, LayerParameter_Tile, LayerParameter_Tile, 1.0);
    }
    
    // Not a Wall
    if (tex2D(WallSampler, input.TexCoord).a > 0.01)
    {
        return float4(LayerParameter_Wall, LayerParameter_Wall, LayerParameter_Wall, 1.0);
    }
    
    return float4(LayerParameter_Other, LayerParameter_Other, LayerParameter_Other, 1.0);
}

technique Layer
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
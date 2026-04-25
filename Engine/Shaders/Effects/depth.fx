sampler2D ScreenSampler : register(s0);

float DepthParameter_BackgroundDepth;
float DepthParameter_TileDepth;
float DepthParameter_NonSolidTileDepth;
float DepthParameter_WallDepth;
float DepthParameter_EntityDepth;
float DepthParameter_PlayerDepth;
float DepthParameter_ProjectileDepth;
float DepthParameter_OtherDepth;

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

struct PixelShaderInput
{
	float4 Position : SV_POSITION;
	float2 TexCoord : TEXCOORD0;
	float4 Color : COLOR0;
};

float4 PixelShaderFunction(PixelShaderInput input) : COLOR0
{
    float layer = tex2D(LayerSampler, input.TexCoord).r;
    
    // Background
    if (abs(layer - LayerParameter_Background) < 0.0001)
    {
        return float4(DepthParameter_BackgroundDepth, DepthParameter_BackgroundDepth, DepthParameter_BackgroundDepth, 1.0);
    }

    // Player
    if (abs(layer - LayerParameter_Player) < 0.0001)
    {
        return float4(DepthParameter_PlayerDepth, DepthParameter_PlayerDepth, DepthParameter_PlayerDepth, 1.0);
    }
    
    // Entity
    if (abs(layer - LayerParameter_Entity) < 0.0001)
    {
        return float4(DepthParameter_EntityDepth, DepthParameter_EntityDepth, DepthParameter_EntityDepth, 1.0);
    }
    
    // Projectile
    if (abs(layer - LayerParameter_Projectile) < 0.0001)
    {
        return float4(DepthParameter_ProjectileDepth, DepthParameter_ProjectileDepth, DepthParameter_ProjectileDepth, 1.0);
    }
    
    // Non Solid Tiles
    if (abs(layer - LayerParameter_NonSolidTile) < 0.0001)
    {
        return float4(DepthParameter_NonSolidTileDepth, DepthParameter_NonSolidTileDepth, DepthParameter_NonSolidTileDepth, 1.0);
    }
   
    // Tiles
    if (abs(layer - LayerParameter_Tile) < 0.0001)
    {
        return float4(DepthParameter_TileDepth, DepthParameter_TileDepth, DepthParameter_TileDepth, 1.0);
    }
    
    // Walls
    if (abs(layer - LayerParameter_Wall) < 0.0001)
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
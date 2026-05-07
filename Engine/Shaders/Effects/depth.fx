sampler2D LayerSampler : register(s0);

float HeightVariationFactor;

float BackgroundDepth;
float TileDepth;
float NonSolidTileDepth;
float WallDepth;
float EntityDepth;
float ProjectileDepth;
float PlayerDepth;
float OtherDepth;

float BackgroundLayer;
float TileLayer;
float NonSolidTileLayer;
float WallLayer;
float EntityLayer;
float ProjectileLayer;
float PlayerLayer;
float OtherLayer;

Texture2D HeightBuffer;

sampler HeightSampler = sampler_state
{
    Texture = (HeightBuffer);
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};


float4 PixelShaderFunction(float2 uv : TEXCOORD0) : COLOR0
{
    float layer = tex2D(LayerSampler, uv).r;
    float epsilon = 0.0001;
    
    float4 depth = float4(OtherDepth, 0.0, 0.0, 1.0);
    
    if (abs(layer - BackgroundLayer) < epsilon) // Background
    {
        depth = float4(BackgroundDepth, 0.0, 0.0, 1.0);
    }
    else if (abs(layer - TileLayer) < epsilon) // Tiles
    {
        depth = float4(TileDepth, 0.0, 0.0, 1.0);
    }
    else if (abs(layer - WallLayer) < epsilon) // Walls
    {
        depth = float4(WallDepth, 0.0, 0.0, 1.0);
    }
    else if (abs(layer - NonSolidTileLayer) < epsilon) // Non Solid Tiles
    {
        depth = float4(NonSolidTileDepth, 0.0, 0.0, 1.0);
    }
    else if (abs(layer - EntityLayer) < epsilon) // Entity
    {
        depth = float4(EntityDepth, 0.0, 0.0, 1.0);
    }
    else if (abs(layer - ProjectileLayer) < epsilon) // Projectile
    {
        depth = float4(ProjectileDepth, 0.0, 0.0, 1.0);
    }
    else if (abs(layer - PlayerLayer) < epsilon) // Player
    {
        depth = float4(PlayerDepth, 0.0, 0.0, 1.0);
    }
    
    float height = (tex2D(HeightSampler, uv).r - 0.5) * HeightVariationFactor;
    depth.r += height;
    
    return depth;
}

technique Depth
{
	pass Pass1
	{
		PixelShader = compile ps_3_0 PixelShaderFunction();
	}
}
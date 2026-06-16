sampler2D LayerSampler : register(s0);

float2 ScreenSize;
float SeedLayer;

Texture2D JFAAirBuffer;

sampler JFAAirSampler = sampler_state
{
    Texture = (JFAAirBuffer);
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

Texture2D JFATileBuffer;

sampler JFATileSampler = sampler_state
{
    Texture = (JFATileBuffer);
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

float4 PixelShaderFunction(float2 uv : TEXCOORD0) : COLOR0
{
    float2 pixelPos = uv * ScreenSize;

    float2 tileSeed = tex2Dlod(JFATileSampler, float4(uv, 0, 0)).rg;
    float2 airSeed = tex2Dlod(JFAAirSampler, float4(uv, 0, 0)).rg;

    float distToTile = (tileSeed.x >= 0.0) ? length(pixelPos - tileSeed) : 1e9;
    float distToAir = (airSeed.x >= 0.0) ? length(pixelPos - airSeed) : 1e9;

    float layer = tex2Dlod(LayerSampler, float4(uv, 0, 0)).r;
    bool isTile = abs(layer - SeedLayer) < 0.01;

    float signedDist = isTile ? -distToAir : distToTile;

    return float4(signedDist, distToTile, 0, 0);
}

technique SDFCombine
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
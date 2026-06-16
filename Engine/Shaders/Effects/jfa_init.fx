sampler2D LayerSampler : register(s0);

float TileLayer;
float2 ScreenSize;

float4 PixelShaderFunction(float2 uv : TEXCOORD0) : COLOR0
{
    float4 seed = float4(-1.0, -1.0, 0.0, 0.0);
    
    float layer = tex2Dlod(LayerSampler, float4(uv.x, uv.y, 0.0, 0.0)).r;
    if (abs(layer - TileLayer) < 0.01)
    {
        float2 val = float2(uv.x, uv.y) * ScreenSize;
        
        seed = float4(val.x, val.y, 0.0, 0.0);
    }
    
    return seed;
}

technique JFA_Init
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
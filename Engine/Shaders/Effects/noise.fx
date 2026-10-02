sampler2D LayerSampler : register(s0);

float2 ScreenPosition;
float2 ViewSize;

float2 DefaultNoise;

float Random(float2 st);
float FBM(float2 uv, float amplitude, float frequency);
float ValueNoise(float2 uv);

float4 PixelShaderFunction(float2 uv : TEXCOORD0) : COLOR0
{
    float epsilon = 0.01;
    
    float2 worldPos = ScreenPosition + uv * ViewSize;
    float2 pos = worldPos / 16.0;

    float value = FBM(pos, DefaultNoise.x, DefaultNoise.y);
    
    return float4(value, 0.0, 0.0, 1.0);
}

float Random(float2 st)
{
    return frac(sin(dot(st.xy, float2(12.9898, 78.233))) * 43758.5453123);
}

float FBM(float2 uv, float amplitude, float frequency)
{
    float value = 0.0;
    
    [loop]
    for (int i = 0; i < 5; i++)
    {
        value += amplitude * ValueNoise(uv * frequency);
        frequency *= 2.0;
        amplitude *= 0.5;
    }
    
    return value;
}

float ValueNoise(float2 uv)
{
    float2 i = floor(uv);
    float2 f = frac(uv);
    
    float a = Random(i);
    float b = Random(i + float2(1.0, 0.0));
    float c = Random(i + float2(0.0, 1.0));
    float d = Random(i + float2(1.0, 1.0));
    
    float2 u = f * f * f * (f * (f * 6.0 - 15.0) + 10.0);
    
    return lerp(a, b, u.x) + (c - a) * u.y * (1.0 - u.x) + (d - b) * u.x * u.y;
}


technique Noise
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
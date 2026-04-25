sampler2D ScreenSampler : register(s0);

float2 ScreenSize;

struct PixelShaderInput
{
    float4 Position : SV_POSITION;
    float2 TexCoord : TEXCOORD0;
    float4 Color : COLOR0;
};

float4 PixelShaderFunction(PixelShaderInput input) : COLOR0
{    
    // Constants (move to uniforms)
    float3 luminance = float3(0.299, 0.587, 0.114);
    float strength = 1.0;
    
    float dx = 1.0 / ScreenSize.x;
    float dy = 1.0 / ScreenSize.y;
    
    // Sample 3x3 kernel and convert to greyscale
    float s00 = dot(tex2D(ScreenSampler, input.TexCoord + float2(-dx, -dy)).rgb, luminance);
    float s01 = dot(tex2D(ScreenSampler, input.TexCoord + float2(0, -dy)).rgb, luminance);
    float s02 = dot(tex2D(ScreenSampler, input.TexCoord + float2(dx, -dy)).rgb, luminance);
    float s10 = dot(tex2D(ScreenSampler, input.TexCoord + float2(-dx, 0)).rgb, luminance);
    float s11 = dot(tex2D(ScreenSampler, input.TexCoord + float2(0, 0)).rgb, luminance);
    float s12 = dot(tex2D(ScreenSampler, input.TexCoord + float2(dx, 0)).rgb, luminance);
    float s20 = dot(tex2D(ScreenSampler, input.TexCoord + float2(-dx, dy)).rgb, luminance);
    float s21 = dot(tex2D(ScreenSampler, input.TexCoord + float2(0, dy)).rgb, luminance);
    float s22 = dot(tex2D(ScreenSampler, input.TexCoord + float2(dx, dy)).rgb, luminance);
    
    float gX = (s02 + (s12 * 2.0) + s22) - (s00 + (s10 * 2.0) + s20);
    float gY = (s00 + (s01 * 2.0) + s02) - (s20 + (s21 * 2.0) + s22);
    
    float3 normal = normalize(float3(-gX * strength, -gY * strength, 1.0));
    
    float3 output = (normal * 0.5) + 0.5;
    
    return float4(output.r, output.g, output.b, 1.0);
}

technique Normal
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
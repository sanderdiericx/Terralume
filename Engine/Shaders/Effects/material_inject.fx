sampler2D InputSampler : register(s0);

float3 Material;

float4 PixelShaderFunction(float2 uv : TEXCOORD0) : COLOR0
{
    float a = tex2D(InputSampler, uv).a;
    return float4(Material.r, Material.g, Material.b, a);
}

technique MaterialInject
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
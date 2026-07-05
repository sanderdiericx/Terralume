sampler2D SDFSampler : register(s0);

float BlurStartDistance;
float BlurEndDistance;

Texture2D WideBlurTarget;

sampler WideBlurSampler = sampler_state
{
    Texture = (WideBlurTarget);
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

Texture2D TightBlurTarget;

sampler TightBlurSampler = sampler_state
{
    Texture = (TightBlurTarget);
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

float4 PixelShaderFunction(float2 uv : TEXCOORD0) : COLOR0
{
    float2 sdf = tex2D(SDFSampler, uv).rg;

    float t = smoothstep(BlurStartDistance, BlurEndDistance, sdf.r);
    
    float4 wide = tex2D(WideBlurSampler, uv);
    float3 tight = tex2D(TightBlurSampler, uv).rgb;
    
    float3 final = lerp(tight, wide.rgb, t);
    
    float luma = dot(final, float3(0.299, 0.587, 0.114));
    final = lerp(final, float3(luma, luma, luma), saturate(t * 0.25));
    
    return float4(final.r, final.g, final.b, wide.a);
}

technique BackgroundBlur
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
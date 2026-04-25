sampler2D AlbedoSampler : register(s0);

float2 ScreenSize;

int WorldThickness;
float LightDepth;
float LightIntensity;

float Constant;
float Linear;
float Quadratic;

float3 Ambient;
float3 Diffuse;
float3 Specular;

Texture2D DepthBuffer;

sampler DepthSampler = sampler_state
{
    Texture = (DepthBuffer);
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

Texture2D NormalBuffer;

sampler NormalSampler = sampler_state
{
    Texture = (NormalBuffer);
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
    float4 albedo = tex2D(AlbedoSampler, input.TexCoord);
    
    float3 viewDir = float3(0.0, 0.0, 1.0);
    float3 lightPos = float3(ScreenSize.x / 2.0, ScreenSize.y / 2.0, LightDepth);
    float3 normal = normalize(tex2D(NormalSampler, input.TexCoord) * 2.0 - 1.0);
    float3 fragPos = float3(input.Position.x, input.Position.y, tex2D(DepthSampler, input.TexCoord).r * WorldThickness);
    
    float3 lightDir = normalize(lightPos - fragPos);
    
    float diff = max(dot(normal, lightDir), 0.0);
    
    float3 halfwayDir = normalize(lightDir + viewDir);
    float spec = pow(max(dot(normal, halfwayDir), 0.0), 32.0);
    
    float distance = length(lightPos - fragPos) / 100.0;
    float attenuation = 1.0 / (Constant + Linear * distance + Quadratic * (distance * distance));
    
    float3 ambient = Ambient * albedo.rgb;
    float3 diffuse = Diffuse * diff * albedo.rgb * LightIntensity;
    float3 specular = Specular * spec * LightIntensity;
    
    diffuse *= attenuation;
    specular *= attenuation;
    
    float3 output = ambient + diffuse + specular;
    
    return float4(output.r, output.g, output.b, albedo.a);
}

technique Lighting
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
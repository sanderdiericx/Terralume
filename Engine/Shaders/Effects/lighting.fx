sampler2D AlbedoSampler : register(s0);

static const float PI = 3.14159265359f;

static const float3x3 SRGB_TO_AP1 = float3x3
(
    0.59719, 0.35458, 0.04823,
    0.07600, 0.90834, 0.01566,
    0.02840, 0.13383, 0.83777
);

static const float3x3 AP1_TO_SRGB = float3x3
(
    1.60475, -0.53108, -0.07367,
    -0.10208, 1.10813, -0.00605,
    -0.00327, -0.07276, 1.17603
);

float2 ScreenSize;
float DepthScale;

int ActiveLights;
float LightDepth;

float Exposure;

float2 LightDataTextureSize;

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

Texture2D MaterialBuffer;

sampler MaterialSampler = sampler_state
{
    Texture = (MaterialBuffer);
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

Texture2D LightPositionTexture;

sampler LightPositionSampler = sampler_state
{
    Texture = (LightPositionTexture);
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

Texture2D LightColorTexture;

sampler LightColorSampler = sampler_state
{
    Texture = (LightColorTexture);
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


float3 CalculatePointLight(float3 pixelPos, float3 lightPos, float3 lightColor, float3 albedo, float3 material, float3 normal, float3 viewDir, float intensity, float range);

float3 BaseReflectivityFunction(float metallic, float3 albedo);
float3 RadianceFunction(float3 lightColor, float intensity, float dist, float range);

float NormalDistributionFunction(float roughness, float3 normal, float3 halfVector);

float GeometrySchlickGGX(float roughness, float3 normal, float3 x);
float GeometrySmith(float roughness, float3 normal, float3 viewDir, float3 lightDir);

float3 FresnelSchlickApproximationFunction(float3 baseReflectivity, float3 viewDir, float3 halfVector);
float3 EnergyConservationFunction(float3 fresnel, float metallic);

float3 CalculateBRDFSpecular(float distribution, float geometry, float3 fresnel, float3 normal, float3 viewDir, float3 lightDir);
float3 CalculateOutgoingLight(float3 energyConservation, float3 albedo, float3 specular, float3 radianceColor, float3 normal, float3 lightDir);

float3 ToneMappingACES(float3 color);
float3 RRTAndODTFit(float3 aces);

float3 ToLinear(float3 sRGB);
float3 ToGamma(float3 linearRGB);

float4 PixelShaderFunction(PixelShaderInput input) : COLOR0
{     
    float4 albedoOriginal = tex2D(AlbedoSampler, input.TexCoord);
    
    if (albedoOriginal.a < 0.01)
    {
        discard;
    }
    
    float3 albedo = ToLinear(albedoOriginal.rgb);
    float3 normal = normalize(tex2D(NormalSampler, input.TexCoord).rgb * 2.0 - 1.0);
    float3 material = tex2D(MaterialSampler, input.TexCoord).rgb;
    
    float pixelZ = tex2D(DepthSampler, input.TexCoord).r * DepthScale;
    float3 pixelPos = float3(input.TexCoord.x * ScreenSize.x, input.TexCoord.y * ScreenSize.y, pixelZ);    
    float3 viewDir = float3(0.0, 0.0, 1.0);
    
    float3 color = float3(0.0, 0.0, 0.0);
    
    for (int i = 0; i < ActiveLights; i++)
    {        
        // Sample light position, intensity, radius and color for this index from the data textures
        float xIndex = fmod((float) i, LightDataTextureSize.x);
        float yIndex = floor((float) i / LightDataTextureSize.x);
        float2 lightUV = (float2(xIndex, yIndex) + 0.5) / LightDataTextureSize;
        
        float4 positionSample = tex2Dlod(LightPositionSampler, float4(lightUV.x, lightUV.y, 0, 0));
        float4 colorSample = tex2Dlod(LightColorSampler, float4(lightUV.x, lightUV.y, 0, 0));
        
        float3 lightPos = float3(positionSample.r, positionSample.g, LightDepth);
        float lightRange = positionSample.b;
        float lightIntensity = positionSample.w;
        
        float3 lightColor = float3(colorSample.r, colorSample.g, colorSample.b);
        
        color += CalculatePointLight(pixelPos, lightPos, lightColor, albedo, material, normal, viewDir, lightIntensity, lightRange);
    }

    float3 emissive = albedo * material.b * 5.0;
    color += emissive;
    
    color *= Exposure;
    color = ToneMappingACES(color);
    
    color = ToGamma(color);
    
    return float4(color.r, color.g, color.b, albedoOriginal.a);
}


float3 CalculatePointLight(float3 pixelPos, float3 lightPos, float3 lightColor, float3 albedo, float3 material, float3 normal, float3 viewDir, float intensity, float range)
{
    float3 lightDir = normalize(lightPos - pixelPos);
    float3 halfVector = normalize(viewDir + lightDir);
    
    float3 baseReflectivity = BaseReflectivityFunction(material.r, albedo);
    float3 radianceColor = RadianceFunction(lightColor, intensity, length(lightPos - pixelPos), range);
    
    float distribution = NormalDistributionFunction(material.g, normal, halfVector);
    float geometry = GeometrySmith(material.g, normal, viewDir, lightDir);
    
    float3 fresnel = FresnelSchlickApproximationFunction(baseReflectivity, viewDir, halfVector);
    float3 energyConservation = EnergyConservationFunction(fresnel, material.r);
    float3 specular = CalculateBRDFSpecular(distribution, geometry, fresnel, normal, viewDir, lightDir);
    
    return CalculateOutgoingLight(energyConservation, albedo, specular, radianceColor, normal, lightDir);
}

float3 BaseReflectivityFunction(float metallic, float3 albedo)
{
    return (1.0 - metallic) * 0.04 + metallic * albedo;
}

float3 RadianceFunction(float3 lightColor, float intensity, float dist, float range)
{
    float scaledDist = dist / range;
    
    float3 incomingLight = lightColor * intensity;
    
    return incomingLight * (1.0 / (scaledDist * scaledDist + 1.0));
}

float NormalDistributionFunction(float roughness, float3 normal, float3 halfVector)
{
    float r = max(roughness, 0.04);

    float alpha = r * r;
    float alpha2 = alpha * alpha;
    
    float NdotH = max(dot(normal, halfVector), 0.0);
    float denom = (NdotH * NdotH) * (alpha2 - 1.0) + 1.0;
    
    return alpha2 / (PI * (denom * denom));
}

float GeometrySchlickGGX(float roughness, float3 normal, float3 x)
{
    float r = max(roughness, 0.04);
    
    float k = ((r + 1.0) * (r + 1.0)) / 8.0;
    float NdotX = max(dot(normal, x), 0.0);
    float denom = NdotX * (1.0 - k) + k;

    return NdotX / denom;
}

float GeometrySmith(float roughness, float3 normal, float3 viewDir, float3 lightDir)
{
    return GeometrySchlickGGX(roughness, normal, viewDir) * GeometrySchlickGGX(roughness, normal, lightDir);
}

float3 FresnelSchlickApproximationFunction(float3 baseReflectivity, float3 viewDir, float3 halfVector)
{
    return baseReflectivity + (1.0 - baseReflectivity) * pow(1.0 - dot(viewDir, halfVector), 5);
}

float3 EnergyConservationFunction(float3 fresnel, float metallic)
{
    return (1.0 - fresnel) * (1.0 - metallic);
}

float3 CalculateBRDFSpecular(float distribution, float geometry, float3 fresnel, float3 normal, float3 viewDir, float3 lightDir)
{
    float3 num = distribution * geometry * fresnel;
    float denom = 4.0 * max(dot(normal, viewDir), 0.0) * max(dot(normal, lightDir), 0.0) + 0.0001;
    
    return num / denom;
}

float3 CalculateOutgoingLight(float3 energyConservation, float3 albedo, float3 specular, float3 radianceColor, float3 normal, float3 lightDir)
{
    return (energyConservation * (albedo / PI) + specular) * radianceColor * max(dot(normal, lightDir), 0.0);
}

float3 ToneMappingACES(float3 color)
{    
    return mul(AP1_TO_SRGB, RRTAndODTFit(mul(SRGB_TO_AP1, color)));
}

float3 RRTAndODTFit(float3 aces)
{
    float3 a = aces * (aces + 0.0245786) - 0.000090537;
    float3 b = aces * (0.983729 * aces + 0.4329510) + 0.238081;
    return a / b;
}

float3 ToLinear(float3 sRGB)
{
    return pow(sRGB, 2.2);
}

float3 ToGamma(float3 linearRGB)
{
    return pow(linearRGB, 1.0 / 2.2);
}

technique Lighting
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
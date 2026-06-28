sampler2D AlbedoSampler : register(s0);

static const float PI = 3.14159265359f;

float2 ScreenSize;
float2 ViewSize;
float2 ScreenPosition;

float DepthScale;
float LightDepth;

float BackgroundLayer;
float TileLayer;
float WallLayer;

float AlbedoVariationFactor;
float OcclusionFactor;

float2 LightDataTextureSize;
float2 LightIndexTextureSize;
float2 SunOcclusionTextureSize;

float VolumetricStrength;
float VolumetricRange;
float VolumetricNoiseContrast;

float3 AmbientColor;
float AmbientStrength;
float AmbientDiffuseWeight;

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

Texture2D NoiseBuffer;

sampler NoiseSampler = sampler_state
{
    Texture = (NoiseBuffer);
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

Texture2D SDFBuffer;

sampler SDFSampler = sampler_state
{
    Texture = (SDFBuffer);
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

Texture2D LightGridTexture;
sampler LightGridSampler = sampler_state
{
    Texture = (LightGridTexture);
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

Texture2D LightIndexTexture;
sampler LightIndexSampler = sampler_state
{
    Texture = (LightIndexTexture);
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

Texture2D SunOcclusionTexture;
sampler SunOcclusionSampler = sampler_state
{
    Texture = (SunOcclusionTexture);
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

float Occlusion(float2 sdf);
float SunOcclusion(float2 uv);

float3 CalculatePointLight(float3 pixelPos, float3 lightPos, float3 lightColor, float3 albedo, float3 material, float3 normal, float3 viewDir, float intensity, float range);
float3 CalculateAmbientLight(float3 albedo, float3 material, float3 normal, float3 viewDir, float2 uv, float2 sdf, float2 texelSize);

float3 ApproximateOpenDirection(float2 uv, float2 texelSize, out bool gradientValid);

float3 BaseReflectivityFunction(float metallic, float3 albedo);
float3 RadianceFunction(float3 lightColor, float intensity, float dist, float range);

float NormalDistributionFunction(float roughness, float3 normal, float3 halfVector);

float GeometrySchlickGGX(float roughness, float3 normal, float3 x);
float GeometrySmith(float roughness, float3 normal, float3 viewDir, float3 lightDir);

float3 FresnelSchlickApproximationFunction(float3 baseReflectivity, float3 viewDir, float3 halfVector);
float3 FresnelSchlickRoughness(float3 baseReflectivity, float NdotV, float roughness);

float3 EnergyConservationFunction(float3 fresnel, float metallic);

float3 CalculateBRDFSpecular(float distribution, float geometry, float3 fresnel, float3 normal, float3 viewDir, float3 lightDir);
float3 CalculateOutgoingLight(float3 energyConservation, float3 albedo, float3 specular, float3 radianceColor, float3 normal, float3 lightDir);

float3 ToLinear(float3 sRGB);

float4 PixelShaderFunction(float2 uv : TEXCOORD0) : COLOR0
{   
    float noise = tex2Dlod(NoiseSampler, float4(uv.x, uv.y, 0.0, 0.0)).r;
    float2 sdf = tex2Dlod(SDFSampler, float4(uv.x, uv.y, 0, 0));
    
    float4 albedoOriginal = tex2D(AlbedoSampler, uv);
    float layer = tex2D(LayerSampler, uv).r;
    
    if (abs(layer - BackgroundLayer) < 0.01 && albedoOriginal.a < 0.01)
    {
        discard;
    }

    // Albedo noise variation
    if (abs(layer - TileLayer) < 0.01 || abs(layer - WallLayer) < 0.01)
    {
        float variation = (noise - 0.5) * AlbedoVariationFactor;
        float3 albedoVariation = float3(-variation, -variation, -variation);
        
        albedoOriginal.rgb = clamp(albedoOriginal.rgb + albedoVariation, 0.0, 1.0);
    }
    
    float3 albedo = ToLinear(albedoOriginal.rgb);
    
    float3 normal = normalize(tex2D(NormalSampler, uv).rgb * 2.0 - 1.0);
    float3 material = tex2D(MaterialSampler, uv).rgb;
    
    float depth = tex2D(DepthSampler, uv).r;
    float pixelZ = depth * DepthScale;
    float3 pixelPos = float3(uv.x * ScreenSize.x, uv.y * ScreenSize.y, pixelZ);
    float3 viewDir = float3(0.0, 0.0, 1.0);
    
    float lightPositionZ = LightDepth * DepthScale;
    
    float4 gridData = tex2Dlod(LightGridSampler, float4(uv.x, uv.y, 0, 0));
    float startIndex = round(gridData.r);
    float lightCount = round(gridData.g);
    
    float3 color = float3(0.0, 0.0, 0.0);
    
    [loop]
    for (int i = 0; i < (int) lightCount; i++)
    {
        float linearIndex = startIndex + (float) i;

        float iy = floor((linearIndex + 0.5) / LightIndexTextureSize.x);
        float ix = linearIndex - (iy * LightIndexTextureSize.x);
        float2 indexUV = (float2(ix, iy) + 0.5) / LightIndexTextureSize;

        float rawIndex = tex2Dlod(LightIndexSampler, float4(indexUV.x, indexUV.y, 0, 0)).r;
        float lightDataIndex = round(rawIndex);
    
        float ly = floor((lightDataIndex + 0.5) / LightDataTextureSize.x);
        float lx = lightDataIndex - (ly * LightDataTextureSize.x);
        float2 lightUV = (float2(lx, ly) + 0.5) / LightDataTextureSize;
    
        float4 positionSample = tex2Dlod(LightPositionSampler, float4(lightUV.x, lightUV.y, 0, 0));
        float4 colorSample = tex2Dlod(LightColorSampler, float4(lightUV.x, lightUV.y, 0, 0));
    
        float3 lightPos = float3(positionSample.r, positionSample.g, lightPositionZ);
        float lightRange = positionSample.b;
        float lightIntensity = positionSample.w;
        float3 lightColor = float3(colorSample.r, colorSample.g, colorSample.b);
    
        color += CalculatePointLight(pixelPos, lightPos, lightColor, albedo, material, normal, viewDir, lightIntensity, lightRange);
        
        float dist = length(lightPos.xy - pixelPos.xy);
        float haloFalloff = exp(-dist / (lightRange * VolumetricRange));
        float bleedAttenuation = saturate(0.5 + sdf.r * 0.05);
        
        float3 volumetric = lightColor * lightIntensity * haloFalloff * bleedAttenuation * VolumetricStrength;
        float volumetricNoise = 1.0 + (noise - 0.5) * VolumetricNoiseContrast;

        color += volumetric * volumetricNoise;
    }
    
    float2 texelSize = 1.0 / ScreenSize;
    color += CalculateAmbientLight(albedo, material, normal, viewDir, uv, sdf, texelSize);
    
    color *= Occlusion(sdf);
    
    float3 emissive = albedo * material.b * 5.0;
    color += emissive;
        
    return float4(color.r, color.g, color.b, albedoOriginal.a);
}

float Occlusion(float2 sdf)
{
    float occlusion = 1.0;
    
    if (sdf.r < 0.0)
    {
        occlusion = exp(OcclusionFactor * sdf.r);
    }

    return occlusion;
}

float SunOcclusion(float2 uv)
{
    float worldPosX = ScreenPosition.x + (uv.x * ViewSize.x);
    float worldPosY = ScreenPosition.y + (uv.y * ViewSize.y);
    
    float worldTileX = floor(worldPosX / 16.0);
    float worldTileY = floor(worldPosY / 16.0);
    
    float minTileX = floor(ScreenPosition.x / 16.0);
    
    float textureIndexX = worldTileX - minTileX;
    
    float sampleX = (textureIndexX + 0.5) / SunOcclusionTextureSize.x;
    
    float occluderTileHeight = tex2Dlod(SunOcclusionSampler, float4(sampleX, 0.0, 0.0, 0.0)).r;
    
    float occlusion = 1.0;
    
    if (worldTileY > occluderTileHeight - 1.0)
    {
        occlusion = 0.0;
    }
    
    return occlusion;
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

float3 CalculateAmbientLight(float3 albedo, float3 material, float3 normal, float3 viewDir, float2 uv, float2 sdf, float2 texelSize)
{
    float metallic = material.r;
    float roughness = material.g;

    bool gradientValid;
    float3 openDir = ApproximateOpenDirection(uv, texelSize, gradientValid);

    float3 ambientSecondary = AmbientColor * float3(0.05, 0.05, 0.08);
    float openWeight = gradientValid ? dot(normal.xy, openDir.xy) * 0.5 + 0.5 : (sdf.r >= 0.0 ? 1.0 : 0.0);
    openWeight = pow(openWeight, 1.5);
    
    float3 hemisphereColor = lerp(ambientSecondary, AmbientColor, openWeight);
    float3 ambientRadiance = hemisphereColor * AmbientStrength;

    float NdotV = max(dot(normal, viewDir), 0.0);
    float3 baseReflectivity = BaseReflectivityFunction(metallic, albedo);
    float3 fresnel = FresnelSchlickRoughness(baseReflectivity, NdotV, roughness);
    float3 energyConservation = EnergyConservationFunction(fresnel, metallic);

    float3 diffuseAmbient = energyConservation * albedo * ambientRadiance * AmbientDiffuseWeight;
    float3 specularAmbient = fresnel * ambientRadiance;

    return (diffuseAmbient + specularAmbient);
}

float3 ApproximateOpenDirection(float2 uv, float2 texelSize, out bool gradientValid)
{
    float right = tex2Dlod(SDFSampler, float4(uv + float2(texelSize.x, 0), 0, 0)).r;
    float left = tex2Dlod(SDFSampler, float4(uv - float2(texelSize.x, 0), 0, 0)).r;
    float up = tex2Dlod(SDFSampler, float4(uv - float2(0, texelSize.y), 0, 0)).r;
    float down = tex2Dlod(SDFSampler, float4(uv + float2(0, texelSize.y), 0, 0)).r;

    float2 grad = float2(right - left, down - up);
    gradientValid = dot(grad, grad) > 0.0001;

    return gradientValid ? normalize(float3(grad, 0.0)) : float3(0, 0, 1);
}

float AmbientLightOcclusion(float2 sdf)
{
    float occlusion = 1.0;
    if (sdf.r < 0.0)
    {
        occlusion = exp(OcclusionFactor * sdf.r * 2.0);
    }
    return occlusion;
}

float3 BaseReflectivityFunction(float metallic, float3 albedo)
{
    return (1.0 - metallic) * 0.04 + metallic * albedo;
}

float3 RadianceFunction(float3 lightColor, float intensity, float dist, float range)
{
    float normalizedDist = dist / range;
    
    float window = saturate(1.0 - normalizedDist * normalizedDist);
    window = window * window;
    
    float3 incomingLight = lightColor * intensity;
    return incomingLight * window / (normalizedDist * normalizedDist + 1.0);
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

float3 FresnelSchlickRoughness(float3 baseReflectivity, float NdotV, float roughness)
{
    float3 smoothness = float3(1.0 - roughness, 1.0 - roughness, 1.0 - roughness);
    float3 maxReflectivity = max(smoothness, baseReflectivity);

    return baseReflectivity + (maxReflectivity - baseReflectivity) * pow(1.0 - NdotV, 5);
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

float3 ToLinear(float3 sRGB)
{
    return pow(sRGB, 2.2);
}

technique Radiance
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
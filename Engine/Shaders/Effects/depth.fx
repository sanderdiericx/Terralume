﻿sampler2D AlbedoTexture : register(s0);

struct PixelShaderInput
{
	float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
	float4 Color : COLOR0;
};

float4 PixelShaderFunction(PixelShaderInput input) : COLOR0
{
	float4 textureColor = tex2D(AlbedoTexture, input.TexCoord);
	
	if (textureColor.a < 0.01)
	{
		return float4(0.9, 0.9, 0.9);
	}




	return float4(0.0, 0.0, 0.0, 0.0);
}

technique Glow
{
	pass Pass1
	{
		PixelShader = compile ps_3_0 PixelShaderFunction();
	}
}
float4x4 World;
float4x4 View;
float4x4 Projection;

uniform extern texture FloorTexture;

sampler FloorSampler = sampler_state
{
    Texture = <FloorTexture>;
    mipfilter = LINEAR; 
};

// TODO: add effect parameters here.

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float3 Normal : NORMAL;
	float2 Texcoord : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float3 Normal : TEXCOORD1;
	float2 Texcoord : TEXCOORD0;
	float3 WorldPos : TEXCOORD2;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
    output.Normal = mul(input.Normal, World);
    output.Texcoord = input.Texcoord;
    output.WorldPos = worldPosition.xyz;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float3 lightPos = float3(0,0,2);
	float3 color = tex2D(FloorSampler, input.Texcoord).rgb;
	float3 normal = normalize((color - 0.5) * 2);
	float scalarDiffuse = dot(-input.Normal, normalize(input.WorldPos - lightPos));

    return float4( color, 1) * scalarDiffuse;
}

float4 WaterShaderFunction(VertexShaderOutput input) : COLOR0
{
	float3 lightPos = float3(0,0,2);
	float3 color = tex2D(FloorSampler, input.Texcoord).rgb;
	float3 normal = normalize((color - 0.5) * 2);
	float scalarDiffuse = dot(-input.Normal, normalize(input.WorldPos - lightPos));

    return float4(0.2,.3,1,.2);
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}


technique Technique2
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 WaterShaderFunction();
    }
}

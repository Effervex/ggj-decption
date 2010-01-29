float4x4 World;
float4x4 View;
float4x4 Projection;

uniform extern texture FloorTexture;
uniform extern texture TreeTexture;
uniform extern texture SceneTexture;

sampler FloorSampler = sampler_state
{
    Texture = <FloorTexture>;
    minfilter = LINEAR;
    magfilter = LINEAR;
    mipfilter = LINEAR; 
};
sampler TreeSampler = sampler_state
{
    Texture = <TreeTexture>;
    minfilter = LINEAR;
    magfilter = LINEAR;
    mipfilter = LINEAR; 
};

sampler SceneSampler = sampler_state
{
    Texture = <SceneTexture>;
    minfilter = LINEAR;
    magfilter = LINEAR;
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
	float4 HomPos : TEXCOORD3;
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
    output.HomPos =  output.Position;

    return output;
}

float4 DoFog(float4 lastColor, float4 homPos) {

float4 fogColor = float4(.43, .49, .43,1);
	return lerp(lastColor,fogColor, clamp(homPos.z / 30*homPos.w,0.4,1));
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float3 lightPos = float3(0,0,2);
	float3 color = tex2D(FloorSampler, input.Texcoord).rgb;
	float3 normal = normalize((color - 0.5) * 2);
	float scalarDiffuse = dot(-input.Normal, normalize(input.WorldPos - lightPos));

float4 lastColor = float4( color, 1) * scalarDiffuse;
    return DoFog(lastColor, input.HomPos);
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
float riverOffset = 0;

float4 WaterShaderFunction(VertexShaderOutput input) : COLOR0
{
	float3 lightPos = float3(0,0,2);
	float3 refractColor = tex2D(FloorSampler, input.Texcoord * .25+ float2(0,-.1*riverOffset) ).rgb;
	float3 normal = normalize((refractColor - 0.5) * 2);
	float2 offset = refractColor.xy * 0.2f;
input.HomPos.y *= -1;
float2 screenCoord = 0.5 + 0.5 * ((input.HomPos.xy + offset) / input.HomPos.w);
float4 returnColor = float4(refractColor*0.35,0) + tex2D(SceneSampler, screenCoord)*.65;
returnColor.xyz = returnColor;

    return returnColor;
}

technique Technique2
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 WaterShaderFunction();
    }
}


float4 TreeShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 color = tex2D(TreeSampler, input.Texcoord);
    return color;
}

technique Technique3
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 TreeShaderFunction();
    }
}

float4 BackgroundShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 color = tex2D(TreeSampler, input.Texcoord) * input.Texcoord.y ;
    return color;
}

technique Technique4
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 BackgroundShaderFunction();
    }
}
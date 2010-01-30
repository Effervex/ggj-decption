float4x4 World;
float4x4 View;
float4x4 Projection;

uniform extern texture FloorTexture;
uniform extern texture TreeTexture;
uniform extern texture SceneTexture;
uniform extern texture CausticsTexture;

sampler FloorSampler = sampler_state
{
    Texture = <FloorTexture>;
    minfilter = LINEAR;
    magfilter = LINEAR;
    mipfilter = LINEAR; 
};
sampler CausticsSampler = sampler_state
{
    Texture = <CausticsTexture>;
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
float4 fogcolor = float4(0,0,0,1);
float time = 0.0f;
float4 DoFog(float4 lastColor, float4 homPos) {

	float4 tmp = lerp(lastColor,fogcolor, clamp(homPos.z / 30*homPos.w,0.085,1));
	tmp.w = lastColor.w;
	return tmp;
}


float riverOffset = 0;

float4 DoCaustics(float2 coord) {
coord*=.75;
 float2 move = float2(0,time);
	return lerp(tex2D(CausticsSampler, move + coord),tex2D(CausticsSampler, move*0.7 +coord*.5) , sin(100*time) * .5 + .5);
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
float causticAmount = clamp(3.5+ input.WorldPos.y * 2,0,1);
	float4 caustics = lerp(DoCaustics(input.WorldPos.xz - float2(0,riverOffset)) * 2.5,1, causticAmount);
	
	float3 color = tex2D(FloorSampler, input.Texcoord).rgb;
	float3 normal = normalize((color - 0.5) * 2);

	float4 lastColor = caustics* float4( color, 1);
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

float4 WaterShaderFunction(VertexShaderOutput input) : COLOR0
{
float edge = lerp(0,.1,sin((input.HomPos.x * 2)  + 1.54));

	float3 lightPos = float3(0,0,2);
	float3 refractColor = tex2D(FloorSampler, input.Texcoord * .25+ float2(0,-.1*riverOffset + time) ).rgb;
	float3 normal = normalize((refractColor - 0.5) * 2);
	float2 offset = refractColor.xy * edge;
input.HomPos.y *= -1;
float2 screenCoord = 0.5 + 0.5 * ((input.HomPos.xy + offset) / input.HomPos.w);
float4 returnColor = float4(refractColor*edge,0) + tex2D(SceneSampler, screenCoord)*.65;
returnColor.xyz = returnColor ;
returnColor.w = 1;
    return DoFog(returnColor, input.HomPos);
}

technique Technique2
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 WaterShaderFunction();
    }
}


bool doFog = false;
float4 TreeShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 color = tex2D(TreeSampler, input.Texcoord);
	if(doFog)
		color = DoFog(color, input.HomPos);
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
	float4 color = tex2D(TreeSampler, input.Texcoord) * (1 - input.Texcoord.y);
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

/////////////

uniform extern texture DebryTexture;
struct PS_INPUT
{
	float4 HomPos : TEXCOORD1;
	float4 WPos : TEXCOORD2;
       float2 TexCoord : TEXCOORD0;
    
};
uniform extern float4x4 WVPMatrix;



sampler DebrySampler = sampler_state
{
	Texture = <DebryTexture>;
};						
float4 PixelShader(PS_INPUT input) : COLOR0
{
    float2 texCoord;

    texCoord = input.TexCoord.xy;
    float4 color  = tex2D(DebrySampler, texCoord);
    color.w = min(color.w, .5+input.WPos.y);
 return DoFog(color, input.HomPos);
    //return tex2D(DebrySampler, texCoord);
}

void VertexShader(float4 pos : POSITION0, 
float2 aux : TEXCOORD0,
	out float4 opos : POSITION0, 
	out float4 HomPos : TEXCOORD1,
	out float4 WPos : TEXCOORD2, 
	out float rad : PSIZE) 
{
WPos = pos;
	opos = mul(pos, WVPMatrix);
	HomPos = opos;
	rad = aux.x;// 
	rad = (31 + aux.x + opos.z) / opos.w;
}

technique Debry
{
	pass P0
	{
		vertexShader = compile vs_2_0 VertexShader();
		pixelShader = compile ps_2_0 PixelShader();
	}
}    

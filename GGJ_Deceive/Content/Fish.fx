float4x4 World;
float4x4 View;
float4x4 Projection;
uniform extern texture Env;
uniform extern texture ScalesTexture;
float Time=0;

sampler SkyboxS = sampler_state
{
    Texture = <Env>;
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    MipFilter = LINEAR;
    AddressU = WRAP;
    AddressV = WRAP;
};
sampler Scales = sampler_state
{
    Texture = <ScalesTexture>;
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    MipFilter = LINEAR;
    AddressU = WRAP;
    AddressV = WRAP;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float3 Normal : NORMAL;
	float2 texcoord : TEXCOORD0;
};

struct VertexShaderOutput
{
float3 N : TEXCOORD1;
    float4 Position : POSITION0;
    float2 texcoord : TEXCOORD0;
    float4 ws : TEXCOORD2;
    float4 env : TEXCOORD3;
    float4 tint : TEXCOORD4;
};

float4 GetColor(float4 Position) {

	float4 tm = 0.51;
	tm = sin(((Position.x + 50)/1) * .7) * .5 + .5;
	tm += .35;
    //tm.gb = (.5 + .5*sin(Position.x * 5)) * .5;
    //tm += 0.1;
    
    float4 green = float4(0.86, .7, 0.4,1);
    float4 blue = float4(0.6, .73, 0.9,1);
    
    float white = float4(.8,.7,.9,1) * .1;
    
    return lerp ( 
    white,    
    lerp(tm * blue, green,(Position.x + 20) / 40),
    
    clamp((Position.y + 13) / 20,0,1));
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
float diffuse = dot(input.N, normalize(float3(1,1,0)));
 
 float4 causticsColor = tex2D(SkyboxS,input.env.xz + float2(Time * 0.06,0));
    
    return tex2D(Scales,input.texcoord) * causticsColor *  GetColor(input.ws) * (.75+diffuse ) * input.tint;
}


VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

float flapAmount = clamp(input.Position.x * 0.2, 0, 4);
float waverAmount = sin(input.Position.z) * 0.5 + .5;

    float4 worldPosition = mul(input.Position + 
    waverAmount * float4(0, cos(Time * 0.5) * 2,0,0) + 
    flapAmount * float4(0,0,sin(2*Time + input.Position.x * 0.04f) * 10,0), World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
    output.ws = input.Position;
       output.env = worldPosition;
	output.texcoord = input.texcoord;
	output.N = mul(input.Normal, World);
       output.tint = float4(1, 1,1,1);
    return output;
}
float puffAmount = 0;
VertexShaderOutput PuffVertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;


float3 t = input.Normal;

float inflated = log( max(puffAmount ,1 )) * 8 ;



    float4 worldPosition = mul(input.Position + float4(normalize(input.Normal.xyz )* inflated,0), World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
    //output.Color = lerp( float3(1,1,1), float3(0.5,0.6,.8), clamp( inflated , 0, 1));
    output.N = normalize(mul(input.Normal, World));
output.texcoord = input.texcoord;
output.ws = input.Position;
       output.env = worldPosition;
       
       float c = 1- clamp(0,.51,puffAmount * .55);
       output.tint = float4(c, 1, c,1);
    return output;
}

technique Technique1
{
    pass Pass1
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}


technique Technique2
{
    pass Pass1
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_2_0 PuffVertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}

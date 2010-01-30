float4x4 View;
float4x4 Projection;

uniform extern texture bloodTexture;

sampler BloodSampler = sampler_state
{
    Texture = <bloodTexture>;
    minfilter = LINEAR;
    magfilter = LINEAR;
    mipfilter = LINEAR; 
};

struct PS_INPUT
{ 
	float4 texCoord : TEXCOORD0;
	float height : TEXCOORD1;
       float4 aux : COLOR;
    
};					
float4 PixelShader(PS_INPUT input) : COLOR0
{
return tex2D(BloodSampler, input.texCoord) * float4(1,1,1, min(.7,-input.height ));
 }

void VertexShader(float4 pos : POSITION0, 

	 float2 texCoord : TEXCOORD0, 
	out float4 opos : POSITION0, 
	out float4 oaux : COLOR, 
	out float height : TEXCOORD1,
	out float2 otexCoord : TEXCOORD0, 
	out float rad : PSIZE) 
{
    float4 worldPosition = pos;
    float4 viewPosition = mul(worldPosition, View);
    opos= mul(viewPosition, Projection);
    
	//HomPos = opos;
	otexCoord = texCoord;
	float darkness = texCoord.x;
	height = worldPosition.y;
	oaux = float4(darkness*darkness,darkness*darkness,darkness*darkness,0.051 * (texCoord.x));
	rad =   ( texCoord.y  * 3 * (1)) * (5 +   opos.z) / opos.w;
}

technique Techniq
{
	pass P0
	{
		vertexShader = compile vs_2_0 VertexShader();
		pixelShader = compile ps_2_0 PixelShader();
	}
}    

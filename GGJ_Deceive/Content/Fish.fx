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
float2 Aux : TEXCOORD0;
    // TODO: add input channels such as texture
    // coordinates and vertex colors here.
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float3 WS : TEXCOORD0;
    float3 norm : TEXCOORD1;
float4 Aux : TEXCOORD2;

    // TODO: add vertex shader outputs such as colors and texture
    // coordinates here. These values will automatically be interpolated
    // over the triangle, and provided as input to your pixel shader.
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
    float alongBody = input.Aux.x;
    float isStripe = clamp(input.Aux.y,  0.1, 0.9);
    
    float4 yellow = float4(.8, .7, .2,1);
    float4 orage = float4(.8,.4,.2,1);
    
output.Aux.xyz = lerp( yellow, orage, alongBody) * isStripe;
output.Aux.w = alongBody;
    // TODO: add your vertex shader code here.
output.WS = worldPosition.xyz;
output.WS.y = input.Normal.y * 0.5 + 0.5;
output.norm  = mul(input.Normal, World);
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    // TODO: add your pixel shader code here.
    float2 texCoord = float2(input.Aux.w * 2, input.WS.y);
    float4 scalesColor = tex2D(Scales,texCoord);
    float4 stripeColor = float4(input.Aux.xyz * lerp(.51, tex2D(SkyboxS, input.WS.xz + float2(0,Time)), input.norm.y), 0);
    float4 causticsColor = tex2D(SkyboxS,input.WS.xz + float2(Time,0));
    
    return scalesColor * stripeColor;
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

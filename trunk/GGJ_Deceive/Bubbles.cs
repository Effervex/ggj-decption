using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GGJ_Deceive
{
    class Bubbles
    {
        static Texture2D bubble;
        static Effect effect;

        static List<VertexPositionNormalTexture> particles = new List<VertexPositionNormalTexture>();

        static public void Initalize() {

            effect = Game1.GetInstance.Content.Load<Effect>("Bubble");
            bubble = Game1.GetInstance.Content.Load<Texture2D>("Debry3");
        }

        static Vector3 random
        {
            get
            {
                return new Vector3((float)River.random.NextDouble() - 0.5f, (float)River.random.NextDouble() - 0.15f, (float)River.random.NextDouble() - 0.5f) * 2f;
            }
        }

        static public void AddBubbles(Vector3 position, int count)
        {

            for (int i = 0; i < count; i++)
            {

                particles.Add(new VertexPositionNormalTexture(position + random * 0.1f,
                    random,
                    new Vector2((float)River.random.NextDouble() * 0.7f, 1.61f * (float)River.random.NextDouble() + .1f)));
            }
        }

        static bool DeadUpdateFilter(VertexPositionNormalTexture p)
        {
            if (p.TextureCoordinate.X <= 0)
                return true;

            return false;

        } 
        static public void Update()
        {
            particles.RemoveAll(DeadUpdateFilter);
            for (int i = 0; i < particles.Count; i++)
            {
                Vector3 n = particles[i].Normal;// .Y += 1; n
                n.Y += 0.01f;
                Vector3 p = particles[i].Position + n * 0.01f;
                Vector2 t = new Vector2(particles[i].TextureCoordinate.X - 0.010f, particles[i].TextureCoordinate.Y);
                particles[i] = new VertexPositionNormalTexture(p, n, t);
            }
            
        }
        static public void Draw() {
            
            if (particles.Count == 0)
                return;

            Game1.GraphicsDevice.RenderState.DepthBufferWriteEnable = false;
            Game1.GraphicsDevice.RenderState.AlphaBlendEnable = true;
            Game1.GraphicsDevice.RenderState.PointSpriteEnable = true;
            effect.Parameters["bloodTexture"].SetValue(bubble);
            
            effect.Parameters["View"].SetValue(Game1.View);
            effect.Parameters["Projection"].SetValue(Game1.Projection);

            effect.Begin();
            foreach (EffectPass p in effect.CurrentTechnique.Passes)
            {

                p.Begin();
                Game1.GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.PointList, particles.ToArray(), 0, particles.Count);
                p.End();
            }
            effect.End();
            
            Game1.GraphicsDevice.RenderState.PointSpriteEnable = false;
            Game1.GraphicsDevice.RenderState.AlphaBlendEnable = false;
            Game1.GraphicsDevice.RenderState.DepthBufferWriteEnable = true;
        }
    }
}

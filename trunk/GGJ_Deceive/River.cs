using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GGJ_Deceive
{
    public class River
    {
        Texture2D riverfloor;
        Texture2D riverwater;

        VertexPositionNormalTexture[] water_vertices = null;
        VertexPositionNormalTexture[] river_vertices = null;
        VertexDeclaration vd;
        short[] river_indicies = null;
        short[] water_indicies = null;
        Effect effect;
        const int segement_vertex_count = 2 * 3 * 3;
        const int segments = 20;
        public void Update()
        {

            for (int i = 0; i < segement_vertex_count * segments; i += segement_vertex_count)
            {
                for (int j = 0; j < segement_vertex_count; j++)
                    river_indicies[j + i] = (short)(i + j);

                CreateSegement(i);
            }
        }

        public void Draw()
        {

            Game1.GraphicsDevice.VertexDeclaration = vd;
            Game1.GraphicsDevice.Textures[0] = riverfloor;

            effect.Parameters["FloorTexture"].SetValue(riverfloor);
            effect.Parameters["View"].SetValue(Game1.View);
            effect.Parameters["World"].SetValue(Matrix.Identity);
            effect.Parameters["Projection"].SetValue(Game1.Projection);

            effect.CurrentTechnique = effect.Techniques["Technique1"];

            effect.Begin();

            foreach (EffectPass p in effect.CurrentTechnique.Passes)
            {
                p.Begin();
                Game1.GetInstance.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(
                    PrimitiveType.TriangleList,
                    river_vertices, 0, (segement_vertex_count * segments) , river_indicies, 0, (segments * 6));
                p.End();
            }

            effect.End();

            Game1.GraphicsDevice.RenderState.AlphaBlendEnable = true;
            Game1.GraphicsDevice.RenderState.AlphaBlendEnable = true;
            Game1.GraphicsDevice.RenderState.SourceBlend = Blend.One;
            Game1.GraphicsDevice.RenderState.DestinationBlend = Blend.One;
            Game1.GraphicsDevice.RenderState.BlendFunction = BlendFunction.Add;

            effect.CurrentTechnique = effect.Techniques["Technique2"];
            effect.Begin();

            foreach (EffectPass p in effect.CurrentTechnique.Passes)
            {
                p.Begin();
                Game1.GetInstance.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList,
                    water_vertices, 0, 4, water_indicies, 0, 2);
                p.End();
            }

            effect.End();
            Game1.GraphicsDevice.RenderState.AlphaBlendEnable = false;
        }

        public float LeftSideBump(float pos)
        {
            return 0.25f * ((float)Math.Sin((double)pos * 1.5));
        }
        public float RightSideBump(float pos)
        {
            return 0.25f * ((float)Math.Cos((double)pos));
        }

        public void CreateSegement(int index)
        {
            float next_step = (index / segement_vertex_count) - 1;
            float prev_step = index / segement_vertex_count;
            float side_depth = 2.0f;

            //Left bank
            river_vertices[0 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(-2 + LeftSideBump(prev_step), 0, prev_step + RightSideBump(prev_step)),
                new Microsoft.Xna.Framework.Vector3(0, 1, 0), new Microsoft.Xna.Framework.Vector2(0, 0));
            river_vertices[1 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(-2 + LeftSideBump(next_step), 0, next_step + RightSideBump(next_step)),
                new Microsoft.Xna.Framework.Vector3(0, 1, 0), new Microsoft.Xna.Framework.Vector2(0, 1));
            river_vertices[2 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(-1 + LeftSideBump(next_step), -side_depth, next_step + RightSideBump(next_step)),
                new Microsoft.Xna.Framework.Vector3(0, 1, 0), new Microsoft.Xna.Framework.Vector2(.33f, 1));

            river_vertices[3 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(-2 + LeftSideBump(prev_step), 0, prev_step + RightSideBump(prev_step)),
                new Microsoft.Xna.Framework.Vector3(0, 1, 0), new Microsoft.Xna.Framework.Vector2(0, 0));
            river_vertices[4 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(-1 + LeftSideBump(next_step), -side_depth, next_step + RightSideBump(next_step)),
                new Microsoft.Xna.Framework.Vector3(0, 1, 0), new Microsoft.Xna.Framework.Vector2(.33f, 1));
            river_vertices[5 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(-1 + LeftSideBump(prev_step), -side_depth, prev_step + RightSideBump(prev_step)),
                new Microsoft.Xna.Framework.Vector3(0, 1, 0), new Microsoft.Xna.Framework.Vector2(.33f, 0));

            //Bottom
            river_vertices[6 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(-1 + LeftSideBump(prev_step), -side_depth, prev_step + RightSideBump(prev_step)),
                new Microsoft.Xna.Framework.Vector3(0, 1, 0), new Microsoft.Xna.Framework.Vector2(0.33f, 0));
            river_vertices[7 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(-1 + LeftSideBump(next_step), -side_depth, next_step + RightSideBump(next_step)),
                new Microsoft.Xna.Framework.Vector3(0, 1, 0), new Microsoft.Xna.Framework.Vector2(0.33f, 1));
            river_vertices[8 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(1 + LeftSideBump(next_step), -side_depth, next_step + RightSideBump(next_step)),
                new Microsoft.Xna.Framework.Vector3(0, 1, 0), new Microsoft.Xna.Framework.Vector2(.66f, 1));

            river_vertices[9 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(-1 + LeftSideBump(prev_step), -side_depth, prev_step + RightSideBump(prev_step)),
                new Microsoft.Xna.Framework.Vector3(0, 1, 0), new Microsoft.Xna.Framework.Vector2(0.33f, 0));
            river_vertices[10 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(1 + LeftSideBump(next_step), -side_depth, next_step + RightSideBump(next_step)),
                new Microsoft.Xna.Framework.Vector3(0, 1, 0), new Microsoft.Xna.Framework.Vector2(.66f, 1));
            river_vertices[11 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(1 + LeftSideBump(prev_step), -side_depth, prev_step + RightSideBump(prev_step)),
                new Microsoft.Xna.Framework.Vector3(0, 1, 0), new Microsoft.Xna.Framework.Vector2(.66f, 0));

            //Right bank
            river_vertices[12 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(1 + LeftSideBump(prev_step), -side_depth, prev_step + RightSideBump(prev_step)),
                new Microsoft.Xna.Framework.Vector3(0, 1, 0), new Microsoft.Xna.Framework.Vector2(0.33f, 0));
            river_vertices[13 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(1 + LeftSideBump(next_step), -side_depth, next_step + RightSideBump(next_step)),
                new Microsoft.Xna.Framework.Vector3(0, 1, 0), new Microsoft.Xna.Framework.Vector2(0.33f, 1));
            river_vertices[14 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(2 + LeftSideBump(next_step), 0, next_step + RightSideBump(next_step)),
                new Microsoft.Xna.Framework.Vector3(0, 1, 0), new Microsoft.Xna.Framework.Vector2(.66f, 1));

            river_vertices[15 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(1 + LeftSideBump(prev_step), -side_depth, prev_step + RightSideBump(prev_step)),
                new Microsoft.Xna.Framework.Vector3(0, 1, 0), new Microsoft.Xna.Framework.Vector2(0.33f, 0));
            river_vertices[16 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(2 + LeftSideBump(next_step), 0, next_step + RightSideBump(next_step)),
                new Microsoft.Xna.Framework.Vector3(0, 1, 0), new Microsoft.Xna.Framework.Vector2(.66f, 1));
            river_vertices[17 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(2 + LeftSideBump(prev_step), 0, prev_step + RightSideBump(prev_step)),
                new Microsoft.Xna.Framework.Vector3(0, 1, 0), new Microsoft.Xna.Framework.Vector2(.66f, 0));
        }

        public void Create()
        {
            riverfloor = Game1.GetInstance.Content.Load<Texture2D>("Riverfloor");
            vd = new VertexDeclaration(Game1.GraphicsDevice, VertexPositionNormalTexture.VertexElements);
            effect = Game1.GetInstance.Content.Load<Effect>("River");

            river_indicies = new short[segement_vertex_count * segments];
            river_vertices = new VertexPositionNormalTexture[segement_vertex_count * segments];

            for (int i = 0; i < segement_vertex_count * segments; i += segement_vertex_count)
            {
                for(int j = 0; j < segement_vertex_count; j++)
                    river_indicies[j + i] = (short)(i + j);

                CreateSegement(i);
            }

            water_indicies = new short[6];
            water_indicies[0] = 0;
            water_indicies[1] = 1;
            water_indicies[2] = 2;
            water_indicies[3] = 0;
            water_indicies[4] = 2;
            water_indicies[5] = 3;

            float water_depth = -0.2f;
            
            water_vertices = new VertexPositionNormalTexture[4];
            water_vertices[0] = new VertexPositionNormalTexture(new Vector3(-3, water_depth, -segments), Vector3.Up, new Vector2(0, 0));
            water_vertices[1] = new VertexPositionNormalTexture(new Vector3(-3, water_depth, segments), Vector3.Up, new Vector2(0, -1));
            water_vertices[2] = new VertexPositionNormalTexture(new Vector3(3, water_depth, segments), Vector3.Up, new Vector2(1, 1));
            water_vertices[3] = new VertexPositionNormalTexture(new Vector3(3, water_depth, -segments), Vector3.Up, new Vector2(1, 0));

        }

        public void Release()
        {
            effect = null;
            river_vertices = null;
        }
    }
}

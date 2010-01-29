using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GGJ_Deceive
{
    public class Tree
    {
        Texture2D texture_a;
        Texture2D texture_b;
        VertexPositionNormalTexture[] vertices = null;
        short[] indicies = null;


        public void Draw(Effect sceneEffect, Matrix transform, int texture)
        {
            sceneEffect.Parameters["TreeTexture"].SetValue(texture == 0 ? texture_a : texture_b);
            sceneEffect.Parameters["View"].SetValue(Game1.View);
            sceneEffect.Parameters["World"].SetValue(transform);
            sceneEffect.Parameters["Projection"].SetValue(Game1.Projection);

            sceneEffect.CurrentTechnique = sceneEffect.Techniques["Technique3"];

            sceneEffect.Begin();

            Game1.GraphicsDevice.RenderState.AlphaTestEnable = true;
            foreach (EffectPass p in sceneEffect.CurrentTechnique.Passes)
            {
                p.Begin();
                Game1.GetInstance.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(
                    PrimitiveType.TriangleList,
                    vertices, 0, 4, indicies, 0, 2);
                p.End();
            }

            sceneEffect.End();
            Game1.GraphicsDevice.RenderState.AlphaTestEnable = false;
        }

        public void Create()
        {
            texture_a = Game1.GetInstance.Content.Load<Texture2D>("Tree1");
            texture_b = Game1.GetInstance.Content.Load<Texture2D>("Tree2");
 
            indicies = new short[6];
            indicies[0] = 0;
            indicies[1] = 1;
            indicies[2] = 2;
            indicies[3] = 0;
            indicies[4] = 2;
            indicies[5] = 3;

            vertices = new VertexPositionNormalTexture[4];
            vertices[0] = new VertexPositionNormalTexture(new Vector3(-1, 0, 0), Vector3.Forward, new Vector2(0, 0));
            vertices[1] = new VertexPositionNormalTexture(new Vector3(-1, 4, 0), Vector3.Forward, new Vector2(0, 1));
            vertices[2] = new VertexPositionNormalTexture(new Vector3(1, 4, 0), Vector3.Forward, new Vector2(1, 1));
            vertices[3] = new VertexPositionNormalTexture(new Vector3(1, 0, 0), Vector3.Forward, new Vector2(1, 0));

        }
    }

    public class River
    {
        static float RiverSpeed = 0.1f;
        static float RiverOffset = 0f;
        List<Matrix> treeInstances = new List<Matrix>();
        const int tree_count = 10;
        Tree riverTree;
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

        Vector3 NextTreeStart()
        {
            float side_pos = (float)((new Random().NextDouble() > 0.5) ?
            new Random().NextDouble() + 3 : -new Random().NextDouble() - 3);
            return new Vector3(side_pos, 0, 0);
        }

        public void Update()
        {
            RiverOffset += RiverSpeed;

            for (int i = 0; i < treeInstances.Count; i++)
            {
                Matrix t = treeInstances[i];
                if (t.Translation.Z > 10f) {
                    
                    t.Translation = NextTreeStart();
                }
                else
                    t.Translation = t.Translation + new Vector3(0, 0, RiverSpeed);

                treeInstances[i] = t;
            }

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

            int i = 0;
            foreach(Matrix t in treeInstances)
                riverTree.Draw(effect, t, i++ % 3);

            //NOW DRAW WATER

            effect.Parameters["World"].SetValue(Matrix.Identity);
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
            return 0.25f * ((float)Math.Sin((double)pos * 0.5) + (float)Math.Cos((double)-pos * 7));
        }
        public float RightSideBump(float pos)
        {
            return 0.25f * ((float)Math.Cos((double)pos) + (float)Math.Sin((double)pos*4));
        }
        public float BottomSideBump(float pos)
        {
            return 0.25f * ((float)Math.Cos((double)pos));
        }

        public void CreateSegement(int index)
        {
            float offset = (RiverOffset % segments);
            float next_step = offset + ((index / segement_vertex_count) - 1);
            float prev_step = offset + (index / segement_vertex_count);
            float side_depth = 2.0f;

            if (next_step > 20)
            {

                prev_step -= segments;
                next_step -= segments;
            }

            float prev_idx = MathHelper.Pi * 2f * (float)((index / segement_vertex_count) + 1) / (float)segments;
            float next_idx = MathHelper.Pi * 2f * (float)((index / segement_vertex_count)) / (float)segments;

            //Left bank
            river_vertices[0 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(-2 + LeftSideBump(prev_idx), 0 + BottomSideBump(prev_idx), prev_step + RightSideBump(prev_idx)),
                new Microsoft.Xna.Framework.Vector3(0, 1, 0), new Microsoft.Xna.Framework.Vector2(0, 0));
            river_vertices[1 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(-2 + LeftSideBump(next_idx), 0 + BottomSideBump(next_idx), next_step + RightSideBump(next_idx)),
                new Microsoft.Xna.Framework.Vector3(0, 1, 0), new Microsoft.Xna.Framework.Vector2(0, 1));
            river_vertices[2 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(-1 + LeftSideBump(next_idx), -side_depth + BottomSideBump(next_idx), next_step + RightSideBump(next_idx)),
                new Microsoft.Xna.Framework.Vector3(0, 1, 0), new Microsoft.Xna.Framework.Vector2(.33f, 1));

            river_vertices[3 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(-2 + LeftSideBump(prev_idx), 0 + BottomSideBump(prev_idx), prev_step + RightSideBump(prev_idx)),
                new Microsoft.Xna.Framework.Vector3(0, 1, 0), new Microsoft.Xna.Framework.Vector2(0, 0));
            river_vertices[4 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(-1 + LeftSideBump(next_idx), -side_depth + BottomSideBump(next_idx), next_step + RightSideBump(next_idx)),
                new Microsoft.Xna.Framework.Vector3(0, 1, 0), new Microsoft.Xna.Framework.Vector2(.33f, 1));
            river_vertices[5 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(-1 + LeftSideBump(prev_idx), -side_depth + BottomSideBump(prev_idx), prev_step + RightSideBump(prev_idx)),
                new Microsoft.Xna.Framework.Vector3(0, 1, 0), new Microsoft.Xna.Framework.Vector2(.33f, 0));

            //Bottom
            river_vertices[6 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(-1 + LeftSideBump(prev_idx), -side_depth + BottomSideBump(prev_idx), prev_step + RightSideBump(prev_idx)),
                new Microsoft.Xna.Framework.Vector3(0, 1, 0), new Microsoft.Xna.Framework.Vector2(0.33f, 0));
            river_vertices[7 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(-1 + LeftSideBump(next_idx), -side_depth + BottomSideBump(next_idx), next_step + RightSideBump(next_idx)),
                new Microsoft.Xna.Framework.Vector3(0, 1, 0), new Microsoft.Xna.Framework.Vector2(0.33f, 1));
            river_vertices[8 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(1 + LeftSideBump(next_idx), -side_depth + BottomSideBump(next_idx), next_step + RightSideBump(next_idx)),
                new Microsoft.Xna.Framework.Vector3(0, 1, 0), new Microsoft.Xna.Framework.Vector2(.66f, 1));

            river_vertices[9 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(-1 + LeftSideBump(prev_idx), -side_depth + BottomSideBump(prev_idx), prev_step + RightSideBump(prev_idx)),
                new Microsoft.Xna.Framework.Vector3(0, 1, 0), new Microsoft.Xna.Framework.Vector2(0.33f, 0));
            river_vertices[10 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(1 + LeftSideBump(next_idx), -side_depth + BottomSideBump(next_idx), next_step + RightSideBump(next_idx)),
                new Microsoft.Xna.Framework.Vector3(0, 1, 0), new Microsoft.Xna.Framework.Vector2(.66f, 1));
            river_vertices[11 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(1 + LeftSideBump(prev_idx), -side_depth + BottomSideBump(prev_idx), prev_step + RightSideBump(prev_idx)),
                new Microsoft.Xna.Framework.Vector3(0, 1, 0), new Microsoft.Xna.Framework.Vector2(.66f, 0));

            //Right bank
            river_vertices[12 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(1 + LeftSideBump(prev_idx), -side_depth + BottomSideBump(prev_idx), prev_step + RightSideBump(prev_idx)),
                new Microsoft.Xna.Framework.Vector3(0, 1, 0), new Microsoft.Xna.Framework.Vector2(0.33f, 0));
            river_vertices[13 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(1 + LeftSideBump(next_idx), -side_depth + BottomSideBump(next_idx), next_step + RightSideBump(next_idx)),
                new Microsoft.Xna.Framework.Vector3(0, 1, 0), new Microsoft.Xna.Framework.Vector2(0.33f, 1));
            river_vertices[14 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(2 + LeftSideBump(next_idx), 0 + BottomSideBump(next_idx), next_step + RightSideBump(next_idx)),
                new Microsoft.Xna.Framework.Vector3(0, 1, 0), new Microsoft.Xna.Framework.Vector2(.66f, 1));

            river_vertices[15 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(1 + LeftSideBump(prev_idx), -side_depth + BottomSideBump(prev_idx), prev_step + RightSideBump(prev_idx)),
                new Microsoft.Xna.Framework.Vector3(0, 1, 0), new Microsoft.Xna.Framework.Vector2(0.33f, 0));
            river_vertices[16 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(2 + LeftSideBump(next_idx), 0 + BottomSideBump(next_idx), next_step + RightSideBump(next_idx)),
                new Microsoft.Xna.Framework.Vector3(0, 1, 0), new Microsoft.Xna.Framework.Vector2(.66f, 1));
            river_vertices[17 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(2 + LeftSideBump(prev_idx), 0 + BottomSideBump(prev_idx), prev_step + RightSideBump(prev_idx)),
                new Microsoft.Xna.Framework.Vector3(0, 1, 0), new Microsoft.Xna.Framework.Vector2(.66f, 0));
        }

        public void Create()
        {
            riverTree = new Tree();
            riverTree.Create();

            for (int i = 0; i < tree_count; i++)
            {
                Matrix scale = Matrix.CreateScale(new Vector3((float)(new Random().NextDouble() * 3 + 0.1)));
                Matrix t = Matrix.CreateTranslation(NextTreeStart() + new Vector3(0, 0, 10 * (float)i / (float)tree_count));
                treeInstances.Add(t*scale);
            }
                

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
            riverTree = null;
            effect = null;
            river_vertices = null;
        }
    }
}

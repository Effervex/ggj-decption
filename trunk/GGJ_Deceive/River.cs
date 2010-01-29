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
        Texture2D texture_c;
        VertexPositionNormalTexture[] vertices = null;
        short[] indicies = null;


        public void Draw(Effect sceneEffect, Matrix transform, int texture)
        {
            sceneEffect.Parameters["TreeTexture"].SetValue(texture == 0 ? texture_a : texture == 1 ? texture_b : texture_c);
            sceneEffect.Parameters["World"].SetValue(transform);

            Game1.GraphicsDevice.RenderState.AlphaTestEnable = true;
            Game1.GraphicsDevice.RenderState.AlphaFunction = CompareFunction.Greater;

            Game1.GraphicsDevice.RenderState.ReferenceAlpha = 122;
            sceneEffect.CurrentTechnique = sceneEffect.Techniques["Technique3"];

            sceneEffect.Begin();

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
            texture_c = Game1.GetInstance.Content.Load<Texture2D>("Tree3");
 
            indicies = new short[6];
            indicies[0] = 0;
            indicies[1] = 1;
            indicies[2] = 2;
            indicies[3] = 0;
            indicies[4] = 2;
            indicies[5] = 3;

            vertices = new VertexPositionNormalTexture[4];
            vertices[0] = new VertexPositionNormalTexture(new Vector3(-3, 0, 0), Vector3.Forward, new Vector2(0, 0));
            vertices[1] = new VertexPositionNormalTexture(new Vector3(-3, 8, 0), Vector3.Forward, new Vector2(0, 1));
            vertices[2] = new VertexPositionNormalTexture(new Vector3(3, 8, 0), Vector3.Forward, new Vector2(1, 1));
            vertices[3] = new VertexPositionNormalTexture(new Vector3(3, 0, 0), Vector3.Forward, new Vector2(1, 0));

        }
    }

    public class River
    {
        public const float BOTTOM = -2;
        public const float BOTTOM_WIDTH = 2;
        public const float TOP_WIDTH = 4;
        public const float BOUNDARY_BUFFER = 0.1f;
        static float RiverSpeed = 0.01f;
        static float RiverOffset = 0f;
        List<Matrix> treeInstances = new List<Matrix>();
        const int tree_count = 10;
        Tree riverTree;
        Texture2D riverfloor;
        Texture2D riverwater;

        VertexPositionNormalTexture[] water_vertices = null;
        VertexPositionNormalTexture[] river_vertices = null;
        VertexPositionNormalTexture[] background_vertices = null;

        short[] background_indicies = null;
        short[] river_indicies = null;
        short[] water_indicies = null;
        Effect effect;
        const int segement_vertex_count = 2 * 3 * 3;
        public const int segments = 20;

        Vector3 NextTreeStart()
        {
            float side_pos = (float)((new Random().NextDouble() > 0.5) ?
            new Random().NextDouble() + 3 : -new Random().NextDouble() - 3);
            return new Vector3(side_pos, (float)(new Random().NextDouble() - 0.5) * 2f, 0);
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

        public void DrawRefracted()
        {
            effect.Parameters["World"].SetValue(Matrix.Identity);
            effect.Parameters["View"].SetValue(Game1.View);
            effect.Parameters["Projection"].SetValue(Game1.Projection);

            effect.CurrentTechnique = effect.Techniques["Technique4"];
            effect.Begin();

            foreach (EffectPass p in effect.CurrentTechnique.Passes)
            {
                p.Begin();
                Game1.GetInstance.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(
                    PrimitiveType.TriangleList,
                    background_vertices, 0, 4, background_indicies, 0, 2);
                p.End();
            }

            effect.End();



            effect.CurrentTechnique = effect.Techniques["Technique1"];
            effect.Parameters["riverOffset"].SetValue(RiverOffset);
            effect.Parameters["FloorTexture"].SetValue(riverfloor);


            effect.Begin();

            foreach (EffectPass p in effect.CurrentTechnique.Passes)
            {
                p.Begin();
                Game1.GetInstance.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(
                    PrimitiveType.TriangleList,
                    river_vertices, 0, (segement_vertex_count * segments), river_indicies, 0, (segments * 6));
                p.End();
            }

            effect.End();

            int i = 0;
            foreach (Matrix t in treeInstances)
                riverTree.Draw(effect, t, i++ % 5);

        }

        public void Draw()
        {
            

            
            //NOW DRAW WATER

            effect.Parameters["World"].SetValue(Matrix.Identity);
            effect.Parameters["SceneTexture"].SetValue(Game1.refractBuffer.GetTexture());
            effect.Parameters["FloorTexture"].SetValue(riverwater);
            Game1.GraphicsDevice.RenderState.AlphaBlendEnable = true;
            Game1.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
            Game1.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
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

            //draw trees again becase they pass thru the water
            int i = 0;
            foreach (Matrix t in treeInstances)
                riverTree.Draw(effect, t, i++ % 5);

            Game1.GraphicsDevice.RenderState.DepthBufferWriteEnable = true;
           // Game1.GraphicsDevice.DepthStencilBuffer = old;
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
            return 0.125f * ((float)Math.Cos((double)pos*2) + (float)Math.Sin((double)pos * .5));
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
            river_vertices[0 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(-TOP_WIDTH / 2 + LeftSideBump(prev_idx), 0 + BottomSideBump(prev_idx), prev_step + RightSideBump(prev_idx)),
                new Microsoft.Xna.Framework.Vector3(0, 1, 0), new Microsoft.Xna.Framework.Vector2(0, 0));
            river_vertices[1 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(-TOP_WIDTH / 2 + LeftSideBump(next_idx), 0 + BottomSideBump(next_idx), next_step + RightSideBump(next_idx)),
                new Microsoft.Xna.Framework.Vector3(0, 1, 0), new Microsoft.Xna.Framework.Vector2(0, 1));
            river_vertices[2 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(-BOTTOM_WIDTH / 2 +LeftSideBump(next_idx), -side_depth + BottomSideBump(next_idx), next_step + RightSideBump(next_idx)),
                new Microsoft.Xna.Framework.Vector3(0, 1, 0), new Microsoft.Xna.Framework.Vector2(.33f, 1));

            river_vertices[3 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(-TOP_WIDTH / 2 + LeftSideBump(prev_idx), 0 + BottomSideBump(prev_idx), prev_step + RightSideBump(prev_idx)),
                new Microsoft.Xna.Framework.Vector3(0, 1, 0), new Microsoft.Xna.Framework.Vector2(0, 0));
            river_vertices[4 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(-BOTTOM_WIDTH / 2 +LeftSideBump(next_idx), -side_depth + BottomSideBump(next_idx), next_step + RightSideBump(next_idx)),
                new Microsoft.Xna.Framework.Vector3(0, 1, 0), new Microsoft.Xna.Framework.Vector2(.33f, 1));
            river_vertices[5 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(-BOTTOM_WIDTH / 2 +LeftSideBump(prev_idx), -side_depth + BottomSideBump(prev_idx), prev_step + RightSideBump(prev_idx)),
                new Microsoft.Xna.Framework.Vector3(0, 1, 0), new Microsoft.Xna.Framework.Vector2(.33f, 0));

            //Bottom
            river_vertices[6 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(-BOTTOM_WIDTH / 2 +LeftSideBump(prev_idx), -side_depth + BottomSideBump(prev_idx), prev_step + RightSideBump(prev_idx)),
                new Microsoft.Xna.Framework.Vector3(0, 1, 0), new Microsoft.Xna.Framework.Vector2(0.33f, 0));
            river_vertices[7 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(-BOTTOM_WIDTH / 2 +LeftSideBump(next_idx), -side_depth + BottomSideBump(next_idx), next_step + RightSideBump(next_idx)),
                new Microsoft.Xna.Framework.Vector3(0, 1, 0), new Microsoft.Xna.Framework.Vector2(0.33f, 1));
            river_vertices[8 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(BOTTOM_WIDTH / 2 +LeftSideBump(next_idx), -side_depth + BottomSideBump(next_idx), next_step + RightSideBump(next_idx)),
                new Microsoft.Xna.Framework.Vector3(0, 1, 0), new Microsoft.Xna.Framework.Vector2(.66f, 1));

            river_vertices[9 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(-BOTTOM_WIDTH / 2 +LeftSideBump(prev_idx), -side_depth + BottomSideBump(prev_idx), prev_step + RightSideBump(prev_idx)),
                new Microsoft.Xna.Framework.Vector3(0, 1, 0), new Microsoft.Xna.Framework.Vector2(0.33f, 0));
            river_vertices[10 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(BOTTOM_WIDTH / 2 +LeftSideBump(next_idx), -side_depth + BottomSideBump(next_idx), next_step + RightSideBump(next_idx)),
                new Microsoft.Xna.Framework.Vector3(0, 1, 0), new Microsoft.Xna.Framework.Vector2(.66f, 1));
            river_vertices[11 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(BOTTOM_WIDTH / 2 +LeftSideBump(prev_idx), -side_depth + BottomSideBump(prev_idx), prev_step + RightSideBump(prev_idx)),
                new Microsoft.Xna.Framework.Vector3(0, 1, 0), new Microsoft.Xna.Framework.Vector2(.66f, 0));

            //Right bank
            river_vertices[12 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(BOTTOM_WIDTH / 2 +LeftSideBump(prev_idx), -side_depth + BottomSideBump(prev_idx), prev_step + RightSideBump(prev_idx)),
                new Microsoft.Xna.Framework.Vector3(0, 1, 0), new Microsoft.Xna.Framework.Vector2(0.33f, 0));
            river_vertices[13 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(BOTTOM_WIDTH / 2 +LeftSideBump(next_idx), -side_depth + BottomSideBump(next_idx), next_step + RightSideBump(next_idx)),
                new Microsoft.Xna.Framework.Vector3(0, 1, 0), new Microsoft.Xna.Framework.Vector2(0.33f, 1));
            river_vertices[14 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(TOP_WIDTH / 2 + LeftSideBump(next_idx), 0 + BottomSideBump(next_idx), next_step + RightSideBump(next_idx)),
                new Microsoft.Xna.Framework.Vector3(0, 1, 0), new Microsoft.Xna.Framework.Vector2(.66f, 1));

            river_vertices[15 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(BOTTOM_WIDTH / 2 +LeftSideBump(prev_idx), -side_depth + BottomSideBump(prev_idx), prev_step + RightSideBump(prev_idx)),
                new Microsoft.Xna.Framework.Vector3(0, 1, 0), new Microsoft.Xna.Framework.Vector2(0.33f, 0));
            river_vertices[16 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(TOP_WIDTH / 2 + LeftSideBump(next_idx), 0 + BottomSideBump(next_idx), next_step + RightSideBump(next_idx)),
                new Microsoft.Xna.Framework.Vector3(0, 1, 0), new Microsoft.Xna.Framework.Vector2(.66f, 1));
            river_vertices[17 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(TOP_WIDTH / 2 + LeftSideBump(prev_idx), 0 + BottomSideBump(prev_idx), prev_step + RightSideBump(prev_idx)),
                new Microsoft.Xna.Framework.Vector3(0, 1, 0), new Microsoft.Xna.Framework.Vector2(.66f, 0));
        }

        public void Create()
        {
            riverTree = new Tree();
            riverTree.Create();

            for (int i = 0; i < tree_count; i++)
            {
                Matrix t = Matrix.CreateTranslation(new Vector3((float)(new Random().Next() - 0.5f * 3.0f), 0, 10 * (float)i / (float)tree_count));
                treeInstances.Add(t);
            }

            riverwater = Game1.GetInstance.Content.Load<Texture2D>("Water");
            riverfloor = Game1.GetInstance.Content.Load<Texture2D>("Riverfloor");
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
            water_vertices[1] = new VertexPositionNormalTexture(new Vector3(-3, water_depth, segments), Vector3.Up, new Vector2(0, segments));
            water_vertices[2] = new VertexPositionNormalTexture(new Vector3(3, water_depth, segments), Vector3.Up, new Vector2(1, segments));
            water_vertices[3] = new VertexPositionNormalTexture(new Vector3(3, water_depth, -segments), Vector3.Up, new Vector2(1, 0));

            background_indicies = new short[6];
            background_indicies[0] = 0;
            background_indicies[1] = 1;
            background_indicies[2] = 2;
            background_indicies[3] = 0;
            background_indicies[4] = 2;
            background_indicies[5] = 3;

            background_vertices = new VertexPositionNormalTexture[4];
            background_vertices[0] = new VertexPositionNormalTexture(new Vector3(-25, -5, -5), Vector3.Zero, new Vector2(0, 0));
            background_vertices[1] = new VertexPositionNormalTexture(new Vector3(-25, 15, -5), Vector3.Zero, new Vector2(0, 1));
            background_vertices[2] = new VertexPositionNormalTexture(new Vector3(25, 15, -5), Vector3.Zero, new Vector2(1, 1));
            background_vertices[3] = new VertexPositionNormalTexture(new Vector3(25, -5, -5), Vector3.Zero, new Vector2(1, 0));

        }

        public void Release()
        {
            riverTree = null;
            effect = null;
            river_vertices = null;
        }

        public static void BoundValues(Vector2 position)
        {
            position.Y = Math.Min(position.Y, -BOUNDARY_BUFFER);
            position.Y = Math.Max(position.Y, BOTTOM + BOUNDARY_BUFFER);
            float xWall = TOP_WIDTH / 2 + position.Y * (TOP_WIDTH - BOTTOM_WIDTH) / 2;
            position.X = Math.Min(position.X, xWall - BOUNDARY_BUFFER);
            position.X = Math.Max(position.X, -xWall + BOUNDARY_BUFFER);
        }

        public static Vector2 GenerateValidPos()
        {
            Random random = new Random();
            double y = -BOUNDARY_BUFFER - random.NextDouble() * (BOTTOM + 2 * BOUNDARY_BUFFER);
            double xWall = TOP_WIDTH / 2 + y * (TOP_WIDTH - BOTTOM_WIDTH) / 2;
            double x = random.NextDouble() * xWall;

            if (random.NextDouble() < 0.5)
                y *= -1;
            if (random.NextDouble() < 0.5)
                x *= -1;
            return new Vector2((float) x, (float) y);
        }
    }
}

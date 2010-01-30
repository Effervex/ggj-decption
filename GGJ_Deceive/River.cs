using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GGJ_Deceive
{
    public class Debry
    {
        static Texture2D texture_a;
        static Texture2D texture_b;
        static Texture2D texture_c;
        static Texture2D texture_d;
        
        Vector3 position;
        float size;
        float speed;
        int texture;

        public Debry()
        {
            size = 64f;
            speed = 1.0f;
            texture = new Random().Next() % 3;
            position = new Vector3((float)new Random().NextDouble() * -1.0f, (float)new Random().NextDouble() * -1.0f, -10);
            
            texture_a = Game1.GetInstance.Content.Load<Texture2D>("Debry1");
            texture_b = Game1.GetInstance.Content.Load<Texture2D>("Debry2");
            texture_c = Game1.GetInstance.Content.Load<Texture2D>("Debry3");
            texture_d = Game1.GetInstance.Content.Load<Texture2D>("Debry4");
        }

        public void Reset()
        {

               position.Z = (float)new Random().NextDouble() * 13.0f - 10.0f;
               position.X = ((float)new Random().NextDouble() - 0.5f) * 2f;
               position.Y = ((float)new Random().NextDouble() + .3f) * -1.5f;

               size = new Random().Next(12, 20);
               speed = (float)new Random().NextDouble() * 0.1f;

               if (new Random().NextDouble() > 0.65f)
                   texture = 2 + new Random().Next() % 2;
               else
                texture = new Random().Next() % 2;
        }

        public void Draw(Effect effect)
        {
            effect.CurrentTechnique = effect.Techniques["Debry"];

            Game1.GraphicsDevice.RenderState.AlphaBlendEnable = true;
            Game1.GraphicsDevice.RenderState.DepthBufferWriteEnable = false;
            Game1.GraphicsDevice.RenderState.PointSpriteEnable = true;
            

            switch (texture)
            {
                case 0:
                    effect.Parameters["DebryTexture"].SetValue(texture_a);
                    break;
                case 1:
                    effect.Parameters["DebryTexture"].SetValue(texture_b);
                    break;
                case 3:
                    effect.Parameters["DebryTexture"].SetValue(texture_d);
                    break;
                default:
                case 2:
                    effect.Parameters["DebryTexture"].SetValue(texture_c);
                    break;
            }

            if (position.Z > 20 || position.Y >= 0.0f)
                Reset();
            else
            {

                position.Z += River.RiverSpeed + speed;
                position.X += 0.052f * ((float)Math.Sin(position.Z * 1.71f) - 0.25f);
                position.Y += 0.052f * ((float)Math.Cos(position.Z * 2.51f) - 0.25f) + 0.025f;
            }

            effect.Parameters["WVPMatrix"].SetValue(Matrix.CreateTranslation(position) * Game1.View * Game1.Projection);

            
            effect.CurrentTechnique = effect.Techniques["Debry"];

            effect.Begin();

            foreach (EffectPass p in effect.CurrentTechnique.Passes)
            {
                p.Begin();
                Game1.GetInstance.GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(
                    PrimitiveType.PointList, new VertexPositionNormalTexture[1] { new VertexPositionNormalTexture(Vector3.Zero, Vector3.UnitZ, new Vector2(size,0)) }, 0, 1);
                p.End();
            }

            effect.End();

            Game1.GraphicsDevice.RenderState.DepthBufferWriteEnable = true;
            Game1.GraphicsDevice.RenderState.AlphaBlendEnable = false;
            Game1.GraphicsDevice.RenderState.PointSpriteEnable = false;
        }

    }

    public class Tree
    {
        Texture2D texture_a;
        Texture2D texture_b;
        Texture2D texture_c;
        VertexPositionNormalTexture[] vertices = null;
        short[] indicies = null;


        public void Draw(Effect sceneEffect, Matrix transform, int texture, bool underwater)
        {
            sceneEffect.Parameters["TreeTexture"].SetValue(texture == 0 ? texture_a : texture == 1 ? texture_b : texture_c);
            sceneEffect.Parameters["World"].SetValue(transform);

            Game1.GraphicsDevice.RenderState.AlphaTestEnable = true;
            Game1.GraphicsDevice.RenderState.AlphaFunction = CompareFunction.Greater;

            Game1.GraphicsDevice.RenderState.ReferenceAlpha = 122;
            sceneEffect.Parameters["doFog"].SetValue(underwater);
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
        static public Texture2D caustics;
        List<Debry> debries = new List<Debry>();
        public const float BOTTOM = -2.5f;
        public const float BOTTOM_WIDTH = 3;
        public const float TOP_WIDTH = 6;
        public const float BOUNDARY_BUFFER = 0.1f;
        public static float RiverSpeed = 0.05f;
        public static float RiverOffset = 0f;
        public static Random random = new Random();
        List<Matrix> treeInstances = new List<Matrix>();
        const int tree_count = 100;
        Tree riverTree;
        Texture2D riverfloor;
        Texture2D background;
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

        Matrix NextTreeStart()
        {
            float side_pos = (float)((random.NextDouble() > 0.5) ?
            random.NextDouble() + 3 : -random.NextDouble() - 3);
            Vector3 t = new Vector3(side_pos, (float)(random.NextDouble() - 1.5) * 2f, -10);
            float s = (float)random.NextDouble() + .5f;
            return Matrix.CreateScale(new Vector3(s, s, s)) * Matrix.CreateTranslation(t);
        }



        public void Update()
        {
            time -= 0.001f;
            if(debries.Count < 40 && new Random().NextDouble() < 0.016)
                debries.Add(new Debry());
            RiverSpeed = Game1.snake.snakeVelocity_;

RiverOffset += RiverSpeed;

            for (int i = 0; i < treeInstances.Count; i++)
            {
                Matrix t = treeInstances[i];
                if (t.Translation.Z > 10f) {
                    
                    t = NextTreeStart();
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
        public static float time = 0.0f;
        public void DrawRefracted()
        {
            effect.Parameters["fogcolor"].SetValue(Game1.fog);
            effect.Parameters["World"].SetValue(Matrix.Identity);
            effect.Parameters["View"].SetValue(Game1.View);
            effect.Parameters["Projection"].SetValue(Game1.Projection);
            effect.Parameters["TreeTexture"].SetValue(background);
            effect.Parameters["time"].SetValue(time);
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
                riverTree.Draw(effect, t, i++ % 5, false);

        }

        public void Draw()
        {


            effect.Parameters["World"].SetValue(Matrix.Identity);
            effect.Parameters["SceneTexture"].SetValue(Game1.refractBuffer.GetTexture());
            effect.Parameters["FloorTexture"].SetValue(riverwater);
            effect.Parameters["CausticsTexture"].SetValue(caustics);
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

            Game1.GraphicsDevice.RenderState.DepthBufferWriteEnable = true;


            //draw trees again becase they pass thru the water
            int i = 0;
            foreach (Matrix t in treeInstances)
                riverTree.Draw(effect, t, i++ % 5, true);

            foreach (Debry d in debries)
                d.Draw(effect);

           
        }

        public float LeftSideBump(float pos)
        {
            return 0.2f * ((float)Math.Sin((double)pos * 0.5) + (float)Math.Cos((double)-pos * 7));
        }
        public float RightSideBump(float pos)
        {
            return 0.2f * ((float)Math.Cos((double)pos) + (float)Math.Sin((double)pos*4));
        }
        public float BottomSideBump(float pos)
        {
            return 0.2f * ((float)Math.Cos((double)pos*2) + (float)Math.Sin((double)pos * .5));
        }
        public Vector3 getNormal(Vector3 pos)
        {
            return Vector3.Normalize(new Vector3(pos.X, pos.Y, 0));
        }
        public void CreateSegement(int index)
        {
            float offset = (RiverOffset % segments);
            float next_step = offset + ((index / segement_vertex_count) - 1);
            float prev_step = offset + (index / segement_vertex_count);
            float side_depth = -BOTTOM;

            if (next_step > 20)
            {

                prev_step -= segments;
                next_step -= segments;
            }

            float prev_idx = MathHelper.Pi * 2f * (float)((index / segement_vertex_count) + 1) / (float)segments;
            float next_idx = MathHelper.Pi * 2f * (float)((index / segement_vertex_count)) / (float)segments;

            //Left bank
            river_vertices[0 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(-TOP_WIDTH / 2 + LeftSideBump(prev_idx), 
                0 + BottomSideBump(prev_idx), 
                prev_step + RightSideBump(prev_idx)),
                Vector3.Zero, new Microsoft.Xna.Framework.Vector2(0, 0));
            river_vertices[1 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(-TOP_WIDTH / 2 + LeftSideBump(next_idx),
                0 + BottomSideBump(next_idx), 
                next_step + RightSideBump(next_idx)),
                Vector3.Zero, new Microsoft.Xna.Framework.Vector2(0, 1));
            river_vertices[2 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(-BOTTOM_WIDTH / 2 +LeftSideBump(next_idx),
                -side_depth + BottomSideBump(next_idx * 2),
                next_step + RightSideBump(next_idx)),
                Vector3.Zero, new Microsoft.Xna.Framework.Vector2(.33f, 1));

            river_vertices[3 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(-TOP_WIDTH / 2 + LeftSideBump(prev_idx),
                0 + BottomSideBump(prev_idx), 
                prev_step + RightSideBump(prev_idx)),
                Vector3.Zero, new Microsoft.Xna.Framework.Vector2(0, 0));
            river_vertices[4 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(-BOTTOM_WIDTH / 2 +LeftSideBump(next_idx),
                -side_depth + BottomSideBump(next_idx * 2),
                next_step + RightSideBump(next_idx)),
               Vector3.Zero, new Microsoft.Xna.Framework.Vector2(.33f, 1));
            river_vertices[5 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(-BOTTOM_WIDTH / 2 +LeftSideBump(prev_idx),
                -side_depth + BottomSideBump(prev_idx * 2), 
                prev_step + RightSideBump(prev_idx)),
                Vector3.Zero, new Microsoft.Xna.Framework.Vector2(.33f, 0));

            //Bottom
            river_vertices[6 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(-BOTTOM_WIDTH / 2 +LeftSideBump(prev_idx),
                -side_depth + BottomSideBump(prev_idx * 2), prev_step + RightSideBump(prev_idx)),
                Vector3.Zero, new Microsoft.Xna.Framework.Vector2(0.33f, 0));
            river_vertices[7 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(-BOTTOM_WIDTH / 2 +LeftSideBump(next_idx),
                -side_depth + BottomSideBump(next_idx * 2), next_step + RightSideBump(next_idx)),
                Vector3.Zero, new Microsoft.Xna.Framework.Vector2(0.33f, 1));
            river_vertices[8 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(BOTTOM_WIDTH / 2 +LeftSideBump(next_idx),
                -side_depth + BottomSideBump(next_idx), next_step + RightSideBump(next_idx)),
                Vector3.Zero, new Microsoft.Xna.Framework.Vector2(.66f, 1));

            river_vertices[9 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(-BOTTOM_WIDTH / 2 +LeftSideBump(prev_idx),
                -side_depth + BottomSideBump(prev_idx * 2), prev_step + RightSideBump(prev_idx)),
                Vector3.Zero, new Microsoft.Xna.Framework.Vector2(0.33f, 0));
            river_vertices[10 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(BOTTOM_WIDTH / 2 +LeftSideBump(next_idx),
                -side_depth + BottomSideBump(next_idx), next_step + RightSideBump(next_idx)),
               Vector3.Zero, new Microsoft.Xna.Framework.Vector2(.66f, 1));
            river_vertices[11 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(BOTTOM_WIDTH / 2 +LeftSideBump(prev_idx),
                -side_depth + BottomSideBump(prev_idx), prev_step + RightSideBump(prev_idx)),
                Vector3.Zero, new Microsoft.Xna.Framework.Vector2(.66f, 0));

            //Right bank
            river_vertices[12 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(BOTTOM_WIDTH / 2 +LeftSideBump(prev_idx), -side_depth + BottomSideBump(prev_idx), prev_step + RightSideBump(prev_idx)),
              Vector3.Zero, new Microsoft.Xna.Framework.Vector2(0.66f, 0));
            river_vertices[13 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(BOTTOM_WIDTH / 2 +LeftSideBump(next_idx), -side_depth + BottomSideBump(next_idx), next_step + RightSideBump(next_idx)),
               Vector3.Zero, new Microsoft.Xna.Framework.Vector2(0.66f, 1));
            river_vertices[14 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(TOP_WIDTH / 2 + LeftSideBump(next_idx), 0 + BottomSideBump(next_idx), next_step + RightSideBump(next_idx)),
                Vector3.Zero, new Microsoft.Xna.Framework.Vector2(1f, 1));

            river_vertices[15 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(BOTTOM_WIDTH / 2 +LeftSideBump(prev_idx), -side_depth + BottomSideBump(prev_idx), prev_step + RightSideBump(prev_idx)),
                Vector3.Zero, new Microsoft.Xna.Framework.Vector2(0.66f, 0));
            river_vertices[16 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(TOP_WIDTH / 2 + LeftSideBump(next_idx), 0 + BottomSideBump(next_idx), next_step + RightSideBump(next_idx)),
              Vector3.Zero, new Microsoft.Xna.Framework.Vector2(1f, 1));
            river_vertices[17 + index] = new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(TOP_WIDTH / 2 + LeftSideBump(prev_idx), 0 + BottomSideBump(prev_idx), prev_step + RightSideBump(prev_idx)),
              Vector3.Zero, new Microsoft.Xna.Framework.Vector2(1f, 0));

            for (int z = index; z < index + 18; z++)
                river_vertices[z].Normal = getNormal(river_vertices[z].Position);
        }

        public void Create()
        {
            riverTree = new Tree();
            riverTree.Create();

            for (int i = 0; i < tree_count; i++)
            {
                Matrix t = Matrix.CreateTranslation(new Vector3((float)(random.Next() - 0.5f * 3.0f), 
                    0, 10 - i));
                treeInstances.Add(t);
            }

            riverwater = Game1.GetInstance.Content.Load<Texture2D>("Water");
            riverfloor = Game1.GetInstance.Content.Load<Texture2D>("Riverfloor");
            background = Game1.GetInstance.Content.Load<Texture2D>("background");
            caustics = Game1.GetInstance.Content.Load<Texture2D>("Caustics");
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
            background_vertices[0] = new VertexPositionNormalTexture(new Vector3(-35, -15, -3), Vector3.Zero, new Vector2(0, 1));
            background_vertices[1] = new VertexPositionNormalTexture(new Vector3(-35, 40, -3), Vector3.Zero, new Vector2(0, 0));
            background_vertices[2] = new VertexPositionNormalTexture(new Vector3(35, 40, -3), Vector3.Zero, new Vector2(1, 0));
            background_vertices[3] = new VertexPositionNormalTexture(new Vector3(35, -15, -3), Vector3.Zero, new Vector2(1, 1));

        }

        public void Release()
        {
            riverTree = null;
            effect = null;
            river_vertices = null;
        }

        public static Vector3 BoundValues(Vector3 position)
        {
            position.Y = Math.Min(position.Y, -BOUNDARY_BUFFER);
            position.Y = Math.Max(position.Y, BOTTOM + BOUNDARY_BUFFER);
            double xWall = BOTTOM_WIDTH / 2 - (position.Y - BOTTOM) * (TOP_WIDTH - BOTTOM_WIDTH) / (2 * BOTTOM) - BOUNDARY_BUFFER;
            position.X = (float) Math.Min(position.X, xWall - BOUNDARY_BUFFER);
            position.X = (float) Math.Max(position.X, -xWall + BOUNDARY_BUFFER);
            return position;
        }
       
        public static Vector2 GenerateValidPos()
        {
            double randVal = random.NextDouble();
            double y = -BOUNDARY_BUFFER + (randVal * (BOTTOM + (2 * BOUNDARY_BUFFER)));
            double xWall = BOTTOM_WIDTH / 2 - (y - BOTTOM) * (TOP_WIDTH - BOTTOM_WIDTH) / (2 * BOTTOM) - BOUNDARY_BUFFER;
            double x = random.NextDouble() * xWall;

            if (random.NextDouble() < 0.5)
                y *= -1;
            if (random.NextDouble() < 0.5)
                x *= -1;
            return new Vector2((float) x, (float) y);
        }
    }
}

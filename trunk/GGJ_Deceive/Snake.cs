using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace GGJ_Deceive
{
    public class Snake
    {
        public const float BODY_COEFFICIENT = 1f;
        public const float MAX_HEAD_SPEED = 0.05f;
        public const short VERTICES_PER_SEGMENT = 4;
        public static float INITIAL_BODY_THICKNESS = 0.1f;
        public const float LENGTH_BY_THICKNESS = 10;
        public const float VELOCITY_COEFFICIENT = 0.03f;
        public const float CURVE_COEFFICIENT = 0.15f;
        public const float IDLE_SNAKE_EPSILON = 0.1f;

        /** The deviance from the centre of the snake. */
        public Vector2[] snakeBody_;
        public float snakeVelocity_;
        public bool horizontalMovement_ = false;
        public bool verticalMovement_ = false;
        public Effect effect;
        public MouseState prevState_;
        public Texture2D scales;
        public VertexPositionNormalTexture[] vertices_;
        
        public static float healthPercent = 100;
        public static int cakeBatterCount = 0;

        public short[] indices_;

        public Snake(int initialSegments)
        {
            snakeBody_ = new Vector2[initialSegments];
            for (int i = 0; i < initialSegments; i++)
            {
                snakeBody_[i] = new Vector2(0);
            }
        }

        public void LoadContent(GraphicsDevice device)
        {
            SetUpVertices();
            SetUpIndices();
            scales = Game1.GetInstance.Content.Load<Texture2D>("Scales");
            effect = Game1.GetInstance.Content.Load<Effect>("Snake");
        }

        public void Initialise()
        {
            prevState_ = Mouse.GetState();
        }

        private void SetUpIndices()
        {
            // For the body
            indices_ = new short[snakeBody_.Length * 6 * VERTICES_PER_SEGMENT];

            for (int i = 1; i < snakeBody_.Length; i++)
            {
                int j = 0;
                for (int k = 0; k < VERTICES_PER_SEGMENT; k++)
                {
                    int nextK = (k + 1) % VERTICES_PER_SEGMENT;
                    int indicesPerFace = VERTICES_PER_SEGMENT * 6;
                    indices_[(i - 1) * indicesPerFace + j++] = (short)(i * VERTICES_PER_SEGMENT + k);
                    indices_[(i - 1) * indicesPerFace + j++] = (short)((i - 1) * VERTICES_PER_SEGMENT + nextK);
                    indices_[(i - 1) * indicesPerFace + j++] = (short)((i - 1) * VERTICES_PER_SEGMENT + k);

                    indices_[(i - 1) * indicesPerFace + j++] = (short)(i * VERTICES_PER_SEGMENT + k);
                    indices_[(i - 1) * indicesPerFace + j++] = (short)(i * VERTICES_PER_SEGMENT + nextK);
                    indices_[(i - 1) * indicesPerFace + j++] = (short)((i - 1) * VERTICES_PER_SEGMENT + nextK);
                }
            }
        }

        private void SetUpVertices()
        {
            vertices_ = new VertexPositionNormalTexture[snakeBody_.Length * VERTICES_PER_SEGMENT];

                // Form the head

                // Form the body segments
                updateVertices();
        }

        public void Update(GameTime gameTime, Rectangle windowSize)
        {
            calculateVelocity();
            moveBody(windowSize);
            updateVertices();

            prevState_ = Mouse.GetState();
        }

        //Vector3 random
        //{
        //    get
        //    {

        //        return new Vector3((float)new Random().NextDouble() - 0.5f,
        //            (float)new Random().NextDouble() - 0.5f,
        //            (float)new Random().NextDouble() - 0.5f);
        //    }
        //}

        private void updateVertices()
        {
            float segmentLength = LENGTH_BY_THICKNESS * INITIAL_BODY_THICKNESS / snakeBody_.Length;
            for (int i = 0; i < snakeBody_.Length; i++)
            {
                float stripe = (i % 2 == 0) ? 0 : 1;
                float alongFrac = MathHelper.Clamp(1f - (float)i / (float)snakeBody_.Length,0.1f,1.0f);
                
                float bodySize = (float) (INITIAL_BODY_THICKNESS * Math.Pow(Math.Log(snakeBody_.Length - i) / Math.Log(snakeBody_.Length), 1));
                vertices_[i * VERTICES_PER_SEGMENT] =
                    new VertexPositionNormalTexture(new Vector3(snakeBody_[i].X, snakeBody_[i].Y + bodySize / 2, i * segmentLength),
                        Vector3.UnitY,
                        new Vector2(alongFrac, stripe));
                vertices_[i * VERTICES_PER_SEGMENT + 1] =
                    new VertexPositionNormalTexture(new Vector3(snakeBody_[i].X - bodySize / 2, snakeBody_[i].Y, i * segmentLength),
                        -Vector3.UnitX,
                       new Vector2(alongFrac, stripe));
                vertices_[i * VERTICES_PER_SEGMENT + 2] =
                    new VertexPositionNormalTexture(new Vector3(snakeBody_[i].X, snakeBody_[i].Y - bodySize / 2f, i * segmentLength),
                        -Vector3.UnitY,
                        new Vector2(alongFrac, stripe));
                vertices_[i * VERTICES_PER_SEGMENT + 3] =
                    new VertexPositionNormalTexture(new Vector3(snakeBody_[i].X + bodySize / 2, snakeBody_[i].Y, i * segmentLength),
                       Vector3.UnitX,
                       new Vector2(alongFrac, stripe));
                bodySize -= 0.002f;
            }
        }

        private void calculateVelocity()
        {
            float absSum = 0;
            int curves = 0;
            float prevDiff = 0;
            float straightFactor = 1;
            for (int i = 1; i < snakeBody_.Length; i++)
            {
                float thisX = snakeBody_[i].X;
                float prevX = snakeBody_[i - 1].X;
                float diff = thisX - prevX;
                // If the curves go a different direction add a curve
                if (diff * prevDiff < 0)
                {
                    curves++;
                    straightFactor = 1 + CURVE_COEFFICIENT;
                }
                else
                {
                    straightFactor -= 2f / snakeBody_.Length;
                }

                absSum += Math.Abs(diff * straightFactor);
                prevDiff = diff;
            }

            // Modify the absolute sum
            //absSum = (float) Math.Pow(absSum + 1, 1 + curves * CURVE_COEFFICIENT) - 1;

            snakeVelocity_ = absSum * VELOCITY_COEFFICIENT;
        }

        private void moveBody(Rectangle windowSize)
        {
            // Move the head towards the mouse
            float relativeX = (float) (2.0 * Mouse.GetState().X / windowSize.Width - 1);
            float relativeY = (float) (2.0 * Mouse.GetState().Y / windowSize.Height - 1) * -1f;

            relativeX = Math.Max(relativeX, -1);
            relativeX = Math.Min(relativeX, 1);
            relativeY = Math.Max(relativeY, -1);
            relativeY = Math.Min(relativeY, 1);

            snakeBody_[0].X += relativeX * MAX_HEAD_SPEED;
            snakeBody_[0].Y += relativeY * snakeVelocity_;
            River.BoundValues(snakeBody_[0]);

            // Propagate changes in the snake head down the body
            for (int i = snakeBody_.Length - 1; i > 0; i--)
            {
                snakeBody_[i].X = snakeBody_[i - 1].X * BODY_COEFFICIENT;
                snakeBody_[i].Y = snakeBody_[i - 1].Y * BODY_COEFFICIENT;
            }
        }

        public void Draw(GameTime gameTime)
        {
            effect.CurrentTechnique = effect.Techniques["Technique1"];
            effect.Parameters["World"].SetValue(Matrix.CreateTranslation(new Vector3(0,-1.5f,9)));
            effect.Parameters["View"].SetValue(Game1.View);
            effect.Parameters["Projection"].SetValue(Game1.Projection);
            effect.Parameters["Env"].SetValue(River.caustics);
            effect.Parameters["ScalesTexture"].SetValue(scales);
            effect.Parameters["Time"].SetValue(River.time);

            effect.Begin();

            foreach (EffectPass p in effect.CurrentTechnique.Passes)
            {
                p.Begin();
                Game1.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, vertices_, 0, vertices_.Length, indices_, 0, indices_.Length / 3);
                p.End();
            }

            effect.End();
            Game1.GraphicsDevice.VertexDeclaration = Game1.vd;

        }
    }
}

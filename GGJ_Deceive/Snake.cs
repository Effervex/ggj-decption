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
        public const float BODY_COEFFICIENT = 0.95f;
        public const float MAX_HEAD_SPEED = 5;
        public const short VERTICES_PER_SEGMENT = 4;
        public const float INITIAL_BODY_THICKNESS = 1;
        public const float SEGMENT_LENGTH = 3;
        public const float ALPHA = 0.6f;
        public const float BETA = 0.1f;
        public const float IDLE_SNAKE_EPSILON = 0.1f;

        /** The deviance from the centre of the snake. */
        public Vector2[] snakeBody_;
        public Vector3 snakeVelocity_;

        public VertexPositionNormalTexture[] vertices_;
        public VertexBuffer vb_;

        public short[] indices_;
        public IndexBuffer ib_;

        public Snake(int initialSegments)
        {
            snakeBody_ = new Vector2[initialSegments];
            for (int i = 0; i < initialSegments; i++)
            {
                snakeBody_[i] = new Vector2();
            }
        }

        public void LoadContent(GraphicsDevice device)
        {
            SetUpVertices();
            SetUpIndices();

            CopyToBuffers(device);
        }

        public void Initialise()
        {
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
                    indices_[i * VERTICES_PER_SEGMENT + j++] = (short)(i * VERTICES_PER_SEGMENT + k);
                    indices_[i * VERTICES_PER_SEGMENT + j++] = (short)((i - 1) * VERTICES_PER_SEGMENT + nextK);
                    indices_[i * VERTICES_PER_SEGMENT + j++] = (short)((i - 1) * VERTICES_PER_SEGMENT + k);
                    indices_[i * VERTICES_PER_SEGMENT + j++] = (short)(i * VERTICES_PER_SEGMENT + k);
                    indices_[i * VERTICES_PER_SEGMENT + j++] = (short)(i * VERTICES_PER_SEGMENT + nextK);
                    indices_[i * VERTICES_PER_SEGMENT + j++] = (short)((i - 1) * VERTICES_PER_SEGMENT + k);
                }
            }
        }

        private void SetUpVertices()
        {
            vertices_ = new VertexPositionNormalTexture[snakeBody_.Length * VERTICES_PER_SEGMENT];

            // Form the head

            // Form the body segments
            float bodySize = INITIAL_BODY_THICKNESS;
            for (int i = 0; i < snakeBody_.Length; i++)
            {
                vertices_[i * VERTICES_PER_SEGMENT] =
                    new VertexPositionNormalTexture(new Vector3(snakeBody_[i].X, snakeBody_[i].Y + bodySize / 2, i * SEGMENT_LENGTH),
                        new Vector3(0, 1, 0), new Vector2(0, 0));
                vertices_[i * VERTICES_PER_SEGMENT + 1] =
                    new VertexPositionNormalTexture(new Vector3(snakeBody_[i].X - bodySize / 2, snakeBody_[i].Y, i * SEGMENT_LENGTH),
                        new Vector3(-1, 0, 0), new Vector2(0, 0));
                vertices_[i * VERTICES_PER_SEGMENT + 2] =
                    new VertexPositionNormalTexture(new Vector3(snakeBody_[i].X, snakeBody_[i].Y - bodySize / 2, i * SEGMENT_LENGTH),
                        new Vector3(0, -1, 0), new Vector2(0, 0));
                vertices_[i * VERTICES_PER_SEGMENT + 3] =
                    new VertexPositionNormalTexture(new Vector3(snakeBody_[i].X + bodySize / 2, snakeBody_[i].Y, i * SEGMENT_LENGTH),
                        new Vector3(1, 0, 0), new Vector2(0, 0));
            }
        }

        private void CopyToBuffers(GraphicsDevice device)
        {
            vb_ = new VertexBuffer(device, vertices_.Length * VertexPositionNormalTexture.SizeInBytes, BufferUsage.WriteOnly);
            vb_.SetData(vertices_);

            ib_ = new IndexBuffer(device, typeof(int), indices_.Length, BufferUsage.WriteOnly);
            ib_.SetData(indices_);
        }

        public void Update(GameTime gameTime, Rectangle windowSize)
        {
            moveBody(windowSize);
            calculateVelocity();
        }

        private void calculateVelocity()
        {
            float sumX = 0;
            float sumY = 0;
            float absSum = 0;
            for (int i = 0; i < snakeBody_.Length; i++)
            {
                sumX += snakeBody_[i].X;
                sumY += snakeBody_[i].Y;
                absSum += Math.Abs(snakeBody_[i].X);
            }

            snakeVelocity_.X += sumX * ALPHA;
            snakeVelocity_.Y += sumY * ALPHA;
            snakeVelocity_.Z -= absSum * BETA;
        }

        private void moveBody(Rectangle windowSize)
        {
            // Move the head towards the mouse
            float relativeX = Mouse.GetState().X - windowSize.Width / 2;
            float relativeY = Mouse.GetState().Y - windowSize.Width / 2;

            relativeX = Math.Max(relativeX, -MAX_HEAD_SPEED);
            relativeX = Math.Min(relativeX, MAX_HEAD_SPEED);
            relativeY = Math.Max(relativeY, -MAX_HEAD_SPEED);
            relativeY = Math.Min(relativeY, MAX_HEAD_SPEED);

            if (Math.Abs(relativeX) < (MAX_HEAD_SPEED * IDLE_SNAKE_EPSILON))
                relativeX = MAX_HEAD_SPEED * IDLE_SNAKE_EPSILON;

            snakeBody_[0].X += relativeX;
            snakeBody_[0].Y += relativeY;
            River.BoundValues(snakeBody_[0]);

            // Propagate changes in the snake head down the body
            for (int i = 1; i < snakeBody_.Length; i++)
            {
                snakeBody_[i].X += snakeBody_[i - 1].X * BODY_COEFFICIENT;
                snakeBody_[i].Y += snakeBody_[i - 1].Y * BODY_COEFFICIENT;
            }
        }

        public void Draw(GameTime gameTime)
        {
            
        }
    }
}

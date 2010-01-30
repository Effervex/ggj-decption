using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GGJ_Deceive
{
    public class Fish : Thing
    {
        // Model constants
        public const float FISH_HEIGHT = 0.3f;
        public const float FISH_WIDTH = 0.05f;
        public const float FISH_LENGTH = 0.3f;
        public const float TAIL_LENGTH = 0.05f;
        public const float TAIL_INDENT = 0.02f;
        public const float NOSE_PERCENT = 0.35f;

        public const float SNAKE_EATING_PERCENT = 0.1f;
        public const float MIN_VELOCITY = 0.001f;
        public const float VELOCITY_RANGE = 0.049f;

        public bool edible_ = true;

        override public void LoadContent()
        {
            base.LoadContent();


            float max = Math.Max(scale_.X, Math.Max(scale_.Y, scale_.Z));
            sphereSize_ = FISH_HEIGHT * max;
            velocity_ = generateVelocity();

            SetUpVertices();
            SetUpIndices();
        }

        private Vector3 generateVelocity()
        {
            return new Vector3(0, 0,
                (float)(MIN_VELOCITY + random_.NextDouble() * VELOCITY_RANGE));
        }

        override public void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            float sinVal = (float)Math.Sin(position_.Z * 10 + River.time * 200) * 0.5f + 0.5f;
            //position_.X += (float) (0.01 * sinVal);

            float sin = ((float)Math.Sin((River.time) * 200)) * 0.015f;
            vertices_[7].Position.X += sin;
            vertices_[9].Position.X += sin;
            vertices_[8].Position.X += sin;

                vertices_[1].Position.Y = sinVal * .1F;
                vertices_[4].Position.Y = -sinVal * .1F;
            
        }

        public override int DoesCollides(Snake snake)
        {
            int collideIndex = base.DoesCollides(snake);
            // If the fish collided with the first 10 percent of the snake
            if ((collideIndex != -1) && (collideIndex < SNAKE_EATING_PERCENT * snake.snakeBody_.Length))
            {
                if (edible_)
                {
                    Blood.AddBlood(position_, River.random.Next(35, 55));
                    // Insert eating code here.
                    Snake.cakeBatterCount++;
                    removeThis = true;
                }
            }
            return collideIndex;
        }

        override public void SetUpVertices()
        {
            vertices_ = new VertexPositionNormalTexture[10];

            // Mouth is initially closed
            // Top of head
            vertices_[0] = new VertexPositionNormalTexture(new Vector3(0, FISH_HEIGHT / 2, 0), new Vector3(0,1,0), new Vector2(0.5f,0));

            // Nose
            vertices_[1] = new VertexPositionNormalTexture(new Vector3(0, 0, FISH_LENGTH * NOSE_PERCENT), new Vector3(0,0,1), new Vector2(0,0.5f));

            // Jawline
            vertices_[2] = new VertexPositionNormalTexture(new Vector3(FISH_WIDTH / 2, 0, 0), new Vector3(), new Vector2(0.5f,0.5f)); // Left side
            vertices_[3] = new VertexPositionNormalTexture(new Vector3(-FISH_WIDTH / 2, 0, 0), new Vector3(), new Vector2(0.5f, 0.5f)); // Right side

            // Lower lip
            vertices_[4] = new VertexPositionNormalTexture(new Vector3(0, 0, FISH_LENGTH * NOSE_PERCENT), new Vector3(0, 0, 1), new Vector2(0, 0.5f));

            // Bottom of head
            vertices_[5] = new VertexPositionNormalTexture(new Vector3(0, -FISH_HEIGHT / 2, 0), new Vector3(0, -1, 0), new Vector2(0.5f, 1));

            // Tail joint
            vertices_[6] = new VertexPositionNormalTexture(new Vector3(0, 0, -FISH_LENGTH * (1 - NOSE_PERCENT)), new Vector3(0, 0, -1), new Vector2(1, 0.5f));

            // Top tail
            vertices_[7] = new VertexPositionNormalTexture(new Vector3(0, FISH_HEIGHT / 2, -FISH_LENGTH * (1 - NOSE_PERCENT) - TAIL_LENGTH), new Vector3(0,1,-1), new Vector2(1.5f,0));

            // Mid tail
            vertices_[8] = new VertexPositionNormalTexture(new Vector3(0, 0, -FISH_LENGTH * (1 - NOSE_PERCENT) - TAIL_INDENT), new Vector3(0,0,-1), new Vector2(1.5f,0.5f));

            // Bottom tail
            vertices_[9] = new VertexPositionNormalTexture(new Vector3(0, -FISH_HEIGHT / 2, -FISH_LENGTH * (1 - NOSE_PERCENT) - TAIL_LENGTH), new Vector3(0,-1,-1), new Vector2(1.5f,1));
        }

        override public void SetUpIndices()
        {
            // 14 faces
            int[] indices = {0,2,1,
                            0,1,3,
                            1,2,3,
                            3,2,4,
                            5,4,2,
                            5,3,4,
                            2,0,6,
                            5,2,6,
                            3,6,0,
                            5,6,3,
                            6,7,8,
                            6,8,7,
                            6,8,9,
                            6,9,8};

            indices_ = indices;
        }
    }
}

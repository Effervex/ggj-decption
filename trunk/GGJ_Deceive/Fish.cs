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
        public const float FISH_HEIGHT = 0.2f;
        public const float FISH_WIDTH = 0.066f;
        public const float FISH_LENGTH = 0.2f;
        public const float TAIL_LENGTH = 0.066f;
        public const float TAIL_INDENT = 0.04f;
        public const float NOSE_PERCENT = 0.066f;

        public const float MAX_WANDERLUST_CHANCE = 0.2f;
        public const float MAX_HEADING = 1.5f;
        public const float RATE_VELOCITY_CHANGE = 0.1f;
        public const float SNAKE_EATING_PERCENT = 0.1f;

        public Vector3 heading_;
        public float wandering_;

        override public void LoadContent()
        {
            base.LoadContent();

            sphereSize_ = FISH_HEIGHT * scale_;
            velocity_ = generateVelocity();
            heading_ = generateHeading();
            wandering_ = (float) (random_.NextDouble() * MAX_WANDERLUST_CHANCE);

            SetUpVertices();
            SetUpIndices();
        }

        private Vector3 generateVelocity()
        {
            return new Vector3(0,0,1f);
            //return new Vector3((float)(random_.NextDouble() - 0.5),
              //  (float)(random_.NextDouble() - 0.5),
                //(float)(random_.NextDouble()));
        }

        private Vector3 generateHeading()
        {
            return new Vector3(0.01f,0,0.1f);
            //return new Vector3((float)(random_.NextDouble() * MAX_HEADING - (2 * MAX_HEADING)),
              //  (float)(random_.NextDouble() * MAX_HEADING - (2 * MAX_HEADING)),
                //(float)(random_.NextDouble() * MAX_HEADING));
        }

        override public void Update()
        {
            base.Update();

            // Modify velocity to match heading
            if (velocity_.X < heading_.X)
                velocity_.X += RATE_VELOCITY_CHANGE;
            if (velocity_.X > heading_.X)
                velocity_.X -= RATE_VELOCITY_CHANGE;
            if (velocity_.Y < heading_.Y)
                velocity_.Y += RATE_VELOCITY_CHANGE;
            if (velocity_.Y > heading_.Y)
                velocity_.Y -= RATE_VELOCITY_CHANGE;
            if (velocity_.Z < heading_.Z)
                velocity_.Z += RATE_VELOCITY_CHANGE;
            if (velocity_.Z > heading_.Z)
                velocity_.Z -= RATE_VELOCITY_CHANGE;

            // Chance to change heading.
            if (random_.NextDouble() < wandering_)
            {
                heading_ = generateHeading();
            }
        }

        public override int DoesCollides(Snake snake)
        {
            int collideIndex = base.DoesCollides(snake);
            // If the fish collided with the first 10 percent of the snake
            if ((collideIndex != -1) && (collideIndex < SNAKE_EATING_PERCENT * snake.snakeBody_.Length))
            {
                // Insert eating code here.

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

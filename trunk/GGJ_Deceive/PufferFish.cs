using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GGJ_Deceive
{
    public class PufferFish : Fish
    {
        public const float PUFF_DISTANCE = 4f;
        public const float INITIAL_LOSS = 1f;
        public const float HEALTH_LOSS = 0.1f;

        public Boolean isPuffed_;

        public int latchIndex;
        public bool latched = false;
        public bool loosed = false;

        override public void LoadContent()
        {
            base.LoadContent();

            edible_ = false;
            isPuffed_ = false;
        }

        public override int DoesCollides(Snake snake)
        {
            if ((!latched) && (!loosed))
            {
                int collideIndex = base.DoesCollides(snake);

                if (collideIndex != -1)
                {
                    // Latch on!
                    latched = true;
                    latchIndex = collideIndex;
                    Vector3 snakePos = snake.snakeBody_[collideIndex];
                    int rotVal = 135 + random_.Next(-20, 20);
                    rotVal *= (random_.Next(2) == 0) ? -1 : 1;
                    rotation = MathHelper.ToRadians(rotVal);
                    snake.attached_.Add(this);
                    Snake.healthPercent -= INITIAL_LOSS;
                }
                return collideIndex;
            }
            return -1;
        }

        override public void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Check if the fish is within distance from Snakey
            float dist = Vector3.Distance(position_, Game1.snake.snakeBody_[0]);
            if ((!isPuffed_) && (dist < PUFF_DISTANCE))
            {
                scale_.Y *= 2;
                scale_.X *= 4;

                // Set the velocity for the snake head
                velocity_ = (Game1.snake.snakeBody_[0] - position_) / dist;
                velocity_ *= (float) (MIN_VELOCITY + random_.NextDouble() * VELOCITY_RANGE);
                isPuffed_ = true;
            }

            if (latched)
            {
                velocity_ = new Vector3(0, 0, -1);

                vertices_[1].Position.Y = 0;
                vertices_[4].Position.Y = 0;

                Blood.AddBlood(position_, River.random.Next(1, 2));
                // Set the position as latched
                position_ = Game1.snake.snakeBody_[latchIndex];
                double sinVal = Math.Sin(River.time * 500);
                rotation += (float) (0.05f * sinVal);

                Snake.healthPercent -= HEALTH_LOSS;
            }
        }
        
        public void Loose()
        {
            velocity_ = new Vector3((float)(random_.NextDouble() - 0.5), (float)(random_.NextDouble() - 0.5), 0.01f);
            latched = false;
            loosed = true;
        }
    }
}

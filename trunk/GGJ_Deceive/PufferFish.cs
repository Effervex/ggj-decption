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
        public Boolean isPuffed_;

        public int latchIndex;
        public bool latched = false;

        override public void LoadContent()
        {
            base.LoadContent();

            edible_ = false;
            isPuffed_ = false;
        }

        public override int DoesCollides(Snake snake)
        {
            if (!latched)
            {
                int collideIndex = base.DoesCollides(snake);

                if (collideIndex != -1)
                {
                    // Latch on!
                    latched = true;
                    latchIndex = collideIndex;
                    Vector3 snakePos = snake.snakeBody_[collideIndex];
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
                // Set the position as latched
                position_ = Game1.snake.snakeBody_[latchIndex];
            }
        }
    }
}

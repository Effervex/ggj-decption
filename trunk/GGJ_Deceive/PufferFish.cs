using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GGJ_Deceive
{
    public class PufferFish : Fish
    {
        public const float PUFF_DISTANCE = 2f;
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
        public override void Draw(Matrix extra, float puff)
        {
            effect.CurrentTechnique = effect.Techniques[isPuffed_ ? "Technique2" : "Technique1"];
            base.Draw(puffrotation, puffTime);
            effect.CurrentTechnique = effect.Techniques["Technique1"];
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

        Matrix puffrotation = Matrix.Identity;
        float puffTime = 0;
        float puffrotScale = 1;
        override public void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            // Check if the fish is within distance from Snakey
            float dist = Vector3.Distance(position_, Game1.snake.snakeBody_[0]);
            if ((!isPuffed_) && (dist < PUFF_DISTANCE))
            {
                
                // Set the velocity for the snake head
                velocity_ = (Game1.snake.snakeBody_[0] - position_) / dist;
                velocity_ *= (float) (MIN_VELOCITY + random_.NextDouble() * VELOCITY_RANGE * 0.1f);
                velocity_.Y += .0025f;
                isPuffed_ = true;
                puffrotScale = (float)River.random.NextDouble() + 0.5f;
            }

            if (isPuffed_)
            {
                Bubbles.AddBubbles(position_ + 0.15f * puffrotation.Backward, 1);
                puffTime += -0.4f*puffrotScale;
                puffrotation = Matrix.CreateRotationX(puffTime * 0.053f)
                    * Matrix.CreateRotationZ(0 * .51f + puffTime * 0.011f);
                //effect.Parameters["puffAmount"].SetValue(Math.Abs(puffTime) + 1);
            }

            if (latched)
            {
                velocity_ = new Vector3(0, 0, -1);

                // Set the position as latched
                position_ = Game1.snake.snakeBody_[latchIndex];
                Blood.AddBlood(position_, River.random.Next(1, 2));
                Bubbles.AddBubbles(position_, River.random.Next(5, 8));
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

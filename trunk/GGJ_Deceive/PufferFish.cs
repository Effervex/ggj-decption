using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GGJ_Deceive
{
    public class PufferFish : Fish
    {
        public const float PUFF_DISTANCE = 1f;
        public Boolean isPuffed_;

        new public void Initialise()
        {
            base.Initialise();

            isPuffed_ = false;
        }

        new public void Update()
        {
            base.Update();

            // Check if the fish is within distance from Snakey
            if (Vector3.Distance(position_, new Vector3(Game1.snake.snakeBody_[0], 0)) < PUFF_DISTANCE)
            {
                isPuffed_ = true;
                // TODO Puff out
            }
        }
    }
}

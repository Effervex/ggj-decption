using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GGJ_Deceive
{
    public abstract class Thing
    {

        public Vector3 position_;
        public Vector3 velocity_;
        public Vector3 heading_;
        private Random random_;

        public void Initialise()
        {
            // Spawn within the 2x2 area
            // X axis: -1 - 1
            // Y axis: 0 - -2

            random_ = new Random();
            
        }


        public void Update()
        {
            position_ += velocity_;

            // Move relative to the snake's speed
            position_.Z += -Game1.Snake.snakeVelocity_.Z;
        }
    }
}

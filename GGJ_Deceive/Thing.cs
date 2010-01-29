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
        protected Random random_;

        public Thing() {
            Initialise();
        }

        public void Initialise()
        {
            // Spawn within the 2x2 area
            // X axis: -1 - 1
            // Y axis: 0 - -2

            random_ = new Random();
            position_ = new Vector3(River.GenerateValidPos(), -River.segments);
            velocity_ = new Vector3(0);
        }

        public void Update()
        {
            position_ += velocity_;

            // Move relative to the snake's speed
            position_.Z += -Game1.snake.snakeVelocity_;
        }

        internal void DoesCollides(Snake snake)
        {
            // TODO Use some bounding spheres
            throw new NotImplementedException();
        }

        internal void Draw()
        {
            // TODO Draw the thing
            throw new NotImplementedException();
        }
    }
}

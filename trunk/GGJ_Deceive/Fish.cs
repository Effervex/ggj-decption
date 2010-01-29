using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GGJ_Deceive
{
    public class Fish : Thing
    {
        public const float MAX_WANDERLUST_CHANCE = 0.2f;
        public const float MAX_HEADING = 1.5f;
        public const float RATE_VELOCITY_CHANGE = 0.1f;

        public Vector3 heading_;
        public float wandering_;

        new public void Initialise()
        {
            base.Initialise();

            velocity_ = generateVelocity();
            heading_ = generateHeading();
            wandering_ = (float) (random_.NextDouble() * MAX_WANDERLUST_CHANCE);
        }

        private Vector3 generateVelocity()
        {
            return new Vector3((float)(random_.NextDouble() - 0.5),
                (float)(random_.NextDouble() - 0.5),
                (float)(-random_.NextDouble()));
        }

        private Vector3 generateHeading()
        {
            return new Vector3((float)(random_.NextDouble() * MAX_HEADING - (2 * MAX_HEADING)),
                (float)(random_.NextDouble() * MAX_HEADING - (2 * MAX_HEADING)),
                (float)(random_.NextDouble() * -MAX_HEADING));
        }

        new public void Update()
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
    }
}

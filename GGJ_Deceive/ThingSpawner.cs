using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GGJ_Deceive
{
    public class ThingSpawner
    {
        public const float CHANCE_TO_SPAWN = 0.01f;
        public const int MAX_OBJECTS = 10;
        public const float MAX_ENEMY_CHANCE = 0.75f;
        public const float INCREASING_CHANCE = 0.0001f;

        public float chanceOfEnemy = 0.2f;

        public List<Thing> things_;

        public ThingSpawner()
        {
            things_ = new List<Thing>();
        }

        public void Update()
        {
            // Check the player collisions with things
            foreach (Thing thing in things_)
            {
                thing.DoesCollides(Game1.snake);
            }

            Random random = new Random();
            if ((things_.Count < MAX_OBJECTS)
                && (random.NextDouble() < CHANCE_TO_SPAWN * -Game1.snake.snakeVelocity_))
            {
                // Spawn a thing

                // Spawn an enemy
                if (random.NextDouble() < chanceOfEnemy)
                {
                    // For now, just use puffer fish. But split the chance between puffer and boat later.
                    things_.Add(new PufferFish());
                }
                else
                {
                    // Spawn a food fish
                    things_.Add(new Fish());
                }
            }

            if (chanceOfEnemy < MAX_ENEMY_CHANCE)
                chanceOfEnemy += INCREASING_CHANCE;
        }

        public void Draw()
        {
            foreach (Thing thing in things_)
            {
                thing.Draw();
            }
        }
    }
}

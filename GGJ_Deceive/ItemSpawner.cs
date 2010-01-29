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
            Random random = new Random();
            if (random.NextDouble() < CHANCE_TO_SPAWN * -Game1.Snake.snakeVelocity_.Z)
            {
                // Spawn a thing

                // Spawn an enemy
                if (random.NextDouble() < chanceOfEnemy)
                {
                }
                else
                {
                    // Spawn a food fish
                }
            }

            if (chanceOfEnemy < MAX_ENEMY_CHANCE)
                chanceOfEnemy += INCREASING_CHANCE;
        }
    }
}

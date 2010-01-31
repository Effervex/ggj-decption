using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GGJ_Deceive
{
    public class ThingSpawner
    {
        public const float CHANCE_TO_SPAWN = 0.05f;
        public const int MAX_OBJECTS = 200;
        public const float MAX_ENEMY_CHANCE = 0.5f;
        public const float INCREASING_CHANCE = 0.00005f;
        public const float INITIAL_CHANCE_OF_ENEMY = 0.1f;

        public float chanceOfEnemy = INITIAL_CHANCE_OF_ENEMY;

        public List<Thing> things_;

        public bool spawnFish = false;
        public bool spawnPufferFish = false;

        public ThingSpawner()
        {
            things_ = new List<Thing>();
        }

        public void LoadContent()
        {
            
        }

        private static bool RemoveThing(Thing thing)
        {
            if ((thing.removeThis) || (thing.position_.Z > River.segments / 2))
                return true;
            else
                return false;
        }


        public void Update(GameTime gameTime)
        {
            // Check the player collisions with things
            foreach (Thing thing in things_)
            {
                thing.Update(gameTime);
                thing.DoesCollides(Game1.snake);
            }
            things_.RemoveAll(RemoveThing);

            Random random = River.random;
            if ((things_.Count < MAX_OBJECTS)
                && (random.NextDouble() < CHANCE_TO_SPAWN * (1 + Game1.snake.snakeVelocity_)) && (spawnFish || spawnPufferFish))
            {
                // Spawn a thing
                Thing thing = null;

                // Spawn an enemy
                float enemyChance = chanceOfEnemy;
                if (!spawnFish)
                    enemyChance = 1;
                if (!spawnPufferFish)
                    enemyChance = 0;
                if (random.NextDouble() < chanceOfEnemy)
                {
                    // For now, just use puffer fish. But split the chance between puffer and boat later.
                    thing = new PufferFish();
                }
                else
                {
                    // Spawn a food fish
                    thing = new Fish();
                }
                thing.LoadContent();
                things_.Add(thing);
            }

            if (spawnPufferFish && spawnFish)
            {
                if (chanceOfEnemy < MAX_ENEMY_CHANCE)
                    chanceOfEnemy += INCREASING_CHANCE;
            }
        }

        public void Draw()
        {
            foreach (Thing thing in things_)
            {
                thing.Draw(Matrix.Identity, 0);
            }
        }
    }
}

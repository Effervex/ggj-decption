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
        public const float MAX_ENEMY_CHANCE = 0.75f;
        public const float INCREASING_CHANCE = 0.0001f;

        public float chanceOfEnemy = 0.2f;

        public List<Thing> things_;

        public bool spawnFish = false;
        public bool spawnPufferFish = false;

        public ThingSpawner()
        {
            things_ = new List<Thing>();
        }
        Thing temp;
        public void LoadContent()
        {
            temp = new Fish();
            things_.Add(temp);
            temp.LoadContent();
            temp.position_ = new Vector3(0, -1.5f, 5f);
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
            temp.position_ = new Vector3(1, -1.5f, 8.5f);
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
                thing.Draw();
            }
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GGJ_Deceive
{
    public abstract class Thing
    {
        public Vector3 position_;
        public Vector3 velocity_;
        protected static Random random_;
        public VertexPositionNormalTexture[] vertices_;
        public int[] indices_;
        public Vector3 scale_;
        public float sphereSize_;
        public Effect effect;
        public bool removeThis = false;
        public float rotation = 0;

        public Thing() {
        }
        
        virtual public void LoadContent()
        {
                effect = Game1.GetInstance.Content.Load<Effect>("Fish");
                effect.CurrentTechnique = effect.Techniques["Technique1"];
            // Spawn within the 2x2 area
            // X axis: -1 - 1
            // Y axis: 0 - -2

            random_ = new Random();
            position_ = new Vector3(River.GenerateValidPos(), -(River.segments / 2));
            velocity_ = new Vector3(0);

            scale_ = new Vector3((float)(0.5 + random_.NextDouble()),
                (float)(0.5 + random_.NextDouble()),
                (float)(0.5 + random_.NextDouble()));

        }

        virtual public void Update(GameTime gameTime)
        {
            position_ += velocity_;

            // Move relative to the snake's speed
            position_.Z += Game1.snake.snakeVelocity_;
            position_ = River.BoundValues(position_);
        }

        /**
         * Returns the index of the snake part collided with, or -1.
         */
        virtual public int DoesCollides(Snake snake)
        {
            for (int i = 0; i < snake.snakeBody_.Length; i++)
            {
                // Snake head is always at the lowest Z-value, so igonre further checks if the thing is still too far away.
                if (position_.Z + sphereSize_ < snake.snakeBody_[i].Z)
                    return -1;

                // Otherwise run through until collision
                float collisionDistance = Snake.GetBodyThickness(snake.snakeBody_.Length, i) + sphereSize_;
                if (Vector3.Distance(snake.snakeBody_[i], position_) <= collisionDistance)
                {
                    // Collision!
                    return i;
                }
            }
            return -1;
        }

        public virtual void Draw(Matrix extra, float puff)
        {
            //effect.CurrentTechnique = effect.Techniques["Technique1"];

            //Matrix m = Matrix.CreateScale(scale_);
            //if (rotation != 0)
            //    m *= Matrix.CreateRotationY(rotation);
            //m *= Matrix.CreateTranslation(new Vector3(position_.X, position_.Y, position_.Z));

            //effect.Parameters["World"].SetValue(m);//Matrix.CreateScale(scale_) * Matrix.CreateTranslation(new Vector3(position_.X, position_.Y, position_.Z)));
            //effect.Parameters["View"].SetValue(Game1.View);
            //effect.Parameters["Projection"].SetValue(Game1.Projection);

            //effect.Begin();

            //foreach (EffectPass p in effect.CurrentTechnique.Passes)
            //{
            //    p.Begin();
            //    Game1.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, vertices_, 0, vertices_.Length, indices_, 0, indices_.Length / 3);
            //    p.End();
            //}

            //effect.End();
        }
    }
}

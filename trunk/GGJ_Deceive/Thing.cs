using System;
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

        public Thing() {
        }
        
        virtual public void LoadContent()
        {
                effect = Game1.GetInstance.Content.Load<Effect>("Fish");
            // Spawn within the 2x2 area
            // X axis: -1 - 1
            // Y axis: 0 - -2

            random_ = new Random();
            position_ = new Vector3(River.GenerateValidPos(), -River.segments);
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

        internal void Draw()
        {
            effect.CurrentTechnique = effect.Techniques["Technique1"];

            Matrix m = Matrix.Identity;
            m.Forward = -Vector3.Normalize(velocity_);
            m.Right = Vector3.Normalize(Vector3.Cross(m.Forward, Vector3.Up));
            m.Up = Vector3.Cross(m.Forward, m.Right);
            m.Translation = position_;

            effect.Parameters["World"].SetValue(Matrix.CreateScale(scale_)*m);//Matrix.CreateScale(scale_) * Matrix.CreateTranslation(new Vector3(position_.X, position_.Y, position_.Z)));
            effect.Parameters["View"].SetValue(Game1.View);
            effect.Parameters["Projection"].SetValue(Game1.Projection);

            effect.Begin();

            foreach (EffectPass p in effect.CurrentTechnique.Passes)
            {
                p.Begin();
                Game1.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, vertices_, 0, vertices_.Length, indices_, 0, indices_.Length / 3);
                p.End();
            }

            effect.End();
        }

        public abstract void SetUpVertices();

        public abstract void SetUpIndices();
    }
}

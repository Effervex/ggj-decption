using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GGJ_Deceive
{
    public class Fish : Thing
    {
        // Model constants
        public const float FISH_HEIGHT = 0.25f;
        public const float FISH_WIDTH = 0.05f;
        public const float FISH_LENGTH = 0.3f;
        public const float TAIL_LENGTH = 0.05f;
        public const float TAIL_INDENT = 0.02f;
        public const float NOSE_PERCENT = 0.35f;

        public const float SNAKE_EATING_PERCENT = 0.1f;
        public const float MIN_VELOCITY = 0.001f;
        public const float VELOCITY_RANGE = 0.049f;

        public bool edible_ = true;
        Model _fish;
        Texture2D _scales;
        override public void LoadContent()
        {
            base.LoadContent();
            _fish = Game1.GetInstance.Content.Load<Model>("fish_static");
            _scales = Game1.GetInstance.Content.Load<Texture2D>("Fish_skin");

            float max = Math.Max(scale_.X, Math.Max(scale_.Y, scale_.Z));
            sphereSize_ = FISH_HEIGHT * max;
            velocity_ = generateVelocity();
            foreach (ModelMesh m in _fish.Meshes)
            {
                for (int i = 0; i < m.MeshParts.Count; i++)
                {
                    m.MeshParts[i].Effect = effect;
                }
            } 
        }

        private Vector3 generateVelocity()
        {
            return new Vector3(0, 0,
                (float)(MIN_VELOCITY + random_.NextDouble() * VELOCITY_RANGE));
        }

        override public void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            
        }
        public override void Draw(Matrix extra, float puff)
        {
            effect.Parameters["Time"].SetValue(Math.Abs(River.time * 50));
            Matrix[] transforms = new Matrix[_fish.Bones.Count];
               _fish.CopyAbsoluteBoneTransformsTo(transforms);

            
            foreach (ModelMesh m in _fish.Meshes)
            {
               // _fish.Root.Transform = Matrix.Identity;
                foreach (Effect e in m.Effects)
                {
                    //effect.CurrentTechnique = effect.Techniques["Technique2"];
                    e.Parameters["puffAmount"].SetValue(Math.Abs(puff) + 1);
                    e.Parameters["ScalesTexture"].SetValue(_scales);

                    e.Parameters["Env"].SetValue(River.caustics);
                    e.Parameters["View"].SetValue(Game1.View);
                    e.Parameters["Projection"].SetValue(Game1.Projection);
                    e.Parameters["World"].SetValue(Matrix.CreateScale(0.01f * .3f) * Matrix.CreateTranslation(0.1f, 0, 0.0f) * Matrix.CreateRotationY(1.5f) * Matrix.CreateRotationY(rotation) * extra * Matrix.CreateTranslation(position_));// * Matrix.CreateTranslation(new Vector3(position_.X, position_.Y, position_.Z)) * transforms[m.ParentBone.Index]);

                    m.Draw();
                }
            }

        }
        
        public override int DoesCollides(Snake snake)
        {
            int collideIndex = base.DoesCollides(snake);
            // If the fish collided with the first 10 percent of the snake
            if ((collideIndex != -1) && (collideIndex < SNAKE_EATING_PERCENT * snake.snakeBody_.Length))
            {
                if (edible_)
                {
                    Blood.AddBlood(position_, River.random.Next(65, 195));
                    Bubbles.AddBubbles(position_, 220);
                    // Insert eating code here.
                    Snake.cakeBatterCount++;
                    Game1.chompInstance.Play();
                    
                    removeThis = true;
                }
            }
            return collideIndex;
        }

    }
}

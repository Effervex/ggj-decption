using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace GGJ_Deceive
{
    public class StateMachine
    {
        public const float TITLE_STAY = 5;
        public const float TITLE_FADE = 0.05f;
        public const int FREEZE_TIME = 3;

        public Boolean frozen_;
        public int freezeStart_;
        public State gameState_;
        public float titleOpacity_;
        public MouseState prevState_;

        private River river_;
        private Snake snake_;
        private ThingSpawner spawner_;

        public void Initialise()
        {
            river_ = Game1.river;
            snake_ = Game1.snake;
            spawner_ = Game1.thingSpawner;
            gameState_ = State.TITLE;
        }

        public void Update(GameTime gameTime, Rectangle clientBounds)
        {
            MouseState currentState = Mouse.GetState();

            // Various states
            switch (gameState_)
            {
                case State.TITLE:
                    titleOpacity_ = TITLE_STAY;

                    // Advance by moving mouse
                    if (!currentState.Equals(prevState_))
                    {
                        gameState_ = State.LEARNING_FORWARD;
                        freezeStart_ = gameTime.ElapsedGameTime.Seconds;
                        frozen_ = true;
                    }
                    break;
                case State.LEARNING_FORWARD:
                    // Alert the user how to move forward in a frozen state
                    if (gameTime.ElapsedGameTime.Seconds - freezeStart_ >= FREEZE_TIME)
                    {
                        frozen_ = false;

                    }
                    break;
            }

            if (!gameState_.Equals(State.TITLE))
                titleOpacity_ -= TITLE_FADE;

            river_.Update();
            snake_.Update(gameTime, clientBounds);

            prevState_ = Mouse.GetState();
        }

        public void Draw(GameTime gameTime)
        {
            // Draw the current state
            Game1.GraphicsDevice.SetRenderTarget(0, Game1.refractBuffer);
            Game1.GraphicsDevice.Clear(Color.SkyBlue);
            
            river_.DrawRefracted();
            
            Game1.GraphicsDevice.SetRenderTarget(0, null);
            Game1.GraphicsDevice.Clear(new Color(Game1.fog));
            
            river_.Draw();
            spawner_.Draw();
            snake_.Draw(gameTime);
            
            // Draw any helpful text

            // Draw the title
            if (titleOpacity_ > 0)
            {
            }
        }
    }

    public enum State
    {
        TITLE,
        LEARNING_FORWARD,
        LEARNING_UPDOWN,
        FISH_ENCOUNTERED,
        EATING_FISH,
        ENEMY_APPROACH,
        ENEMY_ENCOUNTERED,
        NORMAL_GAMEPLAY,
        GAME_OVER
    }
}

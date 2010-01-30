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
        public const int FREEZE_TIME = 3000;

        public Boolean frozen_;
        public double freezeStart_;
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
            // Change this
            gameState_ = State.NORMAL_GAMEPLAY;
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
                        freezeStart_ = gameTime.TotalRealTime.TotalMilliseconds;
                        frozen_ = true;
                    }
                    break;
                case State.LEARNING_FORWARD:
                    // Alert the user how to move forward in a frozen state
                    double gameSeconds = gameTime.TotalRealTime.TotalMilliseconds;
                    if (gameSeconds - freezeStart_ >= FREEZE_TIME)
                    {
                        frozen_ = false;
                        snake_.horizontalMovement_ = true;
                        river_.Update();
                        snake_.Update(gameTime, clientBounds);
                    }
                    break;
                case State.NORMAL_GAMEPLAY:
                    snake_.horizontalMovement_ = true;
                    snake_.verticalMovement_ = true;
                    river_.Update();
                    snake_.Update(gameTime, clientBounds);
                    spawner_.Update();
                    break;
            }

            if (!gameState_.Equals(State.TITLE))
                titleOpacity_ -= TITLE_FADE;

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
            Blood.Draw();
            snake_.Draw(gameTime);
            
            Game1.overlay.DrawHUD();
            // Draw any helpful text
            if (frozen_)
            {
                if (gameState_.Equals(State.LEARNING_FORWARD))
                {
                }
            }

            // Draw the title
            if (titleOpacity_ > 0)
            {
                DrawTitle();
            }
        }

        private void DrawTitle()
        {
            
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

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
        public const float FORWARD_NEXT = 5;
        public const float VERTICAL_NEXT = 5f;

        public Boolean frozen_;
        public double freezeStart_;
        public State gameState_;
        public MouseState prevState_;

        private River river_;
        private Snake snake_;
        private ThingSpawner spawner_;
        private Overlays overlays_;

        public bool displayMoveForward = false;
        public bool displayMoveVertical = false;
        public bool displayEatFish = false;
        public bool displayEnemyFish = false;
        public bool displayGameGoal = false;
        public bool resetYet = false;
        public float forwardMovement = 0;
        public float verticalMovement = 0;
        public Fish incomingFish;

        public void Initialise()
        {
            river_ = Game1.river;
            snake_ = Game1.snake;
            spawner_ = Game1.thingSpawner;
            overlays_ = Game1.overlay;
            gameState_ = State.NORMAL_GAMEPLAY;
        }

        public void Update(GameTime gameTime, Rectangle clientBounds)
        {
            MouseState currentState = Mouse.GetState();

            // Various states
            switch (gameState_)
            {
                case State.TITLE:
                    // Advance by moving mouse
                    if (!currentState.Equals(prevState_))
                    {
                        gameState_ = State.LEARNING_FORWARD;
                        freezeStart_ = gameTime.TotalRealTime.TotalMilliseconds;
                        snake_.verticalMovement_ = false;
                        spawner_.spawnFish = false;
                        spawner_.spawnPufferFish = false;
                    }
                    break;
                case State.LEARNING_FORWARD:
                    // Alert the user how to move forward
                    double gameSeconds = gameTime.TotalRealTime.TotalMilliseconds;
                    if (gameSeconds - freezeStart_ <= FREEZE_TIME)
                        displayMoveForward = true;
                    else
                        displayMoveForward = false;

                    snake_.horizontalMovement_ = true;
                    river_.Update();
                    snake_.Update(gameTime, clientBounds);

                    forwardMovement += snake_.snakeVelocity_;

                    // Advance by moving forward x steps
                    if (forwardMovement >= FORWARD_NEXT)
                    {
                        gameState_ = State.LEARNING_UPDOWN;
                        displayMoveForward = false;
                        freezeStart_ = gameSeconds;
                    }
                    break;
                case State.LEARNING_UPDOWN:
                    // Alert the user how to move vertically
                    gameSeconds = gameTime.TotalRealTime.TotalMilliseconds;
                    if (gameSeconds - freezeStart_ <= FREEZE_TIME)
                        displayMoveVertical = true;
                    else
                        displayMoveVertical = false;

                    snake_.verticalMovement_ = true;
                    river_.Update();
                    float prevY = snake_.snakeBody_[0].Y;
                    snake_.Update(gameTime, clientBounds);

                    verticalMovement += Math.Abs(snake_.snakeBody_[0].Y - prevY);

                    // Advance by moving forward x steps
                    if (verticalMovement >= VERTICAL_NEXT)
                    {
                        gameState_ = State.FISH_ENCOUNTERED;
                        displayMoveVertical = false;
                        freezeStart_ = gameSeconds;
                    }
                    break;
                case State.FISH_ENCOUNTERED:
                    snake_.horizontalMovement_ = true;
                    snake_.verticalMovement_ = true;
                    // Alert the user how to move vertically
                    gameSeconds = gameTime.TotalRealTime.TotalMilliseconds;
                    if (gameSeconds - freezeStart_ <= FREEZE_TIME)
                        displayEatFish = true;
                    else
                        displayEatFish = false;

                    if (incomingFish == null)
                    {
                        incomingFish = new Fish();
                        incomingFish.LoadContent();
                        incomingFish.velocity_.Z *= 10;
                        incomingFish.position_.X = 0.4f;
                        incomingFish.position_.Y = River.BOTTOM / 2;
                        spawner_.things_.Add(incomingFish);
                    }
                    if (incomingFish.position_.Z > 0)
                    {
                        incomingFish.velocity_ = new Vector3(0);
                    }
                    if (incomingFish.position_.Z > River.segments / 2)
                    {
                        freezeStart_ = gameSeconds;
                        incomingFish = null;
                    }

                    river_.Update();
                    int cakeBefore = Snake.cakeBatterCount;
                    snake_.Update(gameTime, clientBounds);
                    spawner_.Update(gameTime);

                    if (Snake.cakeBatterCount > cakeBefore)
                    {
                        gameState_ = State.ENEMY_ENCOUNTERED;
                        displayEatFish = false;
                        freezeStart_ = gameSeconds;
                        incomingFish = null;
                    }
                    break;
                case State.ENEMY_ENCOUNTERED:
                    snake_.horizontalMovement_ = true;
                    snake_.verticalMovement_ = true;
                    // Alert the user how to move vertically
                    gameSeconds = gameTime.TotalRealTime.TotalMilliseconds;
                    if (gameSeconds - freezeStart_ <= FREEZE_TIME)
                        displayEnemyFish = true;
                    else
                        displayEnemyFish = false;

                    if (incomingFish == null)
                    {
                        incomingFish = new PufferFish();
                        incomingFish.LoadContent();
                        incomingFish.velocity_.Z *= 10;
                        incomingFish.position_.X = -0.4f;
                        incomingFish.position_.Y = River.BOTTOM / 2;
                        spawner_.things_.Add(incomingFish);
                    }
                    if (incomingFish.position_.Z > 0)
                    {
                        incomingFish.velocity_ = new Vector3(0);
                    }

                    river_.Update();
                    snake_.Update(gameTime, clientBounds);
                    spawner_.Update(gameTime);

                    if (incomingFish.position_.Z > River.segments / 2)
                    {
                        gameState_ = State.NORMAL_GAMEPLAY;
                        displayEnemyFish = false;
                        freezeStart_ = gameSeconds;
                        incomingFish = null;
                    }
                    break;
                case State.NORMAL_GAMEPLAY:
                    // Alert the user how to move vertically
                    gameSeconds = gameTime.TotalRealTime.TotalMilliseconds;
                    if (gameSeconds - freezeStart_ <= FREEZE_TIME)
                        displayGameGoal = true;
                    else
                        displayGameGoal = false;

                    if (!resetYet)
                    {
                        snake_.horizontalMovement_ = true;
                        snake_.verticalMovement_ = true;
                        spawner_.spawnFish = true;
                        spawner_.spawnPufferFish = true;
                        Snake.beefLevel = 1;
                        Snake.healthPercent = 100;
                        Snake.cakeBatterCount = 0;
                        spawner_.chanceOfEnemy = 0.2f;
                        resetYet = true;
                    }

                    river_.Update();
                    snake_.Update(gameTime, clientBounds);
                    spawner_.Update(gameTime);
                    break;
            }

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

            State outState = State.NOTHING;
            if (displayMoveForward)
                outState = State.LEARNING_FORWARD;
            
            overlays_.DrawHUD(outState);
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
        GAME_OVER,
        NOTHING
    }
}

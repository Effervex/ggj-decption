using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GGJ_Deceive
{
    public class Overlays
    {
        Texture2D HUD;
        Texture2D Tip;
        SpriteBatch batch;
        SpriteFont font;

        Texture2D skull;
        Texture2D bowl;
        Texture2D beef_bar;
        Texture2D screen_blood;

        public const int bowlCount = 3;
        float time = 0f;

        public void LoadContent()
        {
            HUD = Game1.GetInstance.Content.Load<Texture2D>("Overlay");
            Tip = Game1.GetInstance.Content.Load<Texture2D>("Overlay_Tip");
            font = Game1.GetInstance.Content.Load<SpriteFont>("HUD");
            batch = new SpriteBatch(Game1.GraphicsDevice);
            skull = Game1.GetInstance.Content.Load<Texture2D>("death_icon");
            bowl = Game1.GetInstance.Content.Load<Texture2D>("cake_icon");
            beef_bar = Game1.GetInstance.Content.Load<Texture2D>("beef_bar");
            screen_blood = Game1.GetInstance.Content.Load<Texture2D>("screen_blood");
        }
        float dialog_fade = 0;
        float death_blood = -500f;
        public void DrawHUD(State gameState)
        {
            batch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.BackToFront, SaveStateMode.SaveState);
            batch.Draw(HUD, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            
            time += 0.1f;
            float healthFrac = (Snake.healthPercent / (float) Snake.MAX_HEALTH);

            for (int i = 0; i < 3; i++)
            {
                float lerp = MathHelper.Clamp((healthFrac - (i / 3f)) * 3, 0, 1);
                batch.Draw(skull, new Vector2(680 + (skull.Width + 5) * i, 12), null, new Color(255, (byte)(lerp * 255), (byte)(lerp * 255), 255)  , 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }

            for (int i = 0; i < bowlCount; i++)
            {
                batch.Draw(bowl, new Vector2(680 + (skull.Width + 5) * i, 50), null, i < Snake.cakeBatterCount ? Color.White : Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }


            //MessageTip("To move forward, slither the\nmouse left and right", new Vector2(100, 150));


            

            for (int i = 0; i < Snake.beefLevel; i++)
            {
                float beef_frac = (float)i / Snake.MAX_BEEF;
                byte red = (byte)(255 * MathHelper.Lerp(1, 0, beef_frac));
                byte green= (byte)(255 * MathHelper.Lerp(0, 1, beef_frac));

                batch.Draw(beef_bar, new Vector2(530 + (beef_bar.Width * i), 555), null, new Color(red, green, 0), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }

            if(gameState == State.NOTHING)
                death_blood = -500f;

            if (gameState != State.NOTHING)
                WriteText(gameState);

            //if (gameState != State.GAME_OVER && gameState != State.TRY_AGAIN)
            //    death_blood = -500f;

            batch.End();
        }

        public void Update()
        {
        }

        public void Release()
        {
            batch.Dispose();
            HUD.Dispose();
        }

        public void DeathScreen()
        {

            if (death_blood < 500f)
                death_blood += (500f - death_blood) * 0.005f;

            batch.Draw(screen_blood, new Vector2(0, death_blood * 0.7f - screen_blood.Height), null,
                Color.White, 0, Vector2.Zero, 2f, SpriteEffects.None, 0.8f);
            batch.Draw(screen_blood, new Vector2(0, death_blood - screen_blood.Height), null,
                new Color(25, 25, 225, 200), 0, Vector2.Zero, 2f, SpriteEffects.FlipHorizontally, 0.8f);

            float alpa = ( 500f + death_blood) * 0.25f;
            batch.DrawString(font, "Natural selection selected you.", new Vector2(200, 84), new Color(245, 245, 255, (byte)alpa), .012f, Vector2.Zero, 2.15f, SpriteEffects.None, 0f);
            batch.DrawString(font, "You . Are . Dead", new Vector2(290, 140 + alpa * 0.1f), new Color(190, 0,0, (byte)alpa), .012f, Vector2.Zero, 2.15f, SpriteEffects.None, 0f);

        }
        string lastMessage = "";
        public void MessageTip(string message, string number, Vector2 position)
        {
            if (message != lastMessage)
            {
                dialog_fade = 0f;
                lastMessage = message;
            }
            else
            {
                if(dialog_fade < 250)
                    dialog_fade += 3;
            }
            Color color = new Color(255, 255, 255, (byte)dialog_fade);

            batch.Draw(Tip, position, null, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.5f);
            batch.DrawString(font, number, position + new Vector2(240, 33), color, .052f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);
            
            batch.DrawString(font, message, position + new Vector2(57, 64), color, .012f, Vector2.Zero, 1.15f, SpriteEffects.None, 0f);

        }
        public void WriteText(State imageState)
        {
            
            switch (imageState)
            {
                case State.LEARNING_FORWARD:
                    MessageTip("To move forward, slither the\nmouse left and right", 
                        "1", new Vector2(100, 100));
                    //"To move forward, slither the mouse left and right"
                    break;
                case State.LEARNING_UPDOWN:
                    MessageTip("To ascend/descend, move \nmouse up/down while slithering",
                        "2", new Vector2(110, 120));
                    //"To ascend/descend, move mouse up/down while slithering"
                    break;
                case State.FISH_ENCOUNTERED:
                    MessageTip("A fish approaches! Gobble it up\nfor some cake batter!",
                        "3", new Vector2(90, 110));
                    //"A fish approaches! Gobble it up for some cake batter!"
                    break;
                case State.ENEMY_ENCOUNTERED:
                    MessageTip("Careful. Dangerous puffer-fish\nmasquerade as regular ones\nuntil they are close",
                        "4", new Vector2(100, 130));
                    //"Careful. Dangerous puffer-fish masquerade as regular ones until they are close"
                    //"Shake furiously to dislodge attached puffer fish"
                    break;
                case State.NORMAL_GAMEPLAY:
                    
                    //"Fill your belly with cake and grow beefy!"
                    MessageTip("Fill your belly with cake and grow\nbeefy!",
                        "5", new Vector2(105, 110));
                    break;
                case State.TRY_AGAIN:
                    //"Try again (Y/N)?"
                    DeathScreen();
                    MessageTip("   Try again?\n   Y: Yes\n   N: No",
                        "Retry!", new Vector2(120, 220));
                    break;
                case State.FISH_BITE:
                    //"Oh no! A fish has you! Shake vigourously to flick it off"
                    MessageTip("Oh no! A fish has you! Shake\nvigourously to flick it off",
                        "7", new Vector2(110, 120));
                    break;
                case State.GAME_OVER:
                    //"Farewell, brave snake"
                    DeathScreen();
                    break;
            }
        }
    }
}

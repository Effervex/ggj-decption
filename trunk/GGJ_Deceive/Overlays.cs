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
        SpriteBatch batch;
        SpriteFont font;

        Texture2D skull;
        Texture2D bowl;
        Texture2D beef_bar;

        public const int bowlCount = 3;
        float time = 0f;

        public void LoadContent()
        {
            HUD = Game1.GetInstance.Content.Load<Texture2D>("Overlay");
            font = Game1.GetInstance.Content.Load<SpriteFont>("HUD");
            batch = new SpriteBatch(Game1.GraphicsDevice);
            skull = Game1.GetInstance.Content.Load<Texture2D>("death_icon");
            bowl = Game1.GetInstance.Content.Load<Texture2D>("cake_icon");
            beef_bar = Game1.GetInstance.Content.Load<Texture2D>("beef_bar");
        }

        public void DrawHUD()
        {
            batch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.BackToFront, SaveStateMode.SaveState);
            batch.Draw(HUD, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            
            time += 0.1f;
            float healthFrac = (Snake.healthPercent / 100f);

            for (int i = 0; i < 3; i++)
            {
                float lerp = MathHelper.Lerp(0, 1, MathHelper.Clamp(healthFrac - ((float)i / 3f), 1, 0));
                batch.Draw(skull, new Vector2(680 + (skull.Width + 5) * i, 12), null, new Color(255, (byte)(lerp * 255), (byte)(lerp * 255), 255)  , 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }

            for (int i = 0; i < bowlCount; i++)
            {
                batch.Draw(bowl, new Vector2(680 + (skull.Width + 5) * i, 50), null, i < Snake.cakeBatterCount ? Color.White : Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }



            

            for (int i = 0; i < Snake.beefLevel; i++)
            {
                float beef_frac = (float)i / Snake.MAX_BEEF;
                byte red = (byte)(255 * MathHelper.Lerp(1, 0, beef_frac));
                byte green= (byte)(255 * MathHelper.Lerp(0, 1, beef_frac));

                batch.Draw(beef_bar, new Vector2(530 + (beef_bar.Width * i), 555), null, new Color(red, green, 0), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
            
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
    }
}

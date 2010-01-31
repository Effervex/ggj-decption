using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace GGJ_Deceive
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public const float NEAR_PLANE_DIST = 0.1f;

        GraphicsDeviceManager graphics;

        //Game Objects
        public static River river;
        public static Snake snake;
        public static ThingSpawner thingSpawner;
        public static StateMachine stateMachine;
        public static Matrix View;
        public static Matrix Projection;

        public static RenderTarget2D refractBuffer;
        public static Overlays overlay;
        public static VertexDeclaration vd;

        public static SoundEffect chompSound;
        public static SoundEffectInstance chompInstance;
        public static SoundEffect munchSound;
        public static SoundEffectInstance munchInstance;
        public static SoundEffect bubblesSound;
        public static SoundEffectInstance bubblesInstance;
        public static SoundEffect deathSound;
        public static SoundEffectInstance deathInstance;
        public static SoundEffect ambience;
        public static SoundEffectInstance ambienceInstance;
        public static SoundEffect surgeSound;
        public static SoundEffectInstance surgeInstance;

        public static new GraphicsDevice GraphicsDevice
        {

            get
            {
                return GetInstance.GraphicsDevice;
            }
        }
       static Microsoft.Xna.Framework.Game instance;
      public  static Microsoft.Xna.Framework.Game GetInstance
        {
            get
            {
                return instance;
            }
        }

        public Game1()
        {      
            overlay = new Overlays();
            river = new River();
            snake = new Snake(40);
            thingSpawner = new ThingSpawner();
            stateMachine = new StateMachine();
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            instance = this;
            
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
         //   Game1.GraphicsDevice.RenderState.CullMode = CullMode.CullClockwiseFace;
            stateMachine.Initialise();
            Mouse.SetPosition(Window.ClientBounds.Width / 2, Window.ClientBounds.Height / 2);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            chompSound = Content.Load<SoundEffect>("chomp");
            chompInstance = chompSound.CreateInstance();
            munchSound = Content.Load<SoundEffect>("munch");
            munchInstance = munchSound.CreateInstance();
            bubblesSound = Content.Load<SoundEffect>("bubbles");
            bubblesInstance = bubblesSound.CreateInstance();
            deathSound = Content.Load<SoundEffect>("rattlesnakerattle");
            deathInstance = deathSound.CreateInstance();
            ambience = Content.Load<SoundEffect>("ambient");
            ambienceInstance = ambience.CreateInstance();
            surgeSound = Content.Load<SoundEffect>("surge");
            surgeInstance = surgeSound.CreateInstance();

            ambienceInstance.IsLooped = true;
            ambienceInstance.Play();

            // Create a new SpriteBatch, which can be used to draw textures.
            
            refractBuffer = new RenderTarget2D(Game1.GraphicsDevice,
                Game1.GraphicsDevice.Viewport.Width,
                Game1.GraphicsDevice.Viewport.Height, 1, SurfaceFormat.Color);
            
            vd = new VertexDeclaration(Game1.GraphicsDevice, VertexPositionNormalTexture.VertexElements);

            overlay.LoadContent();
            river.Create();
            snake.LoadContent(GraphicsDevice);
            thingSpawner.LoadContent();
            Blood.Initalize();
            Bubbles.Initalize();
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            river.Release();
            overlay.Release();
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();
            
            // Keep mouse in bounds
            BoundMouse();

            View = Matrix.CreateLookAt(new Vector3(0, River.BOTTOM / 2, 10), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(130f),
                graphics.GraphicsDevice.Viewport.Width / graphics.GraphicsDevice.Viewport.Height,
                NEAR_PLANE_DIST, 100f);
            stateMachine.Update(gameTime, Window.ClientBounds);
            Blood.Update();
            Bubbles.Update();
overlay.Update();
            base.Update(gameTime);
        }

        private void BoundMouse()
        {
            int mouseX = Mouse.GetState().X;
            int mouseY = Mouse.GetState().Y;

            mouseX = (int) MathHelper.Clamp(mouseX, 0, Window.ClientBounds.Width);
            mouseY = (int) MathHelper.Clamp(mouseY, 0, Window.ClientBounds.Height);
            Mouse.SetPosition(mouseX, mouseY);
        }
        
        public static Vector4 fog = new Vector4(.73f, .59f, .63f, 1f);

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            Game1.GraphicsDevice.VertexDeclaration = vd;
            byte r = 30;
            byte g = 38;
            byte b = 36;

            fog = new Vector4(r / 255f, g / 255f, b / 255f, 1f);
            stateMachine.Draw(gameTime);
          
            base.Draw(gameTime);
        }
    }
}

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
        GraphicsDeviceManager graphics;
        public static SpriteBatch spriteBatch;

        //Game Objects
        public static River river;
        public static Snake snake;
        public static ThingSpawner thingSpawner;
        public static StateMachine stateMachine;
        public static Matrix View;
        public static Matrix Projection;

        public static RenderTarget2D refractBuffer;

        public static VertexDeclaration vd;

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
            river = new River();
            snake = new Snake(30);
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
            // Create a new SpriteBatch, which can be used to draw textures.

            refractBuffer = new RenderTarget2D(Game1.GraphicsDevice,
                Game1.GraphicsDevice.Viewport.Width,
                Game1.GraphicsDevice.Viewport.Height, 1, SurfaceFormat.Color);
            
            spriteBatch = new SpriteBatch(GraphicsDevice);
            vd = new VertexDeclaration(Game1.GraphicsDevice, VertexPositionNormalTexture.VertexElements);
            
            river.Create();
            snake.LoadContent(GraphicsDevice);
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            river.Release();
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

            View = Matrix.CreateLookAt(new Vector3(0, -1f, 10), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(130f),
                graphics.GraphicsDevice.Viewport.Width / graphics.GraphicsDevice.Viewport.Height,
                0.1f, 100f);
            stateMachine.Update(gameTime, Window.ClientBounds);
            
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
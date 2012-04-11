using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using KinectRagdoll.Kinect;
using KinectRagdoll.Sandbox;
using KinectRagdoll.Drawing;
using KinectRagdoll.Rules;
using KinectRagdoll.Powerups;
using KinectRagdoll.Music;
using KinectRagdoll.Hazards;
using ProjectMercury.Renderers;

namespace KinectRagdoll
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class KinectRagdollGame : Microsoft.Xna.Framework.Game
    {

        public static int WIDTH = 800;
        public static int HEIGHT = 600;

        
        public KinectManager kinectManager;
        public FarseerManager farseerManager;
        public InputManager inputManager;
        public ActionCenter actionCenter;
        public RagdollManager ragdollManager;
        public Toolbox toolbox;
        public ObjectiveManager objectiveManager;
        public PowerupManager powerupManager;
        public Jukebox jukebox;
        public HazardManager hazardManager;
        public ParticleEffectManager particleEffectManager;

        GraphicsDeviceManager graphics;
        Color bkColor;
        SpriteBatch spriteBatch;
        Matrix worldMatrix;
        Matrix farseerProjection;
        Matrix farseerView;
        Matrix spriteBatchProjection;
        Matrix spriteBatchTransformation;

        public static List<Action> pendingUpdates = new List<Action>();

        public static GraphicsDevice graphicsDevice;

        public static KinectRagdollGame Main
        {
            get;
            private set;
        }
        
        

        public KinectRagdollGame()
        {
            Main = this;

            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = WIDTH;
            graphics.PreferredBackBufferHeight = HEIGHT;

           
            Content.RootDirectory = "Content";

            FarseerTextures.Init();
            FarseerTextures.SetGame(this);
            
            kinectManager = new KinectManager();
            farseerManager = new FarseerManager(true, this);
            ragdollManager = new RagdollManager();
            
            actionCenter = new ActionCenter(this);
            inputManager = new InputManager(this);
            
            //spriteHelper = new SpriteHelper();
            objectiveManager = new ObjectiveManager(this);
            powerupManager = new PowerupManager(ragdollManager, farseerManager);
            jukebox = new Jukebox();
            hazardManager = new HazardManager(farseerManager, ragdollManager);
            particleEffectManager = new ParticleEffectManager(graphics, ref farseerProjection);

            toolbox = new Toolbox(this);


            this.IsMouseVisible = true;
            bkColor = Color.CornflowerBlue;

        }



        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

        }

        private void InitializeTransform()
        {
            worldMatrix = Matrix.Identity;
            farseerView = Matrix.Identity;
            ResetTransform();
        }

        private void ResetTransform() {

            farseerProjection = getFarseerProjection();
            spriteBatchProjection = farseerProjection * Matrix.Invert(getScreenProjection());

            farseerManager.setProjection(farseerProjection);
            ProjectionHelper.Init(GraphicsDevice.Viewport, farseerProjection);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            graphicsDevice = GraphicsDevice;

            SpriteHelper.LoadContent(Content);

            BodySound.LoadContent(Content);
            ragdollManager.LoadContent(Content);
            farseerManager.LoadContent();
            objectiveManager.LoadContent(Content);
            toolbox.LoadContent();
            Jukebox.LoadContent(Content);
            particleEffectManager.LoadContent(Content);

            InitializeTransform();
            
            kinectManager.InitKinect();

            Jukebox.Loop("Clay");

            base.LoadContent();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            lock (pendingUpdates)
            {
                foreach (Action a in pendingUpdates)
                {
                    a();
                }

                pendingUpdates.Clear();
            }

            inputManager.Update();
            ragdollManager.Update(kinectManager.skeletonInfo);
            farseerManager.Update(gameTime);
            objectiveManager.Update();
            hazardManager.Update();
            particleEffectManager.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            if (!kinectManager.IsKinectRunning)
            {
                kinectManager.InitKinect();
            }

            base.Update(gameTime);
        }

        

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DarkSlateGray, 1.0f, 0);   
            SetupRendering();

            farseerManager.DrawPhysicsObjects(ref farseerView);

            // Draw sprites in farseer-space
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, RasterizerState.CullNone, null, spriteBatchTransformation);

            ragdollManager.Draw(spriteBatch); // this will draw equipment effects
            powerupManager.Draw(spriteBatch); // this draws pickups
            hazardManager.Draw(spriteBatch); // this draws laser beams
            particleEffectManager.Draw(spriteBatchTransformation);

            spriteBatch.End();

            
            // Draw sprites in screen-space
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            
            toolbox.Draw(spriteBatch);
            farseerManager.DrawFrontEffects(spriteBatch);
            objectiveManager.Draw(spriteBatch);

            spriteBatch.End();

            

            base.Draw(gameTime);

        }



        private void SetupRendering()
        {
            farseerView = createFarseerView();
            ProjectionHelper.Update(farseerView);

            spriteBatchTransformation = farseerView * spriteBatchProjection;
        }

        private Matrix createFarseerView()
        {
            // All vectors here are in farseer coordinates.

            Vector2 center;
            float horizontalBound;
            float verticalBound;

            if (ragdollManager.CameraShouldTrack)
            {
                center = ragdollManager.getRagdollCenter();
                horizontalBound = 10;
                verticalBound = 10;
            }
            else
            {
                center = ProjectionHelper.PixelToFarseer(inputManager.inputHelper.MousePosition);
                horizontalBound = 25;
                verticalBound = 15;
            }

            float top = -farseerView.Translation.Y + verticalBound;
            float bottom = -farseerView.Translation.Y - verticalBound;
            float left = -farseerView.Translation.X - horizontalBound;
            float right = -farseerView.Translation.X + horizontalBound;

            Vector3 translate = new Vector3();
            if (center.X < left) translate.X = center.X - left;
            else if (center.X > right) translate.X = center.X - right;

            if (center.Y > top) translate.Y = center.Y - top;
            else if (center.Y < bottom) translate.Y = center.Y - bottom;


            translate.X = CurveFunc(translate.X, 20);
            translate.Y = CurveFunc(translate.Y, 20);

            Matrix m = Matrix.CreateTranslation(farseerView.Translation.X - translate.X, farseerView.Translation.Y - translate.Y, 0);

            return m;
        }

        private float CurveFunc(float val, float rampup)
        {

            if (val == 0) return 0;

            if (Math.Abs(val) < rampup)
            {
                val = 1/(rampup * 2) * val * val * Math.Sign(val);
            }
            else
            {
                val -= Math.Sign(val) * rampup / 2;
               
            }

            return val;
        }


        public void ToggleFullscreen()
        {

            if (graphics.IsFullScreen) // will return to windowed mode
            {
                graphics.PreferredBackBufferHeight = HEIGHT;
                graphics.PreferredBackBufferWidth = WIDTH;
            }
            else { // will go full screen
                graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
                graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
            }

            graphics.ToggleFullScreen();

            ResetTransform();
        }

        private Matrix getFarseerProjection()
        {
            return Matrix.CreateOrthographicOffCenter(-25 * GraphicsDevice.Viewport.AspectRatio,
                                                            25 * GraphicsDevice.Viewport.AspectRatio, -25, 25, -1, 1);
        }

        private Matrix getScreenProjection()
        {
            return Matrix.CreateOrthographicOffCenter(0f, graphicsDevice.Viewport.Width,
                                                                         graphicsDevice.Viewport.Height, 0f, -1f, 1f);
        }

    }
}

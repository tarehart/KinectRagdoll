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

namespace KinectRagdoll
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class KinectRagdollGame : Microsoft.Xna.Framework.Game
    {

        public static int WIDTH = 1024;
        public static int HEIGHT = 768;

        
        public KinectManager kinectManager;
        public FarseerManager farseerManager;
        public InputManager inputManager;
        public ActionCenter actionCenter;
        public ProjectionHelper projectionHelper;
        public RagdollManager ragdollManager;
        public Toolbox toolbox;
        //public SpriteHelper spriteHelper;
        public ObjectiveManager objectiveManager;
        public PowerupManager powerupManager;
        public Jukebox jukebox;
        public HazardManager hazardManager;
        public BodySound bodySound;

        GraphicsDeviceManager graphics;
        Color bkColor;
        SpriteBatch spriteBatch;
        Matrix viewMatrix;
        Matrix projectionMatrix;
        Matrix worldMatrix;
        Matrix farseerProjection;
        Matrix farseerView;
        BasicEffect basicEffect;
        BasicEffect farseerEffect;
        VertexDeclaration vertexDeclaration;
        Model myModel;
        Model thingModel;


        public List<Action> pendingUpdates = new List<Action>();

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
            bodySound = new BodySound();

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
            viewMatrix = Matrix.CreateLookAt(new Vector3(5, 5, 5), Vector3.Zero, Vector3.Up);



            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(45),  // 45 degree angle
                (float)GraphicsDevice.Viewport.Width /
                (float)GraphicsDevice.Viewport.Height,
                1.0f, 100.0f);


            farseerView = Matrix.Identity;
            farseerProjection = Matrix.CreateOrthographicOffCenter(-25 * GraphicsDevice.Viewport.AspectRatio,
                                                            25 * GraphicsDevice.Viewport.AspectRatio, 25, -25, 0, 1);

            
            farseerManager.setProjection(farseerProjection);
            projectionHelper = new ProjectionHelper(GraphicsDevice.Viewport, farseerProjection * Matrix.CreateScale(1, -1, 1));

        


        }

        private void InitializeEffect()
        {

            vertexDeclaration = new VertexDeclaration(new VertexElement[]
                {
                    new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                    new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
                    new VertexElement(24, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
                }
            );

            farseerEffect = new BasicEffect(graphics.GraphicsDevice);
            


            farseerEffect.Projection = farseerProjection;
            farseerEffect.View = farseerView;

            
            basicEffect = new BasicEffect(graphics.GraphicsDevice);


            basicEffect.AmbientLightColor = new Vector3(0.1f, 0.1f, 0.1f);
            basicEffect.DiffuseColor = new Vector3(1.0f, 1.0f, 1.0f);
            basicEffect.SpecularColor = new Vector3(0.25f, 0.25f, 0.25f);
            basicEffect.SpecularPower = 5.0f;
            basicEffect.Alpha = 1.0f;

            basicEffect.DirectionalLight0.Enabled = true;
            basicEffect.DirectionalLight0.Direction = new Vector3(0, 0, -1);

            basicEffect.LightingEnabled = true;
            basicEffect.TextureEnabled = false;

            basicEffect.View = viewMatrix;
            basicEffect.Projection = projectionMatrix;

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

            
            ragdollManager.LoadContent(Content);
            farseerManager.LoadContent();
            objectiveManager.LoadContent(Content);
            toolbox.LoadContent();
            Jukebox.LoadContent(Content);
            bodySound.LoadContent(Content);
            bodySound.Start();

            InitializeTransform();
            InitializeEffect();
            
            
            kinectManager.InitKinect();
            //kinectManager.initDepthTex();

            //ragdollManager.ragdoll.setDepthTex(kinectManager.depthTex);

            base.LoadContent();
            
            
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            //if (farseerManager.createNew)
            //{
            //    Serializer.Save(farseerManager.world, this, "save.xml");
            //}
        }


        

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            foreach (Action a in pendingUpdates)
            {
                a();
            }

            pendingUpdates.Clear();

            inputManager.Update();
            ragdollManager.Update(kinectManager.skeletonInfo);
            farseerManager.Update(gameTime);
            objectiveManager.Update();
            hazardManager.Update();
            bodySound.Update(kinectManager.skeletonInfo);

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
            
            SetupRendering();
            

            RenderTarget2D renderTarget = RenderFarseerEffectsTexture();

            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, kinectManager.bkColor, 1.0f, 0);

            //DrawHeadTrackingDemo(gameTime);

            DrawSprites(renderTarget);

            projectionHelper.Update(farseerView);

            base.Draw(gameTime);


            
            
        }

       

        private void DrawSprites(RenderTarget2D renderTarget)
        {
            BlendState b = new BlendState();
            
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            //spriteBatch.Draw(kinectManager.depthTex, new Rectangle(GraphicsDevice.PresentationParameters.BackBufferWidth - 640, 0, 640, 480), new Color(1, 1, 1, .5f));
            farseerManager.DrawBasics(ref farseerView);
            spriteBatch.Draw(renderTarget, new Vector2(0, 0), null, Color.White, 0, new Vector2(0, 0), 1, SpriteEffects.FlipVertically, 1);
            toolbox.Draw(spriteBatch);
            farseerManager.DrawFrontEffects(spriteBatch);
            objectiveManager.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void SetupRendering()
        {


            farseerView = createFarseerView();

            farseerEffect.Projection = farseerProjection;
            farseerEffect.View = farseerView;

            // Make sure this is enabled!
            farseerEffect.TextureEnabled = true;
            farseerEffect.VertexColorEnabled = true;



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
                center = projectionHelper.PixelToFarseer(inputManager.inputHelper.MousePosition);
                horizontalBound = 35;
                verticalBound = 25;
            }

            //Rectangle safeZone = new Rectangle(-20, -15, 40, 30);
            

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


        private RenderTarget2D RenderFarseerEffectsTexture()
        {
            RenderTarget2D renderTarget;
            PresentationParameters pp = GraphicsDevice.PresentationParameters;
            renderTarget = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);

            GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Texture, null, null, null, null, farseerEffect);

            ragdollManager.Draw(spriteBatch);
            powerupManager.Draw(spriteBatch);

            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);
            return renderTarget;
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

            farseerProjection = Matrix.CreateOrthographicOffCenter(-25 * GraphicsDevice.Viewport.AspectRatio,
                                                            25 * GraphicsDevice.Viewport.AspectRatio, 25, -25, 0, 1);

            farseerManager.setProjection(farseerProjection);
            farseerEffect.Projection = farseerProjection;

            projectionHelper = new ProjectionHelper(graphicsDevice.Viewport, farseerProjection * Matrix.CreateScale(1, -1, 1));
        }

    }
}

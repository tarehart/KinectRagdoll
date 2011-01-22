using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using KinectTest2.Kinect;
using KinectTest2.Sandbox;
using ManagedNite;

namespace KinectTest2
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class KinectRagdollGame : Microsoft.Xna.Framework.Game
    {

        public static int WIDTH = 1280;
        public static int HEIGHT = 960;

        
        public KinectManager kinectManager;
        public FarseerManager farseerManager;
        public InputManager inputManager;
        public ProjectionHelper projectionHelper;
        public RagdollManager ragdollManager;

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
        Texture2D selectionRec;

        

        public static GraphicsDevice graphicsDevice;
        

        public KinectRagdollGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = WIDTH;
            graphics.PreferredBackBufferHeight = HEIGHT;

           
            Content.RootDirectory = "Content";
            kinectManager = new KinectManager();
            farseerManager = new FarseerManager(true, this);
            ragdollManager = new RagdollManager();
            inputManager = new InputManager(this);
            

            this.IsMouseVisible = true;
            bkColor = Color.CornflowerBlue;

            

            //KinectTest.test();
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

            myModel = Content.Load<Model>("Models\\cube");
            thingModel = Content.Load<Model>("Models\\thing");
            selectionRec = Content.Load<Texture2D>("Materials\\squares");

            ragdollManager.LoadContent(Content);
            farseerManager.LoadContent();


            InitializeTransform();
            InitializeEffect();
            
            
            kinectManager.InitKinect();
            ragdollManager.Init(farseerManager.world, kinectManager);

            
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            if (farseerManager.createNew)
            {
                Serializer.Save(farseerManager.world, this);
            }
            kinectManager.Close();
        }


        

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            inputManager.Update();
            ragdollManager.Update(kinectManager.skeletonInfo);
            farseerManager.Update(gameTime);

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

            DrawHeadTrackingDemo(gameTime);

            DrawSprites(renderTarget);


            base.Draw(gameTime);
        }

        private void DrawSprites(RenderTarget2D renderTarget)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(kinectManager.depthTex, new Rectangle(50, 50, 640, 480), Color.White);
            farseerManager.DrawBasics();
            spriteBatch.Draw(renderTarget, new Vector2(0, 0), null, Color.White, 0, new Vector2(0, 0), 1, SpriteEffects.FlipVertically, 1);
            if (inputManager.selectingRectangle)
                spriteBatch.Draw(selectionRec, inputManager.selectionRectangle, new Color(100, 100, 255, 100));
            farseerManager.DrawFrontEffects(spriteBatch);
            spriteBatch.End();
        }

        private void SetupRendering()
        {
            

            farseerEffect.Projection = farseerProjection;
            farseerEffect.View = farseerView;

            // Make sure this is enabled!
            farseerEffect.TextureEnabled = true;
            farseerEffect.VertexColorEnabled = true;


            Vector3 headLoc = kinectManager.skeletonInfo.head;
            Vector3 screenCenter = new Vector3(-250, 100, 350);

            Vector3 screenToHead = headLoc - screenCenter;

            screenToHead *= .01f;
            screenToHead.Z *= 3;

            //headLoc.X += 250;
            //headLoc.Y -= 100;
            //headLoc.Z *= 2;

            //headLoc *= .01f;


            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                (float)Math.Atan(10 / screenToHead.Z),
                (float)GraphicsDevice.Viewport.Width /
                (float)GraphicsDevice.Viewport.Height,
                1.0f, 100.0f);

            viewMatrix = Matrix.CreateLookAt(screenToHead, Vector3.Zero, Vector3.Up);
        }

        private void DrawHeadTrackingDemo(GameTime gametime)
        {
            DepthStencilState depthState = new DepthStencilState();
            depthState.DepthBufferEnable = true;
            depthState.DepthBufferWriteEnable = true;
            depthState.StencilEnable = true;

            GraphicsDevice.DepthStencilState = depthState;

            DrawMesh(thingModel, new Vector3(0, 0, -5), 1);

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    DrawMesh(myModel, new Vector3(i, j, (float) Math.Sin(gametime.TotalGameTime.TotalMilliseconds / 1000f) * (i + j * 2) - 4), .2f);
                }
            }
            
            //DrawCube(new Vector3(0, 0, -5));

            //DrawCube(new Vector3(0, 0, 0));

            //DrawCube(new Vector3(2, 0, 5));

            //DrawCube((kinectManager.skeletonInfo.rightHand - kinectManager.skeletonInfo.head) * .02f + new Vector3(-4, 4, 10));
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
            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);
            return renderTarget;
        }

        private void DrawMesh(Model m, Vector3 location, float scale)
        {
            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in m.Meshes)
            {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = Matrix.CreateRotationX(MathHelper.ToRadians(-90)) * Matrix.CreateScale(scale) * Matrix.CreateTranslation(location);
                    effect.View = viewMatrix;
                    effect.Projection = projectionMatrix;
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }

        }

        //private void DrawCube(Vector3 location, float scale)
        //{
        //    // Draw the model. A model can have multiple meshes, so loop.
        //    foreach (ModelMesh mesh in myModel.Meshes)
        //    {
        //        // This is where the mesh orientation is set, as well 
        //        // as our camera and projection.
        //        foreach (BasicEffect effect in mesh.Effects)
        //        {
        //            effect.EnableDefaultLighting();
        //            effect.World = Matrix.CreateScale(scale) * Matrix.CreateTranslation(location);
        //            effect.View = viewMatrix;
        //            effect.Projection = projectionMatrix;
        //        }
        //        // Draw the mesh, using the effects set above.
        //        mesh.Draw();
        //    }
        //}
    }
}

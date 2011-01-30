using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics.Joints;
using KinectRagdoll.Sandbox;
using FarseerPhysics.DebugViews;

namespace KinectRagdoll.Kinect
{
    public class FarseerManager
    {

        public World world;
        private DebugViewXNA debugview;
        private Matrix projection;
        public HashSet<Joint> selectedJoints = new HashSet<Joint>();
        public HashSet<Joint> pendingJoints = new HashSet<Joint>();
        private KinectRagdollGame game;
        private Texture2D pointTex;

        private Random rand;

        public static FarseerManager Main = null;

        public bool createNew = true;

        public FarseerManager(bool main, KinectRagdollGame game)
        {
            
            //world.ContactManager = new ContactManager();

            world = new World(new Vector2(0, -20));

            

            debugview = new DebugViewXNA(world);
            debugview.Flags = FarseerPhysics.DebugViewFlags.TexturedShape;

            //World loaded = Serializer.readFromDataContract("graph.xml");
            //world.JointList.AddRange(loaded.JointList);
            //world.BodyList.AddRange(loaded.BodyList);

            


            this.game = game;
            //world = new World(new Vector2(0, 0));
            
            




            rand = new Random();

            if (main)
            {
                Main = this;
            }

        }



        public void LoadContent()
        {

            
            debugview.LoadContent(game.GraphicsDevice, game.Content);

            if (!createNew)
            {
                LoadWorld("save.xml");
            }
            else
            {
                game.ragdollManager.CreateNewRagdoll(game);
                addBounds();
                addSpinningDeath();
            }

            //projection = Matrix.CreateOrthographicOffCenter(0, device.Viewport.Width, device.Viewport.Height, 0, -1, 1);
            pointTex = game.Content.Load<Texture2D>("Materials\\target");
            
            
        }

        public void LoadWorld(String filename)
        {

            SaveFile sf = Serializer.readFromDataContract(filename);
            world = new World(sf.gravity);
            debugview.AttachToWorld(world);
            sf.PopulateWorld(world);
            game.ragdollManager.ragdoll = sf.ragdoll;
            game.ragdollManager.ragdoll.PostLoad(world);
        }

       

        public void addBounds()
        {
            int thickness = 5;

            DebugMaterial material = new DebugMaterial(MaterialType.Waves)
            {
                Color = Color.OliveDrab,
                Scale = 4
            };
            FixtureFactory.CreateRectangle(world, 70, thickness, 1, new Vector2(0, -25), material);
            FixtureFactory.CreateRectangle(world, 70, thickness, 1, new Vector2(0, 25), material);
            FixtureFactory.CreateRectangle(world, thickness, 50, 1, new Vector2(35, 0), material);
            FixtureFactory.CreateRectangle(world, thickness, 50, 1, new Vector2(-35, 0), material);


        }

        public void addSpinningDeath()
        {
            DebugMaterial material = new DebugMaterial(MaterialType.Waves)
            {
                Color = Color.OliveDrab,
                Scale = 4
            };
            Fixture rec = FixtureFactory.CreateRectangle(world, 20, 2, 1, new Vector2(10, 0), material);
            rec.Body.BodyType = BodyType.Dynamic;

            FixedRevoluteJoint joint = new FixedRevoluteJoint(rec.Body, Vector2.Zero, new Vector2(20, 0));
            joint.MotorEnabled = true;
            joint.MotorSpeed = 6;
            joint.MaxMotorTorque = 10000;
            joint.MotorTorque = 10000;
            world.AddJoint(joint);
        }


        public void dropBall()
        {
            int x = rand.Next(KinectRagdollGame.WIDTH);

            DebugMaterial material = new DebugMaterial(MaterialType.Waves)
            {
                Color = Color.OliveDrab,
                Scale = 50
            };

            Fixture f = FixtureFactory.CreateCircle(world, 20, 1, new Vector2(x, 50), material);
            f.Body.Position = new Vector2(x, 50);
            f.Body.BodyType = BodyType.Dynamic;
            f.Restitution = .9f;
            


        }


        public void Update(GameTime gameTime)
        {


            world.Step(Math.Min((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f, (1f / 30f)));
            debugview.Update(gameTime);
            
        }

        public void DrawBasics(ref Matrix view)
        {

            
            debugview.RenderDebugData(ref projection, ref view);
            
        }

        public void DrawFrontEffects(SpriteBatch sb)
        {
            DrawSelectedJoints(sb);

        }

        private void DrawSelectedJoints(SpriteBatch sb)
        {
            foreach (Joint j in selectedJoints)
            {
                Vector2 pixelLoc = game.projectionHelper.FarseerToPixel(j.WorldAnchorA);
                sb.Draw(pointTex, pixelLoc, null, Color.Red, 0, Vector2.One * pointTex.Width / 2, .2f, SpriteEffects.None, 0);

            }

            foreach (Joint j in pendingJoints)
            {
                Vector2 pixelLoc = game.projectionHelper.FarseerToPixel(j.WorldAnchorA);
                sb.Draw(pointTex, pixelLoc, null, Color.Yellow, 0, Vector2.One * pointTex.Width / 2, .2f, SpriteEffects.None, 0);

            }
        }


        //public Matrix Transform { get { return projection; } }

        internal void setProjection(Matrix farseerProjection)
        {
            projection = farseerProjection * Matrix.CreateScale(1, -1, 1);
        }
    }
}

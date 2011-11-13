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
using FarseerPhysics.Common.PolygonManipulation;
using KinectRagdoll.Hazards;

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
            debugview.Flags = FarseerPhysics.DebugViewFlags.TexturedShape | FarseerPhysics.DebugViewFlags.RagdollCustom;

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
                addTurret();
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
            game.ragdollManager.ragdoll.Init(world);
            //game.ragdollManager.ragdoll.setDepthTex(game.kinectManager.depthTex);
            game.objectiveManager.SetObjectives(sf.objectives);
            game.powerupManager.LoadPowerups(sf.powerups);
        }

       

        public void addBounds()
        {
            int thickness = 5;


            Body b = new Body(world);

            FarseerTextures.ApplyTexture(
                FixtureFactory.AttachRectangle(70, thickness, 1, new Vector2(0, -25), b),
                FarseerTextures.TextureType.Normal);

            FarseerTextures.ApplyTexture(
                FixtureFactory.AttachRectangle(70, thickness, 1, new Vector2(0, 25), b),
                FarseerTextures.TextureType.Normal);

            FarseerTextures.ApplyTexture(
                FixtureFactory.AttachRectangle(thickness, 50, 1, new Vector2(35, 0), b),
                FarseerTextures.TextureType.Normal);

            FarseerTextures.ApplyTexture(
                FixtureFactory.AttachRectangle(thickness, 50, 1, new Vector2(-35, 0), b),
                FarseerTextures.TextureType.Normal);


        }

        public void addSpinningDeath()
        {

            Body b = new Body(world);
            Fixture rec = FixtureFactory.AttachRectangle(20, 2, 1, Vector2.Zero, b);
            b.Position = new Vector2(10, 0);
            
            rec.Body.BodyType = BodyType.Dynamic;
            FarseerTextures.ApplyTexture(rec, FarseerTextures.TextureType.Normal);

            FixedRevoluteJoint joint = new FixedRevoluteJoint(rec.Body, Vector2.Zero, new Vector2(20, 0));
            joint.MotorEnabled = true;
            joint.MotorSpeed = 6;
            joint.MaxMotorTorque = 10000;
            joint.MotorTorque = 10000;
            world.AddJoint(joint);
        }

        private void addTurret()
        {
            Turret t = new Turret(new Vector2(10, 0), world, game.ragdollManager);
            game.hazardManager.addHazard(t);

            Turret t2 = new Turret(new Vector2(-10, 0), world, game.ragdollManager);
            game.hazardManager.addHazard(t2);
        }



        public void Update(GameTime gameTime)
        {

            if (world.Enabled)
            {
                world.Step(Math.Min((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f, (1f / 30f)));
                debugview.Update(gameTime);
            }
            
        }

        public void DrawBasics(ref Matrix view)
        {

            if (world.Enabled)
            {
                debugview.RenderDebugData(ref projection, ref view);
            }
            
        }

        public void DrawFrontEffects(SpriteBatch sb)
        {
            if (world.Enabled)
            {
                DrawSelectedJoints(sb);
            }

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

        internal void Pause()
        {
            world.Enabled = false;
        }

        internal void Resume()
        {
            world.Enabled = true;
        }

        internal void Explosion(Vector2 loc)
        {
            CuttingTools.Cut(world, loc + Vector2.UnitX + Vector2.UnitY, loc - Vector2.UnitX - Vector2.UnitY, 0.01f);
            CuttingTools.Cut(world, loc - Vector2.UnitX + Vector2.UnitY, loc + Vector2.UnitX - Vector2.UnitY, 0.01f);
        }
    }
}

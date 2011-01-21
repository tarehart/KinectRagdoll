using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using FarseerPhysics.DebugViews;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics.Joints;
using KinectTest2.Sandbox;

namespace KinectTest2.Kinect
{
    public class FarseerManager
    {

        public World world;
        private DebugViewXNA debugview;
        private KinectManager kinectManager;
        private Matrix projection;
        private Fixture jointCursor;
        private Joint cursedJoint;

        private Random rand;

        public static FarseerManager Main = null;

        public FarseerManager(bool main)
        {
            world = new World(new Vector2(0, -15));
            //world = new World(new Vector2(0, 0));
            debugview = new DebugViewXNA(world);
            debugview.Flags = FarseerPhysics.DebugViewFlags.TexturedShape;
            rand = new Random();

            if (main)
            {
                Main = this;
            }

        }



        public void LoadContent(GraphicsDevice device, ContentManager content, KinectManager kinectManager)
        {

            this.kinectManager = kinectManager;
            debugview.LoadContent(device, content);

            //projection = Matrix.CreateOrthographicOffCenter(0, device.Viewport.Width, device.Viewport.Height, 0, -1, 1);

            
            addBounds();
            addSpinningDeath();
        }

        public void setJointCursor(Joint j)
        {
            DebugMaterial material = new DebugMaterial(MaterialType.Waves)
            {
                Color = Color.Red,
                Scale = 4
            };
            if (jointCursor != null) {
                world.RemoveBody(jointCursor.Body);
            }
            jointCursor = FixtureFactory.CreateCircle(world, 1, 0, material);
            jointCursor.CollisionFilter.CollidesWith = Category.None;
            cursedJoint = j;
            
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
            int x = rand.Next(Game1.WIDTH);

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
            if (jointCursor != null && cursedJoint != null)
            {
                jointCursor.Body.Position = cursedJoint.WorldAnchorA;
            }
        }

        public void Draw()
        {

            
            debugview.RenderDebugData(ref projection);
            
        }


        //public Matrix Transform { get { return projection; } }

        internal void setProjection(Matrix farseerProjection)
        {
            projection = farseerProjection * Matrix.CreateScale(1, -1, 1);
        }
    }
}

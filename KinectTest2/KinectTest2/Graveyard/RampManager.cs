using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using FarseerPhysics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.DebugViews;

namespace KinectTest2.Kinect
{
    class RampManager
    {


        private int PIX_PER_LINE = 60;
        private int MAX_DESCENT = 1000;
       
        
        private World world;
        private KinectManager kinectManager;

        private List<Fixture> lines = new List<Fixture>();
        private Fixture polygon;


        public void Init(World world, KinectManager kinectManager)
        {

            this.world = world;
            this.kinectManager = kinectManager;

            //kinectManager.DataPrepared += new KinectManager.DataPreparedHandler(kinectManager_DataPrepared);
            
            //for (int i = 0; i < Game1.WIDTH; i += PIX_PER_LINE)
            //{
            //    Vector2 start = new Vector2(i, 450);
            //    Vector2 end = new Vector2(i + PIX_PER_LINE, 450);

            //    Body body = BodyFactory.CreateBody(world);
            //    body.BodyType = BodyType.Static;
            //    body.Mass = 1;
            //    EdgeShape edgeShape = new EdgeShape(start, end);
            //    Fixture f = body.CreateFixture(edgeShape, null);

            //    lines.Add(f);
            //}

        }

        void kinectManager_DataPrepared()
        {
            //UpdateRamp(kinectManager.topStripe);
        }



        private int StripeIndex(int seg, int stripeLength)
        {
            int x = seg * PIX_PER_LINE;
            return x * stripeLength / KinectRagdollGame.WIDTH ;
        }


        public void UpdatePolygon(int[] ts)
        {
            if (ts == null) return;

            int[] topStripe = new int[ts.Length];

            Array.Copy(ts, topStripe, ts.Length);

            Vertices verts = new Vertices();

            for (int i = 0; i < topStripe.Length; i += 5)
            {
                int x = i * KinectRagdollGame.WIDTH / topStripe.Length;
                int y = topStripe[i];
                verts.Add(new Vector2(x, y));
            }

            if (polygon != null)
            {

                world.RemoveBody(polygon.Body);
            }

            DebugMaterial material = new DebugMaterial(MaterialType.Waves)
            {
                Color = Color.OliveDrab,
                Scale = 50
            };

            polygon = FixtureFactory.CreateLoopShape(world, verts, 1, material);
            polygon.Restitution = .9f;
            

            
        }


        public void UpdateRamp(int[] ts)
        {

            if (ts == null) return;

            int[] topStripe = new int[ts.Length];

            Array.Copy(ts, topStripe, ts.Length);

            

            for (int i = 0; i < lines.Count; i++)
            {
                EdgeShape e = (EdgeShape)lines[i].Shape;


                if (StripeIndex(i+1, topStripe.Length) >= topStripe.Length)
                {
                    e.Vertex1.Y = topStripe[StripeIndex(i, topStripe.Length)];
                    e.Vertex2.Y = topStripe[StripeIndex(i, topStripe.Length)];
                    //moveSegment(lines[i], topStripe[StripeIndex(i, topStripe.Length)], topStripe[StripeIndex(i, topStripe.Length)]);
                    continue;
                }
                //else if (Math.Abs(topStripe[i * PIX_PER_LINE] - topStripe[(i + 1) * PIX_PER_LINE]) > MAX_DESCENT)
                //{
                //    e.Vertex1.Y = float.MaxValue;
                //    e.Vertex2.Y = float.MaxValue;
                //    continue;
                //}


                e.Vertex1.Y = topStripe[StripeIndex(i, topStripe.Length)];
                e.Vertex2.Y = topStripe[StripeIndex(i + 1, topStripe.Length)];
                //moveSegment(lines[i], topStripe[StripeIndex(i, topStripe.Length)], topStripe[StripeIndex(i + 1, topStripe.Length)]);

                if (i > 1)
                {
                    e.Vertex0 = ((EdgeShape)lines[i - 1].Shape).Vertex1;
                    ((EdgeShape)lines[i - 1].Shape).Vertex3 = e.Vertex2;
                }

                lines[i].Body.ResetMassData();
                
            }
        }

        //private void moveSegment(Fixture f, int y1, int y2)
        //{

            

        //    Vector2 pos = new Vector2(f.Body.Position.X, (y1 + y2) / 2);
        //    float rotation = (float) Math.Atan((y1 - y2) / PIX_PER_LINE);

        //    f.Body.SetTransformIgnoreContacts(ref pos, rotation);
        //}


    }
}

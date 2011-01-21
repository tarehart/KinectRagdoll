using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics;

namespace KinectTest2.Kinect
{
    class OutlineManager
    {

        private Vertices oldPoints;
        private Fixture oldObject;
        private World world;


        public OutlineManager()
        {
           
        }

        public void Init(World world)
        {
            this.world = world;
        }

        public void Update(uint[] data)
        {


            if (data == null) return;

            // get new points
            int scale = (int) Math.Sqrt(data.Length / 12);
            Vertices newPoints = PolygonTools.CreatePolygon(data, scale * 4, scale * 3, true);
            
            usePoints(newPoints);

        }

        public void Update(bool[,] data)
        {
            if (data == null) return;


            List<Vector2> verts = MarchingSquares.getVertices(data, 4, 20);
            Vertices newPoints = new Vertices(verts);

            usePoints(newPoints);
        }

        private void usePoints(Vertices newPoints)
        {
            // create a physics object for the new points
            Fixture loop = FixtureFactory.CreateLoopShape(world, newPoints, 1);

            // handle first run gracefully
            if (oldPoints != null)
            {
                // map new points to old points
                Dictionary<int, Vector2> map = createPointMapping(newPoints, oldPoints);


                // derive physics / dynamics properties from the mapping   

            }


            // store the new points as old points
            if (oldObject != null)
                world.RemoveBody(oldObject.Body);

            oldPoints = newPoints;
            oldObject = loop;

        }


        private Dictionary<int, Vector2> createPointMapping(List<Vector2> newPoints, List<Vector2> oldPoints)
        {
            Dictionary<int, Vector2> map = new Dictionary<int, Vector2>();

            for (int i = 0; i < oldPoints.Count && i < newPoints.Count; i++)
            {
                map.Add(i, oldPoints[i]);
            }

            return map;

        }

    }
}

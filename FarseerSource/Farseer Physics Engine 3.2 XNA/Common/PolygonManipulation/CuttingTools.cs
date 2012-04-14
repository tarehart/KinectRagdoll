using System.Collections.Generic;
using System.Diagnostics;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using System.Linq;
using System;
using FarseerPhysics.Collision;
using FarseerPhysics.DebugViews;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Dynamics.Contacts;

namespace FarseerPhysics.Common.PolygonManipulation
{
    public static class CuttingTools
    {
        //Cutting a shape into two is based on the work of Daid and his prototype BoxCutter: http://www.box2d.org/forum/viewtopic.php?f=3&t=1473

        /// <summary>
        /// Split a fixture into 2 vertice collections using the given entry and exit-point.
        /// </summary>
        /// <param name="fixture">The Fixture to split</param>
        /// <param name="entryPoint">The entry point - The start point</param>
        /// <param name="exitPoint">The exit point - The end point</param>
        /// <param name="splitSize">The size of the split. Think of this as the laser-width</param>
        /// <param name="left">The first collection of vertexes</param>
        /// <param name="right">The second collection of vertexes</param>
        public static void SplitShape(Fixture fixture, Vector2 entryPoint, Vector2 exitPoint, float splitSize,
                                      out Vertices left, out Vertices right)
        {
            Vector2 localEntryPoint = fixture.Body.GetLocalPoint(ref entryPoint);
            Vector2 localExitPoint = fixture.Body.GetLocalPoint(ref exitPoint);

            Vertices v = null;
            switch (fixture.Shape.ShapeType)
            {
                case ShapeType.Polygon:
                    v = (fixture.Shape as PolygonShape).Vertices;
                    break;
                case ShapeType.Circle:
                    v = new Vertices();
                    CircleShape c = fixture.Shape as CircleShape;
                    int segments = 16;
                    double increment = Math.PI * 2.0 / segments;
                    double theta = 0.0;
                    for (int i = 0; i < segments; i++)
                    {
                        v.Add(c.Position + c.Radius * new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta)));
                        theta += increment;
                    }
                    break;
                default:
                    left = new Vertices();
                    right = new Vertices();
                    return;
            }
           
            Vertices vertices = new Vertices(v);
            Vertices[] newPolygon = new Vertices[2];

            for (int i = 0; i < newPolygon.Length; i++)
            {
                newPolygon[i] = new Vertices(vertices.Count);
            }

            int[] cutAdded = { -1, -1 };
            int last = -1;
            for (int i = 0; i < vertices.Count; i++)
            {
                int n;
                //Find out if this vertex is on the old or new shape.
                if (isLeft(localEntryPoint, localExitPoint, vertices[i]))
                //if (Vector2.Dot(MathUtils.Cross(localExitPoint - localEntryPoint, 1), vertices[i] - localEntryPoint) > Settings.Epsilon)
                    n = 0;
                else
                    n = 1;

                if (last != n)
                {
                    //If we switch from one shape to the other add the cut vertices.
                    if (last == 0)
                    {
                        Debug.Assert(cutAdded[0] == -1);
                        cutAdded[0] = newPolygon[last].Count;
                        newPolygon[last].Add(localExitPoint);
                        newPolygon[last].Add(localEntryPoint);
                    }
                    if (last == 1)
                    {
                        Debug.Assert(cutAdded[last] == -1);
                        cutAdded[last] = newPolygon[last].Count;
                        newPolygon[last].Add(localEntryPoint);
                        newPolygon[last].Add(localExitPoint);
                    }
                }

                newPolygon[n].Add(vertices[i]);
                last = n;
            }

            //Add the cut in case it has not been added yet.
            if (cutAdded[0] == -1)
            {
                cutAdded[0] = newPolygon[0].Count;
                newPolygon[0].Add(localExitPoint);
                newPolygon[0].Add(localEntryPoint);
            }
            if (cutAdded[1] == -1)
            {
                cutAdded[1] = newPolygon[1].Count;
                newPolygon[1].Add(localEntryPoint);
                newPolygon[1].Add(localExitPoint);
            }

            for (int n = 0; n < 2; n++)
            {
                Vector2 offset;
                if (cutAdded[n] > 0)
                {
                    offset = (newPolygon[n][cutAdded[n] - 1] - newPolygon[n][cutAdded[n]]);
                }
                else
                {
                    offset = (newPolygon[n][newPolygon[n].Count - 1] - newPolygon[n][0]);
                }
                offset.Normalize();

                newPolygon[n][cutAdded[n]] += splitSize * offset;

                if (cutAdded[n] < newPolygon[n].Count - 2)
                {
                    offset = (newPolygon[n][cutAdded[n] + 2] - newPolygon[n][cutAdded[n] + 1]);
                }
                else
                {
                    offset = (newPolygon[n][0] - newPolygon[n][newPolygon[n].Count - 1]);
                }
                offset.Normalize();

                newPolygon[n][cutAdded[n] + 1] += splitSize * offset;
            }

            left = newPolygon[0];
            right = newPolygon[1];
        }

        private static bool isLeft(Vector2 a, Vector2 b, Fixture c)
        {
            return isLeft(a, b, c.Body.GetWorldPoint(c.Shape.MassData.Centroid));
        }

        private static bool isLeft(Vector2 a, Vector2 b, Vector2 c)
        {
            return Vector2.Dot(MathUtils.Cross(b - a, 1), c - a) > Settings.Epsilon;
        }

        public struct RayCastResult
        {
            public Fixture f;
            public float fr;
            public Vector2 p;
        }

        /// <summary>
        /// This is a high-level function to cuts fixtures inside the given world, using the start and end points.
        /// Note: We don't support cutting when the start or end is inside a shape.
        /// </summary>
        /// <param name="world">The world.</param>
        /// <param name="start">The startpoint.</param>
        /// <param name="end">The endpoint.</param>
        /// <param name="thickness">The thickness of the cut</param>
        public static Vector2 Cut(World world, Vector2 start, Vector2 end, float thickness, Category collisionCategories = Category.None)
        {

            // The left side of the cut will remain part of the existing body;
            // the right side will be made into a new body

            List<Fixture> fixtures = new List<Fixture>();
            List<Vector2> entryPoints = new List<Vector2>();
            List<Vector2> exitPoints = new List<Vector2>();



            List<RayCastResult> results = new List<RayCastResult>();
            //float blockingFraction = float.MaxValue;
            Vector2 stoppingPoint = end;

            //We don't support cutting when the start or end is inside a shape.
            //if (world.TestPoint(start) != null || world.TestPoint(end) != null)
            //    return;

            //Get the entry points
            world.RayCast((f, p, n, fr) =>
                              {
                                  RayCastResult r = new RayCastResult();
                                  r.f = f;
                                  r.p = p;
                                  r.fr = fr;
                                  results.Add(r);

                                  return 1;
                                 
                              }, start, end);


            results = results.OrderBy(p => p.fr).ToList();

            foreach (RayCastResult r in results)
            {
                if ((r.f.CollisionCategories & collisionCategories) != Category.None)
                {
                    stoppingPoint = r.p;
                    break;
                }
                if (!r.f.TestPoint(ref end))
                {
                    if (world.FixtureCut != null)
                        world.FixtureCut(r.f);
                    fixtures.Add(r.f);
                    entryPoints.Add(r.p);
                }
            }


            //Reverse the ray to get the exitpoints
            world.RayCast((f, p, n, fr) =>
                              {
                                  if (fixtures.Contains(f))
                                  {
                                      exitPoints.Add(p);
                                  }
                                  return 1;
                              }, end, start);

            Debug.Assert(entryPoints.Count == exitPoints.Count && entryPoints.Count == fixtures.Count);

            //Fixture containsEnd = world.TestPoint(end);
            //if (containsEnd != null)
            //{
            //    entryPoints.RemoveAt(0);
            //    fixtures.Remove(containsEnd);
            //}
            //Fixture containsStart = world.TestPoint(start);
            //if (containsStart != null)
            //{
            //    exitPoints.RemoveAt(exitPoints.Count - 1);
            //    fixtures.Remove(containsStart);
            //}

            //We only have a single point. We need at least 2
            if (entryPoints.Count + exitPoints.Count < 2)
                return stoppingPoint;

            var query =
                (from fix in fixtures
                select fix.Body).Distinct();

            foreach (Body b in query) 
            {

                if (b == null || b.BodyType == BodyType.Static)
                    continue;

                ContactEdge edge = b.ContactList;
                while (edge != null)
                {
                    Contact c = edge.Contact;
                    edge = edge.Next;
                    world.ContactManager.Destroy(c);
                }

                List<Body> leftBodies = new List<Body>();
                List<Body> rightBodies = new List<Body>();
                //Body rightBody = new Body(world);

                List<Joint> leftJoints = new List<Joint>();
                List<Joint> rightJoints = new List<Joint>();

                foreach (Joint j in b.JointList)
                {
                    if (isLeft(start, end, j.WorldAnchorA))
                        leftJoints.Add(j);
                    else
                        rightJoints.Add(j);
                }

                //List<Fixture> leftList = new List<Fixture>();
                //List<Fixture> rightList = new List<Fixture>();
                Fixture[] bodyFixtures = new Fixture[b.FixtureList.Count];
                b.FixtureList.CopyTo(bodyFixtures);
                b.FixtureList.Clear();
                //leftBodies.Add(b);

                // For each fixture that was sliced through...
                foreach (Fixture fix in (from f in bodyFixtures where fixtures.Contains(f) select f))
                {
                    
                    
                    int i = fixtures.IndexOf(fix);

                    // split this in half and put the halves in the over/under lists
                    Vertices first;
                    Vertices second;
                    SplitShape(fix, entryPoints[i], exitPoints[i], thickness, out first, out second);
                    if (!SanityCheck(first) || !SanityCheck(second))
                    {
                        continue;
                    }
                    PolygonShape leftShape = new PolygonShape(first, fix.Shape.Density);
                    PolygonShape rightShape = new PolygonShape(second, fix.Shape.Density);

                    if (!b.FixtureList.Any())
                    {
                        if (leftShape.MassData.Area > rightShape.MassData.Area)
                        {
                            b.CreateFixture(leftShape, fix.UserData);
                            leftBodies.Add(b);
                            GlomFixture(world, b, rightBodies, rightShape, fix.UserData, rightJoints);
                        }
                        else
                        {
                            b.CreateFixture(rightShape, fix.UserData);
                            rightBodies.Add(b);
                            GlomFixture(world, b, leftBodies, leftShape, fix.UserData, leftJoints);
                        }
                    }
                    else
                    {
                        GlomFixture(world, b, leftBodies, leftShape, fix.UserData, leftJoints);
                        GlomFixture(world, b, rightBodies, rightShape, fix.UserData, rightJoints);
                    }

                                      
                }

                // for each fixture that was NOT sliced through...
                foreach (Fixture fix in (from f in bodyFixtures where !fixtures.Contains(f) select f)) {

                    if (isLeft(start, end, fix))
                    {
                        GlomFixture(world, b, leftBodies, fix.Shape, fix.UserData, leftJoints);
                    }
                    else
                    {
                        GlomFixture(world, b, rightBodies, fix.Shape, fix.UserData, rightJoints);
                        //rightBody.CreateFixture(fix.Shape.Clone(), fix.UserData);
                    }

                    
                }

                foreach (Body bod in leftBodies.Concat(rightBodies))
                {
                    bod.ResetMassData();
                    bod.BodyType = BodyType.Dynamic;
                    bod.Rotation = b.Rotation;
                    bod.LinearVelocity = b.LinearVelocity;
                    bod.AngularVelocity = b.AngularVelocity;
                    bod.Position = b.Position;
                }

                //b.JointList = null;
                //world.RemoveBody(b);

                foreach (Fixture f in bodyFixtures)
                {
                    b.DestroyFixture(f);
                }
                world.ProcessChanges();
            }

            return stoppingPoint;

        }

        private static Fixture GlomFixture(World world, Body oldBody, List<Body> thisSideBodies, Shape shape, DebugMaterial mat, List<Joint> joints)
        {
            foreach (Body lb in thisSideBodies)
            {
                foreach (Fixture lf in lb.FixtureList)
                {
                    Manifold m = new Manifold();
                    PolygonShape newShape = shape.Clone() as PolygonShape;
                    PolygonShape existingShape = lf.Shape as PolygonShape;
                    if (newShape != null && existingShape != null)
                    {
                        Collision.Collision.CollidePolygons(ref m, newShape, ref oldBody.Xf, existingShape, ref oldBody.Xf);
                        if (m.PointCount > 0)
                        {
                            Fixture glommed = lb.CreateFixture(newShape, mat);
                            transferJoints(oldBody, joints, lb, glommed);
                            return glommed;
                        }
                    }
                }
            }

            

            Body lb2 = new Body(world);
            if (!thisSideBodies.Any())
            {
                transferJoints(oldBody, joints, lb2, null);
            }

            lb2.Position = oldBody.Position;
            lb2.Rotation = oldBody.Rotation;
            thisSideBodies.Add(lb2);
            Fixture separate = lb2.CreateFixture(shape.Clone(), mat);
            transferJoints(oldBody, joints, lb2, separate);
            return separate;
        }

        private static void transferJoints(Body oldBody, List<Joint> joints, Body lb, Fixture glommed)
        {
            if (oldBody == lb) return;

            for (int i = joints.Count - 1; i >= 0; i--)
            {
                Joint j = joints[i];
                if (j.BodyA == oldBody)
                {
                    Vector2 testPoint = new Vector2(j.WorldAnchorA.X, j.WorldAnchorA.Y);
                    if (glommed == null || glommed.TestPoint(ref testPoint))
                    {
                        oldBody.JointList.Remove(j);
                        j.BodyA = lb;
                        lb.JointList.Add(j);
                        //joints.RemoveAt(i);

                    }
                }
                else
                {
                    Vector2 testPoint = new Vector2(j.WorldAnchorB.X, j.WorldAnchorB.Y);
                    if (glommed == null || glommed.TestPoint(ref testPoint))
                    {
                        oldBody.JointList.Remove(j);
                        j.BodyB = lb;
                        lb.JointList.Add(j);
                        //joints.RemoveAt(i);
                    }
                }

            }
        }

        private static bool SanityCheck(Vertices vertices)
        {
            if (vertices.Count < 3)
                return false;

            if (vertices.GetArea() < 0.00001f)
                return false;

            for (int i = 0; i < vertices.Count; ++i)
            {
                int i1 = i;
                int i2 = i + 1 < vertices.Count ? i + 1 : 0;
                Vector2 edge = vertices[i2] - vertices[i1];
                if (edge.LengthSquared() < Settings.Epsilon * Settings.Epsilon)
                    return false;
            }

            for (int i = 0; i < vertices.Count; ++i)
            {
                int i1 = i;
                int i2 = i + 1 < vertices.Count ? i + 1 : 0;
                Vector2 edge = vertices[i2] - vertices[i1];

                for (int j = 0; j < vertices.Count; ++j)
                {
                    // Don't check vertices on the current edge.
                    if (j == i1 || j == i2)
                    {
                        continue;
                    }

                    Vector2 r = vertices[j] - vertices[i1];

                    // Your polygon is non-convex (it has an indentation) or
                    // has colinear edges.
                    float s = edge.X * r.Y - edge.Y * r.X;

                    if (s < 0.0f)
                        return false;
                }
            }

            return true;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KinectRagdoll.Ragdoll;
using FarseerPhysics.Dynamics;
using FarseerPhysics.DebugViews;
using Microsoft.Xna.Framework;
using KinectRagdoll.Kinect;
using FarseerPhysics.Factories;
using FarseerPhysics.Common;
using KinectRagdoll.Farseer;

namespace KinectRagdoll.Hazards
{
    class WallPopper : Hazard
    {
        private RagdollMuscle target;
        protected World world;
        protected Body body;
        private float pendingRotation;
        private bool isPendingAttach;

        public WallPopper(Vector2 farseerLoc, World w, RagdollManager r)
        {
            DebugMaterial gray = new DebugMaterial(MaterialType.Blank)
            {
                Color = Color.DarkGray
            };

            body = new Body(w);
            body.Rotation = -(float)Math.PI / 2;
            Vertices popperShape = new Vertices(new Vector2[] {new Vector2(-1.2f, .8f), new Vector2(-1.2f, -.8f), new Vector2(1.2f, -.4f), new Vector2(1.2f, .4f)});
            FixtureFactory.AttachPolygon(popperShape, 1, body, gray);
            body.Position = farseerLoc;
            body.BodyType = BodyType.Dynamic;
            body.IsBullet = true;
            //b.CollidesWith = Category.None;

            Init(w, r);

        }

        public override void Init(FarseerPhysics.Dynamics.World w, Kinect.RagdollManager r)
        {
            world = w;
            target = r.ragdoll;
            body.setWorld(w);

            body.OnCollision += new OnCollisionEventHandler(body_OnCollision);

            world.ProcessChanges();
            IsOperational = true;
        }

        bool body_OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            if (body.BodyType == BodyType.Static) return true;

            Fixture surface;
            if (fixtureA.Body == body)
                surface = fixtureB;
            else
                surface = fixtureA;

            if (surface.Body.BodyType == BodyType.Static && surface.Shape.MassData.Area > 100)
            {
                // rotate to contact normal
                Vector2 normal = contact.Manifold.LocalNormal;

                if (Vector2.Dot(normal, body.LinearVelocity) > 0)
                {
                    return false;
                }

                isPendingAttach = true;
                pendingRotation = (float)Math.Atan2(normal.Y, normal.X);

                body.CollidesWith = Category.None;

                return false;
            }

            return true;
        }


        public override void Update()
        {
            if (body == null || !world.BodyList.Contains(body))
            {
                IsOperational = false;
            }

            if (isPendingAttach)
            {
                body.BodyType = BodyType.Static;
                body.CollidesWith = Category.All;
                body.Rotation = pendingRotation;
                isPendingAttach = false;
            }
            if (body.BodyType == BodyType.Static)
            {
                // we're theoretically clinging to a wall.
                Vector2 aimVector = new Vector2((float)Math.Cos(body.Rotation), (float)Math.Sin(body.Rotation));

                if (FarseerHelper.hasLineOfSight(body.Position, body.Position + aimVector * 100, f => target.OwnsFixture(f), world))
                {
                    JumpOff();
                }

            }
        }

        private void JumpOff()
        {
            body.BodyType = BodyType.Dynamic;

            body.LinearVelocity = new Vector2((float)Math.Cos(body.Rotation), (float)Math.Sin(body.Rotation)) * 100;
            
            //body.ApplyLinearImpulse(new Vector2((float)Math.Cos(body.Rotation), (float)Math.Sin(body.Rotation)) * 200);

        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            // do nothing
        }
    }
}

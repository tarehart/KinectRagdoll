using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using KinectRagdoll.Ragdoll;
using FarseerPhysics.Factories;
using KinectRagdoll.MyMath;
using FarseerPhysics.Common.PhysicsLogic;
using FarseerPhysics.DebugViews;
using KinectRagdoll.Sandbox;
using FarseerPhysics.Common.PolygonManipulation;

namespace KinectRagdoll.Hazards
{
    class Rocket
    {
        [DataMember()]
        protected Body body;

        [DataMember()]
        protected Fixture pivot;

        protected RagdollBase ragdoll;
        protected World world;

        protected float rotationSpeed = 1f;

        public Rocket(Vector2 farseerLoc, World w, RagdollBase r)
        {
            ragdoll = r;
            world = w;
            Alive = true;

            DebugMaterial gray = new DebugMaterial(MaterialType.Blank)
            {
                Color = Color.DarkGray
            };
            body = new Body(w);
            pivot = FixtureFactory.AttachEllipse(.8f, .2f, 8, 1, body, gray);

            body.Position = farseerLoc;
            body.BodyType = BodyType.Dynamic;
            body.IgnoreGravity = true;

            Vector2 toRagdoll = r.Body.Position - farseerLoc;
            body.Rotation = (float) Math.Atan2(toRagdoll.Y, toRagdoll.X);

            body.OnCollision += new OnCollisionEventHandler(body_OnCollision);

        }

        bool body_OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            Explode();
            return false;
        }

        private void Explode()
        {
            if (Alive)
            {
                Vector2 explosionPoint = body.Position;
                world.RemoveBody(body);
                world.ProcessChanges();
                body = null;

                CuttingTools.Cut(world, explosionPoint + new Vector2(2, 2), explosionPoint - new Vector2(2, 2), 0.01f);
                //world.ProcessChanges();
                CuttingTools.Cut(world, explosionPoint + new Vector2(-2, 2), explosionPoint - new Vector2(-2, 2), 0.01f);
                //world.ProcessChanges();

                Explosion e = new Explosion(world);
                e.Activate(explosionPoint, 2, 10);
                
                Alive = false;
                
                pivot = null;

                Vector2 screenLoc = ProjectionHelper.FarseerToPixel(explosionPoint);
                ParticleEffectManager.explosionEffect.Trigger(screenLoc);
            }
        }

        public Vector2 AimVector
        {
            get {
                if (Alive)
                {
                    return new Vector2((float)Math.Cos(body.Rotation), (float)Math.Sin(body.Rotation));
                }
                return new Vector2();
            }
        }


        public void Update()
        {
            if (Alive)
            {
                Vector2 toRagdoll = ragdoll.Body.Position - body.Position;
                float targetAngle = (float)Math.Atan2(toRagdoll.Y, toRagdoll.X);
                float radDiff = MathHelp.getRadDiff(body.Rotation, targetAngle);

                body.ApplyAngularImpulse(MathHelp.clamp(radDiff, rotationSpeed, -rotationSpeed));
                body.ApplyForce(AimVector * 20);
            }
        }

        public void Draw()
        {
            Vector2 aim = AimVector;
            Vector2 screenLoc = ProjectionHelper.FarseerToPixel(body.Position - aim * .5f);
            ParticleEffectManager.flameEffect[0].ReleaseImpulse = aim * -60;
            ParticleEffectManager.flameEffect[0].ReleaseScale.Value = 40;
            ParticleEffectManager.flameEffect.Trigger(screenLoc);
        }

        public bool Alive { get; private set; }
    }
}

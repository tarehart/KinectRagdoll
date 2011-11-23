using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Factories;
using FarseerPhysics.DebugViews;
using KinectRagdoll.Ragdoll;
using KinectRagdoll.MyMath;
using KinectRagdoll.Equipment;
using KinectRagdoll.Kinect;
using System.Runtime.Serialization;

namespace KinectRagdoll.Hazards
{
    [DataContract(Name = "Turret", Namespace = "http://www.imcool.com")]
    class Turret : Hazard
    {
        
        private float rotation;
        private static int fireInterval = 10;
        private int fireClock;
        private static int reloadTime = 50;
        private int reloadClock;
        private static int fireRange = 20;
        private static float rotationSpeed = .1f;

        private World world;
        [DataMember()]
        private Fixture pivot;
        [DataMember()]
        private Fixture barrel;
        private RagdollBase target;
        private State state;
        private int fireCount;
        private static int burstNum = 3;
        private static float fireVel = 1;
        private static float barrelLength = 2;

        public enum State
        {
            Scanning,
            Tracking,
            Firing
        }

        public Turret(Vector2 farseerLoc, World w, RagdollManager r)
        {

            DebugMaterial gray = new DebugMaterial(MaterialType.Blank)
            {
                Color = Color.DarkGray
            };

            Body b = new Body(w);
            pivot = FixtureFactory.AttachCircle(.9f, 1, b, gray);
            barrel = FixtureFactory.AttachRectangle(barrelLength, .5f, 1, new Vector2(barrelLength / 2, 0), b, gray);
            b.Position = farseerLoc;
            b.CollidesWith = Category.None;

            Init(w, r);
        }

        public override void Init(World w, RagdollManager r)
        {
            world = w;
            target = r.ragdoll;

            state = State.Scanning;
        }

        private bool inRange()
        {
            return Vector2.Distance(target.Position, pivot.Body.Position) < fireRange;
        }

        public override void Update()
        {
            switch (state) {
                case State.Scanning:
                    reloadClock--;
                    if (inRange())
                    {
                        state = State.Tracking;
                    }
                    break;
                case State.Tracking:
                    reloadClock--;
                    if (!inRange())
                    {
                        state = State.Scanning;
                    }
                    
                    Vector2 targetVector = target.Position - pivot.Body.Position;
                    float targetAngle = (float) Math.Atan2(targetVector.Y, targetVector.X);
                    float radDiff = MathHelp.getRadDiff(rotation, targetAngle);
                    if (Math.Abs(radDiff) > rotationSpeed)
                    {
                        rotation += MathHelp.clamp(radDiff, rotationSpeed, -rotationSpeed);
                    }
                    else
                    {
                        rotation += radDiff;
                        if (reloadClock <= 0)
                        {
                            state = State.Firing;
                            fireCount = 0;
                            fireClock = 0;
                        }
                    }

                    pivot.Body.Rotation = rotation;

                    break;
                case State.Firing:
                    if (fireClock == 0)
                    {
                        fire();
                        if (fireCount == burstNum)
                        {
                            state = State.Tracking;
                            reloadClock = reloadTime;
                        }
                        else
                            fireClock = fireInterval; // prepare to fire again
                    }
                    else
                    {
                        fireClock--;
                    }
                    break;
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            //throw new NotImplementedException();
        }

        private void fire()
        {
            Vector2 fireVec = new Vector2((float) Math.Cos(rotation), (float) Math.Sin(rotation));
            Vector2 fireLoc = pivot.Body.Position + fireVec * (barrelLength + .3f);
            new PunchGuns.PunchBullet(fireLoc, Vector2.Normalize(fireVec) * fireVel, world);
            fireCount++;
        }

    }
}

﻿using System;
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
using FarseerPhysics.Dynamics.Joints;
using KinectRagdoll.Farseer;

namespace KinectRagdoll.Hazards
{
    [DataContract(Name = "Turret", Namespace = "http://www.imcool.com")]
    [KnownType(typeof(GunTurret))]
    [KnownType(typeof(LaserTurret))]
    [KnownType(typeof(RocketTurret))]
    public abstract class Turret : Hazard
    {
        
        //protected float rotation;
        protected static int reloadTime = 80;
        private int reloadClock;
        private static int fireRange = 20;
        private static float rotationSpeed = .1f;

        protected World world;
        [DataMember()]
        protected Body body;
        [DataMember()]
        protected Fixture pivot;
        [DataMember()]
        protected MotorJoint motor;
        protected RagdollBase target;
        protected State state;

        protected static float barrelLength = 2;

        public enum State
        {
            Scanning,
            Tracking,
            Firing
        }

        public Turret(Vector2 farseerLoc, World w, RagdollManager r) : this(farseerLoc, w, r, null)
        {
        }

        public Turret(Vector2 farseerLoc, World w, RagdollManager r, Fixture f)
        {
            

            DebugMaterial gray = new DebugMaterial(MaterialType.Blank)
            {
                Color = Color.DarkGray
            };

            body = new Body(w);
            pivot = FixtureFactory.AttachCircle(.9f, 1, body, gray);
            FixtureFactory.AttachRectangle(barrelLength, .5f, 1, new Vector2(barrelLength / 2, 0), body, gray);
            body.Position = farseerLoc;
            body.BodyType = BodyType.Dynamic;
            //b.CollidesWith = Category.None;

            if (f == null)
            {

                motor = JointFactory.CreateFixedRevoluteJoint(w, body, Vector2.Zero, farseerLoc);
            }
            else
            {
                motor = new RevoluteJoint(body, f.Body, Vector2.Zero, f.Body.GetLocalPoint(farseerLoc));
                w.AddJoint(motor);
            }

            motor.MotorEnabled = true;
            motor.MaxMotorTorque = 5000;

            Init(w, r);
        }

        public override void Init(World w, RagdollManager r)
        {
            world = w;
            target = r.ragdoll;
            body.setWorld(w);


            IsOperational = true;
            

            state = State.Scanning;
        }

        private bool inRange()
        {
            return Vector2.Distance(target.Position, body.Position) < fireRange;
        }

        public override void Update()
        {
            if (pivot.Body == null || !world.BodyList.Contains(pivot.Body))
            {
                IsOperational = false;
            } 
            else {
                switch (state) {
                    case State.Scanning:
                        scan();
                        break;
                    case State.Tracking:
                        track();

                        break;
                    case State.Firing:
                        fireState();
                        break;
                }
            }
        }

        protected virtual void fireState()
        {
            fire();
            postFire();
        }

        protected virtual void track()
        {
            reloadClock--;
            if (!inRange())
            {
                state = State.Scanning;
            }

            Vector2 targetVector = target.Position - body.Position;
            float targetAngle = (float)Math.Atan2(targetVector.Y, targetVector.X);
            float radDiff = MathHelp.getRadDiff(body.Rotation, targetAngle);
            if (Math.Abs(radDiff) > rotationSpeed)
            {
                body.Rotation += MathHelp.clamp(radDiff, rotationSpeed, -rotationSpeed);
            }
            else
            {
                if (hasLineOfSight()) {
                    body.Rotation += radDiff;
                    if (reloadClock <= 0)
                    {
                        prepFireState();
                        state = State.Firing;
                    }
                }
            }

            body.Rotation = body.Rotation;
        }

        public Vector2 AimVector
        {
            get
            {
                return new Vector2((float)Math.Cos(body.Rotation), (float)Math.Sin(body.Rotation));
            }
        }

        private bool hasLineOfSight()
        {



            Vector2 p1 = pivot.Body.Position + AimVector * barrelLength;
            Vector2 p2 = pivot.Body.Position + AimVector * fireRange;

            return FarseerHelper.hasLineOfSight(p1, p2, f => target.OwnsFixture(f), world);

            //bool hasLOS = false;

            //world.RayCast((f, p, n, fr) =>
            //{
            //    if (target.OwnsFixture(f))
            //        hasLOS = true;

            //    return 0; // terminate the ray cast

            //}, p1, p2);

            //return hasLOS;
        }

        private void scan()
        {
            reloadClock--;
            if (inRange())
            {
                state = State.Tracking;
            }
        }

        protected virtual void beginReloading()
        {
            reloadClock = reloadTime;
        }

        protected abstract void prepFireState();

        protected virtual void postFire()
        {
            state = State.Tracking;
        }

        protected abstract void fire();

    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using KinectRagdoll.Drawing;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using KinectRagdoll.Kinect;
using System.Runtime.Serialization;
using FarseerPhysics.Common.PolygonManipulation;

namespace KinectRagdoll.Hazards
{
    [DataContract(Name = "LaserTurret", Namespace = "http://www.imcool.com")]
    class LaserTurret : Turret
    {

        private const int chargeTime = 100;
        private int chargeCount = chargeTime;
        //protected static int reloadTime = 50;
        protected static int glowTime = 5;
        private int glowCount;
        private Vector2 stoppingPoint;

        public LaserTurret(Vector2 farseerLoc, World w, RagdollManager r) : base(farseerLoc, w, r)
        {
            reloadTime = 50;
        }

        public LaserTurret(Vector2 farseerLoc, World w, RagdollManager r, Fixture f)
            : base(farseerLoc, w, r, f)
        {
            reloadTime = 50;
        }


        protected override void prepFireState()
        {
            chargeCount = chargeTime;
        }

        protected override void fireState()
        {
            chargeCount--;
            if (chargeCount < 0)
            {
                fire();
                postFire();
            }
        }

        protected override void fire()
        {
            Vector2 fireVec = new Vector2((float)Math.Cos(body.Rotation), (float)Math.Sin(body.Rotation));
            Vector2 fireLoc = body.Position + fireVec * (barrelLength);
            Vector2 endPosition = body.Position + fireVec * 20;
            stoppingPoint = CuttingTools.Cut(world, fireLoc, endPosition, 0, Category.Cat3);
            glowCount = glowTime;
        }

        public override void Draw(SpriteBatch sb)
        {
            Vector2 fireVec = new Vector2((float)Math.Cos(body.Rotation), (float)Math.Sin(body.Rotation));
            Vector2 fireLoc = body.Position + fireVec * (barrelLength + .3f);
            Vector2 endPosition = body.Position + fireVec * 20;
            if (state == State.Firing)
            {
                Color c = new Color(1, 0, 0, (chargeTime - chargeCount) * 1.0f / chargeTime);
                SpriteHelper.DrawLine(sb, fireLoc, endPosition, .1f, c);
            }

            if (glowCount > 0) {
                glowCount--;
                SpriteHelper.DrawLine(sb, fireLoc, stoppingPoint, .4f, Color.Red);
            }
        }

        protected override void postFire()
        {
            base.postFire();
            state = State.Tracking;
            beginReloading();
        }

        protected override void beginReloading()
        {
            base.beginReloading();
            chargeCount = chargeTime;
        }
    }
}

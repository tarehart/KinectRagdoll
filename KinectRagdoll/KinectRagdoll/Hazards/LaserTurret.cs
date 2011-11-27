using System;
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

        public LaserTurret(Vector2 farseerLoc, World w, RagdollManager r) : base(farseerLoc, w, r)
        {
            
        }

        public LaserTurret(Vector2 farseerLoc, World w, RagdollManager r, Fixture f)
            : base(farseerLoc, w, r, f)
        {
            
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
            Vector2 fireVec = new Vector2((float)Math.Cos(pivot.Body.Rotation), (float)Math.Sin(pivot.Body.Rotation));
            Vector2 fireLoc = pivot.Body.Position + fireVec * (barrelLength + .3f);
            Vector2 endPosition = pivot.Body.Position + fireVec * 20;
            CuttingTools.Cut(world, fireLoc, endPosition, 0);
        }

        public override void Draw(SpriteBatch sb)
        {
            Vector2 fireVec = new Vector2((float)Math.Cos(pivot.Body.Rotation), (float)Math.Sin(pivot.Body.Rotation));
            Vector2 fireLoc = pivot.Body.Position + fireVec * (barrelLength + .3f);
            Vector2 endPosition = pivot.Body.Position + fireVec * 20;
            if (state == State.Firing)
            {
                if (chargeCount < 5)
                {
                    SpriteHelper.DrawLine(sb, fireLoc, endPosition, .4f, Color.Red);
                }
                else
                {
                    Color c = new Color(1, 0, 0, (chargeTime - chargeCount) * 1.0f / chargeTime);
                    SpriteHelper.DrawLine(sb, fireLoc, endPosition, .1f, c);
                }
                
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

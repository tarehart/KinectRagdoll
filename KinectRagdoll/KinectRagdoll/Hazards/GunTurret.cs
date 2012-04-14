using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using KinectRagdoll.Equipment;
using FarseerPhysics.Dynamics;
using KinectRagdoll.Kinect;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.Serialization;

namespace KinectRagdoll.Hazards
{
    [DataContract(Name = "GunTurret", Namespace = "http://www.imcool.com")]
    public class GunTurret : Turret
    {
        protected static float fireVel = 1;
        protected int fireCount;
        protected static int burstNum = 3;
        protected static int fireInterval = 10;
        protected int fireClock;

        public GunTurret(Vector2 farseerLoc, World w, RagdollManager r)
            : base(farseerLoc, w, r)
        {
        }

        public GunTurret(Vector2 farseerLoc, World w, RagdollManager r, Fixture f)
            : base(farseerLoc, w, r, f)
        {
            
        }

        protected override void fire()
        {
            Vector2 fireVec = AimVector;
            Vector2 fireLoc = body.Position + fireVec * (barrelLength + .3f);
            new PunchGuns.PunchBullet(fireLoc, Vector2.Normalize(fireVec) * fireVel, world);
            fireCount++;
        }

        protected override void fireState()
        {
            if (fireClock == 0)
            {
                fire();
                postFire();

            }
            else
            {
                fireClock--;
            }
        }

        protected override void postFire()
        {
            if (fireCount == burstNum)
            {
                state = State.Tracking;
                beginReloading();
            }
            else
                fireClock = fireInterval; // prepare to fire again
        }


        protected override void prepFireState()
        {
            fireCount = 0;
            fireClock = 0;
        }

        public override void Draw(SpriteBatch sb)
        {
        }

    }
}

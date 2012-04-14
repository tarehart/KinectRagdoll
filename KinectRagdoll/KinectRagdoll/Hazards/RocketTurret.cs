using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using KinectRagdoll.Kinect;
using FarseerPhysics.Dynamics;
using System.Runtime.Serialization;

namespace KinectRagdoll.Hazards
{
    [DataContract(Name = "RocketTurret", Namespace = "http://www.imcool.com")]
    class RocketTurret : Turret
    {

        protected Rocket activeRocket;

        public RocketTurret(Vector2 farseerLoc, World w, RagdollManager r)
            : base(farseerLoc, w, r)
        {
        }

        public RocketTurret(Vector2 farseerLoc, World w, RagdollManager r, Fixture f)
            : base(farseerLoc, w, r, f)
        {
            
        }
        protected override void fire()
        {
            Vector2 fireVec = AimVector;
            Vector2 fireLoc = body.Position + fireVec * (barrelLength + 1f);
            activeRocket = new Rocket(fireLoc, world, target);
        }

        protected override void postFire()
        {
            state = State.Tracking;
            beginReloading();
        }

        protected override void prepFireState()
        {
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            if (activeRocket != null && activeRocket.Alive)
                activeRocket.Draw();
        }

        public override void Update()
        {
            base.Update();

            if (activeRocket != null && activeRocket.Alive)
                activeRocket.Update();
        }
    }
}

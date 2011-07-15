using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using FarseerPhysics.Dynamics;
using KinectRagdoll.Kinect;
using KinectRagdoll.Ragdoll;
using Microsoft.Xna.Framework.Graphics;
using KinectRagdoll.Sandbox;

namespace KinectRagdoll.Powerups
{

    [DataContract(Name = "Pickup", Namespace = "http://www.imcool.com")]
    public abstract class Pickup
    {

        [DataMember()]
        public Fixture Fixture { get; private set; }
        protected RagdollManager ragdollManager;
        protected FarseerManager farseerManager;

        public bool Taken { get; private set; }

        public event EventHandler PickedUp;


        internal Pickup(Fixture f, RagdollManager ragdollManager, FarseerManager farseerManager)
        {
            this.Fixture = f;
            this.ragdollManager = ragdollManager;
            this.farseerManager = farseerManager;

            f.BeforeCollision += new BeforeCollisionEventHandler(f_BeforeCollision);

            ApplyTexture();

        }

        protected abstract void ApplyTexture();


        bool f_BeforeCollision(Fixture fixtureA, Fixture fixtureB)
        {
            Fixture other = fixtureA;
            if (Fixture == fixtureA) other = fixtureB;

            RagdollMuscle ragdoll = ragdollManager.GetFixtureOwner(other);

            if (ragdoll != null)
            {
                DoPickupAction(ragdoll);

                farseerManager.world.RemoveBody(Fixture.Body);
                Taken = true;
                if (PickedUp != null)
                {
                    PickedUp(this, null);
                }
                return false;
            }

            return true;
        }

        protected abstract void DoPickupAction(RagdollMuscle ragdoll);



        public abstract void Draw(SpriteBatch sb);


        internal void RemoveCollisionHandler()
        {
            Fixture.BeforeCollision -= new BeforeCollisionEventHandler(f_BeforeCollision);
            FarseerTextures.ApplyTexture(Fixture, FarseerTextures.TextureType.Normal);
        }
    }
}

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
        public Body Body { get; private set; }
        protected RagdollManager ragdollManager;
        protected FarseerManager farseerManager;

        public bool Taken { get; private set; }

        public event EventHandler PickedUp;


        protected Pickup(RagdollManager r, FarseerManager f) {
            this.ragdollManager = r;
            this.farseerManager = f;
        }


        internal Pickup(Body b, RagdollManager ragdollManager, FarseerManager farseerManager) : this(ragdollManager, farseerManager)
        {
            
            this.Body = b;
            

            foreach (Fixture f in b.FixtureList)
            {
                f.BeforeCollision += new BeforeCollisionEventHandler(f_BeforeCollision);
            }

            ApplyTexture();

        }

        protected abstract void ApplyTexture();


        bool f_BeforeCollision(Fixture fixtureA, Fixture fixtureB)
        {
            Fixture other = fixtureA;
            if (Body.FixtureList.Contains(fixtureA)) other = fixtureB;

            RagdollMuscle ragdoll = ragdollManager.GetFixtureOwner(other);

            if (ragdoll != null)
            {
                DoPickupAction(ragdoll);

                farseerManager.world.RemoveBody(Body);
                Taken = true;
                if (PickedUp != null)
                {
                    PickedUp(this, null);
                }
                return false;
            }

            return true;
        }

        public abstract void DoPickupAction(RagdollMuscle ragdoll);



        public abstract void Draw(SpriteBatch sb);


        internal void RemoveCollisionHandler()
        {
            foreach (Fixture f in Body.FixtureList)
            {
                f.BeforeCollision -= new BeforeCollisionEventHandler(f_BeforeCollision);
            }
            
            
            FarseerTextures.ApplyTexture(Body, FarseerTextures.TextureType.Normal);
        }
    }
}

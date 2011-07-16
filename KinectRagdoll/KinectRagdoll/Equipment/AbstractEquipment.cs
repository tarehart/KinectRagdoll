using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KinectRagdoll.Kinect;
using Microsoft.Xna.Framework.Graphics;
using KinectRagdoll.Ragdoll;
using System.Runtime.Serialization;

namespace KinectRagdoll.Equipment
{
    [DataContract(Name = "AbstractEquipment", Namespace = "http://www.imcool.com")]
    [KnownType(typeof(Flappers))]
    [KnownType(typeof(StabilizedJetpack))]
    [KnownType(typeof(PunchGuns))]
    [KnownType(typeof(SpideySilk))]
    public abstract class AbstractEquipment
    {

        protected bool destroyed;

        public virtual void Destroy()
        {
            destroyed = true;
        }

        public abstract void AttachToRagdoll(RagdollMuscle ragdoll);

        public abstract void Update(SkeletonInfo info);


        public abstract void Draw(SpriteBatch sb);

    }
}

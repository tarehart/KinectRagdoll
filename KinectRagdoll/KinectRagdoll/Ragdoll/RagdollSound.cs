using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using KinectRagdoll.Music;
using KinectRagdoll.Kinect;
using Microsoft.Xna.Framework;

namespace KinectRagdoll.Ragdoll
{
    class RagdollSound : RagdollMuscle
    {
        public BodySound bodySound;

        public RagdollSound(World w, Vector2 p)
            : base(w, p)
        {

        }

        public override void Init(World w)
        {
            base.Init(w);

            bodySound = new BodySound();
            bodySound.Start();
        }

        public override void Update(SkeletonInfo info)
        {

            base.Update(info);

            bodySound.Update(info);
        }

        protected override void knockOut()
        {
            bodySound.Stop();
        }

        protected override void wakeUp()
        {
            bodySound.Start();
        }


    }
}

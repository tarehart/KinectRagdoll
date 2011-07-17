using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using KinectRagdoll.Ragdoll;
using System.Runtime.Serialization;

namespace KinectRagdoll.Equipment
{

    [DataContract(Name = "WristVelocityEquipment", Namespace = "http://www.imcool.com")]
    public abstract class WristVelocityEquipment : AbstractEquipment
    {
        protected Vector3 leftWrist;
        protected Vector3 rightWrist;

        protected Vector3 rightVel; 
        protected Vector3 leftVel;

        private bool wasUncontrolled = true;

        protected RagdollMuscle ragdoll;

        public WristVelocityEquipment(RagdollMuscle ragdoll = null)
        {
            if (ragdoll != null)
            {
                Init(ragdoll);
            }
        }

        public override void Init(RagdollMuscle ragdoll)
        {
            this.ragdoll = ragdoll;
            ragdoll.WakeUp += new EventHandler(Reset);
            ragdoll.PossessedByPlayer += new EventHandler(Reset);
        }

        

        private void Reset(object sender, EventArgs e)
        {
            wasUncontrolled = true;
        }

        public override void Update(Kinect.SkeletonInfo info)
        {
            //Vector3 newRight = info.LocationToGestureSpace(info.rightWrist);
            //Vector3 newLeft = info.LocationToGestureSpace(info.leftWrist);

            if (wasUncontrolled)
            {
                rightVel = Vector3.Zero;
                leftVel = Vector3.Zero;
            }
            else
            {


                rightVel = info.rightWristVel;
                leftVel = info.leftWristVel;
            }

            rightWrist = info.LocationToGestureSpace(info.rightWrist);
            leftWrist = info.LocationToGestureSpace(info.leftWrist);
            if (!ragdoll.asleep && ragdoll.Possessed)
            {
                wasUncontrolled = false;
            }
        }

    }
}

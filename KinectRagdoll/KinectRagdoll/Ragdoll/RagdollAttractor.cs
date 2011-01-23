using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using FarseerPhysics.DebugViews;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics.Joints;

namespace KinectRagdoll.Kinect
{
    public class RagdollAttractor : Ragdoll
    {

   

        private FixedMouseJoint rightHandSpring;
        private FixedMouseJoint leftHandSpring;
        
        public RagdollAttractor(World world, Vector2 position) : base(world, position)
        {
            
        }

        public override void setShoulderToRightHand(Vector2 vec)
        {
            rightHandSpring.WorldAnchorB = _body.Body.Position + vec;
        }

        public override void setShoulderToLeftHand(Vector2 vec)
        {
            leftHandSpring.WorldAnchorB = _body.Body.Position + vec;
        }

        public override void setChestToHead(Vector2 vec)
        {

        }

        public override void setHipToRightFoot(Vector2 vec)
        {

        }

        public override void setHipToLeftFoot(Vector2 vec)
        {

        }

        

        protected override void CreateExtraJoints(World world)
        {
            rightHandSpring = new FixedMouseJoint(_lowerRightArm.Body, _lowerRightArm.Body.Position);
            rightHandSpring.MaxForce = 1000.0f * _lowerRightArm.Body.Mass;
            world.AddJoint(rightHandSpring);

            leftHandSpring = new FixedMouseJoint(_lowerLeftArm.Body, _lowerLeftArm.Body.Position);
            leftHandSpring.MaxForce = 1000.0f * _lowerLeftArm.Body.Mass;
            world.AddJoint(leftHandSpring);
        }

        

    }
}

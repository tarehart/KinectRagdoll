using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics.Joints;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using KinectRagdoll.Ragdoll;
using KinectRagdoll.Drawing;

namespace KinectRagdoll.Equipment
{
    class GrabHands : AbstractEquipment
    {

        private RevoluteJoint jRightGrip;
        private RevoluteJoint jLeftGrip;

        private bool rightGrip;
        private bool leftGrip;

        private const float grabPlane = -.4f;
        private const float releasePlane = -.3f;
        private const float grabVel = -20;

        private int rightHandGrabGrace;
        private int leftHandGrabGrace;
        private const int grabGrace = 30;

        private World world;
        private RagdollMuscle ragdoll;

        public GrabHands(RagdollMuscle ragdoll, World world)
        {
            this.world = world;
            this.ragdoll = ragdoll;

            ragdoll.KnockOut += new EventHandler(ragdoll_KnockOut);

        }

        void ragdoll_KnockOut(object sender, EventArgs e)
        {
            ReleaseLeftGrip();
            ReleaseRightGrip();
        }

        public override void Update(Kinect.SkeletonInfo info)
        {
            if (leftHandGrabGrace > 0) leftHandGrabGrace--;
            if (rightHandGrabGrace > 0) rightHandGrabGrace--;

            if (!rightGrip &&
                    info.rightHand.Z < info.torso.Z + grabPlane &&
                    info.RightHandVel.Z < grabVel)
            {
                rightHandGrabGrace = grabGrace;
                //TryRightGrip();
            }

            if (rightHandGrabGrace > 0)
            {
                TryRightGrip();
            }
            else if (rightGrip && info.rightHand.Z > info.torso.Z + releasePlane)
            {
                ReleaseRightGrip();
            }

            if (!leftGrip &&
                info.leftHand.Z < info.torso.Z + grabPlane &&
                info.LeftHandVel.Z < grabVel)
            {
                leftHandGrabGrace = grabGrace;
                //TryLeftGrip();
            }
            if (leftHandGrabGrace > 0)
            {
                TryLeftGrip();
            }
            else if (leftGrip && info.leftHand.Z > info.torso.Z + releasePlane)
            {
                ReleaseLeftGrip();
            }
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            if (leftGrip)
            {
                //Vector2 pixelLoc = game.projectionHelper.FarseerToPixel(jLeftGrip.WorldAnchorA);
                SpriteHelper.DrawCircle(sb, jLeftGrip.WorldAnchorA, 1, Color.OrangeRed);
            }

            if (rightGrip)
            {
                //Vector2 pixelLoc = game.projectionHelper.FarseerToPixel(jRightGrip.WorldAnchorA);
                SpriteHelper.DrawCircle(sb, jRightGrip.WorldAnchorA, 1, Color.OrangeRed);
            }
        }

        private void TryLeftGrip()
        {
            Vector2 elbowLoc = ragdoll.jLeftArm.WorldAnchorA;
            Vector2 forearmLoc = ragdoll._lowerLeftArm.Body.Position;
            Vector2 gripLoc = forearmLoc + (forearmLoc - elbowLoc) * 2;

            Fixture f = world.TestPoint(gripLoc);
            if (f != null)
            {
                if (jLeftGrip != null) world.RemoveJoint(jLeftGrip);
                jLeftGrip = new RevoluteJoint(ragdoll._lowerLeftArm.Body, f.Body, ragdoll._lowerLeftArm.Body.GetLocalPoint(gripLoc), f.Body.GetLocalPoint(gripLoc));
                world.AddJoint(jLeftGrip);
                leftGrip = true;
            }
        }

        private void ReleaseLeftGrip()
        {
            if (jLeftGrip != null) world.RemoveJoint(jLeftGrip);
            jLeftGrip = null;
            leftGrip = false;
        }

        private void ReleaseRightGrip()
        {
            if (jRightGrip != null) world.RemoveJoint(jRightGrip);
            jRightGrip = null;
            rightGrip = false;
        }

        private void TryRightGrip()
        {
            Vector2 elbowLoc = ragdoll.jRightArm.WorldAnchorA;
            Vector2 forearmLoc = ragdoll._lowerRightArm.Body.Position;
            Vector2 gripLoc = forearmLoc + (forearmLoc - elbowLoc) * 2;

            Fixture f = world.TestPoint(gripLoc);
            if (f != null)
            {
                if (jRightGrip != null) world.RemoveJoint(jRightGrip);
                jRightGrip = new RevoluteJoint(ragdoll._lowerRightArm.Body, f.Body, ragdoll._lowerRightArm.Body.GetLocalPoint(gripLoc), f.Body.GetLocalPoint(gripLoc));
                world.AddJoint(jRightGrip);
                rightGrip = true;
            }
        }

    }
}

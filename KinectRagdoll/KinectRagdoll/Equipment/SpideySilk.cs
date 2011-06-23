using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KinectRagdoll.Ragdoll;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics.Joints;
using KinectRagdoll.Drawing;

namespace KinectRagdoll.Equipment
{
    class SpideySilk : PunchEquipment
    {

        protected int range;
        protected DistanceJoint rightSilk;
        protected DistanceJoint leftSilk;
        protected const float DETACH_RADIUS = 7f;

        private const float silkForce = 1.5f;
        private const float SPEED_THRESHOLD = .5f;
        private const float EXTENSION_THRESHOLD = 7f;

        public SpideySilk(RagdollMuscle ragdoll, World world, int cooldown, int range)
            : base(ragdoll, world, cooldown)
        {
            this.range = range;
            ragdoll.KnockOut += new EventHandler(ragdoll_KnockOut);
        }

        void ragdoll_KnockOut(object sender, EventArgs e)
        {
            UndoSilk(true);
            UndoSilk(false);
        }

        protected override void fire(Vector2 hand, Vector2 handVel, bool rightHand)
        {
            if (rightHand)
            {
                world.RayCast(SilkSlingRight, hand, hand + Vector2.Normalize(handVel) * range);
            }
            else
            {
                world.RayCast(SilkSlingLeft, hand, hand + Vector2.Normalize(handVel) * range);
            }
            
        }

        private float SilkSlingLeft(Fixture f, Vector2 p, Vector2 n, float fr)
        {

            if (f == ragdoll._lowerLeftArm) return -1;

            Vector2 handAnchor = getHandAnchor(false);

           
            UndoSilk(false);
           

            leftSilk = new DistanceJoint(ragdoll._lowerLeftArm.Body, f.Body, ragdoll._lowerLeftArm.Body.GetLocalPoint(handAnchor), f.Body.GetLocalPoint(p));
            leftSilk.Frequency = silkForce;
            leftSilk.DampingRatio = 1;
            leftSilk.Length = 0;
            leftSilk.CollideConnected = true;
            world.AddJoint(leftSilk);

            return fr; 
        }

        private float SilkSlingRight(Fixture f, Vector2 p, Vector2 n, float fr)
        {

            if (f == ragdoll._lowerRightArm) return -1;

            Vector2 handAnchor = getHandAnchor(true);

           
            UndoSilk(true);

            rightSilk = new DistanceJoint(ragdoll._lowerRightArm.Body, f.Body, ragdoll._lowerRightArm.Body.GetLocalPoint(handAnchor), f.Body.GetLocalPoint(p));
            rightSilk.Frequency = silkForce;
            rightSilk.DampingRatio = 1;
            rightSilk.Length = 0;
            rightSilk.CollideConnected = true;
            world.AddJoint(rightSilk);

            return fr;
        }

        private void UndoSilk(bool rightHand)
        {
            if (rightHand)
            {
                if (rightSilk != null)
                {
                    world.RemoveJoint(rightSilk);
                    rightSilk = null;
                }
            }
            else
            {
                if (leftSilk != null)
                {
                    world.RemoveJoint(leftSilk);
                    leftSilk = null;
                }
            }
        }

        private Vector2 getHandAnchor(bool rightHand)
        {

            Vector2 elbowLoc;
            Vector2 forearmLoc;
            if (rightHand)
            {
                elbowLoc = ragdoll.jRightArm.WorldAnchorA;
                forearmLoc = ragdoll._lowerRightArm.Body.Position;
            }
            else
            {
                elbowLoc = ragdoll.jLeftArm.WorldAnchorA;
                forearmLoc = ragdoll._lowerLeftArm.Body.Position;
            }
            
            
            return forearmLoc + (forearmLoc - elbowLoc) * 1.5f;
        }

        public override void Update(Kinect.SkeletonInfo info)
        {
            base.Update(info);

            if (leftSilk != null && handRetracted(leftHand))
            {
                UndoSilk(false);
            }

            if (rightSilk != null && handRetracted(rightHand))
            {
                UndoSilk(true);
            }


        }

        private bool handRetracted(Vector2 hand)
        {

            //Vector2 centerShoulders = (ragdoll.Body.Position + ragdoll._head.Body.Position) / 2;
            Vector2 bodyToHand = hand - ragdoll.Body.Position;
            return bodyToHand.LengthSquared() < DETACH_RADIUS;
            
        }

        protected override bool shouldFire(Vector2 hand, Vector2 handVel, Vector2 shoulder)
        {
            if (handVel.LengthSquared() > SPEED_THRESHOLD)
            {


                Vector2 shoulderToHand = hand - shoulder;
                if (shoulderToHand.LengthSquared() > EXTENSION_THRESHOLD)
                {
                    handVel.Normalize();
                    shoulderToHand.Normalize();
                    return Vector2.Dot(handVel, shoulderToHand) > .7f;
                }
            }

            return false;
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            if (leftSilk != null) {
                SpriteHelper.DrawLine(sb, leftSilk.WorldAnchorA, leftSilk.WorldAnchorB, .3f, Color.SkyBlue);
            }

            if (rightSilk != null) {
                SpriteHelper.DrawLine(sb, rightSilk.WorldAnchorA, rightSilk.WorldAnchorB, .3f, Color.SkyBlue);
            }
            
            
        }

        /// <summary>
        /// Projection overridden in order to ignore body rotation.
        /// </summary>
        /// <param name="hand"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        protected override Vector2 projectToFarseer(Vector3 hand, Kinect.SkeletonInfo info)
        {
            Vector2 v = info.project(hand, RagdollBase.height);
            v += ragdoll.Body.Position;

            return v;
        }
        

    }
}

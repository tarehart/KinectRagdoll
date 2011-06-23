using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using KinectRagdoll.Ragdoll;
using FarseerPhysics.Dynamics;
using KinectRagdoll.Kinect;

namespace KinectRagdoll.Equipment
{
    public abstract class PunchEquipment : AbstractEquipment
    {
        protected Vector2 leftHand;
        protected Vector2 rightHand;
        protected Vector2 leftVel;
        protected Vector2 rightVel;
        protected Vector2 leftShoulder;
        protected Vector2 rightShoulder;

        protected RagdollMuscle ragdoll;
        protected World world;
        private int rightCooldown = 0;
        private int leftCooldown = 0;

        protected int cooldown;
        private const float SPEED_THRESHOLD = .4f;
        private const float EXTENSION_THRESHOLD = 3.5f;

        public PunchEquipment(RagdollMuscle ragdoll, World world, int cooldown)
        {
            this.ragdoll = ragdoll;
            this.world = world;
            this.cooldown = cooldown;
        }

        public override void Update(Kinect.SkeletonInfo info)
        {

            Vector2 newLeft = projectToFarseer(info.leftHand - info.torso, info);
            Vector2 newRight = projectToFarseer(info.rightHand - info.torso, info);
            leftShoulder = projectToFarseer(info.leftShoulder - info.torso, info);
            rightShoulder = projectToFarseer(info.rightShoulder - info.torso, info);

            if (rightCooldown > 0)
            {
                rightCooldown--;
            }
            else
            {
                
                Vector2 rightVel = newRight - rightHand;

                
                if (shouldFire(newRight, rightVel, rightShoulder))
                {
                    fire(newRight, rightVel, true);
                    rightCooldown = cooldown;
                }
                

            }



            if (leftCooldown > 0)
            {
                leftCooldown--;
            }
            else
            {
               
                Vector2 leftVel = newLeft - leftHand;

                
                if (shouldFire(newLeft, leftVel, leftShoulder))
                {
                    fire(newLeft, leftVel, false);
                    leftCooldown = cooldown;
                }
                
            }



            rightHand = newRight;
            leftHand = newLeft;



        }

        protected abstract void fire(Vector2 hand, Vector2 handVel, bool rightHand);
        

        protected virtual bool shouldFire(Vector2 hand, Vector2 handVel, Vector2 shoulder)
        {

            if (handVel.LengthSquared() > SPEED_THRESHOLD)
            {

                
                Vector2 shoulderToHand = hand - shoulder;
                if (shoulderToHand.LengthSquared() > EXTENSION_THRESHOLD)
                {
                    handVel.Normalize();
                    shoulderToHand.Normalize();
                    return Vector2.Dot(handVel, shoulderToHand) > .5f;
                }
            }

            return false;
        }

        protected virtual Vector2 projectToFarseer(Vector3 hand, SkeletonInfo info)
        {
            Vector2 v = info.project(hand, RagdollBase.height);
            Matrix m = Matrix.CreateRotationZ(ragdoll.Body.Rotation);
            v = Vector2.Transform(v, m);
            v += ragdoll.Body.Position;

            return v;
        }


    }
}
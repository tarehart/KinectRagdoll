using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using KinectRagdoll.Ragdoll;
using FarseerPhysics.Dynamics;
using KinectRagdoll.Kinect;
using KinectRagdoll.Drawing;
using System.Runtime.Serialization;

namespace KinectRagdoll.Equipment
{
    [DataContract(Name = "PunchEquipment", Namespace = "http://www.imcool.com")]
    public abstract class PunchEquipment : WristVelocityEquipment
    {
        //protected Vector3 leftHand;
        //protected Vector3 rightHand;
        //protected Vector3 leftVel;
        //protected Vector3 rightVel;
        protected Vector3 leftShoulder;
        protected Vector3 rightShoulder;

        //private Vector2 leftShoulderFarseer;

        //protected RagdollMuscle ragdoll;
        protected World world;
        private int rightCooldown = 0;
        private int leftCooldown = 0;
        

        protected int cooldown = 20;
        private const float SPEED_THRESHOLD = 1.6f;
        protected const float EXTENSION_THRESHOLD = .21f;

        public PunchEquipment(World world, int cooldown, RagdollMuscle ragdoll = null) : base(ragdoll)
        {
            //this.ragdoll = ragdoll;
            this.world = world;
            this.cooldown = cooldown;
        }

        public override void Init(RagdollMuscle ragdoll)
        {
            base.Init(ragdoll);

            this.world = KinectRagdollGame.Main.farseerManager.world;
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
  
            //SpriteHelper.DrawCircle(sb, leftShoulderFarseer, 1, Color.Red);
        }

        

        public override void Update(Kinect.SkeletonInfo info)
        {

            base.Update(info);

            //Vector3 newLeft = info.LocationToGestureSpace(info.leftWrist);
            //Vector3 newRight = info.LocationToGestureSpace(info.rightWrist);

            //Vector2 shoulderBump = KinectLocationToFarseerLocation((info.centerShoulder - info.torso) * .2f, info);


            leftShoulder = info.LocationToGestureSpace(info.leftShoulder);
            rightShoulder = info.LocationToGestureSpace(info.rightShoulder);

            //leftShoulderFarseer = GestureLocationToFarseerLocation(leftShoulder, info);

            //if (!wasAsleep && wasPossessed)
            //{

            if (rightCooldown > 0)
            {
                rightCooldown--;
            }
            else
            {

                //Vector3 rightVel = (newRight - rightWrist) / info.TimeDiff;


                if (shouldFire(rightWrist, rightVel, rightShoulder))
                {
                    fire(rightWrist, rightVel, true);
                    rightCooldown = cooldown;
                }


            }



            if (leftCooldown > 0)
            {
                leftCooldown--;
            }
            else
            {

                //Vector3 leftVel = (newLeft - leftHand) / info.TimeDiff;


                if (shouldFire(leftWrist, leftVel, leftShoulder))
                {

                        
                    fire(leftWrist, leftVel, false);
                    leftCooldown = cooldown;
                }

            }
            //}


            //rightHand = newRight;
            //leftHand = newLeft;
            



        }

        protected abstract void fire(Vector3 hand, Vector3 handVel, bool rightHand);
        

        protected virtual bool shouldFire(Vector3 hand, Vector3 handVel, Vector3 shoulder)
        {

            

            if (handVel.Length() > SPEED_THRESHOLD)
            {
                //Console.WriteLine("speed: " + handVel);
                
                Vector3 shoulderToHand = hand - shoulder;

                //Console.WriteLine("extension: " + shoulderToHand.Length());
                if (shoulderToHand.Length() > EXTENSION_THRESHOLD)
                {
                    handVel.Z = 0;
                    handVel.Normalize();
                    
                    shoulderToHand.Z = 0;
                    shoulderToHand.Normalize();
                    
                    float dot = Vector3.Dot(handVel, shoulderToHand);
                    //Console.WriteLine("dot: " + dot);

                    return dot > .8f;
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kinectLocation">This should be the location of a body part in pure skeletoninfo coordinates</param>
        /// <param name="info"></param>
        /// <returns>The absolute location of the body part in farseer coordinates</returns>
        protected virtual Vector2  KinectLocationToFarseerLocation(Vector3 kinectLocation, SkeletonInfo info)
        {
            // gSpace is just kinect space scaled to accomodate the person's size
            //Vector3 gSpace = info.ToGestureSpace(kinectLocation - info.torso); // this is equivalent to ToGestureSpace(absolute) - ToGestureSpace(torso)
            Vector3 gSpaceLocation = info.LocationToGestureSpace(kinectLocation);

            return GestureLocationToFarseerLocation(gSpaceLocation, info);
        }

        protected Vector2 GestureLocationToFarseerLocation(Vector3 gSpaceLocation, SkeletonInfo info)
        {
            // ragdollSpace is a coordinate system centered on and oriented to the ragdoll's body.
            // This calculation relies on the idea that info.torso and ragdoll.Body.Position should be superimposed.
            //Vector2 ragdollSpace = new Vector2(gSpace.X * RagdollBase.height, gSpace.Y * RagdollBase.height);
            Vector2 ragdollSpace = ragdoll.GestureVectorToRagdollVector(gSpaceLocation);


            // convert from ragdoll space to farseer space by rotating to the ragdoll's rotation and translating to the ragdoll's position.
            Vector2 farseerSpace = ragdoll.RagdollLocationToFarseerLocation(ragdollSpace);


            return farseerSpace;
        }

        


    }
}
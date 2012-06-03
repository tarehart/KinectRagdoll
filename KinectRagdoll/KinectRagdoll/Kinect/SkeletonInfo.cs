using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using Microsoft.Kinect;

namespace KinectRagdoll.Kinect
{
    public class SkeletonInfo
    {
        public Vector3 head;
        public Vector3 leftHand;
        public Vector3 rightHand;
        public Vector3 torso;
        public Vector3 leftFoot;
        public Vector3 rightFoot;
        public Vector3 rightShoulder;
        public Vector3 leftShoulder;
        public Vector3 rightHip;
        public Vector3 leftHip;
        public Vector3 centerHip;
        public Vector3 centerShoulder;
        public Vector3 leftWrist;
        public Vector3 rightWrist;

        // In gesture space
        public Vector3 leftWristVel;
        public Vector3 rightWristVel;

       

        private Vector3 oldRightWrist;
        private Vector3 oldLeftWrist;

        private DateTime latestTime = DateTime.Now;
        private DateTime timeBefore = DateTime.Now - new TimeSpan(0, 0, 1);

        /// <summary>
        /// Seconds elapsed between the previous skeleton update and the current one.
        /// </summary>
        public float TimeDiff
        {
            get
            {
                return (float)(latestTime - timeBefore).TotalSeconds;
            }
        }
        

        //private SkeletonCapability sc;
        private bool _tracking;
        private bool mirror;

        private float skelHeight = 1;

        public SkeletonInfo()
        {
            MakeScarecrow();

            //mirrorMe();
            

        }

        private void MakeScarecrow()
        {
            leftHand = new Vector3(-.3f, .2f, 2);
            rightHand = new Vector3(.3f, .2f, 2);
            leftWrist = new Vector3(-.28f, .2f, 2);
            rightWrist = new Vector3(.28f, .2f, 2);
            leftShoulder = new Vector3(-.1f, .5f, 2);
            rightShoulder = new Vector3(.1f, .5f, 2);
            head = new Vector3(0, .8f, 2);
            torso = new Vector3(0, .25f, 2);
            rightHip = new Vector3(.1f, .1f, 2);
            leftHip = new Vector3(-.1f, .1f, 2);
            rightFoot = new Vector3(.2f, -.8f, 2);
            leftFoot = new Vector3(-.2f, -.8f, 2);
            centerShoulder = new Vector3(0, .5f, 2);
            centerHip = new Vector3(0, .1f, 2);
        }

        //public Vector3 RightHandVel
        //{
        //    get { return rightHand - oldRightHand; }
        //}

        //public Vector3 LeftHandVel
        //{
        //    get { return leftHand - oldLeftHand; }
        //}

        public SkeletonInfo(bool mirror)
        {
            //this.sc = sc;
            this.mirror = mirror;

            MakeScarecrow();
            
        }

        public void Update(Skeleton data)
        {

            Tracking = true;

            //this.myUser = myUser;
            oldRightWrist = rightWrist;
            oldLeftWrist = leftWrist;

            pToV(data.Joints[JointType.HipCenter].Position, ref centerHip);
            pToV(data.Joints[JointType.ShoulderCenter].Position, ref centerShoulder);

            pToV(data.Joints[JointType.Head].Position, ref head);
            

            pToV(data.Joints[JointType.HandRight].Position, ref rightHand);
           

            pToV(data.Joints[JointType.HandLeft].Position, ref leftHand);
            

            pToV(data.Joints[JointType.Spine].Position, ref torso);
            

            pToV(data.Joints[JointType.FootLeft].Position, ref leftFoot);

            pToV(data.Joints[JointType.FootRight].Position, ref rightFoot);

            pToV(data.Joints[JointType.ShoulderLeft].Position, ref leftShoulder);

            pToV(data.Joints[JointType.ShoulderRight].Position, ref rightShoulder);

            pToV(data.Joints[JointType.HipLeft].Position, ref leftHip);

            pToV(data.Joints[JointType.HipRight].Position, ref rightHip);

            pToV(data.Joints[JointType.WristLeft].Position, ref leftWrist);

            pToV(data.Joints[JointType.WristRight].Position, ref rightWrist);

            
 

            if (mirror)
            {
                mirrorMe();

            }
           

            
            skelHeight = (head - leftHip).Length() * 2.4f;

            timeBefore = latestTime;
            latestTime = DateTime.Now;

            leftWristVel = (LocationToGestureSpace(leftWrist) - LocationToGestureSpace(oldLeftWrist)) / TimeDiff;
            rightWristVel = (LocationToGestureSpace(rightWrist) - LocationToGestureSpace(oldRightWrist)) / TimeDiff;

            
        }

        private void mirrorMe()
        {
            Matrix flip = Matrix.CreateScale(-1, 1, 1);
            head = Vector3.Transform(head, flip);
            leftHand = Vector3.Transform(leftHand, flip);
            rightHand = Vector3.Transform(rightHand, flip);
            torso = Vector3.Transform(torso, flip);
            leftFoot = Vector3.Transform(leftFoot, flip);
            rightFoot = Vector3.Transform(rightFoot, flip);
            rightShoulder = Vector3.Transform(rightShoulder, flip);
            leftShoulder = Vector3.Transform(leftShoulder, flip);
            rightHip = Vector3.Transform(rightHip, flip);
            leftHip = Vector3.Transform(leftHip, flip);
        }

        /*public Vector3 getPosition(SkeletonJoint joint)
        {
            if (sc == null) return Vector3.Zero;
            SkeletonJointPosition p = new SkeletonJointPosition();
            sc.GetSkeletonJointPosition(myUser, joint, ref p);
            Vector3 v = Vector3.Zero;
            pToV(p.position, ref v);

            return v;
        }*/

        private void pToV(SkeletonPoint p, ref Vector3 v)
        {
            v.X = p.X;
            v.Y = p.Y;
            v.Z = p.Z;
        }


        public Vector3 LocationToGestureSpace(Vector3 vec)
        {
            if (skelHeight == 0)
            {
                skelHeight = 10;
                Debug.WriteLine("Error: Skeleton height was zero, setting it to 10");
            }
            vec -= torso;
            return vec / skelHeight;
        }

        public Vector3 VectorToGestureSpace(Vector3 vec)
        {

            Debug.Assert(skelHeight != 0);
            return vec / skelHeight;
        }

        /// <summary>
        /// In radians
        /// </summary>
        public float Rotation
        {
            get
            {
                Vector3 upSpine = centerShoulder - centerHip;
                if (upSpine != Vector3.Zero)
                {
                    return (float)(Math.Atan2(upSpine.Y, upSpine.X) - Math.PI / 2);
                }
                return 0;
            }
        }

        public bool Tracking
        {
            get
            {
                return _tracking;
            }
            set
            {
                _tracking = value;
                if (!_tracking)
                {
                    MakeScarecrow();
                }
            }
        }


        private static SkeletonInfo standard;
        public static SkeletonInfo StandardPose
        {
            get
            {
                if (standard == null)
                    standard = new SkeletonInfo();
                return standard;
            }
        }
    }
}

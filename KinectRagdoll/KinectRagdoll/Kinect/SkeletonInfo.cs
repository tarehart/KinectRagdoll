using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using Microsoft.Research.Kinect.Nui;

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

        private Vector3 oldRightHand;
        private Vector3 oldLeftHand;
        

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
            leftShoulder = new Vector3(-.1f, .5f, 2);
            rightShoulder = new Vector3(.1f, .5f, 2);
            head = new Vector3(0, .8f, 2);
            torso = new Vector3(0, .25f, 2);
            rightHip = new Vector3(.1f, .1f, 2);
            leftHip = new Vector3(-.1f, .1f, 2);
            rightFoot = new Vector3(.2f, -.8f, 2);
            leftFoot = new Vector3(-.2f, -.8f, 2);
        }

        public Vector3 RightHandVel
        {
            get { return rightHand - oldRightHand; }
        }

        public Vector3 LeftHandVel
        {
            get { return leftHand - oldLeftHand; }
        }

        public SkeletonInfo(bool mirror)
        {
            //this.sc = sc;
            this.mirror = mirror;

            MakeScarecrow();
            
        }

        public void Update(SkeletonData data)
        {

            Tracking = true;

            //this.myUser = myUser;
            oldRightHand = rightHand;
            oldLeftHand = leftHand;

            pToV(data.Joints[JointID.HipCenter].Position, ref centerHip);
            pToV(data.Joints[JointID.ShoulderCenter].Position, ref centerShoulder);

            pToV(data.Joints[JointID.Head].Position, ref head);
            

            pToV(data.Joints[JointID.HandRight].Position, ref rightHand);
           

            pToV(data.Joints[JointID.HandLeft].Position, ref leftHand);
            

            pToV(data.Joints[JointID.Spine].Position, ref torso);
            

            pToV(data.Joints[JointID.FootLeft].Position, ref leftFoot);

            pToV(data.Joints[JointID.FootRight].Position, ref rightFoot);

            pToV(data.Joints[JointID.ShoulderLeft].Position, ref leftShoulder);

            pToV(data.Joints[JointID.ShoulderRight].Position, ref rightShoulder);

            pToV(data.Joints[JointID.HipLeft].Position, ref leftHip);

            pToV(data.Joints[JointID.HipRight].Position, ref rightHip);

            if (mirror)
            {
                mirrorMe();

            }
           

            
            skelHeight = (head - leftHip).Length() * 2.4f;

            
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

        private void pToV(Vector p, ref Vector3 v)
        {
            v.X = p.X;
            v.Y = p.Y;
            v.Z = p.Z;
        }


        public Vector2 project(Vector3 vec, float desiredHeight)
        {

            Debug.Assert(skelHeight != 0);

            Vector2 v = new Vector2(vec.X, vec.Y);
            v *= desiredHeight / skelHeight;

            return v;
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
    }
}

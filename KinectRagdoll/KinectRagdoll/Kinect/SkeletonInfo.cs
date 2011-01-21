using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using xn;

namespace KinectTest2.Kinect
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
        

        private SkeletonCapability sc;
        private uint myUser;

        private float skelHeight = 1;

        public SkeletonInfo()
        {
            leftHand = new Vector3(-500, 250, 1000);
            rightHand = new Vector3(500, 250, 1000);
            leftShoulder = new Vector3(-150, -50, 1000);
            rightShoulder = new Vector3(150, -50, 1000);
            head = new Vector3(0, 150, 1000);
            torso = new Vector3(0, -300, 1000);
            rightHip = new Vector3(150, -400, 1000);
            leftHip = new Vector3(-150, -400, 1000);
            rightFoot = new Vector3(200, -600, 1000);
            leftFoot = new Vector3(-200, -600, 1000);

            //mirrorMe();
            

        }

        public SkeletonInfo(SkeletonCapability sc, uint myUser, bool mirror)
        {

            this.sc = sc;
            this.myUser = myUser;
            SkeletonJointPosition p = new SkeletonJointPosition();

            sc.GetSkeletonJointPosition(myUser, SkeletonJoint.Head, ref p);
            pToV(p.position, ref head);

            sc.GetSkeletonJointPosition(myUser, SkeletonJoint.RightHand, ref p);
            pToV(p.position, ref rightHand);

            sc.GetSkeletonJointPosition(myUser, SkeletonJoint.LeftHand, ref p);
            pToV(p.position, ref leftHand);

            sc.GetSkeletonJointPosition(myUser, SkeletonJoint.Torso, ref p);
            pToV(p.position, ref torso);

            sc.GetSkeletonJointPosition(myUser, SkeletonJoint.LeftFoot, ref p);
            pToV(p.position, ref leftFoot);

            sc.GetSkeletonJointPosition(myUser, SkeletonJoint.RightFoot, ref p);
            pToV(p.position, ref rightFoot);

            sc.GetSkeletonJointPosition(myUser, SkeletonJoint.LeftFoot, ref p);
            pToV(p.position, ref leftFoot);

            sc.GetSkeletonJointPosition(myUser, SkeletonJoint.RightFoot, ref p);
            pToV(p.position, ref rightFoot);

            sc.GetSkeletonJointPosition(myUser, SkeletonJoint.LeftShoulder, ref p);
            pToV(p.position, ref leftShoulder);

            sc.GetSkeletonJointPosition(myUser, SkeletonJoint.RightShoulder, ref p);
            pToV(p.position, ref rightShoulder);

            sc.GetSkeletonJointPosition(myUser, SkeletonJoint.LeftHip, ref p);
            pToV(p.position, ref leftHip);

            sc.GetSkeletonJointPosition(myUser, SkeletonJoint.RightHip, ref p);
            pToV(p.position, ref rightHip);

            if (mirror)
            {
                mirrorMe();

            }
           

            
            skelHeight = (head - leftHip).Length() * 2.5f;

            
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

        private void pToV(Point3D p, ref Vector3 v)
        {
            v.X = p.X;
            v.Y = p.Y;
            v.Z = p.Z;
        }


        public Vector2 project(Vector3 vec, float desiredHeight)
        {
            Vector2 v = new Vector2(vec.X, vec.Y);
            v *= desiredHeight / skelHeight;

            return v;
        }

    }
}

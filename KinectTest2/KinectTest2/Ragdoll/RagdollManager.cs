using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using xn;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace KinectTest2.Kinect
{
    public class RagdollManager
    {

        private RagdollMuscle ragdoll;
        private KinectManager kinect;

        public static Texture2D thrustTex;

        public RagdollManager()
        {

        }

        public void Init(World world, KinectManager kinect)
        {

            ragdoll = new RagdollMuscle(world, Vector2.Zero);
            this.kinect = kinect;

        }

        public void LoadContent(ContentManager content)
        {
            thrustTex = content.Load<Texture2D>("thrust");
        }


        public void Update(SkeletonInfo info)
        {

           
            Vector3 vec = info.rightHand - info.rightShoulder;
            Vector2 v2 = info.project(vec, ragdoll.height);
            ragdoll.setShoulderToRightHand(v2);

            float pracScaler = v2.X / vec.X;

            vec = info.leftHand - info.leftShoulder;
            v2 = info.project(vec, ragdoll.height);
            ragdoll.setShoulderToLeftHand(v2);

            vec = info.rightFoot - info.rightHip;
            v2 = info.project(vec, ragdoll.height);
            ragdoll.setHipToRightFoot(v2);

            vec = info.leftFoot - info.leftHip;
            v2 = info.project(vec, ragdoll.height);
            ragdoll.setHipToLeftFoot(v2);

            vec = info.head - info.torso;
            v2 = info.project(vec, ragdoll.height);
            ragdoll.setChestToHead(v2);

            ragdoll.tick();


            if (info.torso.Z < info.head.Z)
            {
                //kinect.bkColor = Color.Orange;
                ragdoll.Thrust((info.head.Z - info.torso.Z) * .02f);
                ragdoll.thrustOn = true;
            }
            else
            {
                //kinect.bkColor = Color.Beige;
                ragdoll.thrustOn = false;
                
            }

        }

        public void Draw(SpriteBatch sb)
        {
            
            ragdoll.Draw(sb);

        }

    }
}

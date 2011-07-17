using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KinectRagdoll.Ragdoll;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics.Joints;
using KinectRagdoll.Drawing;
using KinectRagdoll.Kinect;
using System.Runtime.Serialization;

namespace KinectRagdoll.Equipment
{

    [DataContract(Name = "SpideySilk", Namespace = "http://www.imcool.com")]
    class SpideySilk : PunchEquipment
    {

        protected int range = 50;
        protected DistanceJoint rightSilk;
        protected DistanceJoint leftSilk;
        protected const float DETACH_RADIUS = EXTENSION_THRESHOLD - 0f;

        private const float silkForce = 1.7f;
        //private const float SPEED_THRESHOLD = .6f;
        //private const float SPEED_THRESHOLD = .6f;
        //private const float EXTENSION_THRESHOLD = 2.3f;

        public SpideySilk(World world, int cooldown, int range, RagdollMuscle ragdoll = null)
            : base(world, cooldown, ragdoll)
        {
            this.range = range;
        }

        public override void Init(RagdollMuscle ragdoll)
        {
            base.Init(ragdoll);
            ragdoll.KnockOut += new EventHandler(ragdoll_KnockOut);
        }

        void ragdoll_KnockOut(object sender, EventArgs e)
        {
            UndoSilk(true);
            UndoSilk(false);
        }

        protected override void fire(Vector3 hand, Vector3 handVel, bool rightHand)
        {

            Vector2 farseerHandVel = ragdoll.GestureVectorToRagdollVector(handVel);
            Vector2 farseerHandLoc = ragdoll.RagdollLocationToFarseerLocation(ragdoll.GestureVectorToRagdollVector(hand));

            if (rightHand)
            {
                world.RayCast(SilkSlingRight, farseerHandLoc, farseerHandLoc + Vector2.Normalize(farseerHandVel) * range);
            }
            else
            {
                world.RayCast(SilkSlingLeft, farseerHandLoc, farseerHandLoc + Vector2.Normalize(farseerHandVel) * range);
            }
            
        }

        private float SilkSlingLeft(Fixture f, Vector2 p, Vector2 n, float fr)
        {

            if (f == ragdoll._lowerLeftArm) return -1;

            Vector2 handAnchor = getHandAnchor(false);

           
            UndoSilk(false);
           

            leftSilk = new DistanceJoint(ragdoll._lowerLeftArm.Body, f.Body, ragdoll._lowerLeftArm.Body.GetLocalPoint(handAnchor), f.Body.GetLocalPoint(p));
            leftSilk.Frequency = silkForce;
            leftSilk.DampingRatio = .5f;
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
            rightSilk.DampingRatio = .5f;
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

            if (leftSilk != null && handRetracted(leftWrist, leftShoulder))
            {
                UndoSilk(false);
            }

            if (rightSilk != null && handRetracted(rightWrist, rightShoulder))
            {
                UndoSilk(true);
            }


        }

        private bool handRetracted(Vector3 hand, Vector3 shoulder)
        {

            //Vector2 centerShoulders = (ragdoll.Body.Position + ragdoll._head.Body.Position) / 2;
            Vector3 shoulderToHand = hand - shoulder;
            return shoulderToHand.Length() < DETACH_RADIUS;
            
        }

        /*protected override bool shouldFire(Vector3 hand, Vector3 handVel, Vector3 shoulder)
        {
            if (handVel.Length() > SPEED_THRESHOLD)
            {


                Vector2 shoulderToHand = hand - shoulder;
                if (shoulderToHand.Length() > EXTENSION_THRESHOLD)
                {
                    handVel.Normalize();
                    shoulderToHand.Normalize();
                    if (Vector2.Dot(handVel, shoulderToHand) > .6f)
                    {
                        Console.WriteLine("Silked ");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Off Kilter");
                    }
                }
                else
                {
                    Console.WriteLine("Not extended " + shoulderToHand.Length());
                }
            }
            

            
            return false;
        }*/

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            base.Draw(sb);

            if (leftSilk != null) {
                SpriteHelper.DrawLine(sb, leftSilk.WorldAnchorA, leftSilk.WorldAnchorB, .2f, Color.Orange);
            }

            if (rightSilk != null) {
                SpriteHelper.DrawLine(sb, rightSilk.WorldAnchorA, rightSilk.WorldAnchorB, .2f, Color.Orange);
            }
            
            
        }

        public override void Destroy()
        {
            base.Destroy();
            UndoSilk(true);
            UndoSilk(false);
        }

        /// <summary>
        /// Projection overridden in order to ignore body rotation.
        /// </summary>
        /// <param name="hand"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        //protected Vector2 projectToFarseerNoRotation(Vector3 hand, Vector3 torso, Kinect.SkeletonInfo info)
        //{
        //    Vector2 v = info.FlattenAndScale(hand - torso, RagdollBase.height);
        //    v += ragdoll.Body.Position;

        //    return v;
        //}
        

    }
}

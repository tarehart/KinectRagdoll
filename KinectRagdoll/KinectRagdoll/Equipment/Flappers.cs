using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using KinectRagdoll.Ragdoll;
using KinectRagdoll.Drawing;

namespace KinectRagdoll.Equipment
{
    class Flappers : WristVelocityEquipment
    {

        //protected RagdollMuscle ragdoll;
        private bool flappedLeft;
        private bool flappedRight;
        private const float FLAP_MULTIPLIER = 25;



        public Flappers(RagdollMuscle ragdoll = null)
            : base(ragdoll)
        {

        }

        public override void Update(Kinect.SkeletonInfo info)
        {
            base.Update(info);

            flappedLeft = flappedRight = false;

            if (ShouldFlap(leftWrist, leftVel))
            {
                flappedLeft = true;
                ExertFlapLeft(leftVel);
            }

            if (ShouldFlap(rightWrist, rightVel))
            {
                flappedRight = true;
                ExertFlapRight(rightVel);
            }

        }

        protected virtual bool ShouldFlap(Vector3 handLoc, Vector3 handVel)
        {

            if (handVel.Y < -1f && handLoc.Length() > .20f && handLoc.Y < 0)
            {
                //Console.WriteLine("HandVelY: " + handVel.Y + " handLocLength: " + handLoc.Length() + " handLocY: " + handLoc.Y);
                return true;
            }

            return false;
        }

        protected virtual void ExertFlapRight(Vector3 rightHandVel)
        {
            
            Vector2 impulse = Vector2.UnitY * rightHandVel.Length() * FLAP_MULTIPLIER;
            Matrix rot = Matrix.CreateRotationZ(ragdoll.Body.Rotation);
            impulse = Vector2.Transform(impulse, rot);

            ragdoll._upperRightArm.Body.ApplyLinearImpulse(impulse);
            
        }

        protected virtual void ExertFlapLeft(Vector3 leftHandVel)
        {

            Vector2 impulse = Vector2.UnitY * leftHandVel.Length() * FLAP_MULTIPLIER;
            Matrix rot = Matrix.CreateRotationZ(ragdoll.Body.Rotation);
            impulse = Vector2.Transform(impulse, rot);

            ragdoll._upperLeftArm.Body.ApplyLinearImpulse(impulse);

        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            if (flappedRight)
            {
                SpriteHelper.DrawCircle(sb, ragdoll._upperRightArm.Body.Position, 2, Color.Red);
            }

            if (flappedLeft)
            {
                SpriteHelper.DrawCircle(sb, ragdoll._upperLeftArm.Body.Position, 2, Color.Red);
            }
        }

    }
}

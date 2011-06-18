using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KinectRagdoll.Kinect;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using KinectRagdoll.MyMath;
using KinectRagdoll.Ragdoll;

namespace KinectRagdoll.Equipment
{
    public class JetPack : AbstractEquipment
    {

        public bool thrustOn = false;
        private float feetThrust;
        private float rightHandThrust;
        private float leftHandThrust;
        private const float slowDamping = .9f;

        private Random rand;

        private int POST_THRUST_TIME = 100;
        protected int postThrustTimer = 0;

        private RagdollMuscle ragdoll;

        public JetPack(RagdollMuscle ragdoll)
        {
            this.ragdoll = ragdoll;
            rand = new Random();

            ragdoll.KnockOut += new EventHandler(ragdoll_KnockOut);
            ragdoll.WakeUp += new EventHandler(ragdoll_WakeUp);
        }

        void ragdoll_KnockOut(object sender, EventArgs e)
        {
            ragdoll._body.Body.LinearDamping = 0;
            StopThrust();
            ragdoll._body.Body.AngularDamping = 0;
        }

        void ragdoll_WakeUp(object sender, EventArgs e)
        {
            ragdoll._body.Body.LinearDamping = slowDamping;
        }

        


        public override void Update(SkeletonInfo info)
        {
            float thrust = (((info.leftHand.Z + info.rightHand.Z) / 2) - info.torso.Z) * 5f;
            if (thrust > 0)
                StartThrust(thrust);
            else StopThrust();

            if (postThrustTimer > 0)
            {
                postThrustTimer--;
                if (postThrustTimer == 0)
                    ragdoll._body.Body.LinearDamping = 0;

                
            }

            Thrust();

            if (thrustOn)
            {
                Vector3 v3 = info.head - info.torso;
                Vector2 v2 = info.project(v3, RagdollBase.height);


                float personAngle = (float)(Math.Atan2(v2.Y, v2.X) - Math.PI / 2);
                float ragdollAngle = ragdoll._body.Body.Rotation;
                float diff = MathHelp.getRadDiff(ragdollAngle, personAngle);
                float torque = 150;
                if (diff < 0) torque *= -1;
                if (thrustOn) torque *= 2;

                ragdoll._body.Body.ApplyTorque(torque);
                ragdoll._body.Body.AngularDamping = 5f;
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            if (thrustOn && !ragdoll.asleep)
            {
                drawLimbThrust(ragdoll._lowerLeftLeg, sb, feetThrust);
                drawLimbThrust(ragdoll._lowerRightLeg, sb, feetThrust);
                drawLimbThrust(ragdoll._lowerLeftArm, sb, leftHandThrust);
                drawLimbThrust(ragdoll._lowerRightArm, sb, rightHandThrust);

            }
        }

        private void Thrust()
        {
            if (ragdoll.asleep || !thrustOn) return;
            applyLimbThrust(ragdoll._lowerLeftLeg, Math.PI / 2, 3 * feetThrust);
            applyLimbThrust(ragdoll._lowerRightLeg, Math.PI / 2, 3 * feetThrust);
            applyLimbThrust(ragdoll._lowerRightArm, Math.PI / 2, 4f * rightHandThrust);
            applyLimbThrust(ragdoll._lowerLeftArm, Math.PI / 2, 4f * leftHandThrust);
        }

        private void applyLimbThrust(Fixture limb, double angleOffset, float thrustFactor)
        {
            float rot = limb.Body.Rotation + (float)angleOffset;
            Vector2 vec = new Vector2((float)Math.Cos(rot), (float)Math.Sin(rot));
            limb.Body.ApplyLinearImpulse(vec * thrustFactor);

            //DebugMaterial matBody = new DebugMaterial(MaterialType.Squares)
            //{
            //    Color = Color.DeepSkyBlue,
            //    Scale = 8f
            //};

            //limb.Body.BodyType = BodyType.Static;

            //Fixture c = FixtureFactory.CreateCircle(world, .5f, .01f, vec * 10 + limb.Body.Position, matBody);
            //c.CollisionFilter.CollidesWith = Category.None;

            //c = FixtureFactory.CreateCircle(world, .2f, .01f, limb.Body.Position, matBody);
            //c.CollisionFilter.CollidesWith = Category.None;

        }

        public void drawLimbThrust(Fixture limb, SpriteBatch sb, float thrustFactor)
        {

            Vector2 limbLoc = limb.Body.Position;
            float rot = limb.Body.Rotation + (float)Math.PI / 2;
            Vector2 vec = new Vector2((float)Math.Cos(rot), (float)Math.Sin(rot));
            SpriteEffects effect = SpriteEffects.None;
            if (rand.Next(2) == 0) effect = SpriteEffects.FlipHorizontally;
            sb.Draw(RagdollManager.thrustTex, limbLoc - vec * 1.5f, null, Color.White, limb.Body.Rotation + (float)Math.PI, new Vector2(64, 64), .012f * thrustFactor, effect, 0);

        }

        internal void StartThrust(float allThrust)
        {
            if (allThrust < 0)
            {
                StopThrust();
                return;
            }
            thrustOn = true;

            this.feetThrust = allThrust;
            this.rightHandThrust = allThrust;
            this.leftHandThrust = allThrust;

            postThrustTimer = POST_THRUST_TIME;
            ragdoll._body.Body.LinearDamping = slowDamping;

        }

        internal void StartThrust(float feetThrust, float rightHandThrust, float leftHandThrust)
        {
            thrustOn = true;

            if (rightHandThrust < 0) rightHandThrust = 0;
            if (leftHandThrust < 0) leftHandThrust = 0;
            if (feetThrust < 0) feetThrust = 0;

            this.feetThrust = feetThrust;
            this.rightHandThrust = rightHandThrust;
            this.leftHandThrust = leftHandThrust;




            postThrustTimer = POST_THRUST_TIME;
            ragdoll._body.Body.LinearDamping = slowDamping;
        }

        internal void StopThrust()
        {
            thrustOn = false;
            feetThrust = 0;
        }
    }
}

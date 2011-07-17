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
using System.Timers;
using System.Runtime.Serialization;

namespace KinectRagdoll.Equipment
{
    [DataContract(Name = "Jetpack", Namespace = "http://www.imcool.com")]
    public class JetPack : AbstractEquipment
    {

        public bool thrustOn = false;
        //private float feetThrust;
        //private float rightHandThrust;
        //private float leftHandThrust;

        protected float thrust;
        //private const float slowDamping = .9f;

        protected Random rand;

        //private int POST_THRUST_TIME = 100;
        //protected int postThrustTimer = 0;

        protected RagdollMuscle ragdoll;
        private Timer soundTimer;


        public JetPack(RagdollMuscle ragdoll = null)
        {
            
            if (ragdoll != null)
            {
                Init(ragdoll);
            }
        }



        public override void Init(RagdollMuscle ragdoll)
        {

            rand = new Random();

            this.ragdoll = ragdoll;
            
            ragdoll.KnockOut += new EventHandler(ragdoll_KnockOut);
            soundTimer = new Timer(100);
            soundTimer.Elapsed += new ElapsedEventHandler(soundTimer_Elapsed);
        }

        void soundTimer_Elapsed(object sender, ElapsedEventArgs e)
        {

            if (destroyed) soundTimer.Elapsed -= new ElapsedEventHandler(soundTimer_Elapsed);
            else
            {
                float intensity = -(float)Math.Pow(2, -2 * thrust) + 1;

                if (thrust > 0)
                    RagdollManager.thrustSound.Play(intensity, intensity, 0);

            }
                
        }

        protected virtual void ragdoll_KnockOut(object sender, EventArgs e)
        {
            //ragdoll._body.Body.LinearDamping = 0;
            StopThrust();
            //ragdoll._body.Body.AngularDamping = 0;
        }

        //void ragdoll_WakeUp(object sender, EventArgs e)
        //{
        //    ragdoll._body.Body.LinearDamping = slowDamping;
        //}

        


        public override void Update(SkeletonInfo info)
        {

            if (ragdoll.asleep) return;

            thrust = (((info.leftHand.Z + info.rightHand.Z) / 2) - info.torso.Z) * 3f;
            if (thrust > 0)
            {
                if (!thrustOn)
                    StartThrust();

                ApplyThrustForce();
            }
            else if (thrustOn)
            {
                StopThrust();
            }

            

            

            
        }

        public override void Draw(SpriteBatch sb)
        {
            if (thrustOn && !ragdoll.asleep)
            {
                drawLimbThrust(ragdoll._lowerLeftLeg, sb, thrust);
                drawLimbThrust(ragdoll._lowerRightLeg, sb, thrust);
                drawLimbThrust(ragdoll._lowerLeftArm, sb, thrust);
                drawLimbThrust(ragdoll._lowerRightArm, sb, thrust);

            }
        }

        protected void ApplyThrustForce()
        {
            if (ragdoll.asleep || !thrustOn) return;
            float armThrust = 4 * thrust;
            float legThrust = 3 * thrust;
            applyLimbThrust(ragdoll._lowerLeftLeg, Math.PI / 2, 3 * legThrust);
            applyLimbThrust(ragdoll._lowerRightLeg, Math.PI / 2, 3 * legThrust);
            applyLimbThrust(ragdoll._lowerRightArm, Math.PI / 2, 4f * armThrust);
            applyLimbThrust(ragdoll._lowerLeftArm, Math.PI / 2, 4f * armThrust);
        }

        private void applyLimbThrust(Fixture limb, double angleOffset, float thrustFactor)
        {
            float rot = limb.Body.Rotation + (float)angleOffset;
            Vector2 vec = new Vector2((float)Math.Cos(rot), (float)Math.Sin(rot));
            limb.Body.ApplyLinearImpulse(vec * thrustFactor);

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

        protected virtual void StartThrust()
        {
            
            thrustOn = true;
            //RagdollManager.thrustSound.Play(.5f, 0, 0);
            soundTimer.Start();


            //postThrustTimer = POST_THRUST_TIME;
            //ragdoll._body.Body.LinearDamping = slowDamping;

        }

        //internal void StartThrust(float feetThrust, float rightHandThrust, float leftHandThrust)
        //{
        //    thrustOn = true;

        //    if (rightHandThrust < 0) rightHandThrust = 0;
        //    if (leftHandThrust < 0) leftHandThrust = 0;
        //    if (feetThrust < 0) feetThrust = 0;

        //    this.feetThrust = feetThrust;
        //    this.rightHandThrust = rightHandThrust;
        //    this.leftHandThrust = leftHandThrust;




        //    postThrustTimer = POST_THRUST_TIME;
        //    ragdoll._body.Body.LinearDamping = slowDamping;
        //}

        protected virtual void StopThrust()
        {
            thrustOn = false;
            soundTimer.Stop();
            //feetThrust = 0;
        }
    }
}

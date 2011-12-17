using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using KinectRagdoll.Kinect;
using KinectRagdoll.Ragdoll;
using System.Runtime.Serialization;
using KinectRagdoll.Sandbox;

namespace KinectRagdoll.Equipment
{
    [DataContract(Name = "StabilizedJetpack", Namespace = "http://www.imcool.com")]
    class StabilizedJetpack : JetPack
    {

        protected float ticksAfterThrust = -1;
        protected Vector2 stoppingThrustVector;
        //protected float angularStabilizationTorque;
        


        public StabilizedJetpack(RagdollMuscle ragdoll = null) : base(ragdoll)
        {
            
        }

        public override void Update(Kinect.SkeletonInfo info)
        {
            base.Update(info);

            if (!thrustOn && !ragdoll.asleep && ticksAfterThrust >= 0)
            {
                

                if (ticksAfterThrust < 20)
                {
                    Vector2 bodyVel = ragdoll.Body.LinearVelocity;
                    stoppingThrustVector = -.5f * bodyVel;
                    ApplyStoppingForce();
                    ticksAfterThrust++;
                }
                else
                {
                    stoppingThrustVector = Vector2.Zero;
                    ticksAfterThrust = -1;
                }

                
            }


  

        }

        protected override void ragdoll_KnockOut(object sender, EventArgs e)
        {
            base.ragdoll_KnockOut(sender, e);

            stoppingThrustVector = Vector2.Zero;
            ticksAfterThrust = -1;
        }

        private void ApplyStoppingForce()
        {
            ragdoll.Body.ApplyLinearImpulse(stoppingThrustVector);
        }

        protected override void StopThrust()
        {
            base.StopThrust();
            ticksAfterThrust = 0;
            //if (!ragdoll.asleep)
            //    RagdollManager.revThrustSound.Play(.5f, 0, 0);

        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            base.Draw(sb);

            if (!ragdoll.asleep)
                DrawStabilizerThrusters(sb);
        }

        private void DrawStabilizerThrusters(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            DrawAngularStabilizers(sb);
            DrawStoppingThruster(sb);
        }



        private void DrawAngularStabilizers(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            // nothing here yet
        }

        private void DrawStoppingThruster(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            if (stoppingThrustVector.X != 0 && stoppingThrustVector.Y != 0)
            {
                Vector2 particleAngle = new Vector2(-stoppingThrustVector.X, stoppingThrustVector.Y);
                //float rot = (float) Math.Atan2(stoppingThrustVector.Y, stoppingThrustVector.X) + (float)Math.PI / 2;;
                //SpriteEffects effect = SpriteEffects.None;
                //if (rand.Next(2) == 0) effect = SpriteEffects.FlipHorizontally;
                //Vector2 nozzle = Vector2.Normalize(ragdoll.Body.LinearVelocity) * 5 + ragdoll.Body.Position;

                //sb.Draw(RagdollManager.thrustTex, nozzle, null, Color.Orange, rot, new Vector2(64, 64), .001f * stoppingThrustVector.Length(), effect, 0);

                Vector2 screenLoc = ProjectionHelper.FarseerToPixel(ragdoll.Body.Position);
                ParticleEffectManager.flameEffect[0].ReleaseImpulse = particleAngle * 10;
                ParticleEffectManager.flameEffect[0].ReleaseScale.Value = particleAngle.Length() * 5;
                ParticleEffectManager.flameEffect.Trigger(screenLoc);
            }
        }

        
    }
}

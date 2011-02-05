using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using FarseerPhysics.DebugViews;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.Serialization;
using KinectRagdoll.Drawing;
using System.Diagnostics;


namespace KinectRagdoll.Kinect
{
    [DataContract(Name = "RagdollMuscle", Namespace = "http://www.imcool.com")]
    public class RagdollMuscle: Ragdoll
    {

        //protected TargetJoint rightShoulder;
        //protected TargetJoint rightElbow;
        [DataMember()]
        protected int sleepTimer = 0;
        [DataMember()]
        protected int postThrustTimer = 0;
        [DataMember()]
        protected bool asleep = false;
        [DataMember()]
        public bool thrustOn = false;
        private float feetThrust;
        private float rightHandThrust;
        private float leftHandThrust;
        Random rand = new Random();
        private int POST_THRUST_TIME = 100;

        private bool rightGrip;
        private bool leftGrip;
        private World world;


        private RevoluteJoint jRightGrip;
        private RevoluteJoint jLeftGrip;

        private const float slowDamping = .9f;
        private const float grabPlane = -400f;
        private const float releasePlane = -70f;
        private const float grabVel = -20;

        private int rightHandGrabGrace;
        private int leftHandGrabGrace;
        private const int grabGrace = 30;
       

        public RagdollMuscle(World w, Vector2 position) : base(w, position)
        {
            this.world = w;
            _head.AfterCollision += HeadCollision;
            

        }

        public override void Update(SkeletonInfo info)
        {

            
            base.Update(info);

            tick();

            if (!asleep)
            {

                if (!rightGrip &&
                    info.rightHand.Z < info.torso.Z + grabPlane &&
                    info.RightHandVel.Z < grabVel)
                {
                    rightHandGrabGrace = grabGrace;
                    //TryRightGrip();
                }

                if (rightHandGrabGrace > 0)
                {
                    TryRightGrip();
                }
                else if (rightGrip && info.rightHand.Z > info.torso.Z + releasePlane)
                {
                    ReleaseRightGrip();
                }

                if (!leftGrip &&
                    info.leftHand.Z < info.torso.Z + grabPlane &&
                    info.LeftHandVel.Z < grabVel)
                {
                    leftHandGrabGrace = grabGrace;
                    //TryLeftGrip();
                }
                if (leftHandGrabGrace > 0)
                {
                    TryLeftGrip();
                }
                else if (leftGrip && info.leftHand.Z > info.torso.Z + releasePlane)
                {
                    ReleaseLeftGrip();
                }


                if (info.torso.Z < info.head.Z)
                {
                    StartThrust(
                        (info.head.Z - info.torso.Z) * .02f,
                        (info.rightHand.Z - info.torso.Z) * .0008f,
                        (info.leftHand.Z - info.torso.Z) * .0008f);
                }
                else
                {
                    StopThrust();

                }
            }
        }

        private void TryLeftGrip()
        {
            Vector2 elbowLoc = jLeftArm.WorldAnchorA;
            Vector2 forearmLoc = _lowerLeftArm.Body.Position;
            Vector2 gripLoc = forearmLoc + (forearmLoc - elbowLoc) * 2;

            Fixture f = world.TestPoint(gripLoc);
            if (f != null)
            {
                if (jLeftGrip != null) world.RemoveJoint(jLeftGrip);
                jLeftGrip = new RevoluteJoint(_lowerLeftArm.Body, f.Body, _lowerLeftArm.Body.GetLocalPoint(gripLoc), f.Body.GetLocalPoint(gripLoc));
                world.AddJoint(jLeftGrip);
                leftGrip = true;
            }
        }

        private void ReleaseLeftGrip()
        {
            if (jLeftGrip != null) world.RemoveJoint(jLeftGrip);
            jLeftGrip = null;
            leftGrip = false;
        }

        private void ReleaseRightGrip()
        {
            if (jRightGrip != null) world.RemoveJoint(jRightGrip);
            jRightGrip = null;
            rightGrip = false;
        }

        private void TryRightGrip()
        {
            Vector2 elbowLoc = jRightArm.WorldAnchorA;
            Vector2 forearmLoc = _lowerRightArm.Body.Position;
            Vector2 gripLoc = forearmLoc + (forearmLoc - elbowLoc) * 2;

            Fixture f = world.TestPoint(gripLoc);
            if (f != null)
            {
                if (jRightGrip != null) world.RemoveJoint(jRightGrip);
                jRightGrip = new RevoluteJoint(_lowerRightArm.Body, f.Body, _lowerRightArm.Body.GetLocalPoint(gripLoc), f.Body.GetLocalPoint(gripLoc));
                world.AddJoint(jRightGrip);
                rightGrip = true;
            }
        }


        public void PostLoad(World w)
        {
            rand = new Random();
            world = w;
        }

        public void HeadCollision(Fixture f1, Fixture f2, Contact contact)
        {
            
            
            float maxImpulse = 0.0f;
            for (int i = 0; i < contact.Manifold.PointCount; ++i)
            {
                maxImpulse = Math.Max(maxImpulse, contact.Manifold.Points[i].NormalImpulse);
            }
            if (maxImpulse >= 100)
            {
                knockOut();
                sleepTimer = 200;
            }

            
        }


        
       

        

        public void tick()
        {

            if (asleep)
            {
                sleepTimer--;
                if (sleepTimer <= 0)
                {
                    wakeUp();
                }
            }

            if (postThrustTimer > 0)
            {
                postThrustTimer--;
                if (postThrustTimer == 0)
                    _body.Body.LinearDamping = 0;
            }

            Thrust();

            if (leftHandGrabGrace > 0) leftHandGrabGrace--;
            if (rightHandGrabGrace > 0) rightHandGrabGrace--;
        }

        private void knockOut()
        {
            asleep = true;
            jRightArm.MotorEnabled = false;
            jRightArmBody.MotorEnabled = false;
            jLeftArm.MotorEnabled = false;
            jLeftArmBody.MotorEnabled = false;
            jLeftLeg.MotorEnabled = false;
            jLeftLegBody.MotorEnabled = false;
            jRightLeg.MotorEnabled = false;
            jRightLegBody.MotorEnabled = false;
            _body.Body.LinearDamping = 0;
            StopThrust();
            _body.Body.AngularDamping = 0;
            ReleaseLeftGrip();
            ReleaseRightGrip();
        }

        private void wakeUp()
        {
            asleep = false;
            jRightArm.MotorEnabled = true;
            jRightArmBody.MotorEnabled = true;
            jLeftArm.MotorEnabled = true;
            jLeftArmBody.MotorEnabled = true;
            jLeftLeg.MotorEnabled = true;
            jLeftLegBody.MotorEnabled = true;
            jRightLeg.MotorEnabled = true;
            jRightLegBody.MotorEnabled = true;
            _body.Body.LinearDamping = slowDamping;
        }


        public override void setChestToHead(Vector2 vec)
        {

            if (asleep) return;

            float personAngle = (float)(Math.Atan2(vec.Y, vec.X) - Math.PI / 2);
            float ragdollAngle = _body.Body.Rotation;
            float diff = getRadDiff(ragdollAngle, personAngle);
            float torque = 150;
            if (diff < 0) torque *= -1;
            if (thrustOn) torque *= 2;

            _body.Body.ApplyTorque(torque);
            _body.Body.AngularDamping = 5f;

        }


        public override void setShoulderToRightHand(Vector2 vec)
        {
            float elbowAngle = getElbowAngle(vec.Length() * 1.2f);
            float shoulderAngle = -getShoulderAngle(vec, elbowAngle);

            setJointMotor(jRightArm, elbowAngle);
            setJointMotor(jRightArmBody, shoulderAngle);
        }


        public override void setShoulderToLeftHand(Vector2 vec)
        {
            float elbowAngle = -getElbowAngle(vec.Length() * 1.2f);
            float shoulderAngle = -getShoulderAngle(vec, elbowAngle);

            setJointMotor(jLeftArm, elbowAngle);
            setJointMotor(jLeftArmBody, shoulderAngle);
        }

        

        public override void setHipToLeftFoot(Vector2 vec)
        {
            float elbowAngle = getElbowAngle(vec.Length() * .8f);
            float shoulderAngle = -getShoulderAngle(vec, elbowAngle);

            setJointMotor(jLeftLeg, elbowAngle);
            setJointMotor(jLeftLegBody, shoulderAngle);
        }

        public override void setHipToRightFoot(Vector2 vec)
        {
            float elbowAngle = -getElbowAngle(vec.Length() * .8f);
            float shoulderAngle = -getShoulderAngle(vec, elbowAngle);

            setJointMotor(jRightLeg, elbowAngle);
            setJointMotor(jRightLegBody, shoulderAngle);
        }


        protected void setJointMotor(RevoluteJoint joint, float targetAngle)
        {
            float radDiff = getRadDiff(joint.JointAngle, targetAngle);
            joint.MotorSpeed = radDiff * 10;
            joint.MaxMotorTorque = 1700;

        }

        protected override void CreateExtraJoints(World world)
        {
            
        }

        protected override void CreateArmJoints(World world)
        {
            
            jLeftArm = new RevoluteJoint(
                _lowerLeftArm.Body, _upperLeftArm.Body, new Vector2(0, 1), new Vector2(0, -1));
            jLeftArm.CollideConnected = false;
            jLeftArm.MotorEnabled = true;
            jLeftArm.ReferenceAngle = (float)Math.PI;
            world.AddJoint(jLeftArm);

            jLeftArmBody = new RevoluteJoint(
                _upperLeftArm.Body, _body.Body, new Vector2(0, 1), new Vector2(-1, 1.5f));
            jLeftArmBody.CollideConnected = false;
            jLeftArmBody.MotorEnabled = true;
            jLeftArmBody.ReferenceAngle = (float)Math.PI / 2;
            world.AddJoint(jLeftArmBody);

            jRightArm = new RevoluteJoint(
                _lowerRightArm.Body, _upperRightArm.Body, new Vector2(0, 1), new Vector2(0, -1));
            jRightArm.CollideConnected = false;
            jRightArm.MotorEnabled = true;
            jRightArm.ReferenceAngle = (float)Math.PI;
            world.AddJoint(jRightArm);

            jRightArmBody = new RevoluteJoint(
                _upperRightArm.Body, _body.Body, new Vector2(0, 1), new Vector2(1, 1.5f));
            jRightArmBody.CollideConnected = false;
            jRightArmBody.MotorEnabled = true;
            jRightArmBody.ReferenceAngle = -(float)Math.PI / 2;
            world.AddJoint(jRightArmBody);
            
            
        }


        protected override void CreateLegJoints(World world)
        {
            jLeftLeg = new RevoluteJoint(_lowerLeftLeg.Body, _upperLeftLeg.Body,
                                                       new Vector2(0, 1.1f), new Vector2(0, -1));
            jLeftLeg.CollideConnected = false;
            jLeftLeg.MotorEnabled = true;
            jLeftLeg.ReferenceAngle = (float)Math.PI;
            world.AddJoint(jLeftLeg);

            jLeftLegBody = new RevoluteJoint(_upperLeftLeg.Body, _body.Body,
                                                           new Vector2(0, 1.1f), new Vector2(-0.8f, -1.9f));
            jLeftLegBody.CollideConnected = false;
            jLeftLegBody.MotorEnabled = true;
            jLeftLegBody.ReferenceAngle = -(float)Math.PI / 2;
            world.AddJoint(jLeftLegBody);

            jRightLeg = new RevoluteJoint(_lowerRightLeg.Body, _upperRightLeg.Body,
                                                        new Vector2(0, 1.1f), new Vector2(0, -1));
            jRightLeg.CollideConnected = false;
            jRightLeg.MotorEnabled = true;
            jRightLeg.ReferenceAngle = (float)Math.PI;
            world.AddJoint(jRightLeg);

            jRightLegBody = new RevoluteJoint(_upperRightLeg.Body, _body.Body,
                                                            new Vector2(0, 1.1f), new Vector2(0.8f, -1.9f));
            jRightLegBody.CollideConnected = false;
            jRightLegBody.MotorEnabled = true;
            jRightLegBody.ReferenceAngle = (float)Math.PI / 2;
            world.AddJoint(jRightLegBody);
        }

       


        private float getElbowAngle(float handDistance)
        {
            if (handDistance >= 2 * elbowDistance)
                return (float)Math.PI;
            return 2 * (float)Math.Asin((handDistance / 2) / elbowDistance);
        }

        private float getShoulderAngle(Vector2 toHand, float elbowAngle)
        {
            float handAngle = (float)Math.Atan2(toHand.Y, toHand.X);

            return handAngle - (float)Math.PI / 2 + elbowAngle / 2;
        }

        private float getRadDiff(float p, float targetAngle)
        {


            float a = targetAngle - p;
            while (a < -Math.PI) a += 2 * (float)Math.PI;
            while (a > Math.PI) a -= 2 * (float)Math.PI;
            
            //float a = targetAngle - p;
            //a %= (float)Math.PI * 2;

            //if (a > Math.PI)
            //{

            //    a -= 2 * (float)Math.PI;
                    
            //}

            Debug.Assert(Math.Abs(a) <= Math.PI);
            return a;
        }


       

        private void Thrust()
        {
            if (asleep || !thrustOn) return;
            applyLimbThrust(_lowerLeftLeg, Math.PI / 2, 3 * feetThrust);
            applyLimbThrust(_lowerRightLeg, Math.PI / 2, 3 * feetThrust);
            applyLimbThrust(_lowerRightArm, Math.PI / 2, 4f * rightHandThrust);
            applyLimbThrust(_lowerLeftArm, Math.PI / 2, 4f * leftHandThrust);
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

        public void Draw(SpriteBatch sb)
        {
            if (thrustOn && !asleep)
            {
                drawLimbThrust(_lowerLeftLeg, sb, feetThrust);
                drawLimbThrust(_lowerRightLeg, sb, feetThrust);
                drawLimbThrust(_lowerLeftArm, sb, leftHandThrust);
                drawLimbThrust(_lowerRightArm, sb, rightHandThrust);
                
            }

            if (leftGrip)
            {
                //Vector2 pixelLoc = game.projectionHelper.FarseerToPixel(jLeftGrip.WorldAnchorA);
                SpriteHelper.DrawCircle(sb, jLeftGrip.WorldAnchorA, 1, Color.OrangeRed);
            }

            if (rightGrip)
            {
                //Vector2 pixelLoc = game.projectionHelper.FarseerToPixel(jRightGrip.WorldAnchorA);
                SpriteHelper.DrawCircle(sb, jRightGrip.WorldAnchorA, 1, Color.OrangeRed);
            }

        }

        public void drawLimbThrust(Fixture limb, SpriteBatch sb, float thrustFactor)
        {

            Vector2 limbLoc = limb.Body.Position;
            float rot = limb.Body.Rotation + (float)Math.PI / 2;
            Vector2 vec = new Vector2((float)Math.Cos(rot), (float)Math.Sin(rot));
            SpriteEffects effect = SpriteEffects.None;
            if (rand.Next(2) == 0) effect = SpriteEffects.FlipHorizontally;
            sb.Draw(RagdollManager.thrustTex, limbLoc - vec * 1.5f, null, Color.White, limb.Body.Rotation + (float) Math.PI, new Vector2(64, 64), .012f * thrustFactor, effect, 0);
            
        }

        internal void StartThrust(float feetThrust, float rightHandThrust, float leftHandThrust)
        {
            thrustOn = true;

            if (rightHandThrust < 0) rightHandThrust = 0;
            if (leftHandThrust < 0) leftHandThrust = 0;
            if (feetThrust < 0) feetThrust = 0;

            this.feetThrust = feetThrust;
            //this.rightHandThrust = rightHandThrust + feetThrust;
            //this.leftHandThrust = leftHandThrust + feetThrust;
            this.rightHandThrust = feetThrust;
            this.leftHandThrust = feetThrust;

            

            postThrustTimer = POST_THRUST_TIME;
            _body.Body.LinearDamping = slowDamping;
        }

        internal void StopThrust()
        {
            thrustOn = false;
            feetThrust = 0;
        }

        
    }
}

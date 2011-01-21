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


namespace KinectTest2.Kinect
{
    class RagdollMuscle: Ragdoll
    {

        //protected TargetJoint rightShoulder;
        //protected TargetJoint rightElbow;

        protected int sleepTimer = 0;
        protected bool asleep = false;
        public bool thrustOn = false;
        Random rand = new Random();
        

        public RagdollMuscle(World world, Vector2 position) : base(world, position)
        {

            _head.AfterCollision += HeadCollision;
            

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
                return;
            }
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
        }


        public override void setChestToHead(Vector2 vec)
        {

            if (asleep) return;

            float personAngle = (float)(Math.Atan2(vec.Y, vec.X) - Math.PI / 2);
            float ragdollAngle = _body.Body.Rotation;
            float diff = getRadDiff(ragdollAngle, personAngle);
            float torque = 200;
            if (diff < 0) torque *= -1;
            if (thrustOn) torque *= 2;

            _body.Body.ApplyTorque(torque);
            _body.Body.AngularDamping = .5f;

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
            joint.MotorSpeed = radDiff * 20;
            joint.MaxMotorTorque = 1500;

        }

        protected override void CreateExtraJoints(World world)
        {
            _body.Body.LinearDamping = .9f;
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

            a %= 2 * (float)Math.PI;

            if (a > Math.PI) a = (float)Math.PI - a;

            return a;
        }


       

        public void Thrust(float thrustFactor)
        {
            if (asleep) return;
            applyLimbThrust(_lowerLeftLeg, Math.PI / 2, 3 * thrustFactor);
            applyLimbThrust(_lowerRightLeg, Math.PI / 2, 3 * thrustFactor);
            applyLimbThrust(_lowerRightArm, Math.PI / 2, 4f * thrustFactor);
            applyLimbThrust(_lowerLeftArm, Math.PI / 2, 4f * thrustFactor);
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
                drawLimbThrust(_lowerLeftLeg, sb);
                drawLimbThrust(_lowerRightLeg, sb);
                drawLimbThrust(_lowerLeftArm, sb);
                drawLimbThrust(_lowerRightArm, sb);
                
            }
        }

        public void drawLimbThrust(Fixture limb, SpriteBatch sb)
        {

            Vector2 limbLoc = limb.Body.Position;
            float rot = limb.Body.Rotation + (float)Math.PI / 2;
            Vector2 vec = new Vector2((float)Math.Cos(rot), (float)Math.Sin(rot));
            SpriteEffects effect = SpriteEffects.None;
            if (rand.Next(2) == 0) effect = SpriteEffects.FlipHorizontally;
            sb.Draw(RagdollManager.thrustTex, limbLoc - vec * 1.5f, null, Color.White, limb.Body.Rotation + (float) Math.PI, new Vector2(64, 64), .012f, effect, 0);
            
        }
    }
}

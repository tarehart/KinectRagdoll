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
using KinectRagdoll.Kinect;
using KinectRagdoll.MyMath;
using KinectRagdoll.Equipment;


namespace KinectRagdoll.Ragdoll
{
    [DataContract(Name = "RagdollMuscle", Namespace = "http://www.imcool.com")]
    public class RagdollMuscle: RagdollBase
    { 

        //protected TargetJoint rightShoulder;
        //protected TargetJoint rightElbow;
        protected int sleepTimer = 0;
        protected int wakeTimer = 0;

        [DataMember()]
        internal bool asleep { get; private set; }
        
        Random rand = new Random();
        
        private const int WAKE_TIME = 40;

        
        private World world;

        private Texture2D depthTex;
        private Vector2 depthTexLoc;
        private float depthTexRot;
        private float depthTexScale;




        private List<AbstractEquipment> equipment;
       

        public RagdollMuscle(World w, Vector2 position) : base(w, position)
        {

            Init(w);
            

            

        }

        public void Init(World w)
        {
            this.world = w;
            _head.AfterCollision += HeadCollision;
            rand = new Random();

            equipment = new List<AbstractEquipment>();
            equipment.Add(new StabilizedJetpack(this));
            equipment.Add(new PunchGuns(this, world, 20));
            equipment.Add(new SpideySilk(this, world, 80, 100));
        
        }

        public override void Update(SkeletonInfo info)
        {

            base.Update(info);

            tick();

            depthTexLoc = new Vector2(info.torso.X, info.torso.Y - .13f) * depthTex.Height * .5f + new Vector2(depthTex.Width / 2, depthTex.Height / 2); // this will need to be more complex
            depthTexRot = _body.Body.Rotation - info.Rotation;
            depthTexScale = info.torso.Z * .035f;

            
            if (!asleep)
            {
                foreach (AbstractEquipment e in equipment)
                {
                    e.Update(info);
                }
            }
            
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
            else
            {
                wakeTimer++;
                if (wakeTimer == WAKE_TIME)
                {
                    foreach (Fixture f in _allFixtures)
                    {
                        f.CollisionFilter.CollisionGroup = 0;
                    }
                }
            }

            

            
        }

        public event EventHandler KnockOut;
        public event EventHandler WakeUp;

        private void knockOut()
        {
            if (!asleep)
                RagdollManager.crackSound.Play();

            asleep = true;
            jRightArm.MotorEnabled = false;
            jRightArmBody.MotorEnabled = false;
            jLeftArm.MotorEnabled = false;
            jLeftArmBody.MotorEnabled = false;
            jLeftLeg.MotorEnabled = false;
            jLeftLegBody.MotorEnabled = false;
            jRightLeg.MotorEnabled = false;
            jRightLegBody.MotorEnabled = false;

            

            KnockOut(this, null);
           
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

            if (WakeUp != null)
                WakeUp(this, null);

            

            wakeTimer = 0;
            foreach (Fixture f in _allFixtures)
            {
                if (f != _head)
                    f.CollisionFilter.CollisionGroup = -1;
            }

        }


        public override void setChestToHead(Vector2 vec)
        {

            if (asleep) return;

            float personAngle = (float)(Math.Atan2(vec.Y, vec.X) - Math.PI / 2);
            float ragdollAngle = _body.Body.Rotation;
            float diff = MathHelp.getRadDiff(ragdollAngle, personAngle);
            float torque = 500;
            if (diff < 0) torque *= -1;

            _body.Body.ApplyTorque(torque);
            _body.Body.AngularDamping = 10f;

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
            float radDiff = MathHelp.getRadDiff(joint.JointAngle, targetAngle);
            joint.MotorSpeed = radDiff * 10;
            joint.MaxMotorTorque = 2500;

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

        

        public void Draw(SpriteBatch sb)
        {


            foreach (AbstractEquipment e in equipment)
            {
                e.Draw(sb);
            }

            if (depthTex != null && !asleep)
            {
                //sb.Draw(depthTex, _body.Body.Position, null, Color.White, depthTexRot, depthTexLoc, .2f, SpriteEffects.FlipVertically, .5f);

                sb.Draw(depthTex, _body.Body.Position, null, new Color(1, 1, 1, .8f), depthTexRot, depthTexLoc, depthTexScale, SpriteEffects.FlipVertically, 0);

                //sb.Draw(depthTex, new Vector2(-10, -10), Color.Blue);
            }
            
        }







        internal void setDepthTex(Texture2D depthTex)
        {
            this.depthTex = depthTex;
        }



        
    }
}

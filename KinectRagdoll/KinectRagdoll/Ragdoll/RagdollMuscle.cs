﻿using System;
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
using KinectRagdoll.Music;
using Microsoft.Xna.Framework.Content;


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
        private const int KNOCK_OUT_HIT = 120;


        
        private World world;

        //private Texture2D depthTex;
        //private Vector2 depthTexLoc;
        //private float depthTexRot;
        //private float depthTexScale;

        [DataMember()]
        private List<AbstractEquipment> equipment = new List<AbstractEquipment>();




        public List<AbstractEquipment> Equipment {
            get { return equipment; }
            set {
                foreach (AbstractEquipment e in equipment)
                {
                    e.Destroy();
                }
                equipment = value;
                foreach (AbstractEquipment e in equipment)
                {
                    e.Init(this);
                }
            } 
        }
       

        public RagdollMuscle(World w, Vector2 position) : base(w, position)
        {

            Init(w);

        }

       

        public virtual void Init(World w)
        {
            this.world = w;
            _head.FixtureList[0].AfterCollision += HeadCollision;
            rand = new Random();

            foreach (AbstractEquipment e in equipment)
            {
                e.Init(this);
            }

            
            //equipment = new List<AbstractEquipment>();
            //equipment.Add(new StabilizedJetpack(this));
            //equipment.Add(new PunchGuns(world, 20, this));
            //equipment.Add(new Flappers(this));
            //equipment.Add(new SpideySilk(world, 80, 100, this));
        
        }


        public override void Update(SkeletonInfo info)
        {

            if (wakeTimer < WAKE_TIME)
            {
                base.Update(SkeletonInfo.StandardPose);
            }
            else
            {
                base.Update(info);
            }

            

            tick();

            //depthTexLoc = new Vector2(info.torso.X, info.torso.Y - .13f) * depthTex.Height * .5f + new Vector2(depthTex.Width / 2, depthTex.Height / 2); // this will need to be more complex
            //depthTexRot = _body.Rotation - info.Rotation;
            //depthTexScale = info.torso.Z * .035f;

            
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
            if (wakeTimer < WAKE_TIME) return;
            
            float maxImpulse = 0.0f;
            for (int i = 0; i < contact.Manifold.PointCount; ++i)
            {
                maxImpulse = Math.Max(maxImpulse, contact.Manifold.Points[i].NormalImpulse);
            }
            if (maxImpulse >= KNOCK_OUT_HIT)
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
                    foreach (Body b in AllBodies)
                    {
                        b.CollisionGroup = 0;
                    }
                }
            }

            

            
        }

        public event EventHandler KnockOut;
        public event EventHandler WakeUp;

        protected virtual void knockOut()
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

            
            if (KnockOut != null)
                KnockOut(this, null);

        }

        protected virtual void wakeUp()
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
            foreach (Body b in AllBodies)
            {
                if (b != _head)
                    b.CollisionGroup = -1;
            }

        }


        public override void setChestToHead(Vector2 vec)
        {

            if (asleep) return;

            float personAngle = (float)(Math.Atan2(vec.Y, vec.X) - Math.PI / 2);
            float ragdollAngle = _body.Rotation;
            float diff = MathHelp.getRadDiff(ragdollAngle, personAngle);
            float torque = 800;
            if (diff < 0) torque *= -1;

            _body.ApplyTorque(torque);
            _body.AngularDamping = 20f;

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
                _lowerLeftArm, _upperLeftArm, new Vector2(0, 1), new Vector2(0, -1));
            jLeftArm.CollideConnected = false;
            jLeftArm.MotorEnabled = true;
            jLeftArm.ReferenceAngle = (float)Math.PI;
            world.AddJoint(jLeftArm);

            jLeftArmBody = new RevoluteJoint(
                _upperLeftArm, _body, new Vector2(0, 1), new Vector2(-1, 1.5f));
            jLeftArmBody.CollideConnected = false;
            jLeftArmBody.MotorEnabled = true;
            jLeftArmBody.ReferenceAngle = (float)Math.PI / 2;
            world.AddJoint(jLeftArmBody);

            jRightArm = new RevoluteJoint(
                _lowerRightArm, _upperRightArm, new Vector2(0, 1), new Vector2(0, -1));
            jRightArm.CollideConnected = false;
            jRightArm.MotorEnabled = true;
            jRightArm.ReferenceAngle = (float)Math.PI;
            world.AddJoint(jRightArm);

            jRightArmBody = new RevoluteJoint(
                _upperRightArm, _body, new Vector2(0, 1), new Vector2(1, 1.5f));
            jRightArmBody.CollideConnected = false;
            jRightArmBody.MotorEnabled = true;
            jRightArmBody.ReferenceAngle = -(float)Math.PI / 2;
            world.AddJoint(jRightArmBody);
            
            
        }


        protected override void CreateLegJoints(World world)
        {
            jLeftLeg = new RevoluteJoint(_lowerLeftLeg, _upperLeftLeg,
                                                       new Vector2(0, 1.1f), new Vector2(0, -1));
            jLeftLeg.CollideConnected = false;
            jLeftLeg.MotorEnabled = true;
            jLeftLeg.ReferenceAngle = (float)Math.PI;
            world.AddJoint(jLeftLeg);

            jLeftLegBody = new RevoluteJoint(_upperLeftLeg, _body,
                                                           new Vector2(0, 1.1f), new Vector2(-0.8f, -1.9f));
            jLeftLegBody.CollideConnected = false;
            jLeftLegBody.MotorEnabled = true;
            jLeftLegBody.ReferenceAngle = -(float)Math.PI / 2;
            world.AddJoint(jLeftLegBody);

            jRightLeg = new RevoluteJoint(_lowerRightLeg, _upperRightLeg,
                                                        new Vector2(0, 1.1f), new Vector2(0, -1));
            jRightLeg.CollideConnected = false;
            jRightLeg.MotorEnabled = true;
            jRightLeg.ReferenceAngle = (float)Math.PI;
            world.AddJoint(jRightLeg);

            jRightLegBody = new RevoluteJoint(_upperRightLeg, _body,
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

            //if (depthTex != null && !asleep)
            //{
                
            //    sb.Draw(depthTex, _body.Position, null, new Color(1, 1, 1, .1f), depthTexRot, depthTexLoc, depthTexScale, SpriteEffects.FlipVertically, 0);

               
            //}
            
        }







        //internal void setDepthTex(Texture2D depthTex)
        //{
        //    this.depthTex = depthTex;
        //}



        
    }
}

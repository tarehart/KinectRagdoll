﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using Microsoft.Xna.Framework;
using FarseerPhysics.DebugViews;
using FarseerPhysics.Factories;
using KinectRagdoll.Sandbox;
using System.Runtime.Serialization;
using KinectRagdoll.Kinect;

namespace KinectRagdoll.Ragdoll
{
    [DataContract(Name = "Ragdoll", Namespace = "http://www.imcool.com")]
    public abstract class RagdollBase
    {
        protected const float ArmDensity = .5f;
        protected const float LegDensity = .8f;
        protected const float LimbAngularDamping = .1f;
        protected static float elbowDistance = 2;

        [DataMember()]
        internal Fixture _body { get; private set; }
        [DataMember()]
        internal Fixture _head { get; private set; }

        [DataMember()]
        internal Fixture _lowerLeftArm { get; private set; }
        [DataMember()]
        internal Fixture _lowerLeftLeg { get; private set; }
        [DataMember()]
        internal Fixture _lowerRightArm { get; private set; }
        [DataMember()]
        internal Fixture _lowerRightLeg { get; private set; }

        [DataMember()]
        internal Fixture _upperLeftArm { get; private set; }
        [DataMember()]
        internal Fixture _upperLeftLeg { get; private set; }
        [DataMember()]
        internal Fixture _upperRightArm { get; private set; }
        [DataMember()]
        internal Fixture _upperRightLeg { get; private set; }

        [DataMember()]
        internal RevoluteJoint jRightArm;
        [DataMember()]
        internal RevoluteJoint jRightArmBody;
        [DataMember()]
        internal RevoluteJoint jLeftArm;
        [DataMember()]
        internal RevoluteJoint jLeftArmBody;
        [DataMember()]
        internal RevoluteJoint jRightLeg;
        [DataMember()]
        internal RevoluteJoint jRightLegBody;
        [DataMember()]
        internal RevoluteJoint jLeftLeg;
        [DataMember()]
        internal RevoluteJoint jLeftLegBody;

        [DataMember()]
        protected List<Fixture> _allFixtures;

        //protected World world;

         [DataMember()]
        public static float height = 10;


        public RagdollBase(World world, Vector2 position)
        {
            //this.world = world;
            CreateBody(world, position);
            CreateJoints(world);

        }

        public virtual void Update(SkeletonInfo info)
        {

            Vector3 vec = info.rightHand - info.rightShoulder;
            Vector2 v2 = info.project(vec, RagdollBase.height);
            setShoulderToRightHand(v2);

            //float pracScaler = v2.X / vec.X;

            vec = info.leftHand - info.leftShoulder;
            v2 = info.project(vec, RagdollBase.height);
            setShoulderToLeftHand(v2);

            vec = info.rightFoot - info.rightHip;
            v2 = info.project(vec, RagdollBase.height);
            setHipToRightFoot(v2);

            vec = info.leftFoot - info.leftHip;
            v2 = info.project(vec, RagdollBase.height);
            setHipToLeftFoot(v2);

            vec = info.head - info.torso;
            v2 = info.project(vec, RagdollBase.height);
            setChestToHead(v2);

            
        }


        public void Throw(Vector2 velocity)
        {
            foreach (Fixture f in _allFixtures)
            {
                f.Body.LinearVelocity = velocity;
            }
        }

        public abstract void setShoulderToRightHand(Vector2 vec);

        public abstract void setShoulderToLeftHand(Vector2 vec);

        public abstract void setChestToHead(Vector2 vec);

        public abstract void setHipToRightFoot(Vector2 vec);

        public abstract void setHipToLeftFoot(Vector2 vec);


        public Body Body
        {
            get { return _body.Body; }
        }

        //Torso
        private void CreateBody(World world, Vector2 position)
        {
            DebugMaterial matHead = new DebugMaterial(MaterialType.Face)
            {
                Color = Color.DeepSkyBlue,
                Scale = 2f
            };
            DebugMaterial matBody = new DebugMaterial(MaterialType.Squares)
            {
                Color = Color.DeepSkyBlue,
                Scale = 8f
            };

            //Head
            _head = FixtureFactory.CreateCircle(world, .9f, LegDensity, matHead);
            _head.Body.BodyType = BodyType.Dynamic;
            _head.Body.AngularDamping = LimbAngularDamping;
            //_head.Body.Mass = 2;
            _head.Body.Position = position;
            

            //Body
            _body = FixtureFactory.CreateRectangle(world, 2, 4, LegDensity, matBody);
            _body.Body.BodyType = BodyType.Dynamic;
            //_body.Body.Mass = 2;
            
            _body.Body.Position = position + new Vector2(0, -3);

            //Left Arm
            _lowerLeftArm = FixtureFactory.CreateRectangle(world, .7f, elbowDistance, ArmDensity, matBody);
            _lowerLeftArm.Body.BodyType = BodyType.Dynamic;
            _lowerLeftArm.Body.AngularDamping = LimbAngularDamping;
            //_lowerLeftArm.Body.Mass = 2;
            _lowerLeftArm.Friction = .2f;
            _lowerLeftArm.Body.Rotation = -1.4f;
            _lowerLeftArm.Body.Position = position + new Vector2(-4, -2.2f);

            _upperLeftArm = FixtureFactory.CreateRectangle(world, .7f, elbowDistance, ArmDensity, matBody);
            _upperLeftArm.Body.BodyType = BodyType.Dynamic;
            _upperLeftArm.Body.AngularDamping = LimbAngularDamping;
           // _upperLeftArm.Body.Mass = 2;
            _upperLeftArm.Body.Rotation = -1.4f;
            _upperLeftArm.Body.Position = position + new Vector2(-2, -1.8f);
            

            //Right Arm
            _lowerRightArm = FixtureFactory.CreateRectangle(world, .7f, elbowDistance, ArmDensity, matBody);
            _lowerRightArm.Body.BodyType = BodyType.Dynamic;
            _lowerRightArm.Body.AngularDamping = LimbAngularDamping;
            //_lowerRightArm.Body.Mass = 2;
            _lowerRightArm.Friction = .3f;
            _lowerRightArm.Body.Rotation = 1.4f;
            _lowerRightArm.Body.Position = position + new Vector2(4, -2.2f);

            _upperRightArm = FixtureFactory.CreateRectangle(world, .7f, elbowDistance, ArmDensity, matBody);
            _upperRightArm.Body.BodyType = BodyType.Dynamic;
            _upperRightArm.Body.AngularDamping = LimbAngularDamping;
            //_upperRightArm.Body.Mass = 2;
            _upperRightArm.Body.Rotation = 1.4f;
            _upperRightArm.Body.Position = position + new Vector2(2, -1.8f);
            

            //Left Leg
            _lowerLeftLeg = FixtureFactory.CreateRectangle(world, .7f, 2f, LegDensity, matBody);
            _lowerLeftLeg.Body.BodyType = BodyType.Dynamic;
            _lowerLeftLeg.Body.AngularDamping = LimbAngularDamping;
           // _lowerLeftLeg.Body.Mass = 2;
            _lowerLeftLeg.Friction = .5f;
            _lowerLeftLeg.Body.Position = position + new Vector2(-0.6f, -8);

            _upperLeftLeg = FixtureFactory.CreateRectangle(world, .7f, 2f, LegDensity, matBody);
            _upperLeftLeg.Body.BodyType = BodyType.Dynamic;
            _upperLeftLeg.Body.AngularDamping = LimbAngularDamping;
            _upperLeftLeg.Body.Mass = 2;
            _upperLeftLeg.Body.Position = position + new Vector2(-0.6f, -6);

            //Right Leg
            _lowerRightLeg = FixtureFactory.CreateRectangle(world, .7f, 2f, LegDensity, matBody);
            _lowerRightLeg.Body.BodyType = BodyType.Dynamic;
            _lowerRightLeg.Body.AngularDamping = LimbAngularDamping;
            //_lowerRightLeg.Body.Mass = 2;
            _lowerRightLeg.Friction = .5f;
            _lowerRightLeg.Body.Position = position + new Vector2(0.6f, -8);
            

            _upperRightLeg = FixtureFactory.CreateRectangle(world, .7f, 2f, LegDensity, matBody);
            _upperRightLeg.Body.BodyType = BodyType.Dynamic;
            _upperRightLeg.Body.AngularDamping = LimbAngularDamping;
            //_upperRightLeg.Body.Mass = 2;
            _upperRightLeg.Body.Position = position + new Vector2(0.6f, -6);


            _upperRightArm.CollisionFilter.IgnoreCollisionWith(_head);
            _upperLeftArm.CollisionFilter.IgnoreCollisionWith(_head);
            


            _allFixtures = new List<Fixture>();
            _allFixtures.Add(_body);
            _allFixtures.Add(_head);
            _allFixtures.Add(_lowerLeftArm);
            _allFixtures.Add(_lowerLeftLeg);
            _allFixtures.Add(_lowerRightArm);
            _allFixtures.Add(_lowerRightLeg);
            _allFixtures.Add(_upperLeftArm);
            _allFixtures.Add(_upperLeftLeg);
            _allFixtures.Add(_upperRightArm);
            _allFixtures.Add(_upperRightLeg);

           
        }

        protected virtual void CreateExtraJoints(World world)
        {

        }

        private void CreateJoints(World world)
        {
           
            CreateHeadJoint(world);

            CreateArmJoints(world);

            CreateLegJoints(world);

            CreateExtraJoints(world);
        }

        protected virtual void CreateLegJoints(World world)
        {
            jLeftLeg = new RevoluteJoint(_lowerLeftLeg.Body, _upperLeftLeg.Body,
                                                       new Vector2(0, 1.1f), new Vector2(0, -1));
            jLeftLeg.CollideConnected = false;
            world.AddJoint(jLeftLeg);

            jLeftLegBody = new RevoluteJoint(_upperLeftLeg.Body, _body.Body,
                                                           new Vector2(0, 1.1f), new Vector2(-0.8f, -1.9f));
            jLeftLegBody.CollideConnected = false;
            world.AddJoint(jLeftLegBody);

            jRightLeg = new RevoluteJoint(_lowerRightLeg.Body, _upperRightLeg.Body,
                                                        new Vector2(0, 1.1f), new Vector2(0, -1));
            jRightLeg.CollideConnected = false;
            world.AddJoint(jRightLeg);

            jRightLegBody = new RevoluteJoint(_upperRightLeg.Body, _body.Body,
                                                            new Vector2(0, 1.1f), new Vector2(0.8f, -1.9f));
            jRightLegBody.CollideConnected = false;
            world.AddJoint(jRightLegBody);
        }

        protected virtual void CreateArmJoints(World world)
        {
             jLeftArm = new RevoluteJoint(_lowerLeftArm.Body, _upperLeftArm.Body,
                                                       new Vector2(0, 1), new Vector2(0, -1));

            jLeftArm.CollideConnected = false;
            world.AddJoint(jLeftArm);

             jLeftArmBody = new RevoluteJoint(_upperLeftArm.Body, _body.Body,
                                                           new Vector2(0, 1), new Vector2(-1, 1.5f));
            jLeftArmBody.CollideConnected = false;
            world.AddJoint(jLeftArmBody);

            jRightArm = new RevoluteJoint(_lowerRightArm.Body, _upperRightArm.Body,
                                                        new Vector2(0, 1), new Vector2(0, -1));
            jRightArm.CollideConnected = false;
            world.AddJoint(jRightArm);

            jRightArmBody = new RevoluteJoint(_upperRightArm.Body, _body.Body,
                                                            new Vector2(0, 1), new Vector2(1, 1.5f));

            jRightArmBody.CollideConnected = false;
            world.AddJoint(jRightArmBody);
        }

        protected virtual void CreateHeadJoint(World world)
        {
            RevoluteJoint jHeadBody = new RevoluteJoint(_head.Body, _body.Body,
                                                        new Vector2(0, -1), new Vector2(0, 2));
            jHeadBody.CollideConnected = true;
            world.AddJoint(jHeadBody);
        }

        public bool OwnsFixture(Fixture f)
        {
            return _allFixtures.Contains(f);
        }
    }
}
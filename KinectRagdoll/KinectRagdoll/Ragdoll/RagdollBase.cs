using System;
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
        public bool Possessed { get; private set; }

        [DataMember()]
        internal Body _body { get; private set; }
        [DataMember()]
        internal Body _head { get; private set; }

        [DataMember()]
        internal Body _lowerLeftArm { get; private set; }
        [DataMember()]
        internal Body _lowerLeftLeg { get; private set; }
        [DataMember()]
        internal Body _lowerRightArm { get; private set; }
        [DataMember()]
        internal Body _lowerRightLeg { get; private set; }

        [DataMember()]
        internal Body _upperLeftArm { get; private set; }
        [DataMember()]
        internal Body _upperLeftLeg { get; private set; }
        [DataMember()]
        internal Body _upperRightArm { get; private set; }
        [DataMember()]
        internal Body _upperRightLeg { get; private set; }

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
        public List<Body> AllBodies;

        //protected World world;

         [DataMember()]
        public static float height = 10;

        public event EventHandler PossessedByPlayer;
        public event EventHandler UnpossessedByPlayer;


        public RagdollBase(World world, Vector2 position)
        {
            //this.world = world;
            CreateBody(world, position);
            CreateJoints(world);

        }


        /// <summary>
        /// Converts a location from gesture space to ragdoll space.
        /// Gesture space is the kinect cooridinate system except normalized against user height and centered on the user's torso.
        /// Ragdoll space is the coordinate system centered on and rotated with the ragdoll's body.
        /// </summary>
        /// <param name="gestureSpaceVector">A vector pointing from one position in gesture space to another.</param>
        /// <returns>The equivalent location in ragdoll space.</returns>
        public Vector2 GestureVectorToRagdollVector(Vector3 gestureSpaceLocation)
        {
            return new Vector2(height * gestureSpaceLocation.X, height * gestureSpaceLocation.Y);
        }


        


        /// <summary>
        /// Converts a relative vector (not a location) from kinect space to ragdoll space.
        /// Kinect space is the kinect cooridinate system.
        /// Ragdoll space is the coordinate system centered on and rotated with the ragdoll's body.
        /// </summary>
        /// <param name="gestureSpaceVector">A vector pointing from one position in kinect space to another.</param>
        /// <returns>The equivalent vector in ragdoll space.</returns>
        public Vector2 KinectVectorToRagdollVector(Vector3 kinectVector, SkeletonInfo info)
        {
            return GestureVectorToRagdollVector(info.VectorToGestureSpace(kinectVector));
           
        }

        

        public Vector2 RagdollLocationToFarseerLocation(Vector2 ragdollLocation)
        {

            Vector2 farseerLocation = ragdollLocation;
            Matrix m = Matrix.CreateRotationZ(Body.Rotation);
            farseerLocation = Vector2.Transform(farseerLocation, m);
            farseerLocation += Body.Position;

            return farseerLocation;
        }

        internal Vector2 RagdollVectorToFarseerVector(Vector2 ragdollVector)
        {
            Vector2 farseerVector = ragdollVector;
            Matrix m = Matrix.CreateRotationZ(Body.Rotation);
            farseerVector = Vector2.Transform(farseerVector, m);

            return farseerVector;
        }

        public virtual void Update(SkeletonInfo info)
        {

            Vector3 vec = info.rightHand - info.rightShoulder;
            Vector2 rSpace = KinectVectorToRagdollVector(vec, info);
            setShoulderToRightHand(rSpace);

            //float pracScaler = v2.X / vec.X;

            vec = info.leftHand - info.leftShoulder;
            rSpace = KinectVectorToRagdollVector(vec, info);
            setShoulderToLeftHand(rSpace);

            vec = info.rightFoot - info.rightHip;
            rSpace = KinectVectorToRagdollVector(vec, info);
            setHipToRightFoot(rSpace);

            vec = info.leftFoot - info.leftHip;
            rSpace = KinectVectorToRagdollVector(vec, info);
            setHipToLeftFoot(rSpace);

            vec = info.head - info.torso;
            rSpace = KinectVectorToRagdollVector(vec, info);
            setChestToHead(rSpace);

            if (!Possessed && info.Tracking)
            {
                Possessed = true;
                if (PossessedByPlayer != null)
                {
                    PossessedByPlayer(this, null);
                }
            }
            else if (Possessed && !info.Tracking)
            {
                Possessed = false;
                if (UnpossessedByPlayer != null)
                {
                    UnpossessedByPlayer(this, null);
                }
            }

            
        }


        public void Throw(Vector2 velocity)
        {
            foreach (Body b in AllBodies)
            {
                b.LinearVelocity = velocity;
            }
        }

        public abstract void setShoulderToRightHand(Vector2 vec);

        public abstract void setShoulderToLeftHand(Vector2 vec);

        public abstract void setChestToHead(Vector2 vec);

        public abstract void setHipToRightFoot(Vector2 vec);

        public abstract void setHipToLeftFoot(Vector2 vec);


        public Body Body
        {
            get { return _body; }
        }

        //Torso
        private void CreateBody(World world, Vector2 position)
        {
            Color ragdollColor = Color.Turquoise;

            DebugMaterial matHead = new DebugMaterial(MaterialType.Face)
            {
                Color = ragdollColor,
                Scale = 2f
            };
            DebugMaterial matShirt = new DebugMaterial(MaterialType.Tiles)
            {
                Color = ragdollColor,
                Scale = 2f
            };

            DebugMaterial matShorts = new DebugMaterial(MaterialType.Tiles)
            {
                Color = ragdollColor,
                Scale = 2f
            };

            DebugMaterial matSkin = new DebugMaterial(MaterialType.Blank)
            {
                Color = ragdollColor,
                Scale = 2f
            };

            //Head
            _head = new Body(world);
            FixtureFactory.AttachCircle(.9f, LegDensity, _head, matHead);
            _head.BodyType = BodyType.Dynamic;
            _head.AngularDamping = LimbAngularDamping;
            //_head.Body.Mass = 2;
            _head.Position = position;
            

            //Body
            _body = new Body(world); 
            FixtureFactory.AttachRectangle(2, 4, LegDensity, Vector2.Zero, _body, matShirt);
            _body.BodyType = BodyType.Dynamic;
            //_body.Mass = 2;
            
            _body.Position = position + new Vector2(0, -3);

            //Left Arm
            _lowerLeftArm = new Body(world); 
            FixtureFactory.AttachRectangle(.7f, elbowDistance, ArmDensity, Vector2.Zero, _lowerLeftArm, matSkin);
            _lowerLeftArm.BodyType = BodyType.Dynamic;
            _lowerLeftArm.AngularDamping = LimbAngularDamping;
            //_lowerLeftArm.Mass = 2;
            _lowerLeftArm.Friction = .2f;
            _lowerLeftArm.Rotation = -1.4f;
            _lowerLeftArm.Position = position + new Vector2(-4, -2.2f);

            _upperLeftArm = new Body(world);
            FixtureFactory.AttachRectangle(.7f, elbowDistance, ArmDensity, Vector2.Zero, _upperLeftArm, matShirt);
            _upperLeftArm.BodyType = BodyType.Dynamic;
            _upperLeftArm.AngularDamping = LimbAngularDamping;
           // _upperLeftArm.Body.Mass = 2;
            _upperLeftArm.Rotation = -1.4f;
            _upperLeftArm.Position = position + new Vector2(-2, -1.8f);
            

            //Right Arm
            _lowerRightArm = new Body(world);
            FixtureFactory.AttachRectangle(.7f, elbowDistance, ArmDensity, Vector2.Zero, _lowerRightArm, matSkin);
            _lowerRightArm.BodyType = BodyType.Dynamic;
            _lowerRightArm.AngularDamping = LimbAngularDamping;
            //_lowerRightArm.Mass = 2;
            _lowerRightArm.Friction = .3f;
            _lowerRightArm.Rotation = 1.4f;
            _lowerRightArm.Position = position + new Vector2(4, -2.2f);

            _upperRightArm = new Body(world);
            FixtureFactory.AttachRectangle(.7f, elbowDistance, ArmDensity, Vector2.Zero, _upperRightArm, matShirt);
            _upperRightArm.BodyType = BodyType.Dynamic;
            _upperRightArm.AngularDamping = LimbAngularDamping;
            //_upperRightArm.Body.Mass = 2;
            _upperRightArm.Rotation = 1.4f;
            _upperRightArm.Position = position + new Vector2(2, -1.8f);
            

            //Left Leg
            _lowerLeftLeg = new Body(world); 
            FixtureFactory.AttachRectangle(.7f, 2f, LegDensity, Vector2.Zero, _lowerLeftLeg, matSkin);
            _lowerLeftLeg.BodyType = BodyType.Dynamic;
            _lowerLeftLeg.AngularDamping = LimbAngularDamping;
           // _lowerLeftLeg.Mass = 2;
            _lowerLeftLeg.Friction = .5f;
            _lowerLeftLeg.Position = position + new Vector2(-0.6f, -8);

            _upperLeftLeg = new Body(world); 
            FixtureFactory.AttachRectangle(.7f, 2f, LegDensity, Vector2.Zero, _upperLeftLeg, matShorts);
            _upperLeftLeg.BodyType = BodyType.Dynamic;
            _upperLeftLeg.AngularDamping = LimbAngularDamping;
            _upperLeftLeg.Mass = 2;
            _upperLeftLeg.Position = position + new Vector2(-0.6f, -6);

            //Right Leg
            _lowerRightLeg = new Body(world); 
            FixtureFactory.AttachRectangle(.7f, 2f, LegDensity, Vector2.Zero, _lowerRightLeg, matSkin);
            _lowerRightLeg.BodyType = BodyType.Dynamic;
            _lowerRightLeg.AngularDamping = LimbAngularDamping;
            //_lowerRightLeg.Mass = 2;
            _lowerRightLeg.Friction = .5f;
            _lowerRightLeg.Position = position + new Vector2(0.6f, -8);


            _upperRightLeg = new Body(world); 
            FixtureFactory.AttachRectangle(.7f, 2f, LegDensity, Vector2.Zero, _upperRightLeg, matShorts);
            _upperRightLeg.BodyType = BodyType.Dynamic;
            _upperRightLeg.AngularDamping = LimbAngularDamping;
            //_upperRightLeg.Mass = 2;
            _upperRightLeg.Position = position + new Vector2(0.6f, -6);



            _upperRightArm.IgnoreCollisionWith(_head);
            _upperLeftArm.IgnoreCollisionWith(_head);
            


            AllBodies = new List<Body>();
            AllBodies.Add(_body);
            AllBodies.Add(_head);
            AllBodies.Add(_lowerLeftArm);
            AllBodies.Add(_lowerLeftLeg);
            AllBodies.Add(_lowerRightArm);
            AllBodies.Add(_lowerRightLeg);
            AllBodies.Add(_upperLeftArm);
            AllBodies.Add(_upperLeftLeg);
            AllBodies.Add(_upperRightArm);
            AllBodies.Add(_upperRightLeg);

           
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
            jLeftLeg = new RevoluteJoint(_lowerLeftLeg, _upperLeftLeg,
                                                       new Vector2(0, 1.1f), new Vector2(0, -1));
            jLeftLeg.CollideConnected = false;
            world.AddJoint(jLeftLeg);

            jLeftLegBody = new RevoluteJoint(_upperLeftLeg, _body,
                                                           new Vector2(0, 1.1f), new Vector2(-0.8f, -1.9f));
            jLeftLegBody.CollideConnected = false;
            world.AddJoint(jLeftLegBody);

            jRightLeg = new RevoluteJoint(_lowerRightLeg, _upperRightLeg,
                                                        new Vector2(0, 1.1f), new Vector2(0, -1));
            jRightLeg.CollideConnected = false;
            world.AddJoint(jRightLeg);

            jRightLegBody = new RevoluteJoint(_upperRightLeg, _body,
                                                            new Vector2(0, 1.1f), new Vector2(0.8f, -1.9f));
            jRightLegBody.CollideConnected = false;
            world.AddJoint(jRightLegBody);
        }

        protected virtual void CreateArmJoints(World world)
        {
             jLeftArm = new RevoluteJoint(_lowerLeftArm, _upperLeftArm,
                                                       new Vector2(0, 1), new Vector2(0, -1));

            jLeftArm.CollideConnected = false;
            world.AddJoint(jLeftArm);

             jLeftArmBody = new RevoluteJoint(_upperLeftArm, _body,
                                                           new Vector2(0, 1), new Vector2(-1, 1.5f));
            jLeftArmBody.CollideConnected = false;
            world.AddJoint(jLeftArmBody);

            jRightArm = new RevoluteJoint(_lowerRightArm, _upperRightArm,
                                                        new Vector2(0, 1), new Vector2(0, -1));
            jRightArm.CollideConnected = false;
            world.AddJoint(jRightArm);

            jRightArmBody = new RevoluteJoint(_upperRightArm, _body,
                                                            new Vector2(0, 1), new Vector2(1, 1.5f));

            jRightArmBody.CollideConnected = false;
            world.AddJoint(jRightArmBody);
        }

        protected virtual void CreateHeadJoint(World world)
        {
            RevoluteJoint jHeadBody = new RevoluteJoint(_head, _body,
                                                        new Vector2(0, -1), new Vector2(0, 2));
            jHeadBody.CollideConnected = true;
            world.AddJoint(jHeadBody);
        }

        public bool OwnsFixture(Fixture f)
        {
            foreach (Body b in AllBodies)
            {
                if (b.FixtureList.Contains(f))
                    return true;
            }
            return false;
        }

        internal bool OwnsBody(Body b)
        {
            return AllBodies.Contains(b);
        }

        public Vector2 Position { get { return Body.Position; } }
    }
}

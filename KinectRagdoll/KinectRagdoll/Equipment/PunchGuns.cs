using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using KinectRagdoll.Ragdoll;
using FarseerPhysics.Dynamics;
using KinectRagdoll.Kinect;
using FarseerPhysics.Factories;
using FarseerPhysics.DebugViews;
using FarseerPhysics.Dynamics.Contacts;
using System.Timers;
using System.Runtime.Serialization;

namespace KinectRagdoll.Equipment
{
    [DataContract(Name = "PunchGuns", Namespace = "http://www.imcool.com")]
    public class PunchGuns : PunchEquipment
    {

        public PunchGuns(World world, int cooldown, RagdollMuscle ragdoll = null) : base(world, cooldown, ragdoll)
        {
           
        }

        

        protected override void fire(Vector3 hand, Vector3 handVel, bool rightHand)
        {

            Vector2 farseerHandVel = ragdoll.RagdollVectorToFarseerVector( ragdoll.GestureVectorToRagdollVector(handVel));
            Vector2 farseerHandLoc = ragdoll.RagdollLocationToFarseerLocation(ragdoll.GestureVectorToRagdollVector(hand));


            new PunchBullet(farseerHandLoc + Vector2.Normalize(farseerHandVel) * 1f, farseerHandVel, world);
            
        }
       

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            // Nothing to draw
        }

        public class PunchBullet
        {

            private Fixture bullet;
            private World world;

            DebugMaterial matBullet = new DebugMaterial(MaterialType.Blank)
            {
                Color = Color.OrangeRed,
                Scale = 2f
            };

            public PunchBullet(Vector2 loc, Vector2 vel, World world)
            {
                this.world = world;
                bullet = FixtureFactory.AttachCircle(.4f, 5, new Body(world), matBullet);
                bullet.Body.Position = loc;
                bullet.Body.BodyType = BodyType.Dynamic;
                bullet.Body.LinearVelocity = vel * 70;
                bullet.Body.IsBullet = true;
                bullet.Body.Restitution = .3f;

                Timer time = new Timer(4000);
                time.AutoReset = false;

                time.Elapsed += new ElapsedEventHandler(time_Elapsed);
                time.Start();

                // Stop doing bullet computation after the first collision.
                bullet.AfterCollision += new AfterCollisionEventHandler(delegate(Fixture f1, Fixture f2, Contact c) { bullet.Body.IsBullet = false; });
              
                
            }

            void time_Elapsed(object sender, ElapsedEventArgs e)
            {

                Action a = delegate()
                {
                    world.RemoveBody(bullet.Body);
                };
                lock (KinectRagdollGame.pendingUpdates)
                {
                    KinectRagdollGame.pendingUpdates.Add(a);
                }
            }

        }

    }
}

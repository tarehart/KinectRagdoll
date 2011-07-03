﻿using System;
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

namespace KinectRagdoll.Equipment
{
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
                bullet = FixtureFactory.CreateCircle(world, .4f, 5, matBullet);
                bullet.Body.Position = loc;
                bullet.Body.BodyType = BodyType.Dynamic;
                bullet.Body.LinearVelocity = vel * 70;

                Timer time = new Timer(5000);
                time.AutoReset = false;

                time.Elapsed += new ElapsedEventHandler(time_Elapsed);
                time.Start();
              
                
            }

            void time_Elapsed(object sender, ElapsedEventArgs e)
            {
                world.RemoveBody(bullet.Body);
            }

        }

    }
}

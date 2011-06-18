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

namespace KinectRagdoll.Equipment
{
    public class PunchGuns : AbstractEquipment
    {

        private Vector2 leftPrev;
        private Vector2 rightPrev;
        private RagdollBase ragdoll;
        private World world;
        private int rightCooldown = 0;
        private int leftCooldown = 0;

        private const int COOLDOWN = 20;
        private const float FIRING_THRESHOLD = .3f;
        private const float EXTENSION_THRESHOLD = 3f;

        public PunchGuns(RagdollBase ragdoll, World world)
        {
            this.ragdoll = ragdoll;
            this.world = world;
        }

        public override void Update(Kinect.SkeletonInfo info)
        {

            Vector2 left = projectToFarseer(info.leftHand - info.torso, info);
            Vector2 right = projectToFarseer(info.rightHand - info.torso, info);

            if (rightCooldown > 0)
            {
                rightCooldown--;
            }
            else
            {
                
                Vector2 rightVel = right - rightPrev;

                if (rightVel.LengthSquared() > FIRING_THRESHOLD)
                {
                    if (awayFromBody(right, rightVel))
                    {
                        fire(right, rightVel);
                        rightCooldown = COOLDOWN;
                    }
                }

            }



            if (leftCooldown > 0)
            {
                leftCooldown--;
            }
            else
            {
               
                Vector2 leftVel = left - leftPrev;

                if (leftVel.LengthSquared() > FIRING_THRESHOLD)
                {
                    if (awayFromBody(left, leftVel))
                    {
                        fire(left, leftVel);
                        leftCooldown = COOLDOWN;
                    }
                }
            }



            rightPrev = right;
            leftPrev = left;



        }

        private void fire(Vector2 hand, Vector2 handVel)
        {
            Vector2 velNorm = handVel;
            velNorm.Normalize();
            new PunchBullet(hand + velNorm * 1f, handVel, world);
            
        }

        private bool awayFromBody(Vector2 hand, Vector2 handVel)
        {
            Vector2 bodyToHand = hand - ragdoll.Body.Position;
            if (bodyToHand.LengthSquared() > EXTENSION_THRESHOLD)
            {
                handVel.Normalize();
                bodyToHand.Normalize();
                return Vector2.Dot(handVel, bodyToHand) > .5f;
            }

            return false;
        }

        private Vector2 projectToFarseer(Vector3 hand, SkeletonInfo info)
        {
            Vector2 v = info.project(hand, RagdollBase.height);
            

            Matrix m = Matrix.CreateRotationZ(ragdoll.Body.Rotation);

            v = Vector2.Transform(v, m);

            v += ragdoll.Body.Position;

            return v;

        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            // Nothing to draw
        }

        public class PunchBullet
        {

            private Fixture bullet;
            private World world;
            private int bounceCount;
            

            DebugMaterial matBullet = new DebugMaterial(MaterialType.Blank)
            {
                Color = Color.OrangeRed,
                Scale = 2f
            };

            public PunchBullet(Vector2 loc, Vector2 vel, World world)
            {
                this.world = world;
                bullet = FixtureFactory.CreateCircle(world, .4f, 1, matBullet);
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

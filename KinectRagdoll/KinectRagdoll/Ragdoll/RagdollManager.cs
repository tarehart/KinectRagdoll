using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using KinectRagdoll.Ragdoll;
using Microsoft.Xna.Framework.Audio;

namespace KinectRagdoll.Kinect
{
    public class RagdollManager
    {

        internal RagdollMuscle ragdoll;

        public static Texture2D thrustTex;
        public static SoundEffect thrustSound;
        public static SoundEffect crackSound;
        public static SoundEffect revThrustSound;

        public RagdollManager()
        {
        }

        public void CreateNewRagdoll(KinectRagdollGame game)
        {

            ragdoll = new RagdollMuscle(game.farseerManager.world, Vector2.Zero);
            CameraShouldTrack = true;
            

        }

        public void LoadContent(ContentManager content)
        {
            thrustTex = content.Load<Texture2D>("thrust");
            thrustSound = content.Load<SoundEffect>("Sounds\\rocket");
            crackSound = content.Load<SoundEffect>("Sounds\\crack");
            revThrustSound = content.Load<SoundEffect>("Sounds\\rocketdown");

            
        }


        public void Update(SkeletonInfo info)
        {

            if (ragdoll != null)
            {
                ragdoll.Update(info);
            }

        }

        public void Draw(SpriteBatch sb)
        {
            if (ragdoll != null)
                ragdoll.Draw(sb);

        }


        internal RagdollMuscle GetFixtureOwner(Fixture f)
        {
            if (ragdoll.OwnsFixture(f)) return ragdoll;
            return null;
        }

        internal bool OwnsJoint(FarseerPhysics.Dynamics.Joints.Joint j)
        {
            return false;
        }

        internal Vector2 getRagdollCenter()
        {
            if (ragdoll == null) return Vector2.Zero;
            return ragdoll.Body.Position;
        }



        public bool CameraShouldTrack { get; set; }

        internal bool OwnsBody(Body b)
        {
            return ragdoll.OwnsBody(b);
        }
    }
}

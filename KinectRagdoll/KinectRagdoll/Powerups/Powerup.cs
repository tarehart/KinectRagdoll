using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using KinectRagdoll.Ragdoll;
using KinectRagdoll.Kinect;
using KinectRagdoll.Equipment;
using Microsoft.Xna.Framework.Graphics;
using KinectRagdoll.Sandbox;
using KinectRagdoll.Drawing;
using Microsoft.Xna.Framework;
using System.Runtime.Serialization;

namespace KinectRagdoll.Powerups
{

    [DataContract(Name = "Powerup", Namespace = "http://www.imcool.com")]
    public class Powerup
    {

        [DataMember()]
        public Fixture Fixture { get; private set; }
        private RagdollManager ragdollManager;
        private FarseerManager farseerManager;

        public bool Taken { get; private set; }

        public event EventHandler PickedUp;


        public List<AbstractEquipment> Equipment
        {
            get
            {
                List<AbstractEquipment> e = new List<AbstractEquipment>();
                if (JetPack) e.Add(new StabilizedJetpack());
                if (PeaShooter) e.Add(new PunchGuns(farseerManager.world, 20));
                if (SpiderSilk) e.Add(new SpideySilk(farseerManager.world, 20, 100));
                if (Flappers) e.Add(new Flappers());
                return e;
            }
        }


        internal Powerup(Fixture f, RagdollManager ragdollManager, FarseerManager farseerManager)
        {
            this.Fixture = f;
            this.ragdollManager = ragdollManager;
            this.farseerManager = farseerManager;

            f.BeforeCollision += new BeforeCollisionEventHandler(f_BeforeCollision);

            FarseerTextures.ApplyTexture(Fixture, FarseerTextures.TextureType.Powerup);
        }

        [DataMember()]
        public bool JetPack { get; set; }
        [DataMember()]
        public bool PeaShooter { get; set; }
        [DataMember()]
        public bool SpiderSilk { get; set; }
        [DataMember()]
        public bool Flappers { get; set; }

        bool f_BeforeCollision(Fixture fixtureA, Fixture fixtureB)
        {
            Fixture other = fixtureA;
            if (Fixture == fixtureA) other = fixtureB;

            RagdollMuscle ragdoll = ragdollManager.GetFixtureOwner(other);

            if (ragdoll != null)
            {
                ApplyPowerup(ragdoll);
                farseerManager.world.RemoveBody(Fixture.Body);
                Taken = true;
                if (PickedUp != null)
                {
                    PickedUp(this, null);
                }
                return false;
            }

            return true;
        }

        public void ApplyPowerup(RagdollMuscle ragdoll)
        {
            ragdoll.Equipment = Equipment;
        }

        public void Draw(SpriteBatch sb)
        {
            if (JetPack)
            {
                SpriteHelper.DrawCircle(sb, Fixture.Body.Position + new Vector2(-.5f, .5f), 2, Color.Orange);
            }

            if (Flappers)
            {
                SpriteHelper.DrawCircle(sb, Fixture.Body.Position + new Vector2(.5f, .5f), 2, Color.Blue);
            }
            if (SpiderSilk)
            {
                SpriteHelper.DrawCircle(sb, Fixture.Body.Position + new Vector2(-.5f, -.5f), 2, Color.White);
            }
            if (PeaShooter)
            {
                SpriteHelper.DrawCircle(sb, Fixture.Body.Position + new Vector2(.5f, -.5f), 2, Color.Green);
            }
        }


        internal void RemoveCollisionHandler()
        {
            Fixture.BeforeCollision -= new BeforeCollisionEventHandler(f_BeforeCollision);
            FarseerTextures.ApplyTexture(Fixture, FarseerTextures.TextureType.Normal);
        }
    }
}

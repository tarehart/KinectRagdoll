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
    public class Powerup : Pickup
    {


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


        internal Powerup(Fixture f, RagdollManager ragdollManager, FarseerManager farseerManager) : base(f, ragdollManager, farseerManager)
        {

        }


        [DataMember()]
        public bool JetPack { get; set; }
        [DataMember()]
        public bool PeaShooter { get; set; }
        [DataMember()]
        public bool SpiderSilk { get; set; }
        [DataMember()]
        public bool Flappers { get; set; }


        public void ApplyPowerup(RagdollMuscle ragdoll)
        {
            ragdoll.Equipment = Equipment;
        }

        public override void Draw(SpriteBatch sb)
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


        protected override void ApplyTexture()
        {
            FarseerTextures.ApplyTexture(Fixture, FarseerTextures.TextureType.Powerup);
        }

        protected override void DoPickupAction(RagdollMuscle ragdoll)
        {
            ApplyPowerup(ragdoll);
        }
    }
}

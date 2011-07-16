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
using KinectRagdoll.Music;

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
                if (SpiderSilk) e.Add(new SpideySilk(farseerManager.world, 20, 50));
                if (Flappers) e.Add(new Flappers());
                return e;
            }
        }


        public Powerup(Body b, RagdollManager ragdollManager, FarseerManager farseerManager) : base(b, ragdollManager, farseerManager)
        {

        }

        public Powerup(RagdollManager r, FarseerManager f) : base(r, f)
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
        [DataMember()]
        public string Song { get; set; }

        public void ApplyPowerup(RagdollMuscle ragdoll)
        {
            ragdoll.Equipment = Equipment;
        }

        public override void Draw(SpriteBatch sb)
        {
            if (JetPack)
            {
                SpriteHelper.DrawCircle(sb, Body.Position + new Vector2(-.5f, .5f), 2, Color.Orange);
            }

            if (Flappers)
            {
                SpriteHelper.DrawCircle(sb, Body.Position + new Vector2(.5f, .5f), 2, Color.Blue);
            }
            if (SpiderSilk)
            {
                SpriteHelper.DrawCircle(sb, Body.Position + new Vector2(-.5f, -.5f), 2, Color.White);
            }
            if (PeaShooter)
            {
                SpriteHelper.DrawCircle(sb, Body.Position + new Vector2(.5f, -.5f), 2, Color.Green);
            }
        }


        protected override void ApplyTexture()
        {
            FarseerTextures.ApplyTexture(Body, FarseerTextures.TextureType.Powerup);
        }

        public override void DoPickupAction(RagdollMuscle ragdoll)
        {
            ApplyPowerup(ragdoll);

            if (Song != null)
                Jukebox.Play(Song);
        }
    }
}

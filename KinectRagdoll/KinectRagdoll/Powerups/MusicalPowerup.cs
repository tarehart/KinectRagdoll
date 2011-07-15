using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using KinectRagdoll.Kinect;
using Microsoft.Xna.Framework.Media;
using KinectRagdoll.Music;

namespace KinectRagdoll.Powerups
{
    public class MusicalPowerup : Powerup
    {
        public String Song {get; set;}

        public MusicalPowerup(Fixture f, RagdollManager rm, FarseerManager fm) : base(f, rm, fm)
        {
           
        }

        protected override void DoPickupAction(Ragdoll.RagdollMuscle ragdoll)
        {
            base.DoPickupAction(ragdoll);

            if (Song != null)
                Jukebox.Play(Song);
        }


    }
}

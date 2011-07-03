using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Graphics;
using KinectRagdoll.Kinect;
using KinectRagdoll.Equipment;

namespace KinectRagdoll.Powerups
{
    public class PowerupManager
    {

        private RagdollManager ragdollManager;
        private FarseerManager farseerManager;

        private Dictionary<Fixture, Powerup> powerups = new Dictionary<Fixture,Powerup>();

        public PowerupManager(RagdollManager r, FarseerManager f)
        {
            this.ragdollManager = r;
            this.farseerManager = f;
        }

        public Powerup AddPowerup(Fixture f)
        {
            Powerup p = new Powerup(f, ragdollManager, farseerManager);
            AddPowerup(p);

            return p;
        }

        private void AddPowerup(Powerup p)
        {
            if (powerups.ContainsKey(p.Fixture))
            {
                powerups[p.Fixture] = p;
            }
            else
            {
                powerups.Add(p.Fixture, p);
            }


            p.PickedUp += new EventHandler(p_PickedUp);
        }

        void p_PickedUp(object sender, EventArgs e)
        {
            powerups.Remove((sender as Powerup).Fixture);
        }


        public void Draw(SpriteBatch sb)
        {
            foreach (Powerup p in powerups.Values)
            {
                p.Draw(sb);
            }
        }

        public Powerup getPowerup(Fixture f)
        {
            if (powerups.ContainsKey(f))
            {
                return powerups[f];
            }

            return null;
        }




        internal void RemovePowerup(Fixture f)
        {
            if (powerups.ContainsKey(f))
            {
                powerups[f].RemoveCollisionHandler();
                powerups.Remove(f);

            }
        }

        public void Clear()
        {
            foreach (Powerup p in powerups.Values)
            {
                p.RemoveCollisionHandler();
            }

            powerups.Clear();

        }

        public List<Powerup> Powerups { get { return powerups.Values.ToList<Powerup>(); } }

        internal void LoadPowerups(List<Powerup> list)
        {

            Clear();

            if (list != null)
            {
                foreach (Powerup p in list)
                {
                    Powerup newP = AddPowerup(p.Fixture);
                    newP.SpiderSilk = p.SpiderSilk;
                    newP.JetPack = p.JetPack;
                    newP.PeaShooter = p.PeaShooter;
                    newP.Flappers = p.Flappers;
                }
            }
        }
    }
}

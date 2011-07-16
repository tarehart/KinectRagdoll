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

        private Dictionary<Body, Powerup> powerups = new Dictionary<Body, Powerup>();

        public PowerupManager(RagdollManager r, FarseerManager f)
        {
            this.ragdollManager = r;
            this.farseerManager = f;
        }

        public Powerup AddPowerup(Body b)
        {
            Powerup p = new Powerup(b, ragdollManager, farseerManager);
            AddPowerup(p);

            return p;
        }

        private void AddPowerup(Powerup p)
        {
            if (powerups.ContainsKey(p.Body))
            {
                powerups[p.Body] = p;
            }
            else
            {
                powerups.Add(p.Body, p);
            }


            p.PickedUp += new EventHandler(p_PickedUp);
        }

        void p_PickedUp(object sender, EventArgs e)
        {
            powerups.Remove((sender as Powerup).Body);
        }


        public void Draw(SpriteBatch sb)
        {
            foreach (Powerup p in powerups.Values)
            {
                p.Draw(sb);
            }
        }

        public Powerup getPowerup(Body b)
        {
            if (powerups.ContainsKey(b))
            {
                return powerups[b];
            }

            return null;
        }




        internal void RemovePowerup(Body b)
        {
            if (powerups.ContainsKey(b))
            {
                powerups[b].RemoveCollisionHandler();
                powerups.Remove(b);

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
                    Powerup newP = AddPowerup(p.Body);
                    newP.SpiderSilk = p.SpiderSilk;
                    newP.JetPack = p.JetPack;
                    newP.PeaShooter = p.PeaShooter;
                    newP.Flappers = p.Flappers;
                    newP.Song = p.Song;
                }
            }
        }
    }
}

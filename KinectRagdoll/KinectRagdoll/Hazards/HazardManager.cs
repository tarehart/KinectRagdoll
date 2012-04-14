using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KinectRagdoll.Kinect;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Graphics;

namespace KinectRagdoll.Hazards
{
    public class HazardManager
    {
        private FarseerManager farseerManager;
        private RagdollManager ragdollManager;

        private List<Hazard> hazards = new List<Hazard>();

        public List<Hazard> Hazards { get {return hazards;} }


        public HazardManager(FarseerManager f, RagdollManager r)
        {
            this.farseerManager = f;
            this.ragdollManager = r;
        }

        public void addHazard(Hazard h)
        {
            hazards.Add(h);
        }

        public void Update()
        {
            List<Hazard> removeList = new List<Hazard>();

            foreach (Hazard h in hazards)
            {
                if (h.IsOperational)
                    h.Update();
                else
                    removeList.Add(h);
            }

            foreach (Hazard h in removeList)
            {
                hazards.Remove(h);
            }
        }

        public void Draw(SpriteBatch sb)
        {
            foreach (Hazard h in hazards)
            {
                if (h.IsOperational)
                    h.Draw(sb);
            }
        }

        internal void LoadHazards(List<Hazard> list)
        {
            hazards = list;
            foreach (Hazard h in hazards)
            {
                h.Init(farseerManager.world, ragdollManager);
            }
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KinectRagdoll.Kinect;

namespace KinectRagdoll.Hazards
{
    public class HazardManager
    {
        private FarseerManager farseerManager;
        private RagdollManager ragdollManager;

        private List<Hazard> hazards = new List<Hazard>();

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
            foreach (Hazard h in hazards)
            {
                h.Update();
            }
        }

        
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace KinectRagdoll.Rules
{
    public class Objective
    {
        protected KinectRagdollGame game;

        public Objective(KinectRagdollGame g)
        {
            this.game = g;

        }

        public virtual void Begin()
        {

        }

        public virtual void Update()
        {

        }

        public virtual void Draw(SpriteBatch sb)
        {

        }


        public bool Complete { get; set; }
    }
}

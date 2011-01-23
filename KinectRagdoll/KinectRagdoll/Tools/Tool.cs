using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KinectRagdoll.Sandbox;
using KinectRagdoll;
using Microsoft.Xna.Framework.Graphics;

namespace KinectRagdoll.Sandbox
{
    public abstract class Tool
    {

        protected KinectRagdollGame game;

        public Tool(KinectRagdollGame game)
        {
            this.game = game;
        }

        public abstract void HandleInput();

        public abstract void Draw(SpriteBatch sb);

    }
}

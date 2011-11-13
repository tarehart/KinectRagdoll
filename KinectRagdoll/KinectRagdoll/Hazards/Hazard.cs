using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace KinectRagdoll.Hazards
{
    public abstract class Hazard
    {

        public abstract void Draw(SpriteBatch sb);

        public abstract void Update();

    }
}

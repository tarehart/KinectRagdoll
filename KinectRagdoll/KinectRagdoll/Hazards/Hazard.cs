using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.Serialization;
using FarseerPhysics.Dynamics;
using KinectRagdoll.Kinect;

namespace KinectRagdoll.Hazards
{
    [DataContract(Name = "Hazard", Namespace = "http://www.imcool.com")]
    [KnownType(typeof(Turret))]
    public abstract class Hazard
    {

        public abstract void Init(World w, RagdollManager r);

        public abstract void Draw(SpriteBatch sb);

        public abstract void Update();

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KinectRagdoll.Kinect;
using Microsoft.Xna.Framework.Graphics;

namespace KinectRagdoll.Equipment
{
    public abstract class AbstractEquipment
    {

        public abstract void Update(SkeletonInfo info);


        public abstract void Draw(SpriteBatch sb);
    }
}

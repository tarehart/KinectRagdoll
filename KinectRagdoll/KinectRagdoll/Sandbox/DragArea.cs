using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KinectRagdoll.Sandbox
{
    public class DragArea
    {

        
        private float Ymax;
        private float Ymin;
        private float Xmax;
        private float Xmin;

        public DragArea(Vector2 dragStart, Vector2 dragFinish)
        {
            Xmin = Math.Min(dragStart.X, dragFinish.X);
            Xmax = Math.Max(dragStart.X, dragFinish.X);
            Ymin = Math.Min(dragStart.Y, dragFinish.Y);
            Ymax = Math.Max(dragStart.Y, dragFinish.Y);
        }


        public Rectangle intRectangle { 
            get { return new Rectangle((int)Xmin, (int)Ymin, (int)(Xmax - Xmin), (int)(Ymax - Ymin));} 
        }
        public Vector2 center { get { return new Vector2((Xmin + Xmax) / 2, (Ymin + Ymax) / 2); } }
        public Vector2 topLeft { get { return new Vector2(Xmin, Ymin); } }
        public float width { get { return Xmax - Xmin; } }
        public float height { get { return Ymax - Ymin; } }


        public bool ContainsPixel(Vector2 v)
        {
            return v.X > Xmin && v.X < Xmax && v.Y > Ymin && v.Y < Ymax;
        }


        
        
    }
}

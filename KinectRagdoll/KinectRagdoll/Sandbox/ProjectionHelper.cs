using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KinectRagdoll.Sandbox
{
    public class ProjectionHelper
    {

        private Matrix farseerProjection;
        private Matrix farseerView;
        public Viewport viewport;
        //farseerProjection * Matrix.CreateScale(1, -1, 1)

        public ProjectionHelper(Viewport v, Matrix farseerToPixel)
        {
            this.farseerProjection = farseerToPixel;
            viewport = v;
        }

        public void Update(Matrix view)
        {
            this.farseerView = view;
        }

        public Vector2 PixelToFarseer(Vector2 position)
        {
            Vector3 v = new Vector3(position, 0);
            v = viewport.Unproject(v, farseerProjection , farseerView, Matrix.Identity);
            return new Vector2(v.X, v.Y);
        }


        internal bool InsidePixelBounds(Vector2 v)
        {
            return v.X > 0 && v.Y > 0 && v.X < viewport.Width && v.Y < viewport.Height;
        }

        internal Vector2 FarseerToPixel(Vector2 position)
        {
            Vector3 v = new Vector3(position, 0);
            v = viewport.Project(v, farseerProjection, farseerView, Matrix.Identity);
            return new Vector2(v.X, v.Y);
        }
    }
}

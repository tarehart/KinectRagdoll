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

        private static Matrix farseerProjection;
        private static Matrix farseerView;
        public static Viewport viewport;
        //farseerProjection * Matrix.CreateScale(1, -1, 1)

        public static void Init(Viewport v, Matrix farseerToPixel)
        {
            farseerProjection = farseerToPixel;
            viewport = v;
        }

        public static void Update(Matrix view)
        {
            farseerView = view;
        }

        public static Vector2 PixelToFarseer(Vector2 position)
        {
            Vector3 v = new Vector3(position, 0);
            v = viewport.Unproject(v, farseerProjection , farseerView, Matrix.Identity);
            return new Vector2(v.X, v.Y);
        }


        internal static bool InsidePixelBounds(Vector2 v)
        {
            return v.X > 0 && v.Y > 0 && v.X < viewport.Width && v.Y < viewport.Height;
        }

        internal static Vector2 FarseerToPixel(Vector2 position)
        {
            Vector3 v = new Vector3(position, 0);
            v = viewport.Project(v, farseerProjection, farseerView, Matrix.Identity);
            return new Vector2(v.X, v.Y);
        }
    }
}

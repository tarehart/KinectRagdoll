using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KinectTest2.Sandbox
{
    public class ProjectionHelper
    {

        private Matrix farseerToPixel;
        private Viewport viewport;
        //farseerProjection * Matrix.CreateScale(1, -1, 1)

        public ProjectionHelper(Viewport v, Matrix farseerToPixel)
        {
            this.farseerToPixel = farseerToPixel;
            this.viewport = v;
        }

        public Vector2 PixelToFarseer(Vector2 position)
        {
            Vector3 v = new Vector3(position, 0);
            v = viewport.Unproject(v, farseerToPixel , Matrix.Identity, Matrix.Identity);
            return new Vector2(v.X, v.Y);
        }

    }
}

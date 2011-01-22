using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;

namespace KinectTest2.Sandbox
{
    public class PhysicsObjectForm : GameForm
    {
        public virtual Object PlacePhysicsObject(Vector2 position, World world)
        {
            return null;
        }

        protected override void OnMouseEnter(EventArgs e)
        {

            FormManager.ActiveFixtureForm = this;
            base.OnMouseEnter(e);
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            if (Visible)
                FormManager.ActiveFixtureForm = this;
            
            base.OnVisibleChanged(e);
        }
    }
}

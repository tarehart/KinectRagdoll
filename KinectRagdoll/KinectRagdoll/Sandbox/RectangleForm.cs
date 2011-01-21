using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.DebugViews;

namespace KinectTest2.Sandbox
{
    public partial class RectangleForm : FixtureForm
    {


        public RectangleForm()
        {
            InitializeComponent();
        }

        private float RWidth
        {
            get
            {
                float f = -1;
                float.TryParse(width.Text, out f);
                return f;
            }
        }

        private float RHeight
        {
            get
            {
                float f = -1;
                float.TryParse(height.Text, out f);
                return f;
            }
        }

        private float RRotation
        {
            get
            {
                float f = 0;
                float.TryParse(rotation.Text, out f);
                return f * 2 * (float)Math.PI / 360;
            }
        }

        private BodyType RBodyType
        {
            get
            {
                if (checkBox1.Checked)
                {
                    return BodyType.Static;
                }
                return BodyType.Dynamic;
            }
        }


        public override void PlaceFixture(Vector2 position, World world)
        {
            DebugMaterial m = new DebugMaterial(MaterialType.Stars);
            Fixture f = FixtureFactory.CreateRectangle(world, RWidth, RHeight, 1, position, m);
            f.Body.Rotation = RRotation;
            f.Body.BodyType = RBodyType;
        }

    }
}

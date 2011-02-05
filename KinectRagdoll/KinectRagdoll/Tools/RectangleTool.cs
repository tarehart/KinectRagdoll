using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using KinectRagdoll.Sandbox;
using FarseerPhysics.DebugViews;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using KinectRagdoll.Drawing;

namespace KinectRagdoll.Tools
{
    class RectangleTool : DraggedTool
    {

        public RectangleTool(KinectRagdollGame game) : base(game)
        {
            
        }

        protected override Fixture CreateFixture(DragArea d)
        {
            DebugMaterial m = new DebugMaterial(MaterialType.Waves);
            m.Scale = 4;
            m.Color = Color.Green;

            Fixture f = FixtureFactory.CreateRectangle(game.farseerManager.world, d.width, d.height, 1, d.center, m);
            return f;
        }

        public override void Draw(SpriteBatch sb)
        {
            if (drawing)
                SpriteHelper.DrawRectangle(sb, GetPixelDragArea().intRectangle, new Color(100, 100, 255, 100));
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using KinectRagdoll.Sandbox;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.DebugViews;
using FarseerPhysics.Factories;
using KinectRagdoll.Drawing;

namespace KinectRagdoll.Tools
{
    class CircleTool : DraggedTool
    {

        
        public CircleTool(KinectRagdollGame game) : base(game)
        {
        }

        protected override Fixture CreateFixture(DragArea d)
        {
            DebugMaterial m = new DebugMaterial(MaterialType.Dots);
            m.Scale = 10;
            m.Color = Color.Green;

            Fixture f = FixtureFactory.CreateCircle(game.farseerManager.world, d.diagonal, 1, worldStart, m);
            return f;
        }

        public override void Draw(SpriteBatch sb)
        {

            //sb.Draw(dragTex, game.projectionHelper.FarseerToPixel(worldStart), null, Color.White, 0, new Vector2(dragTex.Width / 2, dragTex.Height / 2), 

            if (drawing)
                SpriteHelper.DrawCircle(sb, game.projectionHelper.FarseerToPixel(worldStart), GetPixelDragArea().diagonal * 2, new Color(100, 100, 255, 100));

            //if (drawing)
            //    sb.Draw(dragTex, GetPixelDragArea().intRectangle, new Color(100, 100, 255, 100));

            
        }
    }
}
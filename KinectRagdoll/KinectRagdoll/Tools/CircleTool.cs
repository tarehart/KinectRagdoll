using System;
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


            Fixture f = FixtureFactory.AttachCircle(d.diagonal, 1, new Body(game.farseerManager.world));
            f.Body.Position = worldStart;
            FarseerTextures.ApplyTexture(f, FarseerTextures.TextureType.Normal);

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

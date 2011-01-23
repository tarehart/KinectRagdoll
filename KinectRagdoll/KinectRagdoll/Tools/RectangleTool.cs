using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KinectRagdoll.Sandbox;
using KinectRagdoll;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.DebugViews;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

namespace KinectRagdoll.Sandbox
{

    

    class RectangleTool : Tool
    {
        private bool drawing = false;
        private Vector2 worldStart;
        private Vector2 pixelStart;
        private DragArea d;
        private Texture2D dragTex;


        public RectangleTool(KinectRagdollGame game, Texture2D dragTex) : base(game)
        {
            this.dragTex = dragTex;
        }

        public override void HandleInput()
        {
            InputHelper input = game.inputManager.inputHelper;
            Vector2 pixel = input.MousePosition;
            Vector2 worldLoc = game.projectionHelper.PixelToFarseer(pixel);

            if (input.IsNewButtonPress(MouseButtons.LeftButton) && !drawing)
            {
                drawing = true;
                worldStart = worldLoc;
                pixelStart = pixel;
                d = new DragArea(worldStart, worldStart);
            }
            else if (input.IsOldButtonPress(MouseButtons.LeftButton) && drawing)
            {
                drawing = false;
                d = new DragArea(worldStart, worldLoc);
                if (d.width > 0 && d.height > 0)
                {

                    DebugMaterial m = new DebugMaterial(MaterialType.Stars);

                    Fixture f = FixtureFactory.CreateRectangle(game.farseerManager.world, d.width, d.height, 1, d.center, m);

                    FormManager.Property.setSelectedObject(f.Body);
                }
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            if (drawing)
                sb.Draw(dragTex, new DragArea(pixelStart, game.inputManager.inputHelper.MousePosition).intRectangle, new Color(100, 100, 255, 100));
        }

        
    }
}

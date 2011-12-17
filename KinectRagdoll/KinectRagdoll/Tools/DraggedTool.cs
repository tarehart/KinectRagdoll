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

namespace KinectRagdoll.Tools
{

    

    public abstract class DraggedTool : Tool
    {
        protected bool drawing = false;
        protected Vector2 worldStart;
        //private Vector2 pixelStart;
        private Vector2 worldLoc;
        //private DragArea d;


        public DraggedTool(KinectRagdollGame game) : base(game)
        {
        }

        public override void HandleInput()
        {
            InputHelper input = game.inputManager.inputHelper;
            Vector2 pixel = input.MousePosition;
            worldLoc = ProjectionHelper.PixelToFarseer(pixel);

            if (input.IsNewButtonPress(MouseButtons.LeftButton) && !drawing)
            {
                drawing = true;
                worldStart = worldLoc;
                //pixelStart = pixel;
                //d = new DragArea(worldStart, worldStart);
            }
            else if (input.IsOldButtonPress(MouseButtons.LeftButton) && drawing)
            {
                drawing = false;
                DragArea d = new DragArea(worldStart, worldLoc);
                if (d.width > 0 && d.height > 0)
                {

                    Fixture f = CreateFixture(d);

                    FormManager.Property.setPendingObjects(new List<object>() { f.Body });
                    //FormManager.Property.setSelectedObject(f.Body);

                    f.Restitution = .3f;

                    if (input.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
                    {
                        f.Body.BodyType = BodyType.Dynamic;
                        FarseerTextures.ApplyTexture(f, FarseerTextures.TextureType.Normal);
                    }
                }
            }
        }

        protected abstract Fixture CreateFixture(DragArea d);


        protected DragArea GetPixelDragArea()
        {
            return new DragArea(ProjectionHelper.FarseerToPixel(worldStart), game.inputManager.inputHelper.MousePosition);
        }

        protected DragArea GetWorldDragArea()
        {
            return new DragArea(worldStart, worldLoc);
        }

        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KinectRagdoll.Sandbox;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using KinectRagdoll.Hazards;

namespace KinectRagdoll.Tools
{
    class WallPopperTool : Tool
    {

        public WallPopperTool(KinectRagdollGame game) : base(game)
        {

        }

        public override void HandleInput()
        {
            InputHelper input = game.inputManager.inputHelper;

            if (input.IsNewButtonPress(MouseButtons.LeftButton))
            {
                Vector2 position = ProjectionHelper.PixelToFarseer(input.MousePosition);

                if (game.farseerManager.world.TestPoint(position) == null)
                {
                    WallPopper popper = new WallPopper(position, game.farseerManager.world, game.ragdollManager);
                    game.hazardManager.addHazard(popper);
                }
                
            }
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KinectRagdoll.Sandbox;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using KinectRagdoll.Rules;

namespace KinectRagdoll.Tools
{
    class ObjectiveTool : Tool
    {

        public ObjectiveTool(KinectRagdollGame g)
            : base(g)
        {

        }

        public override void HandleInput()
        {

             InputHelper input = game.inputManager.inputHelper;

             if (input.IsNewButtonPress(MouseButtons.LeftButton))
             {
                 Vector2 position = game.projectionHelper.PixelToFarseer(input.MousePosition);

                 Objective o = new StopwatchObjective(game, position, 100);
                 game.objectiveManager.AddObjective(o);

                 o.Begin();

             }
            

        }

        public override void Draw(SpriteBatch sb)
        {
            // Do nothing
        }

    }
}

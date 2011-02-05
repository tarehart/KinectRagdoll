using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KinectRagdoll.Sandbox;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using KinectRagdoll.Rules;
using FarseerPhysics.Dynamics;
using FarseerPhysics.DebugViews;

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

                 List<Fixture> list = game.farseerManager.world.TestPointAll(position);

                 if (list.Count > 0)
                 {
                     Fixture f = list[0];
                     StopwatchObjective o = new StopwatchObjective(game, f);
                     game.objectiveManager.objectives.Add(o);
                 }
             }
             else if (input.IsNewButtonPress(MouseButtons.RightButton))
             {
                 Vector2 position = game.projectionHelper.PixelToFarseer(input.MousePosition);

                 List<Fixture> list = game.farseerManager.world.TestPointAll(position);

                 foreach (Fixture f in list)
                 {
                     foreach (Objective o in game.objectiveManager.objectives)
                     {
                         if (((StopwatchObjective)o).fixture == f)
                         {
                             game.objectiveManager.objectives.Remove(o);
                             break;
                         }
                     }
                 }

             }
            

        }

        public override void Draw(SpriteBatch sb)
        {
            // Do nothing
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KinectRagdoll.Sandbox;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using KinectRagdoll.Hazards;

namespace KinectRagdoll.Tools
{
    class LaserTurretTool : Tool
    {
        public LaserTurretTool(KinectRagdollGame game) : base(game)
        {

        }

        public override void HandleInput()
        {
            InputHelper input = game.inputManager.inputHelper;

            if (input.IsNewButtonPress(MouseButtons.LeftButton))
            {
                Vector2 position = ProjectionHelper.PixelToFarseer(input.MousePosition);

                List<Fixture> list = game.farseerManager.world.TestPointAll(position);

                LaserTurret t;

                if (list.Count == 0)
                    t = new LaserTurret(position, game.farseerManager.world, game.ragdollManager);
                else
                    t = new LaserTurret(position, game.farseerManager.world, game.ragdollManager, list[0]);

                game.hazardManager.addHazard(t);
                      
            }
        }

        public override void Draw(SpriteBatch sb)
        {
        }
    }
}

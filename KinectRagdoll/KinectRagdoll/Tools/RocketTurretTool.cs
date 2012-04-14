using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using KinectRagdoll.Sandbox;
using Microsoft.Xna.Framework;
using KinectRagdoll.Hazards;
using Microsoft.Xna.Framework.Graphics;

namespace KinectRagdoll.Tools
{
    class RocketTurretTool : Tool
    {

        public RocketTurretTool(KinectRagdollGame game)
            : base(game)
        {

        }

        public override void HandleInput()
        {
            InputHelper input = game.inputManager.inputHelper;

            if (input.IsNewButtonPress(MouseButtons.LeftButton))
            {
                Vector2 position = ProjectionHelper.PixelToFarseer(input.MousePosition);

                List<Fixture> list = game.farseerManager.world.TestPointAll(position);

                RocketTurret t;

                if (list.Count == 0)
                    t = new RocketTurret(position, game.farseerManager.world, game.ragdollManager);
                else
                    t = new RocketTurret(position, game.farseerManager.world, game.ragdollManager, list[0]);

                game.hazardManager.addHazard(t);

            }
        }

        public override void Draw(SpriteBatch sb)
        {
        }
    }
}

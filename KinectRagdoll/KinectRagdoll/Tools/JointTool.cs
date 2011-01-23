using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KinectRagdoll.Sandbox;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KinectRagdoll.Tools
{
    class JointTool : Tool
    {
        private KinectRagdollGame game;

        public JointTool(KinectRagdollGame game) : base(game)
        {
            this.game = game;
        }

        public override void HandleInput()
        {

            
            InputHelper input = game.inputManager.inputHelper;

            if (input.IsNewButtonPress(MouseButtons.LeftButton))
            {
                Vector2 position = game.projectionHelper.PixelToFarseer(input.MousePosition);

                List<Fixture> list = game.farseerManager.world.TestPointAll(position);


                if (list.Count > 1)
                {
                    RevoluteJoint j = new RevoluteJoint(list[0].Body, list[1].Body, list[0].Body.GetLocalPoint(position), list[1].Body.GetLocalPoint(position));

                    game.farseerManager.world.AddJoint(j);

                    FormManager.Property.setSelectedObject(j);
                }
            }
            
        }

        public override void Draw(SpriteBatch sb)
        {
            // Do nothing
        }
    }
}

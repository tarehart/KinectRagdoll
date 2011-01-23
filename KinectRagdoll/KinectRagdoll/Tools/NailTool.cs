using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KinectRagdoll.Sandbox;
using KinectRagdoll;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using Microsoft.Xna.Framework.Graphics;

namespace KinectRagdoll.Sandbox
{
    class NailTool : Tool
    {

        public NailTool(KinectRagdollGame game)
            : base(game)
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
                    FixedRevoluteJoint j = new FixedRevoluteJoint(list[0].Body, list[0].Body.GetLocalPoint(position), position);

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

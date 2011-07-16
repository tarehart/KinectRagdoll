using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using KinectRagdoll.Drawing;
using KinectRagdoll.Sandbox;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;

namespace KinectRagdoll.Tools
{
    class RopeTool : Tool
    {

        private Body startBody;
        private Vector2 startBodyLocal;

        public RopeTool(KinectRagdollGame game)
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
                        if (startBody == null)
                        {
                            startBody = list[0].Body;
                            startBodyLocal = startBody.GetLocalPoint(position);

                            
                        }
                        else
                        {
                            Body endBody = list[0].Body;
                            Vector2 endBodyLocal = endBody.GetLocalPoint(position);

                            RopeJoint j = new RopeJoint(startBody, endBody, startBodyLocal, endBodyLocal);
                            j.CollideConnected = true;
                            game.farseerManager.world.AddJoint(j);

                            FormManager.Property.setSelectedObject(j);

                            startBody = null;
                        }
                    }
                
            }
            
        }





        public override void Draw(SpriteBatch sb)
        {
            if (startBody != null)
            {
                
                SpriteHelper.DrawLine(sb, game.projectionHelper.FarseerToPixel(startBody.GetWorldPoint(startBodyLocal)), game.inputManager.inputHelper.MousePosition, 2f, Color.Blue);
            }

            
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using KinectRagdoll.Sandbox;

namespace KinectRagdoll.Sandbox
{
    public class InputManager
    {

        private KinectRagdollGame game;
        public InputHelper inputHelper;
        //public DragArea dragArea;
        public bool selectingRectangle;
        public static bool DisregardInputEvents;

        public InputManager(KinectRagdollGame game)
        {
            this.game = game;
            inputHelper = new InputHelper();
        }

        
        public void Update()
        {
            if (DisregardInputEvents)
            {
                return;
            }

            inputHelper.Update();

            //checkExit();
            if (inputHelper.IsPauseGame())
            {
                game.Exit();
            }


            Vector2 position = game.projectionHelper.PixelToFarseer(inputHelper.MousePosition);
            if (inputHelper.IsNewButtonPress(MouseButtons.RightButton))
            {
                FormManager.Property.setSelectedObject(game.farseerManager.world.TestPoint(position));
            }

            game.toolbox.Update();

            



            CheckKeyPresses();

        }

        

       

        private void CheckKeyPresses()
        {
            if (inputHelper.IsKeyDown(Keys.F))
            {
                FormManager.Property.FreezeSelected();
            }

            if (inputHelper.IsKeyDown(Keys.P))
            {
                FormManager.Property.Show();
            }

            if (inputHelper.IsKeyDown(Keys.R))
            {
                FormManager.Property.UnfreezeSelected();
            }
        }
       

    }
}

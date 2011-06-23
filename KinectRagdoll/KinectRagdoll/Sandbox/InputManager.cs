using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using KinectRagdoll.Sandbox;
using System.Threading;
using System.IO;

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
            if (DisregardInputEvents || !game.IsActive)
            {
                return;
            }

            inputHelper.Update();

            //checkExit();
            if (inputHelper.IsPauseGame())
            {
                game.Exit();
            }


            

            game.toolbox.Update();

            



            CheckKeyPresses();

        }

        

       

        private void CheckKeyPresses()
        {
            if (inputHelper.IsNewKeyPress(Keys.F))
            {
                game.actionCenter.PerformAction(ActionCenter.Actions.Freeze);
            }

            if (inputHelper.IsNewKeyPress(Keys.P))
            {
                game.actionCenter.PerformAction(ActionCenter.Actions.PropertyEditor);
            }

            if (inputHelper.IsNewKeyPress(Keys.R))
            {
                game.actionCenter.PerformAction(ActionCenter.Actions.Release);
            }

            if (inputHelper.IsNewKeyPress(Keys.Delete))
            {
                game.actionCenter.PerformAction(ActionCenter.Actions.Delete);
            }

            if (inputHelper.IsNewKeyPress(Keys.C) && inputHelper.IsKeyDown(Keys.LeftControl))
            {
                game.actionCenter.PerformAction(ActionCenter.Actions.Copy);
            }

            if (inputHelper.IsNewKeyPress(Keys.V) && inputHelper.IsKeyDown(Keys.LeftControl))
            {
                FormManager.Property.PasteSelected(game.projectionHelper.PixelToFarseer(inputHelper.MousePosition));
            }

            if (inputHelper.IsNewKeyPress(Keys.Space))
            {
                game.actionCenter.PerformAction(ActionCenter.Actions.StartTimer);
            }

            

            if (inputHelper.IsNewKeyPress(Keys.S) && inputHelper.IsKeyDown(Keys.LeftControl))
            {

                game.actionCenter.PerformAction(ActionCenter.Actions.Save);
                
            }

            if (inputHelper.IsNewKeyPress(Keys.O) && inputHelper.IsKeyDown(Keys.LeftControl))
            {
                game.actionCenter.PerformAction(ActionCenter.Actions.Open);
            }

            if (inputHelper.MouseScrollWheelVelocity != 0)
            {
                FormManager.Property.RotateSelected(inputHelper.MouseScrollWheelVelocity);
            }

           

                
        }

        
    }
}

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
                Fixture f = game.farseerManager.world.TestPoint(position);
                FormManager.Property.setSelectedObject(f);
                if (f != null)
                {
                    FormManager.Property.setPendingObjects(new List<object> { f.Body });
                }
                
            }

            game.toolbox.Update();

            



            CheckKeyPresses();

        }

        

       

        private void CheckKeyPresses()
        {
            if (inputHelper.IsNewKeyPress(Keys.F))
            {
                FormManager.Property.FreezeSelected();
            }

            if (inputHelper.IsNewKeyPress(Keys.P))
            {
                FormManager.Property.Show();
            }

            if (inputHelper.IsNewKeyPress(Keys.R))
            {
                FormManager.Property.UnfreezeSelected();
            }

            if (inputHelper.IsNewKeyPress(Keys.Delete))
            {
                FormManager.Property.DeleteSelected();
            }

            if (inputHelper.IsNewKeyPress(Keys.C) && inputHelper.IsKeyDown(Keys.LeftControl))
            {
                FormManager.Property.CopySelected();
            }

            if (inputHelper.IsNewKeyPress(Keys.V) && inputHelper.IsKeyDown(Keys.LeftControl))
            {
                FormManager.Property.PasteSelected(game.projectionHelper.PixelToFarseer(inputHelper.MousePosition));
            }

            if (inputHelper.IsNewKeyPress(Keys.S) && inputHelper.IsKeyDown(Keys.LeftControl))
            {

                Thread saveThread = new Thread(DoSave);
                saveThread.SetApartmentState(ApartmentState.STA);
                saveThread.Start();
                
            }

            if (inputHelper.IsNewKeyPress(Keys.O) && inputHelper.IsKeyDown(Keys.LeftControl))
            {
                Thread openThread = new Thread(DoOpen);
                openThread.SetApartmentState(ApartmentState.STA);
                openThread.Start();
            }

            if (inputHelper.MouseScrollWheelVelocity != 0)
            {
                FormManager.Property.RotateSelected(inputHelper.MouseScrollWheelVelocity);
            }
        }

        private void DoSave()
        {

            Serializer.Save(game.farseerManager.world, game, "pendingsave.xml");
            if (FormManager.Save.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                File.Delete(FormManager.Save.FileName);
                File.Copy("pendingsave.xml", FormManager.Save.FileName);
            }
        }

        private void DoOpen()
        {

            if (FormManager.Open.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                game.farseerManager.LoadWorld(FormManager.Open.FileName);
            }
        }
    }
}

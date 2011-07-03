using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

namespace KinectRagdoll.Sandbox
{
    public class ActionCenter
    {
        private KinectRagdollGame game;

        public ActionCenter(KinectRagdollGame game)
        {
            this.game = game;
        }

        public enum Actions
        {
            StartTimer,
            ResetTimer,
            Open,
            Save,
            PropertyEditor,
            Copy,
            Paste,
            Delete,
            Freeze,
            Release,
            ToggleCamera,
            PowerupEditor
        }

        public void PerformAction(Actions action)
        {
            switch (action)
            {
                case Actions.StartTimer:
                    game.objectiveManager.Countdown(3);
                    break;
                case Actions.ResetTimer:
                    game.objectiveManager.Reset();
                    break;
                case Actions.Open:
                    Thread openThread = new Thread(DoOpen);
                    openThread.SetApartmentState(ApartmentState.STA);
                    openThread.Start();
                    break;
                case Actions.Save:
                    Thread saveThread = new Thread(DoSave);
                    saveThread.SetApartmentState(ApartmentState.STA);
                    saveThread.Start();
                    break;
                case Actions.PropertyEditor:
                    FormManager.Property.Show();
                    break;
                case Actions.Copy:
                    FormManager.Property.CopySelected();
                    break;
                case Actions.Delete:
                    FormManager.Property.DeleteSelected();
                    break;
                case Actions.Freeze:
                    FormManager.Property.FreezeSelected();
                    break;
                case Actions.Release:
                    FormManager.Property.UnfreezeSelected();
                    break;
                case Actions.ToggleCamera:
                    game.ragdollManager.CameraShouldTrack = !game.ragdollManager.CameraShouldTrack;
                    break;
                case Actions.PowerupEditor:
                    object[] selection = FormManager.Property.getSelectedObjects();
                    if (selection.Length > 0)
                    {
                        PowerupForm p = new PowerupForm(game.powerupManager);
                        p.Show(selection);
                    }
                    
                    break;
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
                Action a = delegate()
                {
                    game.farseerManager.LoadWorld(FormManager.Open.FileName);
                };

                game.pendingUpdates.Add(a);
                
            }

        }

    }
}

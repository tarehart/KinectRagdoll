using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using KinectRagdoll.Music;

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
            PowerupEditor,
            ToggleFullScreen,
            Reload
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
                case Actions.Reload:
                    Thread reloadThread = new Thread(DoReload);
                    reloadThread.SetApartmentState(ApartmentState.STA);
                    reloadThread.Start();
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
                        PowerupForm p = new PowerupForm();
                        p.Show(selection);
                    }
                    
                    break;
                case Actions.ToggleFullScreen:
                    game.ToggleFullscreen();
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
                    Jukebox.Stop();
                    game.ragdollManager.ragdoll.bodySound.Stop();
                    game.farseerManager.LoadWorld(FormManager.Open.FileName);
                };

                KinectRagdollGame.pendingUpdates.Add(a);
                
            }

        }

        private void DoReload()
        {
            if (!String.IsNullOrWhiteSpace(FormManager.Open.FileName))
            {
                Action a = delegate()
                {
                    Jukebox.Stop();
                    game.ragdollManager.ragdoll.bodySound.Stop();
                    game.farseerManager.LoadWorld(FormManager.Open.FileName);
                };

                KinectRagdollGame.pendingUpdates.Add(a);

            }

        }

    }
}

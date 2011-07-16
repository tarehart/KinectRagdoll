using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using KinectRagdoll.Powerups;
using FarseerPhysics.Dynamics;
using KinectRagdoll.Equipment;
using KinectRagdoll.Music;
using KinectRagdoll.Kinect;
using KinectRagdoll.Ragdoll;

namespace KinectRagdoll.Sandbox
{
    public partial class PowerupForm : Form
    {

        private KinectRagdollGame game;
        
        private List<Body> selectedBodies = new List<Body>();

        public PowerupForm()
        {
            InitializeComponent();

            game = KinectRagdollGame.Main;
        }

        public void Show(object[] objects)
        {
            

            selectedBodies.Clear();
            foreach (object o in objects)
            {
                if (o is Fixture && !selectedBodies.Contains((o as Fixture).Body))
                {
                    selectedBodies.Add((o as Fixture).Body);
                }
            }


            bool skipFirst = true;

            musicList.Items.Add("");
            musicList.Items.AddRange(Jukebox.Playlist.ToArray());
           

            foreach (Body b in selectedBodies)
            {
                bool shouldClearSettings = false;

                Powerup p = game.powerupManager.getPowerup(b);
                if (p != null)
                {
                    shouldClearSettings = (populateForm(p) || shouldClearSettings) && !skipFirst;
                }

                if (shouldClearSettings) ClearSettings();

                skipFirst = false;
            }

            base.Show();
        }

        private void ClearSettings()
        {
            birdflap.Checked = false;
            jetpack.Checked = false;
            noShoot.Checked = true;
        }

        /// <summary>
        /// Returns true if there was a change.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private bool populateForm(Powerup p)
        {

            bool changed = false;

            changed = changed || (jetpack.Checked != p.JetPack);
            jetpack.Checked = p.JetPack;

            changed = changed || (birdflap.Checked != p.Flappers);
            birdflap.Checked = p.Flappers;



            changed = changed || (peashooters.Checked != p.PeaShooter || spidersilk.Checked != p.SpiderSilk);
            spidersilk.Checked = p.SpiderSilk;
            peashooters.Checked = p.PeaShooter;

            changed = changed || ((string)musicList.SelectedItem != p.Song);
            musicList.SelectedItem = p.Song;

            return changed;

        }

        private void populatePowerup(Powerup p)
        {
            p.JetPack = jetpack.Checked;
            p.Flappers = birdflap.Checked;
            p.SpiderSilk = spidersilk.Checked;
            p.PeaShooter = peashooters.Checked;

            p.Song = musicList.Text;
        }

        

        private void apply_Click(object sender, EventArgs e)
        {
            //List<AbstractEquipment> equipment = new List<AbstractEquipment>();

            //if (jetpack.Checked) equipment.Add(new StabilizedJetpack());


            foreach (Body b in selectedBodies)
            {

                Powerup p;

                if (b.FixtureList.Count > 0) {
                    RagdollMuscle r = game.ragdollManager.GetFixtureOwner(b.FixtureList[0]);
                    if (r != null)
                    {
                        p = new Powerup(game.ragdollManager, game.farseerManager);
                        populatePowerup(p);
                        p.DoPickupAction(r);
                        continue;
                    }
                }
                

                p = game.powerupManager.AddPowerup(b);
                populatePowerup(p);
            }

            Close();
        }

        private void remove_Click(object sender, EventArgs e)
        {
            foreach (Body b in selectedBodies)
            {
                game.powerupManager.RemovePowerup(b);
            }

            Close();
        }

    }
}

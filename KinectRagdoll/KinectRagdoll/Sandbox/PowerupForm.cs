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

namespace KinectRagdoll.Sandbox
{
    public partial class PowerupForm : Form
    {

        private PowerupManager powerupManager;
        private List<Fixture> selectedFixtures = new List<Fixture>();

        public PowerupForm(PowerupManager p)
        {
            InitializeComponent();

            powerupManager = p;
        }

        public void Show(object[] objects)
        {
            

            selectedFixtures.Clear();
            foreach (object o in objects)
            {
                if (o is Fixture)
                {
                    selectedFixtures.Add(o as Fixture);
                }
            }


            bool skipFirst = true;

            foreach (Fixture f in selectedFixtures)
            {
                bool shouldClearSettings = false;

                Powerup p = powerupManager.getPowerup(f);
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

            return changed;

        }

        private void populatePowerup(Powerup p)
        {
            p.JetPack = jetpack.Checked;
            p.Flappers = birdflap.Checked;
            p.SpiderSilk = spidersilk.Checked;
            p.PeaShooter = peashooters.Checked;
        }

        

        private void apply_Click(object sender, EventArgs e)
        {
            List<AbstractEquipment> equipment = new List<AbstractEquipment>();

            if (jetpack.Checked) equipment.Add(new StabilizedJetpack());


            foreach (Fixture f in selectedFixtures)
            {
                
                Powerup p = powerupManager.AddPowerup(f);
                populatePowerup(p);
            }

            Close();
        }

        private void remove_Click(object sender, EventArgs e)
        {
            foreach (Fixture f in selectedFixtures)
            {
                powerupManager.RemovePowerup(f);
            }

            Close();
        }

    }
}

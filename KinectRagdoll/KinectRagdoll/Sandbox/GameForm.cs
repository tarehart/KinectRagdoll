using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KinectRagdoll.Sandbox
{
    public class GameForm : Form
    {
        

        
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
            base.OnClosing(e);
        }

        protected override void  OnMouseEnter(EventArgs e)
        {
 
            InputManager.DisregardInputEvents = true;
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            InputManager.DisregardInputEvents = false;
            base.OnMouseLeave(e);
        }
    }
}

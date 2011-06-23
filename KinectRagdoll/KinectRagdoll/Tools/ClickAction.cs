using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KinectRagdoll.Sandbox;

namespace KinectRagdoll.Tools
{
    public class ClickAction
    {

        protected ActionCenter.Actions action;
        protected ActionCenter actionCenter;

        public ClickAction(ActionCenter actionCenter, ActionCenter.Actions action)
        {
            this.actionCenter = actionCenter;
            this.action = action;
        }

        public void PerformAction()
        {
            actionCenter.PerformAction(action);
        }

        
    }

}

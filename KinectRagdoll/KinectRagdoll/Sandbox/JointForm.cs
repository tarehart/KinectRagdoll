using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;

namespace KinectRagdoll.Sandbox
{
    public partial class JointForm : PhysicsObjectForm
    {
        public JointForm()
        {
            InitializeComponent();
        }

        public override Object PlacePhysicsObject(Microsoft.Xna.Framework.Vector2 position, FarseerPhysics.Dynamics.World world)
        {
            List<Fixture> list = world.TestPointAll(position);


            if (pin.Checked && list.Count > 0)
            {
                FixedRevoluteJoint j = new FixedRevoluteJoint(list[0].Body, list[0].Body.GetLocalPoint(position), position);
                if (motorEnabled.Checked)
                {
                    j.MotorEnabled = true;
                    float speed;
                    float maxTorque;
                    if (float.TryParse(motorSpeed.Text, out speed)) j.MotorSpeed = speed;
                    if (float.TryParse(motorTorque.Text, out maxTorque)) j.MaxMotorTorque = maxTorque;
                }

                world.AddJoint(j);
                return j;
            }
            
            if (list.Count > 1)
            {
                RevoluteJoint j = new RevoluteJoint(list[0].Body, list[1].Body, list[0].Body.GetLocalPoint(position), list[1].Body.GetLocalPoint(position));
                if (motorEnabled.Checked)
                {
                    j.MotorEnabled = true;
                    float speed;
                    float maxTorque;
                    if (float.TryParse(motorSpeed.Text, out speed)) j.MotorSpeed = speed;
                    if (float.TryParse(motorTorque.Text, out maxTorque)) j.MaxMotorTorque = maxTorque;
                }
                world.AddJoint(j);
                return j;
            }

            return base.PlacePhysicsObject(position, world);
        }

        private void motorEnabled_CheckedChanged(object sender, EventArgs e)
        {
            motorSpeed.Enabled = motorEnabled.Checked;
            motorTorque.Enabled = motorEnabled.Checked;
        }
    }
}

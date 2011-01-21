using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.DebugViews;
using FarseerPhysics.Dynamics.Joints;
using KinectTest2.Kinect;

namespace KinectTest2.Sandbox
{
    public partial class PropertyEditorForm : GameForm
    {
        private static Stack<List<Object>> history = new Stack<List<Object>>();
        private FarseerManager farseerManager;
        private object oldTexture;
        private object oldHoverTexture;
        private object lastHovered;
        private DebugMaterial selectTexture;
        private DebugMaterial hoverTexture;
        private MyComboBox comboBox;

        public PropertyEditorForm()
        {
            InitializeComponent();


            this.farseerManager = FarseerManager.Main;
            SetObject(farseerManager.world);

           selectTexture = new DebugMaterial(MaterialType.Stars)
            {
                Color = Microsoft.Xna.Framework.Color.Red,
                Scale = 2f
            };

           comboBox = new MyComboBox();
           Controls.Add(comboBox);
           comboBox.Location = comboPlaceholder.Location;
           comboBox.Width = comboPlaceholder.Width;
           comboBox.Height = comboPlaceholder.Height;
           comboPlaceholder.Visible = false;

           
           comboBox.SelectedIndexChanged += new EventHandler(comboBox_SelectedIndexChanged);
           comboBox.DropdownItemSelected += new MyComboBox.DropdownItemSelectedEventHandler(comboBox_DropdownItemSelected);
        }

        void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (comboBox.SelectedIndex < 0 || comboBox.SelectedIndex > comboBox.Items.Count) return;

            Object o = comboBox.Items[comboBox.SelectedIndex];
            List<Object> list = new List<object>();
            list.Add(o);
            history.Push(list);
            grid.SelectedObject = o;
        }





        void comboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            //List<Object> list = new List<object>();
            //list.Add(comboBox.SelectedValue);
            //history.Push(list);
            //grid.SelectedObject = comboBox.SelectedValue;
        }

        void comboBox_DropdownItemSelected(object sender, MyComboBox.DropdownItemSelectedEventArgs e)
        {

            if (e.SelectedItem < 0 || e.SelectedItem > comboBox.Items.Count) return;

            Object o = comboBox.Items[e.SelectedItem];
            doHighlighting(lastHovered, comboBox.Items[e.SelectedItem]);
            lastHovered = o;
        }

        private void zoomIn_Click(object sender, EventArgs e)
        {

            if (grid.SelectedGridItem.Value == null) return;

            if (TryLists(grid.SelectedGridItem.Value)) return;

            if (grid.SelectedGridItem.Value is IEnumerable<object>) {
                return;
            }

            if (HasProperties(grid.SelectedGridItem.Value.GetType()))
            {
                List<Object> list = new List<object>();
                list.Add(grid.SelectedGridItem.Value);
                history.Push(list);
                grid.SelectedObject = grid.SelectedGridItem.Value;
            }
        }

        private bool TryLists(object p)
        {
            Type t = grid.SelectedGridItem.Value.GetType();
            
            if (t == typeof(List<Body>))
            {
                List<Object> list = new List<Object>();
                List<Body> bodies = (List<Body>)p;
                list.AddRange(bodies);
                history.Push(list);
                grid.SelectedObjects = list.ToArray();
                return true;
            }

            return false;
        }

        private void zoomOut_Click(object sender, EventArgs e)
        {
            if (history.Count > 0)
            {
                grid.SelectedObjects = history.Pop().ToArray();
            }
        }

        private bool HasProperties(Type type)
        {
            if (type.IsPrimitive ||
                type == typeof(Decimal) ||
                type == typeof(String) ||
                type == typeof(Vector2) ||
                type == typeof(Vector3))
            {
                return false;
            }

            return type.GetProperties().Length > 0;
        }

        internal void SetObject(object o)
        {
            grid.SelectedObject = o;

            history.Clear();
            List<Object> list = new List<Object>();
            list.Add(o);
            history.Push(list);
            
        }

        private void grid_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
        {


            if (e.NewSelection.Value is IEnumerable<Object>)
            {

                IEnumerable<Object> list = (IEnumerable<Object>)e.NewSelection.Value;
                PopulateComboBox(list);
            }

            if (e.NewSelection.Value is JointEdge)
            {
                HashSet<Joint> joints = new HashSet<Joint>();
                JointEdge j = (JointEdge)e.NewSelection.Value;
                while (j != null)
                {
                    joints.Add(j.Joint);
                    j = j.Next;
                }

                PopulateComboBox(joints);

            }

            Object oldObj = null;
            Object newObj = null;
            if (e.OldSelection != null) oldObj = e.OldSelection.Value;
            if (e.NewSelection != null) newObj = e.NewSelection.Value;

            doHighlighting(oldObj, newObj);

        }

        private void PopulateComboBox(IEnumerable<Object> list)
        {
            Controls.Remove(comboBox);
            comboBox = new MyComboBox();
            Controls.Add(comboBox);
            comboBox.Location = comboPlaceholder.Location;
            comboBox.Width = comboPlaceholder.Width;
            comboBox.Height = comboPlaceholder.Height;

            comboBox.Items.AddRange(list.ToArray<object>());
            comboBox.SelectedText = "Values available";

            comboBox.SelectedIndexChanged += new EventHandler(comboBox_SelectedIndexChanged);
            comboBox.DropdownItemSelected += new MyComboBox.DropdownItemSelectedEventHandler(comboBox_DropdownItemSelected);
     
        }

        private void doHighlighting(Object old, Object newObj)
        {
            if (old != null)
            {
                Type tOld = old.GetType();

                if (tOld == typeof(Body))
                {
                    Body b = (Body)old;
                    if (oldTexture != null)
                        b.FixtureList[0].UserData = oldTexture;
                }
                else if (tOld == typeof(Fixture))
                {
                    Fixture f = (Fixture)old;
                    if (oldTexture != null)
                        f.UserData = oldTexture;
                }
            }

            if (newObj != null)
            {

                Type tNew = newObj.GetType();


                if (tNew == typeof(Body))
                {
                    Body b = (Body)newObj;
                    oldTexture = b.FixtureList[0].UserData;
                    b.FixtureList[0].UserData = selectTexture;
                }
                else if (tNew == typeof(Fixture))
                {
                    Fixture f = (Fixture)newObj;
                    oldTexture = f.UserData;
                    f.UserData = selectTexture;
                }
                else if (newObj is Joint)
                {
                    Joint j = (Joint)newObj;
                    farseerManager.setJointCursor(j);
                }


            }
        }
    }
}

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
        private Stack<Object> history = new Stack<Object>();
        private FarseerManager farseerManager;
        private DebugMaterial selectTexture;
        private DebugMaterial pendingTexture;
        private Dictionary<Object, DebugMaterial> materialBank = new Dictionary<object, DebugMaterial>();

        public PropertyEditorForm()
        {
            InitializeComponent();


            this.farseerManager = FarseerManager.Main;
            setSelectedObject(farseerManager.world);

            selectTexture = new DebugMaterial(MaterialType.Stars)
            {
                Color = Microsoft.Xna.Framework.Color.Red,
                Scale = 2f
            };

            pendingTexture = new DebugMaterial(MaterialType.Dots)
            {
                Color = Microsoft.Xna.Framework.Color.Yellow,
                Scale = 2f
            };

        }




        public void setSelectedObject(Object o, bool noHistory = false)
        {

            prepareForObjChange(noHistory);


            grid.SelectedObject = o;
            doHighlighting(null, o);

        }

        

        public void setSelectedObjects(IEnumerable<object> objects, bool noHistory = false, bool maintainSelection = false)
        {
            prepareForObjChange(noHistory, maintainSelection);

            grid.SelectedObjects = objects.ToArray<object>();

            foreach (Object b in objects)
            {
                doHighlighting(null, b);
            }

        }


        private void prepareForObjChange(bool noHistory, bool maintainSelection = false)
        {
            if (!noHistory)
            {
                if (grid.SelectedObject != null)
                {
                    history.Push(grid.SelectedObject);
                }
                else if (grid.SelectedObjects != null && grid.SelectedObjects.Length > 0)
                {
                    history.Push(grid.SelectedObjects);
                }
            }

            if (!maintainSelection)
                selectNone_Click(null, null);

            object[] highlighted = materialBank.Keys.ToArray<object>();
            foreach (Object b in highlighted)
            {
                doHighlighting(b, null);
            }
        }

       

        private void zoomIn_Click(object sender, EventArgs e)
        {

            if (grid.SelectedGridItem.Value == null) return;

            if (grid.SelectedGridItem.Value is IEnumerable<object>) {
                return;
            }

            if (HasProperties(grid.SelectedGridItem.Value.GetType()))
            {
                setSelectedObject(grid.SelectedGridItem.Value);
            }
        }


        private void zoomOut_Click(object sender, EventArgs e)
        {
            if (history.Count > 0)
            {
                Object o = history.Pop();
                if (o is IEnumerable<object>)
                {
                    IEnumerable<object> list = (IEnumerable<object>)o;
                    setSelectedObjects(list, true);
                }
                else
                {
                    setSelectedObject(o, true);
                }
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

        //internal void SetObject(object o)
        //{
        //    grid.SelectedObject = o;

        //    history.Clear();
        //    List<Object> list = new List<Object>();
        //    list.Add(o);
        //    history.Push(list);
            
        //}

        private void grid_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
        {


            if (e.NewSelection.Value is IEnumerable<Object>)
            {

                IEnumerable<Object> list = (IEnumerable<Object>)e.NewSelection.Value;
                PopulateSelectList(list);
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

                PopulateSelectList(joints);

            }

            Object oldObj = null;
            Object newObj = null;
            if (e.OldSelection != null) oldObj = e.OldSelection.Value;
            if (e.NewSelection != null) newObj = e.NewSelection.Value;

            doHighlighting(oldObj, newObj);

        }

        private void PopulateSelectList(IEnumerable<Object> list)
        {
            //Controls.Remove(comboBox);
            //comboBox = new MyComboBox();
            //Controls.Add(comboBox);
            //comboBox.Location = comboPlaceholder.Location;
            //comboBox.Width = comboPlaceholder.Width;
            //comboBox.Height = comboPlaceholder.Height;

            //comboBox.Items.AddRange(list.ToArray<object>());
            //comboBox.SelectedText = "Values available";

            //comboBox.SelectedIndexChanged += new EventHandler(comboBox_SelectedIndexChanged);
            //comboBox.DropdownItemSelected += new MyComboBox.DropdownItemSelectedEventHandler(comboBox_DropdownItemSelected);

            selectNone_Click(null, null);
            multipleSelect.Items.Clear();

            multipleSelect.Items.AddRange(list.ToArray<object>());

            selectFromList.Text = "Go to Selected";

     
        }


        private void doHighlighting(Object old, Object newObj, bool pending = false)
        {
            if (old != null)
            {
                //Type tOld = old.GetType();

                if (old is Body)
                {
                    Body b = (Body)old;
                    if (b.FixtureList != null)
                    {
                        if (materialBank.ContainsKey(b.FixtureList[0]))
                            b.FixtureList[0].UserData = materialBank[b.FixtureList[0]];
                        materialBank.Remove(b.FixtureList[0]);
                    }
                }
                else if (old is Fixture)
                {
                    Fixture f = (Fixture)old;
                    if (materialBank.ContainsKey(f))
                        f.UserData = materialBank[f];
                    materialBank.Remove(f);
                }
                else if (old is Joint)
                {
                    Joint j = (Joint)old;
                    farseerManager.selectedJoints.Remove(j);
                    farseerManager.pendingJoints.Remove(j);
                }
            }

            if (newObj != null)
            {

                //Type tNew = newObj.GetType();


                if (newObj is Body)
                {
                    Body b = (Body)newObj;
                    if (b.FixtureList != null)
                    {
                        if (!materialBank.ContainsKey(b.FixtureList[0]))
                            materialBank.Add(b.FixtureList[0], (DebugMaterial)b.FixtureList[0].UserData);
                        if (pending)
                        {
                            b.FixtureList[0].UserData = pendingTexture;
                        }
                        else
                        {
                            b.FixtureList[0].UserData = selectTexture;
                        }
                    }
                }
                else if (newObj is Fixture)
                {
                    Fixture f = (Fixture)newObj;
                    if (!materialBank.ContainsKey(f))
                        materialBank.Add(f, (DebugMaterial)f.UserData);
                    if (pending)
                    {
                        f.UserData = pendingTexture;
                    }
                    else
                    {
                        f.UserData = selectTexture;
                    }
                }
                else if (newObj is Joint)
                {
                    Joint j = (Joint)newObj;
                    if (pending)
                    {
                        farseerManager.pendingJoints.Add(j);
                        farseerManager.selectedJoints.Remove(j);
                    }
                    else
                    {
                        farseerManager.selectedJoints.Add(j);
                        farseerManager.pendingJoints.Remove(j);
                    }
                }


            }
        }

        

        private void selectFromList_Click(object sender, EventArgs e)
        {
            if (multipleSelect.CheckedIndices.Count == 0) return;

            if (multipleSelect.CheckedIndices.Count == 1)
            {
                setSelectedObject(multipleSelect.CheckedItems[0]);
            }
            else
            {
                
                object[] objs = new object[multipleSelect.CheckedItems.Count];
                multipleSelect.CheckedItems.CopyTo(objs, 0);
                setSelectedObjects(objs, false, true);
            }
        }

        private void multipleSelect_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            Object value = multipleSelect.Items[e.Index];

            if (e.NewValue == CheckState.Checked)
            {
                doHighlighting(null, value, true);
            }
            else
            {
                doHighlighting(value, null, true);
            }

            int numChecked = multipleSelect.CheckedIndices.Count - 1;
            if (e.NewValue == CheckState.Checked) numChecked += 2;


            if (numChecked > 1)
            {
                selectFromList.Text = "Batch Edit Selected";
            }
            else
            {
                selectFromList.Text = "Go to Selected";
            }
        }

        private void selectWorld_Click(object sender, EventArgs e)
        {
            setSelectedObject(farseerManager.world);
        }

        private void deleteSelected_Click(object sender, EventArgs e)
        {
            List<Object> toRemove = new List<object>();

            foreach (Object o in multipleSelect.CheckedItems)
            {
                if (o is Fixture || o is Body || o is Joint)
                {
                    doHighlighting(o, null); // remove from material bank
                    toRemove.Add(o);

                    if (o is Fixture)
                        farseerManager.world.RemoveBody(((Fixture)o).Body);
                    else if (o is Body)
                        farseerManager.world.RemoveBody((Body)o);
                    else if (o is Joint)
                        farseerManager.world.RemoveJoint((Joint)o);
                }
            }

            foreach (Object o in toRemove)
            {
                multipleSelect.Items.Remove(o);
            }
        }


        internal void setPendingObjects(List<object> selected)
        {
            PopulateSelectList(selected);
            selectAll_Click(null, null);
        }

        private void selectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < multipleSelect.Items.Count; i++)
            {
                multipleSelect.SetItemChecked(i, true);
            }
        }

        private void selectNone_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < multipleSelect.Items.Count; i++)
            {
                multipleSelect.SetItemChecked(i, false);
            }
        }

        private void selectBodies_Click(object sender, EventArgs e)
        {
            selectNone_Click(sender, e);
            
            for (int i = 0; i < multipleSelect.Items.Count; i++)
            {
                if (multipleSelect.Items[i] is Body)
                    multipleSelect.SetItemChecked(i, true);
            }
        }

        private void selectJoints_Click(object sender, EventArgs e)
        {

            selectNone_Click(sender, e);
            
            for (int i = 0; i < multipleSelect.Items.Count; i++)
            {
                if (multipleSelect.Items[i] is Joint)
                    multipleSelect.SetItemChecked(i, true);
            }
        }

        private void freezeSelected_Click(object sender, EventArgs e)
        {
            foreach (Object o in multipleSelect.CheckedItems)
            {
                if (o is Fixture)
                {
                    ((Fixture)o).Body.IsStatic = true;
                }
                else if (o is Body)
                {
                    ((Body)o).IsStatic = true;

                }
                
            }
        }

        private void unfreezeSelected_Click(object sender, EventArgs e)
        {
            foreach (Object o in multipleSelect.CheckedItems)
            {
                if (o is Fixture)
                {
                    ((Fixture)o).Body.IsStatic = false;
                }
                else if (o is Body)
                {
                    ((Body)o).IsStatic = false;

                }
            }
        }
    }
}

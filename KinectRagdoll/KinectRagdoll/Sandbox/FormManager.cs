using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KinectRagdoll.Sandbox
{


    class FormManager
    {

        private static RectangleForm rectangleForm;
        private static PropertyEditorForm propertyForm;
        private static JointForm jointForm;
        private static SaveFileDialog save;
        private static OpenFileDialog open;

        private static PhysicsObjectForm activeFixtureForm;

        public static RectangleForm Rectangle
        {
            get
            {
                if (rectangleForm == null) 
                    rectangleForm = new RectangleForm();
                return rectangleForm;
            }

        }

        public static PropertyEditorForm Property
        {
            get
            {
                if (propertyForm == null)
                    propertyForm = new PropertyEditorForm();
                return propertyForm;
            }

        }

        public static JointForm Joint
        {
            get
            {
                if (jointForm == null)
                    jointForm = new JointForm();
                return jointForm;
            }

        }

        public static SaveFileDialog Save
        {
            get {
                if (save == null)
                {
                    save = new SaveFileDialog();
                }
                return save;
            }

        }

        public static OpenFileDialog Open
        {
            get
            {
                if (open == null)
                {
                    open = new OpenFileDialog();
                }
                return open;
            }

        }


        public static PhysicsObjectForm ActiveFixtureForm { get { return activeFixtureForm; } set { activeFixtureForm = value; } }
    }
}

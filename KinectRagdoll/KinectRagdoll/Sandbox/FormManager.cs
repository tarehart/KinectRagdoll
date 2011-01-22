using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KinectTest2.Sandbox
{


    class FormManager
    {

        private static RectangleForm rectangleForm;
        private static PropertyEditorForm propertyForm;
        private static JointForm jointForm;

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




        public static PhysicsObjectForm ActiveFixtureForm { get { return activeFixtureForm; } set { activeFixtureForm = value; } }
    }
}

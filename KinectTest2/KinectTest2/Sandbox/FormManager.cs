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



    }
}

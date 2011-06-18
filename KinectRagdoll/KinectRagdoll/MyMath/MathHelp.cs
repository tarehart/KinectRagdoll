using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace KinectRagdoll.MyMath
{
    public class MathHelp
    {

        public static float getRadDiff(float p, float targetAngle)
        {


            float a = targetAngle - p;
            while (a < -Math.PI) a += 2 * (float)Math.PI;
            while (a > Math.PI) a -= 2 * (float)Math.PI;

            Debug.Assert(Math.Abs(a) <= Math.PI);
            return a;
        }
    }
}

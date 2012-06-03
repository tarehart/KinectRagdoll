using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;

namespace KinectRagdoll.Farseer
{
    class FarseerHelper
    {
        public static bool hasLineOfSight(Vector2 eye, Vector2 farthestGaze, Predicate<Fixture> belongsToTarget, World w)
        {

            bool hasLOS = false;

            w.RayCast((f, p, n, fr) =>
            {
                if (belongsToTarget(f))
                    hasLOS = true;

                return 0; // terminate the ray cast

            }, eye, farthestGaze);

            return hasLOS;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace GameEngine.Utility
{
    // may contain multiple helpful vector methods that either opentk doesnt have or i cant find
    public static class Vec3Helper
    {
        public static double DistanceBetween_Approx(Vector3 A, Vector3 B)
        {
            return Math.Pow(A.X - B.X, 2) + Math.Pow(A.Y - B.Y, 2) + Math.Pow(A.Z - B.Z, 2);
        }

        public static double DistanceBetween(Vector3 A, Vector3 B)
        {
            double dist2 = Math.Pow(A.X - B.X, 2) + Math.Pow(A.Y - B.Y, 2) + Math.Pow(A.Z - B.Z, 2);

            return Math.Sqrt(dist2);
        }

        /// <summary>
        /// returns an incremental vector in a step towards a vector
        /// </summary>
        /// <param name="current">Point at which movement begins</param>
        /// <param name="target">Destination</param>
        /// <param name="maxDistDelta">Max distance to be moved in a step</param>
        /// <returns></returns>
        public static Vector3 MoveTowards(Vector3 current, Vector3 target, float maxDistDelta)
        {
            Vector3 deltaVector = target - current;
            float magnitude = deltaVector.Length;

            if (magnitude <= maxDistDelta || magnitude == 0)
                return target; // currrent is at the target. return target for safety

            return deltaVector / magnitude * maxDistDelta + current; 
        }
    }
}

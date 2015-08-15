using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace netLogic.Utilities
{
    public static class SharpMath
    {
        /// <summary>
        /// Linearly interpolates from a source value to a destination value
        /// </summary>
        /// <param name="src">The source value (for pct = 0)</param>
        /// <param name="dst">The destination value (for pct = 1)</param>
        /// <param name="pct">The percentage of the interpolation in the range [0, 1]</param>
        /// <returns>src + (dst - src) * pct</returns>
        public static float Lerp(float src, float dst, float pct)
        {
            return (src + (dst - src) * pct);
        }

        public static float CosinusInterpolate(float src, float dst, float pct)
        {
            float cos = (1 + (float)System.Math.Cos(pct * System.Math.PI)) / 2.0f;
            return (src + (dst - src) * cos);
        }

        public static float mirrorAngle(float angle)
        {
            /*while (angle < 0)
                angle += 360;
            while (angle > 360)
                angle -= 360;*/

            return 360 - angle;
        }

        public static float mirrorAngleRadian(float angle)
        {
            return (float)(Math.PI * 2 - angle);
        }
    }
}

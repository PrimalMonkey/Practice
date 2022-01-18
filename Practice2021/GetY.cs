using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Practice2021
{
    class GetY
    {
        public static double getY(double x)
        {
            double ans = Math.Pow(x, 3) + 2.0 * Math.Pow(x, 2) - 9.0 * x - 18.0;
            return ans;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Practice2021
{
    public static class CalcP
    {
        public static void calcKardano(double a, double b, double c, double d, ref int type, ref double p1, ref double p2, ref double p3)
        {
            double eps = 1E-14;
            double p = (3 * a * c - b * b) / (3 * a * a);
            double q = (2 * b * b * b - 9 * a * b * c + 27 * a * a * d) / (27 * a * a * a);
            double det = q * q / 4 + p * p * p / 27;
            if (Math.Abs(det) < eps)
                det = 0;
            if (det > 0)
            {
                type = 1; // Один вещественный, два комплексных корня
                double u = -q / 2 + Math.Sqrt(det);
                u = Math.Exp(Math.Log(u) / 3);
                double yy = u - p / (3 * u);
                p1 = yy - b / (3 * a); // Первый корень
                p2 = -(u - p / (3 * u)) / 2 - b / (3 * a);
                p3 = Math.Sqrt(3) / 2 * (u + p / (3 * u));
            }
            else
            {
                if (det < 0)
                {
                    type = 2; // Три вещественных корня
                    double fi;
                    if (Math.Abs(q) < eps) // q = 0
                        fi = Math.PI / 2;
                    else
                    {
                        if (q < 0) // q < 0
                            fi = Math.Atan(Math.Sqrt(-det) / (-q / 2));
                        else 
                            fi = Math.Atan(Math.Sqrt(-det) / (-q / 2)) + Math.PI;
                    }
                    double r = 2 * Math.Sqrt(-p / 3);
                    p1 = r * Math.Cos(fi / 3) - b / (3 * a);
                    p2 = r * Math.Cos((fi + 2 * Math.PI) / 3) - b / (3 * a);
                    p3 = r * Math.Cos((fi + 4 * Math.PI) / 3) - b / (3 * a);
                }
                else // Дискриминант = 0
                {
                    if (Math.Abs(q) < eps)
                    {
                        type = 4; // 3 кратных корня 
                        p1 = -b / (3 * a);  
                        p2 = -b / (3 * a);
                        p3 = -b / (3 * a);
                    }
                    else
                    {
                        type = 3; // 2 кратных корня
                        double u = Math.Exp(Math.Log(Math.Abs(q) / 2) / 3);
                        if (q < 0)
                            u = -u;
                        p1 = -2 * u - b / (3 * a);
                        p2 = u - b / (3 * a);
                        p3 = u - b / (3 * a);
                    }
                }
            }
        }
    }
}

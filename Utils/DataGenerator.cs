using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1.Utils
{
    public static class DataGenerator
    {
        private static readonly Random rand = new Random();

        public static List<double> Constant(int m, int n, double value = 1.0)
        {
            var list = new List<double>();
            for (int i = m; i <= n; i++)
                list.Add(value);
            return list;
        }

        public static List<double> Linear(int m, int n, double a = 1.0, double b = 0.0)
        {
            var list = new List<double>();
            for (int i = m; i <= n; i++)
                list.Add(a * i + b);
            return list;
        }

        public static List<double> Quadratic(int m, int n, double a = 1.0, double b = 0.0, double c = 0.0)
        {
            var list = new List<double>();
            for (int i = m; i <= n; i++)
                list.Add(a * i * i + b * i + c);
            return list;
        }

        public static List<double> Normal(int m, int n, double mean = 1.5, double stdDev = 1.0)
        {
            var list = new List<double>();
            for (int i = m; i <= n; i++)
            {
                double u1 = 1.0 - rand.NextDouble();
                double u2 = 1.0 - rand.NextDouble();
                double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                                       Math.Sin(2.0 * Math.PI * u2);
                double randNormal = mean + stdDev * randStdNormal;
                list.Add(randNormal);
            }
            return list;
        }

        public static List<double> Uniform(int m, int n, double min = 1.0, double max = 5.0)
        {
            var list = new List<double>();
            for (int i = m; i <= n; i++)
                list.Add(min + rand.NextDouble() * (max - min));
            return list;
        }
    }
}
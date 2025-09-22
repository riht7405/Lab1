using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lab1.Interfaces;
using Lab1.Plot;
using Lab1.Utils;
using OxyPlot;

namespace Lab1.Tasks
{
    namespace Lab1.Tasks
    {
        public class PowerAlgorithmTask : ITask
        {
            private readonly int m, n;
            private readonly double baseValue;
            private readonly CounterMode mode;

            public PowerAlgorithmTask(int m, int n, double baseValue = 2.0, CounterMode mode = CounterMode.TimeAndSteps)
            {
                this.m = m;
                this.n = n;
                this.baseValue = baseValue;
                this.mode = mode;
            }

            public string Name => "Power Algorithm (Fast)";
            public PlotModel? LastPlotModel { get; private set; }

            public void Run()
            {
                var times = new List<double>();
                var theoryRaw = new List<double>();

                for (int exp = m; exp <= n; exp++)
                {
                    var counter = new PerformanceCounter(mode);

                    counter.Start();
                    double result = FastPower(baseValue, exp, counter);
                    counter.Stop();

                    times.Add(counter.Steps); // считаем шаги
                    theoryRaw.Add(Math.Log(exp, 2)); // O(log n)
                }
                //нормирование по шагам
                double coef = times[^1] / theoryRaw[^1];
                var theory = theoryRaw.Select(v => v * coef).ToList();

                LastPlotModel = Plotter.CreateLinePlot(Name, times, m, theory);
                LastPlotModel.InvalidatePlot(true);
            }




            private double FastPower(double a, int p, PerformanceCounter counter)
            {
                counter.IncrementStep();
                if (p == 0) return 1;
                if (p % 2 == 0)
                {
                    double half = FastPower(a, p / 2, counter);
                    return half * half;
                }
                else
                {
                    return a * FastPower(a, p - 1, counter);
                }
            }
        }

    }
}
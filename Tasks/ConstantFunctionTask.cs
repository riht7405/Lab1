using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lab1.Interfaces;
using Lab1.Utils;
using Lab1.Plot;
using OxyPlot;

namespace Lab1.Tasks
{
    public class ConstantFunctionTask : ITask
    {
        private readonly int m, n;
        private readonly double value;
        private readonly CounterMode mode;

        public ConstantFunctionTask(int m, int n, double value = 1.0, CounterMode mode = CounterMode.TimeAndSteps)
        {
            this.m = m;
            this.n = n;
            this.value = value;
            this.mode = mode;
        }

        public string Name => "Constant Function (f(v) = 1)";
        public PlotModel? LastPlotModel { get; private set; }
        public void Run()
        {
            var times = new List<double>();
            var theoryRaw = new List<double>();

            for (int size = m; size <= n; size++)
            {
                var data = DataGenerator.Constant(1, size, value);
                var counter = new PerformanceCounter(mode);

                counter.Start();
                counter.AddSteps(data.Count);
                counter.Stop();

                times.Add(counter.ElapsedMs / 1000.0);
                theoryRaw.Add(1); // O(1)
            }
            //нормирование
            double coef = times[^1] / theoryRaw[^1];
            var theory = theoryRaw.Select(v => v * coef).ToList();

            LastPlotModel = Plotter.CreateLinePlot(Name, times, m, theory);
            LastPlotModel.InvalidatePlot(true);
        }

    }
}
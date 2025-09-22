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
            int repeats = 5;

            for (int size = m; size <= n; size++)
            {
                double totalTime = 0;

                for (int r = 0; r < repeats; r++)
                {
                    var data = DataGenerator.Constant(1, size, value);
                    var counter = new PerformanceCounter(mode);

                    counter.Start();
                    counter.AddSteps(data.Count);
                    counter.Stop();

                    totalTime += counter.ElapsedMs / 1000.0;
                }

                times.Add(totalTime / repeats);
            }

            LastPlotModel = Plotter.CreateLinePlot(Name, times, m);
            LastPlotModel.InvalidatePlot(true);
        }
    }
}
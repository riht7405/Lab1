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
    public class TimSortTask : ITask
    {
        private readonly int m, n;
        private readonly CounterMode mode;

        public TimSortTask(int m, int n, CounterMode mode = CounterMode.TimeAndSteps)
        {
            this.m = m;
            this.n = n;
            this.mode = mode;
        }

        public string Name => "TimSort (Array.Sort)";
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
                    var data = DataGenerator.Uniform(1, size, 1, 100);
                    var arr = data.ToArray();
                    var counter = new PerformanceCounter(mode);

                    counter.Start();
                    Array.Sort(arr);
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
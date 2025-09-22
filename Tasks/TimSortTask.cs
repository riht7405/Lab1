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
            var theoryRaw = new List<double>();

            for (int size = m; size <= n; size++)
            {
                var arr = DataGenerator.Uniform(1, size, 1, 100).ToArray();
                var counter = new PerformanceCounter(mode);

                counter.Start();
                Array.Sort(arr);
                counter.Stop();

                times.Add(counter.ElapsedMs / 1000.0);
                theoryRaw.Add(size * Math.Log(size)); // O(n log n)
            }
            //нормирование 
            double coef = times[^1] / theoryRaw[^1];
            var theory = theoryRaw.Select(v => v * coef).ToList();

            LastPlotModel = Plotter.CreateLinePlot(Name, times, m, theory);
            LastPlotModel.InvalidatePlot(true);
        }


    }
}
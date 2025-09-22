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
    public class SumElementsTask : ITask
    {
        private readonly int m, n;
        private readonly CounterMode mode;

        public SumElementsTask(int m, int n, CounterMode mode = CounterMode.TimeAndSteps)
        {
            this.m = m;
            this.n = n;
            this.mode = mode;
        }

        public string Name => "Sum of Elements";
        public PlotModel? LastPlotModel { get; private set; }
        public void Run()
        {
            var times = new List<double>();
            var theoryRaw = new List<double>();

            for (int size = m; size <= n; size++)
            {
                var data = DataGenerator.Uniform(1, size, 1, 10);
                var counter = new PerformanceCounter(mode);

                counter.Start();
                double sum = 0;
                foreach (var v in data)
                {
                    sum += v;
                    counter.IncrementStep();
                }
                counter.Stop();

                times.Add(counter.ElapsedMs / 1000.0);
                theoryRaw.Add(size); // O(n)
            }
            //нормирование
            double coef = times[^1] / theoryRaw[^1];
            var theory = theoryRaw.Select(v => v * coef).ToList();

            LastPlotModel = Plotter.CreateLinePlot(Name, times, m, theory);
            LastPlotModel.InvalidatePlot(true);
        }

    }
}
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
            var counter = new PerformanceCounter(mode);
            counter.Start();

            var data = DataGenerator.Uniform(m, n, 1, 10);
            double sum = 0;
            foreach (var v in data)
            {
                sum += v;
                counter.IncrementStep();
            }

            counter.Stop();
            Console.WriteLine($"{Name}: сумма={sum:F3}, время={counter.ElapsedMs:F3} мс, шаги={counter.Steps}");

            // Для наглядности строим график исходных данных
            LastPlotModel = Plotter.CreateLinePlot(Name, data, m);
        }
    }

}
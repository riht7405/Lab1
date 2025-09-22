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
    public class ProductElementsTask : ITask
    {
        private readonly int m, n;
        private readonly CounterMode mode;

        public ProductElementsTask(int m, int n, CounterMode mode = CounterMode.TimeAndSteps)
        {
            this.m = m;
            this.n = n;
            this.mode = mode;
        }

        public string Name => "Product of Elements";
        public PlotModel? LastPlotModel { get; private set; }
        public void Run()
        {
            var counter = new PerformanceCounter(mode);
            counter.Start();

            var data = DataGenerator.Uniform(m, n, 1, 5);
            double product = 1;
            foreach (var v in data)
            {
                product *= v;
                counter.IncrementStep();
            }

            counter.Stop();
            Console.WriteLine($"{Name}: произведение={product:E3}, время={counter.ElapsedMs:F3} мс, шаги={counter.Steps}");

            // Для наглядности строим график исходных данных
            LastPlotModel = Plotter.CreateLinePlot(Name, data, m);
        }
    }
}
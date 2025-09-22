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
            var counter = new PerformanceCounter(mode);
            counter.Start();

            var data = DataGenerator.Constant(m, n, value);
            counter.AddSteps(data.Count);

            counter.Stop();
            Console.WriteLine($"{Name}: {data.Count} элементов, время={counter.ElapsedMs:F3} мс, шаги={counter.Steps}");

            // Сохраняем модель графика для Runner
            LastPlotModel = Plotter.CreateLinePlot(Name, data, m);
        }
    }

}
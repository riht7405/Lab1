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
            var counter = new PerformanceCounter(mode);
            var data = DataGenerator.Uniform(m, n, 1, 100);

            counter.Start();
            var arr = data.ToArray();
            Array.Sort(arr);
            counter.AddSteps(arr.Length); // условно считаем шаги по числу элементов
            counter.Stop();

            Console.WriteLine($"{Name}: {arr.Length} элементов, время={counter.ElapsedMs:F3} мс, шаги={counter.Steps}");

            // Для визуализации — график исходных данных
            LastPlotModel = Plotter.CreateLinePlot(Name, data, m);
        }
    }
}
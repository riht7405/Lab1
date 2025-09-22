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
    public class BubbleSortTask : ITask
    {
        private readonly int m, n;
        private readonly CounterMode mode;

        public BubbleSortTask(int m, int n, CounterMode mode = CounterMode.TimeAndSteps)
        {
            this.m = m;
            this.n = n;
            this.mode = mode;
        }

        public string Name => "Bubble Sort";

        public PlotModel? LastPlotModel { get; private set; }

        public void Run()
        {
            var counter = new PerformanceCounter(mode);
            var data = DataGenerator.Uniform(m, n, 1, 100);

            counter.Start();
            var arr = data.ToArray();
            for (int i = 0; i < arr.Length - 1; i++)
            {
                for (int j = 0; j < arr.Length - i - 1; j++)
                {
                    counter.IncrementStep();
                    if (arr[j] > arr[j + 1])
                        (arr[j], arr[j + 1]) = (arr[j + 1], arr[j]);
                }
            }
            counter.Stop();

            Console.WriteLine($"{Name}: {arr.Length} элементов, время={counter.ElapsedMs:F3} мс, шаги={counter.Steps}");
            Console.WriteLine($"Data.Count = {data.Count}");

            // Строим график
            LastPlotModel = Plotter.CreateLinePlot(Name, data, m);

            // Форсируем перерисовку
            LastPlotModel.InvalidatePlot(true);
        }

    }

}
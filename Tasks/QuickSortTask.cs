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
    public class QuickSortTask : ITask
    {
        private readonly int m, n;
        private readonly CounterMode mode;

        public QuickSortTask(int m, int n, CounterMode mode = CounterMode.TimeAndSteps)
        {
            this.m = m;
            this.n = n;
            this.mode = mode;
        }

        public string Name => "Quick Sort";
        public PlotModel? LastPlotModel { get; private set; }
        public void Run()
        {
            var counter = new PerformanceCounter(mode);
            var data = DataGenerator.Uniform(m, n, 1, 100);

            counter.Start();
            var arr = data.ToArray();
            QuickSort(arr, 0, arr.Length - 1, counter);
            counter.Stop();
            Console.WriteLine($"{Name}: {arr.Length} элементов, время={counter.ElapsedMs:F3} мс, шаги={counter.Steps}");

            // Для визуализации — график исходных данных
            LastPlotModel = Plotter.CreateLinePlot(Name, data, m);
        }

        private void QuickSort(double[] arr, int left, int right, PerformanceCounter counter)
        {
            if (left >= right) return;
            double pivot = arr[(left + right) / 2];
            int i = left, j = right;
            while (i <= j)
            {
                while (arr[i] < pivot) { i++; counter.IncrementStep(); }
                while (arr[j] > pivot) { j--; counter.IncrementStep(); }
                if (i <= j)
                {
                    (arr[i], arr[j]) = (arr[j], arr[i]);
                    i++; j--;
                }
            }
            QuickSort(arr, left, j, counter);
            QuickSort(arr, i, right, counter);
        }
    }

}
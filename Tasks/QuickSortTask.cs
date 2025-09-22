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
            var times = new List<double>();
            var theoryRaw = new List<double>();
            int repeats = 5;

            for (int size = m; size <= n; size++)
            {
                double total = 0;
                for (int r = 0; r < repeats; r++)
                {
                    var arr = DataGenerator.Uniform(1, size, 1, 100).ToArray();
                    var counter = new PerformanceCounter(mode);

                    counter.Start();
                    QuickSort(arr, 0, arr.Length - 1, counter);
                    counter.Stop();

                    total += mode == CounterMode.StepsOnly ? counter.Steps : counter.ElapsedMs / 1000.0;
                }

                times.Add(total / repeats);
                theoryRaw.Add(size * Math.Log(size)); // O(n log n)
            }

            // нормирование по последним значениям
            double coef = times[^1] / theoryRaw[^1];
            var theory = theoryRaw.Select(v => v * coef).ToList();

            LastPlotModel = Plotter.CreateLinePlot(Name, times, m, theory);
            LastPlotModel.InvalidatePlot(true);
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
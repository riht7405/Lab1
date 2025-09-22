using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lab1.Interfaces;
using Lab1.Plot;
using Lab1.Utils;
using OxyPlot;
using System.ComponentModel;

namespace Lab1.Tasks
{
    public class BubbleSortTask : ITask, INotifyPropertyChanged
    {
        private readonly int m, n;
        private readonly CounterMode mode;
        private PlotModel? _lastPlotModel;

        public BubbleSortTask(int m, int n, CounterMode mode = CounterMode.TimeAndSteps)
        {
            this.m = m;
            this.n = n;
            this.mode = mode;
        }

        public string Name => "Bubble Sort";

        public PlotModel? LastPlotModel
        {
            get => _lastPlotModel;
            private set
            {
                _lastPlotModel = value;
                OnPropertyChanged(nameof(LastPlotModel));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

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
                    for (int i = 0; i < arr.Length - 1; i++)
                        for (int j = 0; j < arr.Length - i - 1; j++)
                        {
                            counter.IncrementStep();
                            if (arr[j] > arr[j + 1])
                                (arr[j], arr[j + 1]) = (arr[j + 1], arr[j]);
                        }
                    counter.Stop();

                    total += mode == CounterMode.StepsOnly ? counter.Steps : counter.ElapsedMs / 1000.0;
                }

                times.Add(total / repeats);
                theoryRaw.Add(Math.Pow(size, 2)); // O(n²)
            }

            // нормирование по последним значениям
            double coef = times[^1] / theoryRaw[^1];
            var theory = theoryRaw.Select(v => v * coef).ToList();

            LastPlotModel = Plotter.CreateLinePlot(Name, times, m, theory);
            LastPlotModel.InvalidatePlot(true);
        }

    }
}
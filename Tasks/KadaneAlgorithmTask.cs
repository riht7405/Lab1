using System;
using System.Collections.Generic;
using System.Linq;
using Lab1.Interfaces;
using Lab1.Plot;
using Lab1.Utils;
using OxyPlot;

namespace Lab1.Tasks
{
    public class KadaneAlgorithmTask : ITask
    {
        private readonly int m, n;
        private readonly CounterMode mode;

        public KadaneAlgorithmTask(int m, int n, CounterMode mode = CounterMode.TimeAndSteps)
        {
            this.m = m;
            this.n = n;
            this.mode = mode;
        }

        public string Name => "Kadane's Algorithm (Max Subarray Sum)";
        public PlotModel? LastPlotModel { get; private set; }

        public void Run()
        {
            var times = new List<double>();
            var theoryRaw = new List<double>();
            int repeats = 3; // Несколько повторов для усреднения

            for (int size = m; size <= n; size++)
            {
                double totalTime = 0;

                for (int r = 0; r < repeats; r++)
                {
                    // Генерируем массив с отрицательными числами для интересных случаев
                    var arr = DataGenerator.Uniform(1, size, -100, 100).ToArray();
                    var counter = new PerformanceCounter(mode);

                    counter.Start();
                    double maxSum = KadaneAlgorithm(arr, counter);
                    counter.Stop();

                    totalTime += mode == CounterMode.StepsOnly ? counter.Steps : counter.ElapsedMs / 1000.0;
                }

                times.Add(totalTime / repeats);
                theoryRaw.Add(size); // O(n)
            }

            // Нормирование теоретической кривой
            double coef = times[^1] / theoryRaw[^1];
            var theory = theoryRaw.Select(v => v * coef).ToList();

            LastPlotModel = Plotter.CreateLinePlot(Name, times, m, theory);
            LastPlotModel.InvalidatePlot(true);
        }

        /// <summary>
        /// Алгоритм Кадане для нахождения максимальной суммы подмассива
        /// </summary>
        private double KadaneAlgorithm(double[] arr, PerformanceCounter counter)
        {
            if (arr == null || arr.Length == 0)
                return 0;

            double maxSoFar = arr[0];
            double maxEndingHere = arr[0];

            counter.IncrementStep(); // Учёт инициализации

            for (int i = 1; i < arr.Length; i++)
            {
                counter.IncrementStep(); // Учёт каждой итерации
                
                // Выбираем: продолжить текущий подмассив или начать новый
                maxEndingHere = Math.Max(arr[i], maxEndingHere + arr[i]);
                // Обновляем глобальный максимум
                maxSoFar = Math.Max(maxSoFar, maxEndingHere);

                counter.IncrementStep(); // Учёт двух операций сравнения
            }

            return maxSoFar;
        }

        /// <summary>
        /// Расширенная версия, которая также возвращает индексы подмассива
        /// </summary>
        private (double maxSum, int start, int end) KadaneAlgorithmWithIndices(double[] arr, PerformanceCounter counter)
        {
            if (arr == null || arr.Length == 0)
                return (0, 0, 0);

            double maxSoFar = arr[0];
            double maxEndingHere = arr[0];
            int start = 0, end = 0;
            int tempStart = 0;

            counter.IncrementStep(); // Учёт инициализации

            for (int i = 1; i < arr.Length; i++)
            {
                counter.IncrementStep(); // Учёт каждой итерации
                
                if (arr[i] > maxEndingHere + arr[i])
                {
                    maxEndingHere = arr[i];
                    tempStart = i;
                }
                else
                {
                    maxEndingHere += arr[i];
                }

                if (maxEndingHere > maxSoFar)
                {
                    maxSoFar = maxEndingHere;
                    start = tempStart;
                    end = i;
                }

                counter.IncrementStep(); // Учёт операций сравнения
            }

            return (maxSoFar, start, end);
        }
    }
}
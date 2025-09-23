using System;
using System.Collections.Generic;
using System.Linq;
using Lab1.Interfaces;
using Lab1.Plot;
using Lab1.Utils;
using OxyPlot;

namespace Lab1.Tasks
{
    public class BoyerMooreMajorityTask : ITask
    {
        private readonly int m, n;
        private readonly CounterMode mode;

        public BoyerMooreMajorityTask(int m, int n, CounterMode mode = CounterMode.TimeAndSteps)
        {
            this.m = m;
            this.n = n;
            this.mode = mode;
        }

        public string Name => "Boyer-Moore Majority Algorithm";
        public PlotModel? LastPlotModel { get; private set; }

        public void Run()
        {
            var times = new List<double>();
            var theoryRaw = new List<double>();
            int repeats = 5;

            for (int size = m; size <= n; size++)
            {
                double totalTime = 0;

                for (int r = 0; r < repeats; r++)
                {
                    // Генерируем массив с majority element
                    var arr = GenerateArrayWithMajority(size);
                    var counter = new PerformanceCounter(mode);

                    counter.Start();
                    double majority = FindMajorityElement(arr, counter);
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
        /// Генерирует массив с majority element (элемент, встречающийся > n/2 раз)
        /// </summary>
        private double[] GenerateArrayWithMajority(int size)
        {
            var random = new Random();
            double majorityValue = random.NextDouble() * 100; // Случайное значение для majority
            
            double[] arr = new double[size];
            
            // Количество majority элементов: больше половины
            int majorityCount = size / 2 + 1;
            
            // Заполняем majority элементами
            for (int i = 0; i < majorityCount; i++)
            {
                arr[i] = majorityValue;
            }
            
            // Заполняем оставшиеся случайными значениями (не равными majority)
            for (int i = majorityCount; i < size; i++)
            {
                double randomValue;
                do
                {
                    randomValue = random.NextDouble() * 100;
                } while (Math.Abs(randomValue - majorityValue) < 0.001); // Гарантируем отличие
                
                arr[i] = randomValue;
            }
            
            // Перемешиваем массив для рандомизации
            ShuffleArray(arr, random);
            
            return arr;
        }

        /// <summary>
        /// Алгоритм Бойера-Мура для поиска majority element
        /// </summary>
        private double FindMajorityElement(double[] arr, PerformanceCounter counter)
        {
            if (arr == null || arr.Length == 0)
                throw new ArgumentException("Массив не может быть пустым");

            double candidate = arr[0];
            int count = 1;

            counter.IncrementStep(); // Учёт инициализации

            // Первый проход: поиск кандидата
            for (int i = 1; i < arr.Length; i++)
            {
                counter.IncrementStep(); // Учёт каждой итерации

                if (count == 0)
                {
                    candidate = arr[i];
                    count = 1;
                    counter.IncrementStep(); // Учёт обновления кандидата
                }
                else if (Math.Abs(arr[i] - candidate) < 0.001) // Сравнение double с погрешностью
                {
                    count++;
                    counter.IncrementStep(); // Учёт увеличения счётчика
                }
                else
                {
                    count--;
                    counter.IncrementStep(); // Учёт уменьшения счётчика
                }
            }

            // Второй проход: проверка, что кандидат действительно majority
            count = 0;
            counter.IncrementStep(); // Сброс счётчика

            for (int i = 0; i < arr.Length; i++)
            {
                counter.IncrementStep(); // Учёт каждой итерации проверки
                
                if (Math.Abs(arr[i] - candidate) < 0.001)
                {
                    count++;
                    counter.IncrementStep(); // Учёт увеличения счётчика
                }
            }

            counter.IncrementStep(); // Финальная проверка

            if (count > arr.Length / 2)
            {
                return candidate;
            }
            else
            {
                throw new InvalidOperationException("Majority element не найден");
            }
        }

        /// <summary>
        /// Алгоритм Бойера-Мура с возвратом индексов вхождения
        /// </summary>
        private (double majority, List<int> indices) FindMajorityElementWithIndices(double[] arr, PerformanceCounter counter)
        {
            if (arr == null || arr.Length == 0)
                throw new ArgumentException("Массив не может быть пустым");

            // Первый проход: поиск кандидата
            double candidate = arr[0];
            int count = 1;

            counter.IncrementStep();

            for (int i = 1; i < arr.Length; i++)
            {
                counter.IncrementStep();

                if (count == 0)
                {
                    candidate = arr[i];
                    count = 1;
                    counter.IncrementStep();
                }
                else if (Math.Abs(arr[i] - candidate) < 0.001)
                {
                    count++;
                    counter.IncrementStep();
                }
                else
                {
                    count--;
                    counter.IncrementStep();
                }
            }

            // Второй проход: сбор индексов и проверка
            var indices = new List<int>();
            count = 0;
            counter.IncrementStep();

            for (int i = 0; i < arr.Length; i++)
            {
                counter.IncrementStep();
                
                if (Math.Abs(arr[i] - candidate) < 0.001)
                {
                    indices.Add(i);
                    count++;
                    counter.IncrementStep();
                }
            }

            counter.IncrementStep();

            if (count > arr.Length / 2)
            {
                return (candidate, indices);
            }
            else
            {
                throw new InvalidOperationException("Majority element не найден");
            }
        }

        /// <summary>
        /// Перемешивание массива (алгоритм Фишера-Йейтса)
        /// </summary>
        private void ShuffleArray(double[] arr, Random random)
        {
            for (int i = arr.Length - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (arr[i], arr[j]) = (arr[j], arr[i]);
            }
        }
    }
}
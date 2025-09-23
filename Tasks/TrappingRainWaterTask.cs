using System;
using System.Collections.Generic;
using System.Linq;
using Lab1.Interfaces;
using Lab1.Plot;
using Lab1.Utils;
using OxyPlot;

namespace Lab1.Tasks
{
    public class TrappingRainWaterTask : ITask
    {
        private readonly int m, n;
        private readonly CounterMode mode;

        public TrappingRainWaterTask(int m, int n, CounterMode mode = CounterMode.TimeAndSteps)
        {
            this.m = m;
            this.n = n;
            this.mode = mode;
        }

        public string Name => "Trapping Rain Water (Two Pointers)";
        public PlotModel? LastPlotModel { get; private set; }

        public void Run()
        {
            var times = new List<double>();
            var theoryRaw = new List<double>();
            int repeats = 3;

            for (int size = m; size <= n; size++)
            {
                double totalTime = 0;

                for (int r = 0; r < repeats; r++)
                {
                    // Генерируем массив высот
                    var height = GenerateHeightArray(size);
                    var counter = new PerformanceCounter(mode);

                    counter.Start();
                    int waterVolume = TrapRainWater(height, counter);
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
        /// Генерирует массив высот для задачи "сбор дождевой воды"
        /// </summary>
        private int[] GenerateHeightArray(int size)
        {
            var random = new Random();
            int[] height = new int[size];
            
            // Создаем "долины" и "пики" для интересных случаев
            for (int i = 0; i < size; i++)
            {
                // Создаем случайный ландшафт с высотами от 0 до 20
                height[i] = random.Next(0, 21);
            }
            
            // Гарантируем, что есть хотя бы одна "ловушка" для воды
            if (size >= 3)
            {
                int trapIndex = random.Next(1, size - 1);
                height[trapIndex] = 0; // Создаем впадину
                height[trapIndex - 1] = random.Next(5, 15); // Левый барьер
                height[trapIndex + 1] = random.Next(5, 15); // Правый барьер
            }
            
            return height;
        }

        /// <summary>
        /// Алгоритм двух указателей для подсчета захваченной дождевой воды
        /// </summary>
        private int TrapRainWater(int[] height, PerformanceCounter counter)
        {
            if (height == null || height.Length < 3)
                return 0;

            int left = 0;
            int right = height.Length - 1;
            int leftMax = 0;
            int rightMax = 0;
            int water = 0;

            counter.IncrementStep(); // Учёт инициализации

            while (left <= right)
            {
                counter.IncrementStep(); // Учёт каждой итерации цикла

                if (height[left] <= height[right])
                {
                    // Обрабатываем левую сторону
                    if (height[left] >= leftMax)
                    {
                        leftMax = height[left];
                    }
                    else
                    {
                        water += leftMax - height[left];
                    }
                    left++;
                    counter.IncrementStep(); // Учёт сравнения и добавления воды
                }
                else
                {
                    // Обрабатываем правую сторону
                    if (height[right] >= rightMax)
                    {
                        rightMax = height[right];
                    }
                    else
                    {
                        water += rightMax - height[right];
                    }
                    right--;
                    counter.IncrementStep(); // Учёт сравнения и добавления воды
                }
            }

            return water;
        }

        /// <summary>
        /// Альтернативная реализация с предварительным вычислением максимумов
        /// </summary>
        private int TrapRainWaterDynamicProgramming(int[] height, PerformanceCounter counter)
        {
            if (height == null || height.Length < 3)
                return 0;

            int n = height.Length;
            int[] leftMax = new int[n];
            int[] rightMax = new int[n];
            int water = 0;

            counter.IncrementStep(); // Учёт инициализации

            // Заполняем leftMax: максимальная высота слева от каждой позиции
            leftMax[0] = height[0];
            for (int i = 1; i < n; i++)
            {
                leftMax[i] = Math.Max(leftMax[i - 1], height[i]);
                counter.IncrementStep(); // Учёт сравнения и присваивания
            }

            // Заполняем rightMax: максимальная высота справа от каждой позиции
            rightMax[n - 1] = height[n - 1];
            for (int i = n - 2; i >= 0; i--)
            {
                rightMax[i] = Math.Max(rightMax[i + 1], height[i]);
                counter.IncrementStep(); // Учёт сравнения и присваивания
            }

            // Вычисляем объем воды для каждой позиции
            for (int i = 0; i < n; i++)
            {
                water += Math.Min(leftMax[i], rightMax[i]) - height[i];
                counter.IncrementStep(); // Учёт минимума и вычитания
            }

            return water;
        }

        /// <summary>
        /// Версия, которая возвращает детальную информацию о водяных "ловушках"
        /// </summary>
        private (int totalWater, List<(int index, int water)> traps) TrapRainWaterDetailed(int[] height, PerformanceCounter counter)
        {
            if (height == null || height.Length < 3)
                return (0, new List<(int, int)>());

            int left = 0;
            int right = height.Length - 1;
            int leftMax = 0;
            int rightMax = 0;
            int totalWater = 0;
            var traps = new List<(int index, int water)>();

            counter.IncrementStep();

            while (left <= right)
            {
                counter.IncrementStep();

                if (height[left] <= height[right])
                {
                    if (height[left] >= leftMax)
                    {
                        leftMax = height[left];
                    }
                    else
                    {
                        int waterAtPosition = leftMax - height[left];
                        totalWater += waterAtPosition;
                        traps.Add((left, waterAtPosition));
                    }
                    left++;
                    counter.IncrementStep();
                }
                else
                {
                    if (height[right] >= rightMax)
                    {
                        rightMax = height[right];
                    }
                    else
                    {
                        int waterAtPosition = rightMax - height[right];
                        totalWater += waterAtPosition;
                        traps.Add((right, waterAtPosition));
                    }
                    right--;
                    counter.IncrementStep();
                }
            }

            return (totalWater, traps);
        }

        /// <summary>
        /// Визуализация массива высот и водяных ловушек (для отладки)
        /// </summary>
        private void VisualizeHeightArray(int[] height, List<(int index, int water)> traps)
        {
            int maxHeight = height.Max();
            
            for (int level = maxHeight; level >= 0; level--)
            {
                for (int i = 0; i < height.Length; i++)
                {
                    if (height[i] > level)
                    {
                        Console.Write("█"); // Столбец
                    }
                    else if (traps.Any(t => t.index == i && t.water > 0 && level < height[i] + traps.First(t => t.index == i).water))
                    {
                        Console.Write("~"); // Вода
                    }
                    else
                    {
                        Console.Write(" "); // Пустота
                    }
                }
                Console.WriteLine();
            }
        }
    }
}
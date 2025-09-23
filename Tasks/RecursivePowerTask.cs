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
    public class RecursivePowerTask : ITask
    {
        private readonly int m, n;
        private readonly double baseValue;
        private readonly CounterMode mode;

        public RecursivePowerTask(int m, int n, double baseValue = 2.0, CounterMode mode = CounterMode.TimeAndSteps)
        {
            this.m = m;
            this.n = n;
            this.baseValue = baseValue;
            this.mode = mode;
        }

        public string Name => "Recursive Power Algorithm (xⁿ = x * xⁿ⁻¹)";
        public PlotModel? LastPlotModel { get; private set; }

        public void Run()
        {
            var times = new List<double>();
            var theoryRaw = new List<double>();

            for (int exp = m; exp <= n; exp++)
            {
                var counter = new PerformanceCounter(mode);

                counter.Start();
                double result = RecursivePower(baseValue, exp, counter);
                counter.Stop();

                times.Add(mode == CounterMode.StepsOnly ? counter.Steps : counter.ElapsedMs / 1000.0);
                theoryRaw.Add(exp); // O(n) - линейная сложность (по глубине рекурсии)
            }

            double coef = times[^1] / theoryRaw[^1];
            var theory = theoryRaw.Select(v => v * coef).ToList();

            LastPlotModel = Plotter.CreateLinePlot(Name, times, m, theory);
            LastPlotModel.InvalidatePlot(true);
        }

        /// <summary>
        /// Рекурсивное определение: xⁿ = x * xⁿ⁻¹
        /// Сложность: O(n) по глубине рекурсии
        /// </summary>
        private double RecursivePower(double x, int n, PerformanceCounter counter)
        {
            counter.IncrementStep(); // Учет вызова функции

            if (n < 0) throw new ArgumentException("Показатель степени должен быть неотрицательным");
            if (n == 0) return 1.0;
            if (n == 1) return x;

            return x * RecursivePower(x, n - 1, counter);
        }

        /// <summary>
        /// Оптимизированная рекурсивная версия с хвостовой рекурсией
        /// </summary>
        private double RecursivePowerTailOptimized(double x, int n, double accumulator, PerformanceCounter counter)
        {
            counter.IncrementStep();

            if (n < 0) throw new ArgumentException("Показатель степени должен быть неотрицательным");
            if (n == 0) return accumulator;
            if (n == 1) return x * accumulator;

            return RecursivePowerTailOptimized(x, n - 1, x * accumulator, counter);
        }

        /// <summary>
        /// Обертка для хвостовой рекурсии
        /// </summary>
        private double RecursivePowerTail(double x, int n, PerformanceCounter counter)
        {
            return RecursivePowerTailOptimized(x, n, 1.0, counter);
        }
    }
}
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
    public class FastPowerTask : ITask
    {
        private readonly int m, n;
        private readonly double baseValue;
        private readonly CounterMode mode;

        public FastPowerTask(int m, int n, double baseValue = 2.0, CounterMode mode = CounterMode.TimeAndSteps)
        {
            this.m = m;
            this.n = n;
            this.baseValue = baseValue;
            this.mode = mode;
        }

        public string Name => "Fast Power Algorithm (Recursive Divide & Conquer)";
        public PlotModel? LastPlotModel { get; private set; }

        public void Run()
        {
            var times = new List<double>();
            var theoryRaw = new List<double>();

            for (int exp = m; exp <= n; exp++)
            {
                var counter = new PerformanceCounter(mode);

                counter.Start();
                double result = FastPowerDivideConquer(baseValue, exp, counter);
                counter.Stop();

                times.Add(mode == CounterMode.StepsOnly ? counter.Steps : counter.ElapsedMs / 1000.0);
                theoryRaw.Add(Math.Log(exp + 1, 2)); // O(log n)
            }

            double coef = times[^1] / theoryRaw[^1];
            var theory = theoryRaw.Select(v => v * coef).ToList();

            LastPlotModel = Plotter.CreateLinePlot(Name, times, m, theory);
            LastPlotModel.InvalidatePlot(true);
        }

        /// <summary>
        /// Быстрый алгоритм возведения в степень (разделяй и властвуй)
        /// Сложность: O(log n)
        /// </summary>
        private double FastPowerDivideConquer(double x, int n, PerformanceCounter counter)
        {
            counter.IncrementStep(); // Учет вызова функции

            if (n < 0) throw new ArgumentException("Показатель степени должен быть неотрицательным");
            if (n == 0) return 1.0;
            if (n == 1) return x;
            if (x == 0.0) return 0.0;
            if (x == 1.0) return 1.0;

            if (n % 2 == 0)
            {
                // Четная степень: xⁿ = (x²)ⁿ/²
                double halfPower = FastPowerDivideConquer(x * x, n / 2, counter);
                counter.IncrementStep(); // Учет умножения при возврате
                return halfPower;
            }
            else
            {
                // Нечетная степень: xⁿ = x * xⁿ⁻¹
                double almostPower = FastPowerDivideConquer(x, n - 1, counter);
                counter.IncrementStep(); // Учет умножения
                return x * almostPower;
            }
        }

        /// <summary>
        /// Улучшенная версия с мемоизацией (для демонстрации)
        /// </summary>
        private double FastPowerMemoized(double x, int n, Dictionary<int, double> memo, PerformanceCounter counter)
        {
            counter.IncrementStep();

            if (n < 0) throw new ArgumentException("Показатель степени должен быть неотрицательным");
            if (n == 0) return 1.0;
            if (n == 1) return x;
            if (x == 0.0) return 0.0;
            if (x == 1.0) return 1.0;

            // Проверка кэша
            if (memo.ContainsKey(n))
            {
                return memo[n];
            }

            double result;
            if (n % 2 == 0)
            {
                result = FastPowerMemoized(x * x, n / 2, memo, counter);
            }
            else
            {
                result = x * FastPowerMemoized(x, n - 1, memo, counter);
            }

            // Сохранение в кэш
            memo[n] = result;
            return result;
        }

        /// <summary>
        /// Обертка для версии с мемоизацией
        /// </summary>
        private double FastPowerMemoizedWrapper(double x, int n, PerformanceCounter counter)
        {
            var memo = new Dictionary<int, double>();
            return FastPowerMemoized(x, n, memo, counter);
        }
    }
}
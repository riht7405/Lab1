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
    public class NaivePowerTask : ITask
    {
        private readonly int m, n;
        private readonly double baseValue;
        private readonly CounterMode mode;

        public NaivePowerTask(int m, int n, double baseValue = 2.0, CounterMode mode = CounterMode.TimeAndSteps)
        {
            this.m = m;
            this.n = n;
            this.baseValue = baseValue;
            this.mode = mode;
        }

        public string Name => "Naive Power Algorithm (xⁿ = x*x*...*x)";
        public PlotModel? LastPlotModel { get; private set; }

        public void Run()
        {
            var times = new List<double>();
            var theoryRaw = new List<double>();

            for (int exp = m; exp <= n; exp++)
            {
                var counter = new PerformanceCounter(mode);

                counter.Start();
                double result = NaivePower(baseValue, exp, counter);
                counter.Stop();

                times.Add(mode == CounterMode.StepsOnly ? counter.Steps : counter.ElapsedMs / 1000.0);
                theoryRaw.Add(exp); // O(n) - линейная сложность
            }

            // Нормирование теоретической кривой
            double coef = times[^1] / theoryRaw[^1];
            var theory = theoryRaw.Select(v => v * coef).ToList();

            LastPlotModel = Plotter.CreateLinePlot(Name, times, m, theory);
            LastPlotModel.InvalidatePlot(true);
        }

        /// <summary>
        /// Очевидный алгоритм возведения в степень: xⁿ = x * x * ... * x (n раз)
        /// Сложность: O(n)
        /// </summary>
        private double NaivePower(double x, int n, PerformanceCounter counter)
        {
            if (n < 0) throw new ArgumentException("Показатель степени должен быть неотрицательным");
            if (n == 0) return 1.0;

            double result = 1.0;
            for (int i = 0; i < n; i++)
            {
                result *= x;
                counter.IncrementStep(); // Учитываем каждое умножение
            }

            return result;
        }

        /// <summary>
        /// Версия с защитой от переполнения и дополнительной проверкой
        /// </summary>
        private double NaivePowerSafe(double x, int n, PerformanceCounter counter)
        {
            if (n < 0) throw new ArgumentException("Показатель степени должен быть неотрицательным");
            if (n == 0) return 1.0;
            if (x == 0.0) return 0.0;
            if (x == 1.0) return 1.0;

            double result = 1.0;
            for (int i = 0; i < n; i++)
            {
                // Проверка на переполнение (для больших степеней)
                if (double.IsInfinity(result * x))
                {
                    throw new OverflowException("Переполнение при возведении в степень");
                }
                result *= x;
                counter.IncrementStep();
            }

            return result;
        }
    }
}
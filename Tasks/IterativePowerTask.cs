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
    public class ClassicFastPowerTask : ITask
    {
        private readonly int m, n;
        private readonly double baseValue;
        private readonly CounterMode mode;

        public ClassicFastPowerTask(int m, int n, double baseValue = 2.0, CounterMode mode = CounterMode.TimeAndSteps)
        {
            this.m = m;
            this.n = n;
            this.baseValue = baseValue;
            this.mode = mode;
        }

        public string Name => "Classic Fast Power Algorithm (Iterative)";
        public PlotModel? LastPlotModel { get; private set; }

        public void Run()
        {
            var times = new List<double>();
            var theoryRaw = new List<double>();

            for (int exp = m; exp <= n; exp++)
            {
                var counter = new PerformanceCounter(mode);

                counter.Start();
                double result = ClassicFastPower(baseValue, exp, counter);
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
        /// Классический быстрый алгоритм возведения в степень (итеративная версия)
        /// Сложность: O(log n)
        /// </summary>
        private double ClassicFastPower(double x, int n, PerformanceCounter counter)
        {
            if (n < 0) throw new ArgumentException("Показатель степени должен быть неотрицательным");
            if (n == 0) return 1.0;
            if (x == 0.0) return 0.0;
            if (x == 1.0) return 1.0;

            double result = 1.0;
            double currentBase = x;
            int currentExponent = n;

            while (currentExponent > 0)
            {
                counter.IncrementStep(); // Учет каждой итерации

                if (currentExponent % 2 == 1)
                {
                    result *= currentBase;
                    counter.IncrementStep(); // Учет умножения
                }

                currentBase *= currentBase; // Возведение в квадрат
                counter.IncrementStep(); // Учет возведения в квадрат
                
                currentExponent /= 2; // Уменьшение показателя в 2 раза
            }

            return result;
        }

        /// <summary>
        /// Альтернативная реализация с битовыми операциями
        /// </summary>
        private double ClassicFastPowerBitwise(double x, int n, PerformanceCounter counter)
        {
            if (n < 0) throw new ArgumentException("Показатель степени должен быть неотрицательным");
            if (n == 0) return 1.0;

            double result = 1.0;
            double currentBase = x;
            int currentExponent = n;

            while (currentExponent > 0)
            {
                counter.IncrementStep();

                // Проверка младшего бита
                if ((currentExponent & 1) == 1)
                {
                    result *= currentBase;
                    counter.IncrementStep();
                }

                currentBase *= currentBase;
                counter.IncrementStep();
                
                currentExponent >>= 1; // Битовый сдвиг вправо (деление на 2)
            }

            return result;
        }
    }
}
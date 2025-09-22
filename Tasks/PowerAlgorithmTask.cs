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
    namespace Lab1.Tasks
    {
        public class PowerAlgorithmTask : ITask
        {
            private readonly double baseValue;
            private readonly int exponent;
            private readonly CounterMode mode;

            public PowerAlgorithmTask(double baseValue, int exponent, CounterMode mode = CounterMode.TimeAndSteps)
            {
                this.baseValue = baseValue;
                this.exponent = exponent;
                this.mode = mode;
            }

            public string Name => "Power Algorithm (Naive & Fast)";
            public PlotModel? LastPlotModel { get; private set; }

            public void Run()
            {
                // Наивный метод + собираем значения для графика роста a^k
                var counter = new PerformanceCounter(mode);
                counter.Start();

                double naiveResult = 1;
                var series = new List<double>();
                series.Add(1); // a^0

                for (int i = 1; i <= exponent; i++)
                {
                    naiveResult *= baseValue;
                    series.Add(naiveResult);
                    counter.IncrementStep();
                }

                counter.Stop();
                Console.WriteLine($"{Name} (Naive): {naiveResult}, время={counter.ElapsedMs:F3} мс, шаги={counter.Steps}");

                // Быстрое возведение в степень
                counter = new PerformanceCounter(mode);
                counter.Start();
                double fastResult = FastPower(baseValue, exponent, counter);
                counter.Stop();
                Console.WriteLine($"{Name} (Fast): {fastResult}, время={counter.ElapsedMs:F3} мс, шаги={counter.Steps}");

                // Визуализируем рост последовательности a^k, k=0..exponent
                LastPlotModel = Plotter.CreateLinePlot($"a^k, a={baseValue}", series, 0);
            }

            private double FastPower(double a, int n, PerformanceCounter counter)
            {
                counter.IncrementStep();
                if (n == 0) return 1;
                if (n % 2 == 0)
                {
                    double half = FastPower(a, n / 2, counter);
                    return half * half;
                }
                else
                {
                    return a * FastPower(a, n - 1, counter);
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lab1.Tasks;
using OxyPlot;
using Lab1.Utils;
using Lab1.Plot;

namespace Lab1.Services
{
    public static class GraphService
    {
        /// <summary>
        /// Запускает полный набор алгоритмов
        /// </summary>
            public static List<PlotModel> GetAllPlots()
            {
                var runner = new Runner();

                runner.AddTask(new ConstantFunctionTask(10, 1000, 1.0, CounterMode.TimeOnly));
                runner.AddTask(new SumElementsTask(10, 1000, CounterMode.TimeOnly));
                runner.AddTask(new ProductElementsTask(10, 1000, CounterMode.TimeOnly));
                runner.AddTask(new PolynomialEvaluationTask(10, 1000, CounterMode.TimeOnly));
                runner.AddTask(new KadaneAlgorithmTask(10, 1000, CounterMode.TimeOnly));
                runner.AddTask(new BoyerMooreMajorityTask(10, 1000, CounterMode.TimeOnly));
                runner.AddTask(new SlidingWindowAnagramTask(10, 1000, CounterMode.TimeOnly));
                runner.AddTask(new TrappingRainWaterTask(10, 1000, CounterMode.TimeOnly)); // ← ДОБАВЛЕНО
                runner.AddTask(new BubbleSortTask(1, 1000, CounterMode.TimeOnly));
                runner.AddTask(new QuickSortTask(10, 1000, CounterMode.TimeOnly));
                runner.AddTask(new TimSortTask(10, 1000, CounterMode.TimeOnly));
                runner.AddTask(new FastPowerTask(2, 1000, 2.0, CounterMode.StepsOnly));

                runner.AddTask(new NaivePowerTask(2, 1000, 2.0, CounterMode.StepsOnly));
                runner.AddTask(new RecursivePowerTask(2, 1000, 2.0, CounterMode.StepsOnly));
                runner.AddTask(new FastPowerTask(2, 1000, 2.0, CounterMode.StepsOnly));
                runner.AddTask(new ClassicFastPowerTask(2, 1000, 2.0, CounterMode.StepsOnly));

                runner.RunAll();
                return runner.Models;
            }

            public static List<PlotModel> GetSelectedPlots()
            {
                var runner = new Runner();

                runner.AddTask(new FastPowerTask(2, 1000, 2.0, CounterMode.StepsOnly));
                runner.AddTask(new ClassicFastPowerTask(2, 1000, 2.0, CounterMode.StepsOnly));
                runner.AddTask(new NaivePowerTask(2, 1000, 2.0, CounterMode.StepsOnly));
                runner.AddTask(new RecursivePowerTask(2, 1000, 2.0, CounterMode.StepsOnly));
                runner.AddTask(new KadaneAlgorithmTask(10, 1000, CounterMode.TimeOnly));
                runner.AddTask(new BoyerMooreMajorityTask(10, 1000, CounterMode.TimeOnly));
                runner.AddTask(new SlidingWindowAnagramTask(10, 1000, CounterMode.TimeOnly));
                runner.AddTask(new TrappingRainWaterTask(10, 1000, CounterMode.TimeOnly)); // ← ДОБАВЛЕНО

                runner.RunAll();
                return runner.Models;
            }
    }
}
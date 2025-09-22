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
    public class PolynomialEvaluationTask : ITask
    {
        private readonly int degree;
        private readonly CounterMode mode;

        public PolynomialEvaluationTask(int degree, CounterMode mode = CounterMode.TimeAndSteps)
        {
            this.degree = degree;
            this.mode = mode;
        }

        public string Name => "Polynomial Evaluation (Naive & Horner)";
        public PlotModel? LastPlotModel { get; private set; }
        public void Run()
        {
            var coeffs = DataGenerator.Uniform(0, degree, 1, 5);
            double x = 1.5;

            // Наивный метод
            var counter = new PerformanceCounter(mode);
            counter.Start();
            double naive = 0;
            for (int k = 0; k < coeffs.Count; k++)
            {
                naive += coeffs[k] * Math.Pow(x, degree - k);
                counter.IncrementStep();
            }
            counter.Stop();
            Console.WriteLine($"{Name} (Naive): {naive:F3}, время={counter.ElapsedMs:F3} мс, шаги={counter.Steps}");

            // Горнер
            counter = new PerformanceCounter(mode);
            counter.Start();
            double horner = coeffs[0];
            for (int k = 1; k < coeffs.Count; k++)
            {
                horner = horner * x + coeffs[k];
                counter.IncrementStep();
            }
            counter.Stop();
            Console.WriteLine($"{Name} (Horner): {horner:F3}, время={counter.ElapsedMs:F3} мс, шаги={counter.Steps}");

            // Для отображения: покажем ряд коэффициентов как линию
            LastPlotModel = Plotter.CreateLinePlot("Polynomial coefficients", coeffs, 0);
        }
    }
}
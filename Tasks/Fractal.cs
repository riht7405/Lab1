using System;
using System.Collections.Generic;
using System.Linq;
using Lab1.Interfaces;
using Lab1.Plot;
using Lab1.Utils;
using OxyPlot;
using static Lab1.Tasks.FlowerFractalAlgorithm;

namespace Lab1.Tasks
{
    public class FlowerFractalTask : ITask
    {
        private readonly int minDepth, maxDepth;
        private readonly CounterMode mode;

        public FlowerFractalTask(int minDepth, int maxDepth, CounterMode mode = CounterMode.TimeAndSteps)
        {
            this.minDepth = minDepth;
            this.maxDepth = maxDepth;
            this.mode = mode;
        }

        public string Name => "Flower Fractal Algorithm Complexity";
        public PlotModel? LastPlotModel { get; private set; }

        public void Run()
        {
            var steps = new List<double>();
            var theoryRaw = new List<double>();

            for (int depth = minDepth; depth <= maxDepth; depth++)
            {
                var counter = new PerformanceCounter(mode);

                counter.Start();
                // Запускаем фрактальный алгоритм
                var fractal = new FlowerFractalAlgorithm();
                fractal.DrawFlowerFractal(new PointF(0, 0), 100.0f, depth, counter);
                counter.Stop();

                steps.Add(mode == CounterMode.StepsOnly ? counter.Steps : counter.ElapsedMs / 1000.0);

                // Теоретическая оценка: экспоненциальная сложность O(branches^depth)
                // branches = 6 (основные ветви) + 1 (внутренняя окружность) = 7
                theoryRaw.Add(Math.Pow(7, depth)); // O(7^depth)
            }

            // Нормирование теоретической кривой
            double coef = steps[^1] / theoryRaw[^1];
            var theory = theoryRaw.Select(v => v * coef).ToList();

            LastPlotModel = Plotter.CreateLinePlot(Name, steps, minDepth, theory);
            LastPlotModel.InvalidatePlot(true);
        }
        
    }

    // ЧИСТЫЙ АЛГОРИТМ БЕЗ ГРАФИКИ - только рекурсивная логика
    public class FlowerFractalAlgorithm
    {
        // Основной рекурсивный метод с измерением производительности
        public void DrawFlowerFractal(PointF center, float radius, int depth, PerformanceCounter counter)
        {
            // БАЗОВЫЙ СЛУЧАЙ - выход из рекурсии
            if (depth <= 0 || radius < 0.1)
                return;

            // ОСНОВНАЯ ОПЕРАЦИЯ - "рисование" окружности (1 шаг)
            DrawCircle(center, radius, depth, counter);

            // РЕКУРСИВНЫЙ СЛУЧАЙ 1: 6 дочерних окружностей
            int childCount = 6;
            for (int i = 0; i < childCount; i++)
            {
                double angle = 2 * Math.PI * i / childCount;
                float distance = radius * 0.8f;

                // Вычисляем новый центр
                PointF newCenter = new PointF(
                    center.X + distance * (float)Math.Cos(angle),
                    center.Y + distance * (float)Math.Sin(angle)
                );

                // НОВЫЙ РЕКУРСИВНЫЙ ВЫЗОВ с уменьшенной глубиной
                float newRadius = radius * 0.4f;
                DrawFlowerFractal(newCenter, newRadius, depth - 1, counter);
            }

            // РЕКУРСИВНЫЙ СЛУЧАЙ 2: внутренняя окружность (только для depth >= 3)
            if (depth >= 3)
            {
                float innerRadius = radius * 0.5f;
                DrawFlowerFractal(center, innerRadius, depth - 2, counter);
            }
        }

        // Вспомогательный метод - считаем как одну операцию
        private void DrawCircle(PointF center, float radius, int depth, PerformanceCounter counter)
        {
            int a = Convert.ToInt32(radius) * Convert.ToInt32(depth); // 1 операция - "рисование" окружности
        }
        public class PointF
        {
            public float X { get; set; }
            public float Y { get; set; }
            public PointF(float x, float y) { X = x; Y = y; }
        }
    }
}
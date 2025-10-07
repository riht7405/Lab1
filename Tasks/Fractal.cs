using System;
using System.Collections.Generic;
using System.Linq;
using Lab1.Interfaces;
using Lab1.Plot;
using Lab1.Utils;
using OxyPlot;

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
            if (depth <= 0 || radius < 1)
                return;

            // ОСНОВНАЯ ОПЕРАЦИЯ - "рисование" окружности (1 шаг)
            DrawCircle(center, radius, depth, counter);
            
            // РЕКУРСИВНЫЙ СЛУЧАЙ 1: 6 дочерних окружностей
            int childCount = 6;
            for (int i = 0; i < childCount; i++)
            {
                counter.IncrementStep(); // Учет каждой итерации цикла
                
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
                counter.IncrementStep(); // Учет проверки условия
                
                float innerRadius = radius * 0.5f;
                DrawFlowerFractal(center, innerRadius, depth - 2, counter);
            }
        }

        // Вспомогательный метод - считаем как одну операцию
        private void DrawCircle(PointF center, float radius, int depth, PerformanceCounter counter)
        {
            counter.IncrementStep(); // 1 операция - "рисование" окружности
        }

        // Альтернативная версия с точным подсчетом операций
        public int CalculateTotalOperations(int depth)
        {
            if (depth <= 0) return 0;
            
            // Базовые операции на текущем уровне:
            // 1 (DrawCircle) + 6 итераций + 1 дополнительный вызов (если depth >= 3)
            int currentLevelOps = 1 + 6; // DrawCircle + цикл из 6 итераций
            
            // Рекурсивные вызовы
            int recursiveOps = 0;
            for (int i = 0; i < 6; i++)
            {
                recursiveOps += CalculateTotalOperations(depth - 1);
            }
            
            // Дополнительный внутренний вызов
            if (depth >= 3)
            {
                currentLevelOps += 1; // учет дополнительного вызова
                recursiveOps += CalculateTotalOperations(depth - 2);
            }
            
            return currentLevelOps + recursiveOps;
        }

        // Версия с мемоизацией для эффективного расчета
        public long CalculateOperationsMemoized(int depth, Dictionary<int, long> memo)
        {
            if (depth <= 0) return 0;
            if (memo.ContainsKey(depth)) return memo[depth];
            
            long operations = 1 + 6; // DrawCircle + 6 итераций
            
            // Рекурсивные вызовы для 6 дочерних окружностей
            for (int i = 0; i < 6; i++)
            {
                operations += CalculateOperationsMemoized(depth - 1, memo);
            }
            
            // Внутренняя окружность для depth >= 3
            if (depth >= 3)
            {
                operations += 1 + CalculateOperationsMemoized(depth - 2, memo);
            }
            
            memo[depth] = operations;
            return operations;
        }
    }

    // Класс PointF для алгоритма
    public class PointF
    {
        public float X { get; set; }
        public float Y { get; set; }
        public PointF(float x, float y) { X = x; Y = y; }
    }
}
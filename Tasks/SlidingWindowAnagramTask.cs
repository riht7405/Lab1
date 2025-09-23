using System;
using System.Collections.Generic;
using System.Linq;
using Lab1.Interfaces;
using Lab1.Plot;
using Lab1.Utils;
using OxyPlot;

namespace Lab1.Tasks
{
    public class SlidingWindowAnagramTask : ITask
    {
        private readonly int m, n;
        private readonly CounterMode mode;

        public SlidingWindowAnagramTask(int m, int n, CounterMode mode = CounterMode.TimeAndSteps)
        {
            this.m = m;
            this.n = n;
            this.mode = mode;
        }

        public string Name => "Sliding Window Anagram Search";
        public PlotModel? LastPlotModel { get; private set; }

        public void Run()
        {
            var times = new List<double>();
            var theoryRaw = new List<double>();
            int repeats = 3;

            for (int textSize = m; textSize <= n; textSize++)
            {
                double totalTime = 0;

                for (int r = 0; r < repeats; r++)
                {
                    // Генерируем текст и паттерн для поиска анаграмм
                    var (text, pattern) = GenerateTextWithAnagram(textSize);
                    var counter = new PerformanceCounter(mode);

                    counter.Start();
                    var anagramIndices = FindAnagrams(text, pattern, counter);
                    counter.Stop();

                    totalTime += mode == CounterMode.StepsOnly ? counter.Steps : counter.ElapsedMs / 1000.0;
                }

                times.Add(totalTime / repeats);
                theoryRaw.Add(textSize); // O(n)
            }

            // Нормирование теоретической кривой
            double coef = times[^1] / theoryRaw[^1];
            var theory = theoryRaw.Select(v => v * coef).ToList();

            LastPlotModel = Plotter.CreateLinePlot(Name, times, m, theory);
            LastPlotModel.InvalidatePlot(true);
        }

        /// <summary>
        /// Генерирует текст и паттерн, гарантируя наличие анаграмм
        /// </summary>
        private (string text, string pattern) GenerateTextWithAnagram(int textSize)
        {
            var random = new Random();
            
            // Генерируем случайный паттерн длиной 5-10 символов
            int patternLength = random.Next(5, Math.Min(11, textSize / 2));
            string pattern = GenerateRandomString(patternLength, random);
            
            // Генерируем текст, гарантируя наличие анаграммы паттерна
            string text = GenerateTextWithGuaranteedAnagram(textSize, pattern, random);
            
            return (text, pattern);
        }

        /// <summary>
        /// Генерирует случайную строку из латинских букв
        /// </summary>
        private string GenerateRandomString(int length, Random random)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Range(0, length)
                .Select(_ => chars[random.Next(chars.Length)])
                .ToArray());
        }

        /// <summary>
        /// Генерирует текст, гарантированно содержащий анаграмму паттерна
        /// </summary>
        private string GenerateTextWithGuaranteedAnagram(int textSize, string pattern, Random random)
        {
            // Создаем анаграмму паттерна
            string anagram = new string(pattern.ToCharArray().OrderBy(_ => random.Next()).ToArray());
            
            // Генерируем текст, вставляя анаграмму в случайную позицию
            int insertPosition = random.Next(0, textSize - pattern.Length + 1);
            
            string prefix = GenerateRandomString(insertPosition, random);
            string suffix = GenerateRandomString(textSize - insertPosition - pattern.Length, random);
            
            return prefix + anagram + suffix;
        }

        /// <summary>
        /// Алгоритм скользящего окна для поиска всех анаграмм паттерна в тексте
        /// </summary>
        private List<int> FindAnagrams(string text, string pattern, PerformanceCounter counter)
        {
            var result = new List<int>();
            
            if (pattern.Length > text.Length)
                return result;

            // Частоты символов в паттерне
            var patternFreq = new int[26];
            // Частоты символов в текущем окне
            var windowFreq = new int[26];

            counter.IncrementStep(); // Учёт инициализации массивов

            // Заполняем частоты для паттерна
            foreach (char c in pattern)
            {
                patternFreq[c - 'a']++;
                counter.IncrementStep();
            }

            // Инициализируем окно размером с паттерн
            for (int i = 0; i < pattern.Length; i++)
            {
                windowFreq[text[i] - 'a']++;
                counter.IncrementStep();
            }

            // Проверяем первое окно
            if (AreFrequenciesEqual(patternFreq, windowFreq, counter))
            {
                result.Add(0);
            }

            // Сдвигаем окно по тексту
            for (int i = pattern.Length; i < text.Length; i++)
            {
                counter.IncrementStep(); // Учёт каждой итерации

                // Убираем левый символ из окна
                char leftChar = text[i - pattern.Length];
                windowFreq[leftChar - 'a']--;
                counter.IncrementStep();

                // Добавляем правый символ в окно
                char rightChar = text[i];
                windowFreq[rightChar - 'a']++;
                counter.IncrementStep();

                // Проверяем, является ли текущее окно анаграммой
                if (AreFrequenciesEqual(patternFreq, windowFreq, counter))
                {
                    result.Add(i - pattern.Length + 1);
                }
            }

            return result;
        }

        /// <summary>
        /// Проверяет, равны ли частоты символов в двух массивах
        /// </summary>
        private bool AreFrequenciesEqual(int[] freq1, int[] freq2, PerformanceCounter counter)
        {
            for (int i = 0; i < 26; i++)
            {
                counter.IncrementStep(); // Учёт каждого сравнения
                if (freq1[i] != freq2[i])
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Оптимизированная версия с использованием счетчика совпадений
        /// </summary>
        private List<int> FindAnagramsOptimized(string text, string pattern, PerformanceCounter counter)
        {
            var result = new List<int>();
            
            if (pattern.Length > text.Length)
                return result;

            var patternFreq = new int[26];
            var windowFreq = new int[26];

            counter.IncrementStep();

            // Заполняем частоты для паттерна и первого окна
            for (int i = 0; i < pattern.Length; i++)
            {
                patternFreq[pattern[i] - 'a']++;
                windowFreq[text[i] - 'a']++;
                counter.IncrementStep();
            }

            // Считаем количество совпадающих частот
            int matchCount = 0;
            for (int i = 0; i < 26; i++)
            {
                counter.IncrementStep();
                if (patternFreq[i] == windowFreq[i])
                    matchCount++;
            }

            // Если все 26 частот совпадают - первое окно является анаграммой
            if (matchCount == 26)
            {
                result.Add(0);
            }

            // Сдвигаем окно
            for (int i = pattern.Length; i < text.Length; i++)
            {
                counter.IncrementStep();

                // Обрабатываем убираемый символ (левый)
                char leftChar = text[i - pattern.Length];
                int leftIndex = leftChar - 'a';
                
                // Если частоты были равны, то после уменьшения станут разными
                if (windowFreq[leftIndex] == patternFreq[leftIndex])
                    matchCount--;
                
                windowFreq[leftIndex]--;
                counter.IncrementStep();

                // Если после уменьшения частоты снова равны
                if (windowFreq[leftIndex] == patternFreq[leftIndex])
                    matchCount++;
                
                counter.IncrementStep();

                // Обрабатываем добавляемый символ (правый)
                char rightChar = text[i];
                int rightIndex = rightChar - 'a';
                
                // Если частоты были равны, то после увеличения станут разными
                if (windowFreq[rightIndex] == patternFreq[rightIndex])
                    matchCount--;
                
                windowFreq[rightIndex]++;
                counter.IncrementStep();

                // Если после увеличения частоты снова равны
                if (windowFreq[rightIndex] == patternFreq[rightIndex])
                    matchCount++;
                
                counter.IncrementStep();

                // Если все 26 частот совпадают
                if (matchCount == 26)
                {
                    result.Add(i - pattern.Length + 1);
                }
            }

            return result;
        }
    }
}
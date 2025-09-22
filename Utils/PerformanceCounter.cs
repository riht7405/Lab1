using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Lab1.Utils
{
    public enum CounterMode
    {
        TimeOnly,       // Считаем только время
        StepsOnly,      // Считаем только шаги
        TimeAndSteps    // Считаем и время, и шаги
    }

    public class PerformanceCounter
    {
        private readonly Stopwatch stopwatch = new Stopwatch();
        private readonly CounterMode mode;

        public long Steps { get; private set; }
        public TimeSpan Elapsed => stopwatch.Elapsed;
        public double ElapsedMs => stopwatch.Elapsed.TotalMilliseconds;

        public PerformanceCounter(CounterMode mode = CounterMode.TimeAndSteps)
        {
            this.mode = mode;
        }

        public void Start()
        {
            Steps = 0;
            if (mode != CounterMode.StepsOnly)
                stopwatch.Restart();
        }

        public void IncrementStep()
        {
            if (mode != CounterMode.TimeOnly)
                Steps++;
        }

        public void AddSteps(long count)
        {
            if (mode != CounterMode.TimeOnly)
                Steps += count;
        }

        public void Stop()
        {
            if (mode != CounterMode.StepsOnly)
                stopwatch.Stop();
        }
    }
}
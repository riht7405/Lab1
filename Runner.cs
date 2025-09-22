using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lab1.Interfaces;
using OxyPlot;

namespace Lab1
{
    public class Runner
    {
        private readonly List<ITask> tasks = new List<ITask>();

        // Графики, собранные после выполнения задач
        public List<PlotModel> Models { get; } = new List<PlotModel>();

        public void AddTask(ITask task) => tasks.Add(task);

        public void RunAll()
        {
            Models.Clear();

            foreach (var task in tasks)
            {
                Console.WriteLine($"Запуск: {task.Name}");
                task.Run();

                if (task.LastPlotModel != null)
                    Models.Add(task.LastPlotModel);

                Console.WriteLine();
            }
        }
    }
}
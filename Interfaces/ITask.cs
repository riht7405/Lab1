using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;

namespace Lab1.Interfaces
{
    public interface ITask
    {
        string Name { get; }
        void Run();

        // Последняя построенная модель графика этой задачи
        PlotModel? LastPlotModel { get; }
    }
}
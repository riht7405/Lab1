using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;

namespace Lab1.Plot
{
    public static class Plotter
    {
        public static PlotModel CreateLinePlot(string title, IList<double> values, int startIndex)
        {
            var model = new PlotModel { Title = title };

            var xAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "День",
                Minimum = startIndex,
                Maximum = startIndex + values.Count - 1
            };

            var yAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Мощность (ср.)"
            };

            model.Axes.Add(xAxis);
            model.Axes.Add(yAxis);

            var series = new LineSeries { Title = "y" };
            for (int i = 0; i < values.Count; i++)
            {
                series.Points.Add(new DataPoint(startIndex + i, values[i]));
            }

            model.Series.Add(series);

            return model;
        }
    }
}
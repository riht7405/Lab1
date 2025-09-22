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
        public static PlotModel CreateLinePlot(string title, IList<double> experimental, int startIndex, IList<double>? theoretical = null)
        {
            var plotModel = new PlotModel
            {
                Title = title,
                Background = OxyColors.White
            };

            var xAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Размерность входных данных n",
                Minimum = startIndex,
                Maximum = startIndex + experimental.Count - 1,
                MajorGridlineStyle = LineStyle.Solid,
                MajorGridlineColor = OxyColors.LightGray
            };

            var yAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = theoretical == null ? "Среднее время выполнения (сек)" : "Значение",
                MajorGridlineStyle = LineStyle.Solid,
                MajorGridlineColor = OxyColors.LightGray
            };

            plotModel.Axes.Add(xAxis);
            plotModel.Axes.Add(yAxis);

            // Экспериментальная серия (зелёная)
            var expSeries = new LineSeries
            {
                Color = OxyColors.Lime,
                StrokeThickness = 2,
                MarkerType = MarkerType.None,
                Title = "Эксперимент"
            };
            for (int i = 0; i < experimental.Count; i++)
                expSeries.Points.Add(new DataPoint(startIndex + i, experimental[i]));
            plotModel.Series.Add(expSeries);

            // Теоретическая серия (красная)
            if (theoretical != null)
            {
                var theorySeries = new LineSeries
                {
                    Color = OxyColors.Red,
                    StrokeThickness = 2,
                    MarkerType = MarkerType.None,
                    Title = "Теория"
                };
                for (int i = 0; i < theoretical.Count; i++)
                    theorySeries.Points.Add(new DataPoint(startIndex + i, theoretical[i]));
                plotModel.Series.Add(theorySeries);
            }

            return plotModel;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Series;

namespace Lab1 // замени на своё namespace, если у тебя другое
{
    public class MainViewModel
    {
        public PlotModel CurrentPlot { get; }

        public MainViewModel()
        {
            // Создаём модель графика
            var plotModel = new PlotModel { Title = "Пример графика" };

            // Добавляем линию
            var lineSeries = new LineSeries
            {
                Title = "Sine wave",
                MarkerType = MarkerType.Circle
            };

            // Заполняем точками
            for (double x = 0; x <= 10; x += 0.5)
            {
                lineSeries.Points.Add(new DataPoint(x, Math.Sin(x)));
            }

            plotModel.Series.Add(lineSeries);

            // Присваиваем модель свойству
            CurrentPlot = plotModel;
        }
    }
}

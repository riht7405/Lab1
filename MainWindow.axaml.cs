using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using OxyPlot;
using System.Linq;
using Lab1.Tasks;
using System.Collections.ObjectModel;
using Lab1.Services;

namespace Lab1
{
    public partial class MainWindow : Window
    {
        // Коллекция графиков, к которой будет привязан TabControl в XAML
        public ObservableCollection<PlotModel> Plots { get; } = new();

        public MainWindow()
        {
            InitializeComponent();
            var plots = GraphService.GetSelectedPlots();
            foreach (var plot in plots)
                Plots.Add(plot);
            DataContext = this;
            Console.WriteLine($"Plots.Count = {Plots.Count}");

        }

    }
}
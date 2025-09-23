using Avalonia;
using Lab1.Tasks;
using Lab1.Utils;
using Avalonia.ReactiveUI;

namespace Lab1
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args) =>
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                         .UsePlatformDetect()
                         .LogToTrace().UseReactiveUI();
    }
}
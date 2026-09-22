using System.Windows;
using System.Windows.Threading;
using HotelApp.Models;
using Microsoft.EntityFrameworkCore;
using HotelApp.Data;
using HotelApp.Services;

namespace HotelApp
{
    public partial class App : Application
    {
        public static User? CurrentUser { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Глобальный обработчик необработанных исключений
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                var exception = args.ExceptionObject as Exception;
                MessageBox.Show(
                    $"Критическая ошибка:\n\n{exception?.Message}\n\nStack trace:\n{exception?.StackTrace}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            };

            DispatcherUnhandledException += (sender, args) =>
            {
                MessageBox.Show(
                    $"Ошибка UI:\n\n{args.Exception.Message}\n\nStack trace:\n{args.Exception.StackTrace}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                args.Handled = true;
            };

            using var context = new AppDbContext();
            context.Database.Migrate();
            DatabaseSeeder.Seed(context);
        }
    }
}
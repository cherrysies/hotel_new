using System;
using System.Windows;
using System.Windows.Threading;
using HotelApp.Models;
using Microsoft.EntityFrameworkCore;
using HotelApp.Data;
using HotelApp.Services;
using HotelApp.Views; // <-- ЭТО БЫЛО ПРОПУЩЕНО

namespace HotelApp
{
    public partial class App : Application
    {
        public static User? CurrentUser { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Глобальный обработчик исключений
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
                    $"Ошибка UI:\n\n{args.Exception.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                args.Handled = true;
            };

            // Инициализация БД
            try
            {
                using var context = new AppDbContext();
                context.Database.EnsureCreated(); // Используем EnsureCreated вместо Migrate для простоты
                DatabaseSeeder.Seed(context);
            }
            catch (Exception dbEx)
            {
                MessageBox.Show($"Ошибка инициализации БД: {dbEx.Message}", "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var loginWindow = new LoginWindow();
            loginWindow.Show();
        }
    }
}
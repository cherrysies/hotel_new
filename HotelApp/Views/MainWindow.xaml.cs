using System.Windows;
using HotelApp.Models;

namespace HotelApp.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadUserInfo();
        }

        private void LoadUserInfo()
        {
            if (App.CurrentUser != null)
            {
                string roleText = App.CurrentUser.Role switch
                {
                    Role.Admin => "Администратор",
                    Role.Manager => "Менеджер",
                    Role.Client => "Клиент",
                    _ => "Неизвестно"
                };

                txtUserInfo.Text = $"Пользователь: {App.CurrentUser.FullName} | Роль: {roleText}";
            }
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            App.CurrentUser = null;
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}
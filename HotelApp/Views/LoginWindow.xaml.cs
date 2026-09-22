using System.Linq;
using System.Windows;
using HotelApp.Data;
using HotelApp.Models;

namespace HotelApp.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = pwdPassword.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                txtError.Text = "Введите логин и пароль";
                return;
            }

            using var context = new AppDbContext();
            var user = context.Users.FirstOrDefault(u => u.Login == login);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                txtError.Text = "Неверный логин или пароль";
                return;
            }

            App.CurrentUser = user;

            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
using System.Linq;
using System.Windows;
using HotelApp.Data;
using HotelApp.Models;

namespace HotelApp.Views
{
    public partial class RoomEditWindow : Window
    {
        private readonly AppDbContext _context;
        private readonly Room? _roomToEdit;

        public RoomEditWindow(Room? room)
        {
            InitializeComponent();
            _context = new AppDbContext();
            _roomToEdit = room;

            // Заполняем ComboBox типами номеров
            cmbType.ItemsSource = _context.RoomTypes.ToList();

            if (_roomToEdit != null)
            {
                Title = "Редактирование номера";
                txtNumber.Text = _roomToEdit.Number;
                cmbType.SelectedValue = _roomToEdit.RoomTypeId;
                txtPrice.Text = _roomToEdit.PricePerDay.ToString();
            }
            else
            {
                Title = "Добавление нового номера";
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNumber.Text) || cmbType.SelectedValue == null || !decimal.TryParse(txtPrice.Text, out decimal price))
            {
                MessageBox.Show("Заполните все поля корректно.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_roomToEdit == null)
            {
                // Добавление
                var newRoom = new Room
                {
                    Number = txtNumber.Text.Trim(),
                    RoomTypeId = (int)cmbType.SelectedValue,
                    Floor = 1, // По умолчанию, можно добавить поле в UI
                    PricePerDay = price,
                    Status = RoomStatus.Available
                };
                _context.Rooms.Add(newRoom);
            }
            else
            {
                // Редактирование
                _roomToEdit.Number = txtNumber.Text.Trim();
                _roomToEdit.RoomTypeId = (int)cmbType.SelectedValue;
                _roomToEdit.PricePerDay = price;
                _context.Rooms.Update(_roomToEdit);
            }

            _context.SaveChanges();
            DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
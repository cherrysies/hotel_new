using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using HotelApp.Data;
using HotelApp.Models;

namespace HotelApp.Views
{
    public partial class BookingEditWindow : Window
    {
        private readonly AppDbContext _context;
        private readonly Booking? _bookingToEdit;
        private decimal _currentPricePerDay = 0;

        public BookingEditWindow(Booking? booking)
        {
            InitializeComponent();
            _context = new AppDbContext();
            _bookingToEdit = booking;

            try
            {
                // Заполняем пользователей
                if (App.CurrentUser?.Role == Role.Client)
                {
                    cmbUser.ItemsSource = new[] { App.CurrentUser };
                    cmbUser.SelectedValue = App.CurrentUser.Id;
                    cmbUser.IsEnabled = false;
                }
                else
                {
                    cmbUser.ItemsSource = _context.Users.Where(u => u.Role == Role.Client).ToList();
                    cmbUser.IsEnabled = true;
                }

                // Заполняем номера
                var rooms = _bookingToEdit == null 
                    ? _context.Rooms.Where(r => r.Status == RoomStatus.Available).ToList()
                    : _context.Rooms.ToList();
                
                cmbRoom.ItemsSource = rooms;

                if (_bookingToEdit != null)
                {
                    Title = "Редактирование бронирования";
                    cmbUser.SelectedItem = _context.Users.FirstOrDefault(u => u.Id == _bookingToEdit.UserId);
                    cmbRoom.SelectedItem = rooms.FirstOrDefault(r => r.Id == _bookingToEdit.RoomId);
                    dpCheckIn.SelectedDate = _bookingToEdit.CheckIn;
                    dpCheckOut.SelectedDate = _bookingToEdit.CheckOut;
                    
                    var room = rooms.FirstOrDefault(r => r.Id == _bookingToEdit.RoomId);
                    if (room != null) _currentPricePerDay = room.PricePerDay;
                    
                    CalculateTotal();
                }
                else
                {
                    Title = "Новое бронирование";
                    dpCheckIn.SelectedDate = DateTime.Today;
                    dpCheckOut.SelectedDate = DateTime.Today.AddDays(1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CmbRoom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cmbRoom.SelectedItem is Room room)
                {
                    _currentPricePerDay = room.PricePerDay;
                    CalculateTotal();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка выбора номера: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Date_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            CalculateTotal();
        }

        private void CalculateTotal()
        {
            if (dpCheckIn.SelectedDate.HasValue && dpCheckOut.SelectedDate.HasValue)
            {
                var days = (dpCheckOut.SelectedDate.Value - dpCheckIn.SelectedDate.Value).Days;
                if (days > 0)
                {
                    var total = days * _currentPricePerDay;
                    txtTotal.Text = $"Итого: {total:N0} ₽ ({days} сут.)";
                }
                else
                {
                    txtTotal.Text = "Итого: 0 ₽ (Проверьте даты)";
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmbUser.SelectedItem is not User selectedUser ||
                    cmbRoom.SelectedItem is not Room selectedRoom ||
                    !dpCheckIn.SelectedDate.HasValue ||
                    !dpCheckOut.SelectedDate.HasValue)
                {
                    MessageBox.Show("Заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Преобразуем все даты в UTC
                var checkIn = dpCheckIn.SelectedDate.Value.Date;
                var checkOut = dpCheckOut.SelectedDate.Value.Date;

                // Устанавливаем Kind=Utc
                checkIn = DateTime.SpecifyKind(checkIn, DateTimeKind.Utc);
                checkOut = DateTime.SpecifyKind(checkOut, DateTimeKind.Utc);

                if (checkOut <= checkIn)
                {
                    MessageBox.Show("Дата выезда должна быть позже даты заезда.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var days = (checkOut - checkIn).Days;
                var total = days * _currentPricePerDay;

                if (_bookingToEdit == null)
                {
                    var newBooking = new Booking
                    {
                        UserId = selectedUser.Id,
                        RoomId = selectedRoom.Id,
                        CheckIn = checkIn,
                        CheckOut = checkOut,
                        TotalAmount = total,
                        Status = BookingStatus.Pending,
                        CreatedAt = DateTime.Now  // Уже UTC
                    };

                    _context.Bookings.Add(newBooking);
                    _context.SaveChanges();

                    selectedRoom.Status = RoomStatus.Occupied;
                    _context.SaveChanges();
                }
                else
                {
                    _bookingToEdit.UserId = selectedUser.Id;
                    _bookingToEdit.RoomId = selectedRoom.Id;
                    _bookingToEdit.CheckIn = checkIn;
                    _bookingToEdit.CheckOut = checkOut;
                    _bookingToEdit.TotalAmount = total;
                    _context.Bookings.Update(_bookingToEdit);
                    _context.SaveChanges();
                }

                DialogResult = true;
            }
            catch (Exception ex)
            {
                string errorMessage = $"Ошибка сохранения: {ex.Message}";

                if (ex.InnerException != null)
                {
                    errorMessage += $"\n\nInner Exception: {ex.InnerException.Message}";
                }

                MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
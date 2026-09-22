using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using HotelApp.Data;
using HotelApp.Models;
using HotelApp.Views;

namespace HotelApp.ViewModels
{
    public class BookingDisplay
    {
        public int Id { get; set; }
        public string GuestName { get; set; } = string.Empty;
        public string RoomNumber { get; set; } = string.Empty;
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public decimal TotalAmount { get; set; }
        public BookingStatus Status { get; set; }
        public Booking OriginalBooking { get; set; } = null!;
    }

    public class BookingsViewModel : INotifyPropertyChanged
    {
        private readonly AppDbContext _context;
        private ObservableCollection<BookingDisplay> _allBookings = new();
        private ICollectionView _bookingsView = null!;

        private string _searchText = string.Empty;
        private int _selectedStatusId = 0;

        public event PropertyChangedEventHandler? PropertyChanged;

        public BookingsViewModel()
        {
            _context = new AppDbContext();
            
            CreateBookingCommand = new RelayCommand(_ => OpenEditWindow(null), _ => true);
            EditBookingCommand = new RelayCommand(_ => OpenEditWindow(GetSelectedBooking()?.OriginalBooking), _ => IsAdminOrManager && GetSelectedBooking() != null);
            DeleteBookingCommand = new RelayCommand(DeleteBooking, _ => IsAdmin && GetSelectedBooking() != null);

            LoadBookings();
        }

        public bool IsAdmin => App.CurrentUser?.Role == Role.Admin;
        public bool IsAdminOrManager => App.CurrentUser?.Role == Role.Admin || App.CurrentUser?.Role == Role.Manager;

        public ObservableCollection<BookingDisplay> AllBookings
        {
            get => _allBookings;
            set { _allBookings = value; OnPropertyChanged(); }
        }

        public ICollectionView BookingsView
        {
            get => _bookingsView;
            set { _bookingsView = value; OnPropertyChanged(); }
        }

        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(); ApplyFilters(); }
        }

        public int SelectedStatusId
        {
            get => _selectedStatusId;
            set { _selectedStatusId = value; OnPropertyChanged(); ApplyFilters(); }
        }

        public List<string> Statuses { get; } = new();

        public ICommand CreateBookingCommand { get; }
        public ICommand EditBookingCommand { get; }
        public ICommand DeleteBookingCommand { get; }

        private void LoadBookings()
        {
            var bookings = _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Room)
                .OrderByDescending(b => b.CreatedAt)
                .ToList();

            AllBookings.Clear();
            foreach (var b in bookings)
            {
                // Клиент видит только свои брони, Менеджер и Админ видят все
                if (App.CurrentUser?.Role == Role.Client && b.UserId != App.CurrentUser.Id)
                    continue;

                AllBookings.Add(new BookingDisplay
                {
                    Id = b.Id,
                    GuestName = b.User?.FullName ?? "Неизвестно",
                    RoomNumber = b.Room?.Number ?? "Неизвестно",
                    CheckIn = b.CheckIn,
                    CheckOut = b.CheckOut,
                    TotalAmount = b.TotalAmount,
                    Status = b.Status,
                    OriginalBooking = b
                });
            }

            BookingsView = CollectionViewSource.GetDefaultView(AllBookings);

            if (Statuses.Count == 0)
            {
                Statuses.Add("Все статусы");
                Statuses.Add("Ожидает");
                Statuses.Add("Подтверждено");
                Statuses.Add("Заселён");
                Statuses.Add("Выписан");
                Statuses.Add("Отменено");
            }

            ApplyFilters();
        }

        private BookingDisplay? GetSelectedBooking() => BookingsView?.CurrentItem as BookingDisplay;

        private void OpenEditWindow(Booking? booking)
        {
            var window = new BookingEditWindow(booking);
    
            if (window.ShowDialog() == true)
            {
                LoadBookings();
            }
        }

        private void DeleteBooking(object? parameter)
        {
            var display = GetSelectedBooking();
            if (display == null) return;

            var result = MessageBox.Show($"Удалить бронирование №{display.Id}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                _context.Bookings.Remove(display.OriginalBooking);
                _context.SaveChanges();
                LoadBookings();
                MessageBox.Show("Бронирование удалено.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public void ApplyFilters()
        {
            if (BookingsView == null) return;

            BookingsView.Filter = item =>
            {
                if (item is not BookingDisplay b) return false;

                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    bool matches = b.GuestName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                                   b.RoomNumber.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
                    if (!matches) return false;
                }

                if (SelectedStatusId > 0)
                {
                    var statusMap = new Dictionary<int, BookingStatus>
                    {
                        { 1, BookingStatus.Pending },
                        { 2, BookingStatus.Confirmed },
                        { 3, BookingStatus.CheckedIn },
                        { 4, BookingStatus.CheckedOut },
                        { 5, BookingStatus.Cancelled }
                    };

                    if (statusMap.ContainsKey(SelectedStatusId) && b.Status != statusMap[SelectedStatusId])
                        return false;
                }

                return true;
            };
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using HotelApp.Data;
using HotelApp.Models;
using HotelApp.Views;

namespace HotelApp.ViewModels
{
    public class SortOption
    {
        public string DisplayName { get; set; } = string.Empty;
        public string PropertyName { get; set; } = string.Empty;
    }

    public class RoomsViewModel : INotifyPropertyChanged
    {
        private readonly AppDbContext _context;
        private ObservableCollection<Room> _allRooms = new();
        private ICollectionView _roomsView = null!;

        private string _searchText = string.Empty;
        private int _selectedRoomTypeId = 0;
        private int _selectedStatusId = 0;
        private string _sortProperty = "Number";
        private bool _sortAscending = true;

        public event PropertyChangedEventHandler? PropertyChanged;

        public RoomsViewModel()
        {
            _context = new AppDbContext();

            AddRoomCommand = new RelayCommand(_ => OpenEditWindow(null), _ => IsAdmin);
            EditRoomCommand = new RelayCommand(_ => OpenEditWindow(GetSelectedRoom()), _ => IsAdmin && GetSelectedRoom() != null);
            DeleteRoomCommand = new RelayCommand(DeleteRoom, _ => IsAdmin && GetSelectedRoom() != null);

            LoadRooms();
        }

        public bool IsAdmin => App.CurrentUser?.Role == Role.Admin;

        public ObservableCollection<Room> AllRooms
        {
            get => _allRooms;
            set { _allRooms = value; OnPropertyChanged(); }
        }

        public ICollectionView RoomsView
        {
            get => _roomsView;
            set { _roomsView = value; OnPropertyChanged(); }
        }

        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(); ApplyFilters(); }
        }

        public int SelectedRoomTypeId
        {
            get => _selectedRoomTypeId;
            set { _selectedRoomTypeId = value; OnPropertyChanged(); ApplyFilters(); }
        }

        public int SelectedStatusId
        {
            get => _selectedStatusId;
            set { _selectedStatusId = value; OnPropertyChanged(); ApplyFilters(); }
        }

        public string SortProperty
        {
            get => _sortProperty;
            set { _sortProperty = value; OnPropertyChanged(); ApplySort(); }
        }

        public bool SortAscending
        {
            get => _sortAscending;
            set { _sortAscending = value; OnPropertyChanged(); ApplySort(); }
        }

        public List<RoomType> RoomTypes { get; } = new();
        public List<string> Statuses { get; } = new();
        public List<SortOption> SortOptions { get; } = new();

        public ICommand AddRoomCommand { get; }
        public ICommand EditRoomCommand { get; }
        public ICommand DeleteRoomCommand { get; }

        private void LoadRooms()
        {
            var rooms = _context.Rooms.OrderBy(r => r.Number).ToList();
            AllRooms = new ObservableCollection<Room>(rooms);
            RoomsView = CollectionViewSource.GetDefaultView(AllRooms);

            if (RoomTypes.Count == 0)
            {
                RoomTypes.Add(new RoomType { Id = 0, Name = "Все типы" });
                RoomTypes.AddRange(_context.RoomTypes.ToList());
            }

            if (Statuses.Count == 0)
            {
                Statuses.Add("Все статусы");
                Statuses.Add("Свободен");
                Statuses.Add("Занят");
                Statuses.Add("Уборка");
            }

            if (SortOptions.Count == 0)
            {
                SortOptions.Add(new SortOption { DisplayName = "По номеру", PropertyName = "Number" });
                SortOptions.Add(new SortOption { DisplayName = "По цене", PropertyName = "PricePerDay" });
                SortOptions.Add(new SortOption { DisplayName = "По этажу", PropertyName = "Floor" });
            }

            ApplyFilters();
            ApplySort();
        }

        private Room? GetSelectedRoom()
        {
            return RoomsView?.CurrentItem as Room;
        }

        private void OpenEditWindow(Room? room)
        {
            var window = new RoomEditWindow(room);
            
            if (window.ShowDialog() == true)
            {
                LoadRooms();
            }
        }

        private void DeleteRoom(object? parameter)
        {
            var room = GetSelectedRoom();
            if (room == null) return;

            var result = MessageBox.Show(
                $"Удалить номер {room.Number}?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                _context.Rooms.Remove(room);
                _context.SaveChanges();
                LoadRooms();
            }
        }

        public void ApplyFilters()
        {
            if (RoomsView == null) return;

            RoomsView.Filter = room =>
            {
                if (room is not Room r) return false;

                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    bool matches = r.Number.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                                  (r.Description?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false);
                    if (!matches) return false;
                }

                if (SelectedRoomTypeId > 0 && r.RoomTypeId != SelectedRoomTypeId)
                    return false;

                if (SelectedStatusId > 0)
                {
                    var statusMap = new Dictionary<int, RoomStatus>
                    {
                        { 1, RoomStatus.Available },
                        { 2, RoomStatus.Occupied },
                        { 3, RoomStatus.Cleaning }
                    };

                    if (statusMap.ContainsKey(SelectedStatusId) && r.Status != statusMap[SelectedStatusId])
                        return false;
                }

                return true;
            };
        }

        public void ApplySort()
        {
            if (RoomsView == null) return;

            RoomsView.SortDescriptions.Clear();
            if (!string.IsNullOrWhiteSpace(SortProperty))
            {
                var direction = SortAscending ? ListSortDirection.Ascending : ListSortDirection.Descending;
                RoomsView.SortDescriptions.Add(new SortDescription(SortProperty, direction));
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
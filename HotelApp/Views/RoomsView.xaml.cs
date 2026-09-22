using System.Windows;
using System.Windows.Controls;
using HotelApp.ViewModels;

namespace HotelApp.Views
{
    public partial class RoomsView : UserControl
    {
        public RoomsView()
        {
            InitializeComponent();
        }

        private void BtnToggleSort_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is RoomsViewModel vm)
            {
                vm.SortAscending = !vm.SortAscending;
            }
        }
    }
}
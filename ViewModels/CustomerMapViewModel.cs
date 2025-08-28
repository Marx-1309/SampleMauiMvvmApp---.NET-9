using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SampleMauiMvvmApp.Models;
using SampleMauiMvvmApp.Services;
using System.Collections.ObjectModel;

namespace SampleMauiMvvmApp.ViewModels
{
    public partial class CustomerMapViewModel : BaseViewModel
    {
        private readonly CustomerMapService _customerMapService;

        [ObservableProperty]
        private ObservableCollection<ReadingDto> customers = new();

        public CustomerMapViewModel(CustomerMapService customerMapService)
        {
            Title = "Customer Map";
            _customerMapService = customerMapService;
        }

        [RelayCommand]
        public async Task LoadCustomersAsync()
        {
            var response = await _customerMapService.GetCustomersWithCoordinatesAsync();
            if (response != null)
            {
                Customers = new ObservableCollection<ReadingDto>(response);
            }
        }
    }
}

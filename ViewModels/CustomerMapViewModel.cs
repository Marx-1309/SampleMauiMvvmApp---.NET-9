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

        [ObservableProperty]
        decimal newLatitude;

        [ObservableProperty]
        decimal newLongitude;

        public CustomerMapViewModel(CustomerMapService customerMapService)
        {
            Title = "Customer Map";
            _customerMapService = customerMapService;
        }

        [RelayCommand]
        public async Task LoadCustomersAsync(string readingsStatus)
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var response = await _customerMapService.GetCustomersWithCoordinatesAsync(readingsStatus);
                if (response != null)
                {
                    Customers = new ObservableCollection<ReadingDto>(response);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task SaveCustomerLocationAsync(string customerNo, decimal latitude, decimal longitude)
        {
            if (IsBusy) return;
            if (string.IsNullOrEmpty(customerNo)) return;

            try
            {
                IsBusy = true;

                bool isSuccess = await _customerMapService.UpdateCustomerLocationAsync(customerNo, latitude, longitude);
                if (isSuccess)
                {
                    await App.Current.MainPage.DisplayAlert("Success", "Customer location updated successfully.", "OK");
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Error", "Failed to update customer location.", "OK");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}


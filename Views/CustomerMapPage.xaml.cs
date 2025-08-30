using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using SampleMauiMvvmApp.ViewModels;

namespace SampleMauiMvvmApp.Views
{
    public partial class CustomerMapPage : ContentPage
    {
        private readonly CustomerMapViewModel _viewModel;

        public CustomerMapPage(CustomerMapViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Show action sheet for reading status
            string readingStatus = await Shell.Current.DisplayActionSheet(
                "Select Reading Status",
                "Cancel",
                null,
                "Captured",
                "Uncaptured",
                "All Readings");

            // If user cancels or closes the sheet, default to "All Readings"
            if (string.IsNullOrWhiteSpace(readingStatus) || readingStatus == "Cancel")
                readingStatus = "All Readings";

            await LoadPinsAsync(readingStatus);
        }

        private async Task LoadPinsAsync(string readingStatus)
        {
            // Load data into ViewModel
            await _viewModel.LoadCustomersAsync(readingStatus);

            customerMap.Pins.Clear();

            foreach (var customer in _viewModel.Customers)
            {
                if (customer.Latitude.HasValue && customer.Longitude.HasValue)
                {
                    var pin = new Pin
                    {
                        Label = $"Cust: {customer.CUSTOMER_NUMBER}",
                        Address = $"Meter: {customer.METER_NUMBER}",
                        Type = PinType.Place,
                        Location = new Microsoft.Maui.Devices.Sensors.Location(
                            (double)customer.Latitude.Value,
                            (double)customer.Longitude.Value)
                    };

                    customerMap.Pins.Add(pin);
                }
            }

            // Center map on the first customer with coordinates
            var firstCustomer = _viewModel.Customers.FirstOrDefault(c => c.Latitude.HasValue && c.Longitude.HasValue);
            if (firstCustomer != null)
            {
                customerMap.MoveToRegion(MapSpan.FromCenterAndRadius(
                    new Microsoft.Maui.Devices.Sensors.Location(
                        (double)firstCustomer.Latitude.Value,
                        (double)firstCustomer.Longitude.Value),
                    Distance.FromKilometers(1)));
            }
        }
    }
}

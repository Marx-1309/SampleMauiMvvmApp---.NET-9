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

            await _viewModel.LoadCustomersAsync();

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
                        Location = new Microsoft.Maui.Devices.Sensors.Location((double)customer.Latitude.Value, (double)customer.Longitude.Value)
                    };
                    customerMap.Pins.Add(pin);
                }
            }

            // Center map on first customer
            var firstCustomer = _viewModel.Customers.FirstOrDefault();
            if (firstCustomer != null && firstCustomer.Latitude.HasValue && firstCustomer.Longitude.HasValue)
            {
                customerMap.MoveToRegion(MapSpan.FromCenterAndRadius(
                    new Microsoft.Maui.Devices.Sensors.Location((double)firstCustomer.Latitude.Value, (double)firstCustomer.Longitude.Value),
                    Distance.FromKilometers(1)));
            }
        }
    }
}

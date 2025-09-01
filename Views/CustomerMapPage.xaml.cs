using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using SampleMauiMvvmApp.ViewModels;

namespace SampleMauiMvvmApp.Views
{
    [QueryProperty(nameof(CustomerNumber), "CustomerNumber")]
    public partial class CustomerMapPage : ContentPage
    {
        private readonly CustomerMapViewModel _viewModel;

        public string CustomerNumber { get; set; }

        private Pin? selectedPin;

        public CustomerMapPage(CustomerMapViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;

            // Handle user tapping on the map
            customerMap.MapClicked += OnMapClicked;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Try to get current location and center the map there first
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }

            if (status == PermissionStatus.Granted)
            {
                try
                {
                    var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                    var currentLocation = await Geolocation.GetLocationAsync(request);

                    if (currentLocation != null)
                    {
                        var mapSpan = MapSpan.FromCenterAndRadius(currentLocation, Distance.FromKilometers(1));
                        customerMap.MoveToRegion(mapSpan);
                    }
                }
                catch (Exception ex)
                {
                    // Handle case where location services are off or another error occurs
                    Console.WriteLine($"Error getting current location: {ex.Message}");
                }
            }

            // After attempting to center the map, load the customer-specific data
            if (!string.IsNullOrEmpty(CustomerNumber))
            {
                // Single-customer mode (manual location set)
                var customer = _viewModel.Customers.FirstOrDefault(c => c.CUSTOMER_NUMBER == CustomerNumber);
                if (customer != null && customer.Latitude.HasValue && customer.Longitude.HasValue)
                {
                    var location = new Microsoft.Maui.Devices.Sensors.Location(
                        (double)customer.Latitude.Value,
                        (double)customer.Longitude.Value);

                    // If we have a specific customer location, move the map to it
                    customerMap.MoveToRegion(MapSpan.FromCenterAndRadius(location, Distance.FromKilometers(1)));

                    // Drop existing pin
                    selectedPin = new Pin
                    {
                        Label = $"Cust: {customer.CUSTOMER_NUMBER}",
                        Address = $"Meter: {customer.METER_NUMBER}",
                        Type = PinType.Place,
                        Location = location
                    };
                    customerMap.Pins.Add(selectedPin);
                }
            }
            else
            {
                // Multi-customer view with filter
                string readingStatus = await Shell.Current.DisplayActionSheet(
                    "Select Reading Status",
                    "Cancel",
                    null,
                    "Captured",
                    "Uncaptured",
                    "All Readings");

                if (string.IsNullOrWhiteSpace(readingStatus) || readingStatus == "Cancel")
                    readingStatus = "All Readings";

                await LoadPinsAsync(readingStatus);
            }
        }

        private async Task LoadPinsAsync(string readingStatus)
        {
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

        private void OnMapClicked(object sender, MapClickedEventArgs e)
        {
            if (!string.IsNullOrEmpty(CustomerNumber))
            {
                // Clear existing pin
                if (selectedPin != null)
                    customerMap.Pins.Remove(selectedPin);

                // Add new pin where user tapped
                selectedPin = new Pin
                {
                    Label = $"Cust: {CustomerNumber}",
                    Type = PinType.Place,
                    Location = new Microsoft.Maui.Devices.Sensors.Location(e.Location.Latitude, e.Location.Longitude)
                };

                customerMap.Pins.Add(selectedPin);

                // TODO: Update DB or ViewModel with new coordinates
                _viewModel.SaveCustomerLocationAsync(CustomerNumber, (decimal)e.Location.Latitude, (decimal)e.Location.Longitude);
            }
        }
    }
}
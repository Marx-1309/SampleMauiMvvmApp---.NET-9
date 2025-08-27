﻿namespace SampleMauiMvvmApp.ViewModels
{
    public partial class MonthViewModel : BaseViewModel
    {
        public ObservableCollection<Month> Months { get; } = new();
        public ObservableCollection<Reading> listReadings { get; set; } = new ObservableCollection<Reading> { };
        public ObservableCollection<Customer> customer { get; set; } = new();

        [ObservableProperty]
        private bool isRefreshing;

        [ObservableProperty]
        private string myTitle;

        [ObservableProperty]
        public static int cMonth;

        [ObservableProperty]
        public static string sMonth;

        [ObservableProperty]
        private string syncTime;

        [ObservableProperty]
        public static decimal lastReadingByCustId;

        private string message = string.Empty;

        [ObservableProperty]
        private int currentYear;

        private MonthService monthService;
        private CustomerService customerService;
        private ReadingService readingService;
        private ReadingExportService readingExportService;
        private IConnectivity connectivity;

        public MonthViewModel(
            MonthService _monthService,
            ReadingExportService _readingExportService,
            IConnectivity _connectivity,
            ReadingService _readingService,
            CustomerService _customerService
            )
        {
            Title = "Readings By Month";
            connectivity = _connectivity;
            monthService = _monthService;
            readingService = _readingService;
            readingExportService = _readingExportService;
            customerService = _customerService;
        }

        [RelayCommand]
        public async Task<decimal> GetLastReadingByCustomerId(string customer)
        {
            var LastReadingByCustomerId = await readingService.GetLastReadingByIdAsync(customer);
            LastReadingByCustId = (decimal)LastReadingByCustomerId.CURRENT_READING;
            return LastReadingByCustId;
        }

        [RelayCommand]
        private async Task GetMonthsAsync()
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                await Task.Delay(500);
                var months = await monthService.GetListOfMonthsFromSqlite();
                if (Months.Count != 0)
                    Months.Clear();
                if (months == null)
                {
                    await Shell.Current.DisplayAlert("Error!", "Failed to fetch data", "OK");
                    return;
                }

                foreach (var month in months)
                {
                    month.IsActive = await monthService.IsMonthPopulated(month);
                    Months.Add(month);
                }
                ;
                if (Months.Count == 0)
                {
                    await Shell.Current.DisplayAlert("Error!", "Failed to fetch data", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to get months: {ex.Message}");
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        public async Task GoToListOfReadingsByMonth(Month monthId)
        {
            if (monthId.MonthID <= 0) return;
            var readings = await readingService.GetReadingsByMonthId(monthId.MonthID);
            foreach (var item in readings)
            {
                if (item.ReadingTaken == false)
                {
                    item.ReadingTaken = true;
                }
                ;
            }
            if (readings.Count == 0)
            {
                await Shell.Current.DisplayAlert("No Readings", $"No records found in {monthId.MonthName}", "OK");
                return;
            }
            CMonth = monthId.MonthID;
            SMonth = $"Successfully Synced for : {monthId.MonthName} ";
            SyncTime = System.DateTime.UtcNow.Hour + $":{System.DateTime.UtcNow.Minute}";
            listReadings.Clear();
            foreach (var reading in readings)
            {
                listReadings.Add(reading);
            }
            //listReadings.AddRange(readings);
            MyTitle = $"{monthId.MonthName}";
            await Shell.Current.GoToAsync(nameof(ListOfReadingByMonthPage), true, new Dictionary<string, object>
                            {
                                {"Month", monthId }
                            });
        }

        [RelayCommand]
        public async Task GoBackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        public async Task SyncByMonthIdAsync()
        {
            try
            {
                if (IsBusy) return;
                IsBusy = true;
                var response = await readingService.SyncReadingsByMonthIdAsync(CMonth);
                if (response > 0)
                {
                    await Shell.Current.DisplayAlert(
                                "Data Recycling Notice",
                                "Your data will be automatically recycled.\n\nClick OK to continue.",
                                "OK");

                    await readingExportService.FlushAndSeed();
                }
                message = readingService.StatusMessage;
                return;
            }
            catch
            {
                await Shell.Current.DisplayAlert($"{message} ,Unable to sync readings ", "Please Try again", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ShowAlert(string message)
        {
            await Shell.Current.DisplayAlert("Info", message, "Ok");
        }
    }
}
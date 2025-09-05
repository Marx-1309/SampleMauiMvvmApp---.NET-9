using CommunityToolkit.Maui.Core;
using SampleMauiMvvmApp.Services;
using System.Collections.ObjectModel;

namespace SampleMauiMvvmApp.ViewModels
{
    public partial class MenuViewModel : BaseViewModel
    {
        private readonly ReadingExportService _readingExportService;

        public ObservableCollection<SampleMauiMvvmApp.Models.Menu> Menus { get; }

        // Constructor using Dependency Injection
        public MenuViewModel(ReadingExportService readingExportService)
        {
            _readingExportService = readingExportService ?? throw new ArgumentNullException(nameof(readingExportService));

            Menus = new ObservableCollection<SampleMauiMvvmApp.Models.Menu>
            {
                new Models.Menu
                {
                    Name = "Abnormal Consumption",
                    Image = "abnormal_use_icon.png",
                    Label= "",
                    Url = "ExceptionReadingListPage",
                    IsActive=false
                },
                new Models.Menu
                {
                    Name = "My Notes",
                    Image = "notes_icon.png",
                    Label= "",
                    Url = "NotesListPage",
                    IsActive=false
                },
                new Models.Menu
                {
                    Name = "Scan For New Customer(s)",
                    Image = "scan_db_icon.png",
                    Label= "",
                    Url = "SyncNewCustomersPage",
                    IsActive=true
                },
                new Models.Menu
                {
                    Name = "Recycle Readings",
                    Image = "export_sync.png",
                    Label= "",
                    Url = "ReflushPage",
                    IsActive=true
                },
                new Models.Menu
                {
                    Name = "Integrated Services",
                    Image = "cloud_intergration.png",
                    Label= "",
                    Url = "ReflushPage",
                    IsActive=true
                },
                new Models.Menu
                {
                    Name = "Google Maps",
                    Image = "map_icon3.jpg",
                    Label= "",
                    Url = "CustomerMapPage",
                    IsActive=true
                }
            };
        }

        #region Snackbar / Toast

        public Snackbar SnackBar(string text = "This is a Snackbar")
        {
            var snackbarOptions = new SnackbarOptions
            {
                BackgroundColor = Colors.Red,
                TextColor = Colors.Green,
                ActionButtonTextColor = Colors.Yellow,
                CornerRadius = new CornerRadius(10),
                Font = Microsoft.Maui.Font.SystemFontOfSize(14),
                ActionButtonFont = Microsoft.Maui.Font.SystemFontOfSize(14),
                CharacterSpacing = 0.5
            };

            Action action = async () =>
            {
                await Shell.Current.DisplayAlert("Notice", "You triggered the snackbar action", "OK");
            };

            TimeSpan duration = TimeSpan.FromSeconds(5);

            return (Snackbar)Snackbar.Make(text, action, "Dismiss", duration, snackbarOptions);
        }

        #endregion

        #region Commands

        [RelayCommand]
        private async Task GoToDetails(Models.Menu menu)
        {
            if (menu == null) return;

            try
            {
                if (menu.Name == "Integrated Services")
                {
                    var response = await AppShell.Current.DisplayActionSheet("Select Option", "Cancel", null, "CityTaps", "Others");

                    if (response == "CityTaps")
                    {
                        DisplayToast("This service is not available");
                        return;
                    }
                    else
                    {
                        DisplayToast("This service is not available");
                        return;
                    }
                }

                if (!string.IsNullOrEmpty(menu.Url))
                    await Shell.Current.GoToAsync(menu.Url);
            }
            catch (Exception ex)
            {
                // Optionally log exception
                DisplayToast($"Error navigating: {ex.Message}");
            }
        }

        [RelayCommand]
        public async Task ConfirmLogout()
        {
            var pendingCount = await _readingExportService.PendingNotSyncedReadings();

            if (pendingCount > 0)
            {
                await Shell.Current.DisplayAlert(
                    $"{pendingCount} reading(s) not uploaded!",
                    "Please sync the pending readings, and try again!",
                    "OK");
                return;
            }

            bool isConfirm = await Shell.Current.DisplayAlert(
                "Logout or switch users",
                $"You are about to logout of {Preferences.Default.Get("username", "user")} profile",
                "OK", "Cancel");

            if (!isConfirm) return;

            try
            {
                IsBusy = true;
                await Task.Delay(TimeSpan.FromSeconds(1));

                SecureStorage.Remove("Token");
                Preferences.Default.Clear();

                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion
    }
}

namespace SampleMauiMvvmApp.ViewModels
{
    public partial class BaseViewModel : ObservableObject
    {
        public BaseViewModel()
        {
        }

        [ObservableProperty]
        private string title;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        private bool isBusy;

        public bool IsNotBusy => !IsBusy;

        [ObservableProperty]
        private string statusMessage;

        private string tstMsg = "You Can Proceed Using The App! ";

        public void DisplayToast(string tstMsg)
        {
            Toast.Make(tstMsg, CommunityToolkit.Maui.Core.ToastDuration.Long, 10).Show();
        }
    }
}
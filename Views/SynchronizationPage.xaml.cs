namespace SampleMauiMvvmApp.Views;

[QueryProperty("Refresh", "Refresh")]
public partial class SynchronizationPage : ContentPage
{
    private ReadingViewModel viewModel;

    public SynchronizationPage(ReadingViewModel _viewModel)
    {
        InitializeComponent();
        viewModel = _viewModel;
        BindingContext = _viewModel;
    }
}
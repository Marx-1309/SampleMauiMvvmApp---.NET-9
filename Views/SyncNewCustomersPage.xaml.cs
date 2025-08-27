namespace SampleMauiMvvmApp.Views;

public partial class SyncNewCustomersPage : ContentPage
{
    private ReadingViewModel viewModel;

    public SyncNewCustomersPage(ReadingViewModel _viewModel)
    {
        InitializeComponent();
        viewModel = _viewModel;
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        viewModel.ScanForNewlyAddedCustomerReadingsCommand.Execute(null);
    }
}
namespace SampleMauiMvvmApp.Views;

public partial class CustomerDetailPage : ContentPage
{
    private CustomerDetailViewModel _viewModel;

    public CustomerDetailPage(CustomerDetailViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;

        ReadingFaker readingFaker = new();
        _viewModel.VmReading = new ReadingWrapper(readingFaker.Generate());
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.CustDisplayDetailsCommand.Execute(null);
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
    }
}
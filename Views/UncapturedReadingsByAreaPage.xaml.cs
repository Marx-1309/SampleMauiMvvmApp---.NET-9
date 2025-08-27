namespace SampleMauiMvvmApp.Views;

public partial class UncapturedReadingsByAreaPage : ContentPage
{
    private ReadingViewModel _viewModel;

    public UncapturedReadingsByAreaPage(ReadingViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        this.BindingContext = viewModel;
    }

    //protected override void OnAppearing()
    //{
    //    base.OnAppearing();
    //    _viewModel.GoToListOfUncapturedReadingsByAreaCommand.Execute(null);
    //}
}
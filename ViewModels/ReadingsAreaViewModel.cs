namespace SampleMauiMvvmApp.ViewModels
{
    [QueryProperty("Readings", "Readings")]
    public partial class ReadingsAreaViewModel : BaseViewModel
    {
        private DbContext dbContext;
        private ReadingService readingService;
        private MonthService monthService;
        private CustomerService customerService;

        [ObservableProperty]
        private List<ReadingWrapper> reading;

        public ReadingsAreaViewModel(DbContext _dbContext, ReadingService readingService,
            CustomerService _customerService, MonthService _monthService, IGeolocation geolocation)
        {
            Title = "Customer Detail Page";
            this.dbContext = _dbContext;
            this.readingService = readingService;
            this.customerService = _customerService;
            this.monthService = _monthService;
        }
    }
}
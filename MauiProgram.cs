namespace SampleMauiMvvmApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseSkiaSharp()
            .UseMauiCommunityToolkit()
            .UseMauiMaps()
            //.UseLocalNotification()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
#if DEBUG
        builder.Logging.AddDebug();
#endif
        builder.Services.AddSingleton(Connectivity.Current);
        builder.Services.AddSingleton(Geolocation.Default);
        builder.Services.AddSingleton(Map.Default);
        builder.Services.AddTransient<BaseService>();
        builder.Services.AddSingleton<NotesService>();
        builder.Services.AddSingleton<DbContext>();
        builder.Services.AddSingleton<CustomerService>();
        builder.Services.AddTransient<ReadingService>();
        builder.Services.AddSingleton<ReadingExportService>();
        builder.Services.AddSingleton<MonthService>();
        builder.Services.AddSingleton<CustomerMapService>();
        builder.Services.AddSingleton<AuthenticationService>();
        builder.Services.AddTransient<AppShell>();

        builder.Services.AddSingleton<OnboardingPage>();
        builder.Services.AddSingleton<LoadingPage>();
        builder.Services.AddSingleton<SynchronizationPage>();
        builder.Services.AddSingleton<SyncNewCustomersPage>();
        builder.Services.AddSingleton<OnboardingViewModel>();
        builder.Services.AddTransient<ReflushPage>();
        builder.Services.AddTransient<NotesDetailsPage>();
        builder.Services.AddSingleton<NotesListPage>();
        builder.Services.AddSingleton<LoginPage>();
        builder.Services.AddSingleton<LogoutPage>();
        builder.Services.AddSingleton<ListOfReadingByMonthPage>();
        builder.Services.AddSingleton<MonthCustomerTabPage>();
        builder.Services.AddTransient<UncapturedReadingsPage>();
        builder.Services.AddSingleton<MonthPage>();
        builder.Services.AddTransientWithShellRoute<ExceptionReadingListPage, ReadingViewModel>(nameof(ExceptionReadingListPage));
        builder.Services.AddTransient<CustomerDetailPage>();
        builder.Services.AddTransient<CapturedReadingsPage>();
        builder.Services.AddTransient<LocationPage>();
        builder.Services.AddScoped<UncapturedReadingsByAreaPage>();
        builder.Services.AddTransient<CustomerMapPage>();

        builder.Services.AddSingleton<LoginViewModel>();
        builder.Services.AddSingleton<LoadingViewModel>();
        builder.Services.AddSingleton<LogoutViewModel>();
        builder.Services.AddSingleton<ReadingViewModel>();
        builder.Services.AddTransient<NotesViewModel>();
        builder.Services.AddSingleton<MonthViewModel>();
        builder.Services.AddScoped<CustomerDetailViewModel>();
        builder.Services.AddSingleton<CustomerViewModel>();
        builder.Services.AddTransient<CustomerMapViewModel>();

        builder.Services.AddAutoMapper(typeof(ClassDtoMapping));

        return builder.Build();
    }
}
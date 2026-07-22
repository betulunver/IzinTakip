using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Markup;
using IzinTakip.Services;
using IzinTakip.ViewModels;
using IzinTakip.Views;
using Microsoft.Extensions.Logging;

namespace IzinTakip;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseMauiCommunityToolkitMarkup()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Services
        builder.Services.AddSingleton<IApiService, ApiService>();
        builder.Services.AddSingleton<IAuthService, AuthService>();

        // ViewModels
        builder.Services.AddTransient<EmailVerificationViewModel>();
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<LeaveRequestViewModel>();
        builder.Services.AddTransient<LeaveListViewModel>();
        builder.Services.AddTransient<LeaveSummaryViewModel>();
        builder.Services.AddTransient<SuccessViewModel>();

        // Pages
        builder.Services.AddTransient<EmailVerificationPage>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<LeaveRequestPage>();
        builder.Services.AddTransient<LeaveListPage>();
        builder.Services.AddTransient<LeaveSummaryPage>();
        builder.Services.AddTransient<SuccessPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}

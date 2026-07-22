using IzinTakip.Services;
using IzinTakip.Views;

namespace IzinTakip;

public partial class AppShell : Shell
{
    private readonly IAuthService _authService;

    public AppShell(IAuthService authService)
    {
        InitializeComponent();
        _authService = authService;

        Routing.RegisterRoute("EmailVerificationPage", typeof(EmailVerificationPage));
        Routing.RegisterRoute("LeaveSummaryPage", typeof(LeaveSummaryPage));
        Routing.RegisterRoute("SuccessPage", typeof(SuccessPage));
    }

    protected override async void OnNavigated(ShellNavigatedEventArgs args)
    {
        base.OnNavigated(args);

        if (args.Source == ShellNavigationSource.ShellItemChanged)
            return;

        var isLoggedIn = await _authService.IsLoggedInAsync();
        var currentRoute = Current.CurrentState.Location.ToString();

        if (isLoggedIn && currentRoute.Contains("LoginPage"))
        {
            await GoToAsync("//MainPage");
        }
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IzinTakip.Services;

namespace IzinTakip.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    private readonly IApiService _apiService;
    private readonly IAuthService _authService;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _otpCode = string.Empty;

    [ObservableProperty]
    private bool _isOtpSent;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private bool _statusIsError;

    private const int ResendCooldownSeconds = 60;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SendOtpCommand))]
    private bool _canResend = true;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ResendCountdownText))]
    private int _resendSecondsRemaining;

    private CancellationTokenSource? _resendCts;

    public string ResendCountdownText => $"Kodu tekrar gönder · 0:{ResendSecondsRemaining:D2}";

    public LoginViewModel(IApiService apiService, IAuthService authService)
    {
        _apiService = apiService;
        _authService = authService;
        Title = "Giriş Yap";
    }

    [RelayCommand(CanExecute = nameof(CanResend))]
    private async Task SendOtp()
    {
        if (string.IsNullOrWhiteSpace(Email))
        {
            StatusIsError = true;
            StatusMessage = "Lütfen e-posta adresinizi girin.";
            return;
        }

        IsBusy = true;
        StatusMessage = string.Empty;

        try
        {
            var result = await _apiService.LoginOtpAsync(Email.Trim().ToLower());
            if (result.Success)
            {
                IsOtpSent = true;
                StatusIsError = false;
                StatusMessage = "Giriş kodu e-posta adresinize gönderildi.";
                StartResendCountdown();
            }
            else
            {
                StatusIsError = true;
                StatusMessage = result.Message;
            }
        }
        catch (Exception ex)
        {
            StatusIsError = true;
            StatusMessage = $"Hata: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void StartResendCountdown()
    {
        _resendCts?.Cancel();
        var cts = new CancellationTokenSource();
        _resendCts = cts;

        CanResend = false;
        ResendSecondsRemaining = ResendCooldownSeconds;

        _ = RunResendCountdownAsync(cts.Token);
    }

    private async Task RunResendCountdownAsync(CancellationToken token)
    {
        try
        {
            while (ResendSecondsRemaining > 0)
            {
                await Task.Delay(1000, token);
                ResendSecondsRemaining--;
            }
            CanResend = true;
        }
        catch (TaskCanceledException)
        {
        }
    }

    [RelayCommand]
    private async Task ConfirmOtp()
    {
        if (string.IsNullOrWhiteSpace(OtpCode))
        {
            StatusIsError = true;
            StatusMessage = "Lütfen giriş kodunu girin.";
            return;
        }

        IsBusy = true;
        StatusMessage = string.Empty;

        try
        {
            var result = await _apiService.ConfirmOtpAsync(Email.Trim().ToLower(), OtpCode.Trim());
            if (result.Success && result.Data != null)
            {
                await _authService.SaveSessionAsync(result.Data);
                await Shell.Current.GoToAsync("//MainPage");
            }
            else
            {
                StatusIsError = true;
                StatusMessage = result.Message;
            }
        }
        catch (Exception ex)
        {
            StatusIsError = true;
            StatusMessage = $"Hata: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task GoToVerification()
    {
        await Shell.Current.GoToAsync("EmailVerificationPage");
    }
}

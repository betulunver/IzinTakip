using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IzinTakip.Services;

namespace IzinTakip.ViewModels;

public partial class EmailVerificationViewModel : BaseViewModel
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _verificationCode = string.Empty;

    [ObservableProperty]
    private bool _isCodeSent;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private bool _statusIsError;

    private const int ResendCooldownSeconds = 60;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SendVerificationCodeCommand))]
    private bool _canResend = true;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ResendCountdownText))]
    private int _resendSecondsRemaining;

    private CancellationTokenSource? _resendCts;

    public string ResendCountdownText => $"Kodu tekrar gönder · 0:{ResendSecondsRemaining:D2}";

    public EmailVerificationViewModel(IApiService apiService)
    {
        _apiService = apiService;
        Title = "E-Posta Doğrulama";
    }

    [RelayCommand(CanExecute = nameof(CanResend))]
    private async Task SendVerificationCode()
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
            var result = await _apiService.VerifyEmailAsync(Email.Trim().ToLower());
            if (result.Success)
            {
                IsCodeSent = true;
                StatusIsError = false;
                StatusMessage = "Doğrulama kodu e-posta adresinize gönderildi.";
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
    private async Task ConfirmVerification()
    {
        if (string.IsNullOrWhiteSpace(VerificationCode))
        {
            StatusIsError = true;
            StatusMessage = "Lütfen doğrulama kodunu girin.";
            return;
        }

        IsBusy = true;
        StatusMessage = string.Empty;

        try
        {
            var result = await _apiService.ConfirmCodeAsync(Email.Trim().ToLower(), VerificationCode.Trim());
            if (result.Success)
            {
                StatusIsError = false;
                StatusMessage = "E-posta doğrulaması başarılı! Giriş yapabilirsiniz.";
                await Task.Delay(1500);
                await Shell.Current.GoToAsync("..");
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
}

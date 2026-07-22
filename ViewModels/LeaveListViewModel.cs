using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IzinTakip.Models;
using IzinTakip.Services;

namespace IzinTakip.ViewModels;

public partial class LeaveListViewModel : BaseViewModel
{
    private readonly IApiService _apiService;
    private readonly IAuthService _authService;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private string _userName = string.Empty;

    [ObservableProperty]
    private string _userBirim = string.Empty;

    [ObservableProperty]
    private string _initials = string.Empty;

    public ObservableCollection<LeaveRequest> Leaves { get; } = new();

    public LeaveListViewModel(IApiService apiService, IAuthService authService)
    {
        _apiService = apiService;
        _authService = authService;
        Title = "İzinlerim";
    }

    [RelayCommand]
    private async Task LoadLeaves()
    {
        IsBusy = true;
        StatusMessage = string.Empty;

        try
        {
            var user = await _authService.GetCurrentUserAsync();
            if (user == null) return;

            UserName = user.AdSoyad;
            UserBirim = "Proje Geliştirme Yönetimi";
            Initials = string.Join("", user.AdSoyad.Split(' ').Where(s => s.Length > 0).Select(s => s[0]));

            var result = await _apiService.GetLeavesAsync(user.Email);
            if (result.Success && result.Data != null)
            {
                Leaves.Clear();
                foreach (var leave in Enumerable.Reverse(result.Data))
                    Leaves.Add(leave);
            }
            else
            {
                StatusMessage = result.Message;
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Hata: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task Logout()
    {
        var confirmed = await Shell.Current.DisplayAlert(
            "Oturumu Kapat",
            "Oturumunuz kapatılacaktır. Emin misiniz?",
            "Evet, Çıkış Yap",
            "Vazgeç");

        if (!confirmed) return;

        await _authService.ClearSessionAsync();
        await Shell.Current.GoToAsync("//LoginPage");
    }

    [RelayCommand]
    private async Task GoToNewLeave()
    {
        await Shell.Current.GoToAsync("//MainPage/LeaveRequestPage");
    }
}

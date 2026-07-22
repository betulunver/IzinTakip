using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IzinTakip.Models;
using IzinTakip.Services;

namespace IzinTakip.ViewModels;

public partial class LeaveRequestViewModel : BaseViewModel
{
    private readonly IApiService _apiService;
    private readonly IAuthService _authService;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _adSoyad = string.Empty;

    private const string SabitBirim = "Proje Geliştirme Yönetimi";

    [ObservableProperty]
    private string _selectedIzinSuresi = string.Empty;

    [ObservableProperty]
    private DateTime _baslangicTarihi = DateTime.Today;

    [ObservableProperty]
    private DateTime _bitisTarihi = DateTime.Today;

    [ObservableProperty]
    private DateTime _iseDonus = DateTime.Today.AddDays(1);

    [ObservableProperty]
    private string _aciklama = string.Empty;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    public List<string> IzinSureleri { get; } = Enumerable.Range(1, 50)
        .Select(gun => $"{gun} gün")
        .ToList();

    public LeaveRequestViewModel(IApiService apiService, IAuthService authService)
    {
        _apiService = apiService;
        _authService = authService;
        Title = "Yeni İzin Talebi";
    }

    [RelayCommand]
    private async Task LoadUser()
    {
        var user = await _authService.GetCurrentUserAsync();
        if (user != null)
        {
            Email = user.Email;
            AdSoyad = user.AdSoyad;
        }
    }

    [RelayCommand]
    private async Task SubmitLeave()
    {
        if (string.IsNullOrWhiteSpace(SelectedIzinSuresi))
        {
            StatusMessage = "Lütfen izin süresi seçin.";
            return;
        }
        if (BitisTarihi < BaslangicTarihi)
        {
            StatusMessage = "Bitiş tarihi başlangıç tarihinden önce olamaz.";
            return;
        }
        if (IseDonus <= BitisTarihi)
        {
            StatusMessage = "Göreve başlama tarihi bitiş tarihinden sonra olmalıdır.";
            return;
        }

        var leave = new LeaveRequest
        {
            Email = Email,
            AdSoyad = AdSoyad,
            Birim = SabitBirim,
            IzinSuresi = SelectedIzinSuresi,
            BaslangicTarihi = BaslangicTarihi.ToString("yyyy-MM-dd"),
            BitisTarihi = BitisTarihi.ToString("yyyy-MM-dd"),
            IseDonus = IseDonus.ToString("yyyy-MM-dd"),
            Aciklama = Aciklama
        };

        var navParam = new Dictionary<string, object> { { "LeaveData", leave } };
        await Shell.Current.GoToAsync("LeaveSummaryPage", navParam);
    }

    [RelayCommand]
    private async Task Logout()
    {
        await _authService.ClearSessionAsync();
        await Shell.Current.GoToAsync("//LoginPage");
    }

    [RelayCommand]
    private async Task GoToLeaveList()
    {
        await Shell.Current.GoToAsync("//MainPage/LeaveListPage");
    }
}

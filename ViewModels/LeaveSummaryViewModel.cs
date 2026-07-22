using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IzinTakip.Models;
using IzinTakip.Services;

namespace IzinTakip.ViewModels;

[QueryProperty(nameof(LeaveData), "LeaveData")]
public partial class LeaveSummaryViewModel : BaseViewModel
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private LeaveRequest _leaveData = new();

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    public LeaveSummaryViewModel(IApiService apiService)
    {
        _apiService = apiService;
        Title = "Talebi Onayla";
    }

    [RelayCommand]
    private async Task ConfirmAndSubmit()
    {
        IsBusy = true;
        StatusMessage = string.Empty;

        try
        {
            var result = await _apiService.SubmitLeaveAsync(LeaveData);
            if (result.Success)
            {
                await Shell.Current.GoToAsync($"SuccessPage?StartDate={LeaveData.BaslangicTarihi}&EndDate={LeaveData.BitisTarihi}&Days={LeaveData.IzinSuresi}");
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
    private async Task GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IzinTakip.Converters;

namespace IzinTakip.ViewModels;

[QueryProperty(nameof(StartDate), "StartDate")]
[QueryProperty(nameof(EndDate), "EndDate")]
[QueryProperty(nameof(Days), "Days")]
public partial class SuccessViewModel : BaseViewModel
{
    [ObservableProperty]
    private string _startDate = string.Empty;

    [ObservableProperty]
    private string _endDate = string.Empty;

    [ObservableProperty]
    private string _days = string.Empty;

    public string SummaryText =>
        $"{DateDisplayConverter.Format(StartDate)} – {DateDisplayConverter.Format(EndDate)} tarihleri için {Days} izin talebiniz amir onayına gönderildi.";

    public SuccessViewModel()
    {
        Title = "Talep Alındı";
    }

    partial void OnStartDateChanged(string value) => OnPropertyChanged(nameof(SummaryText));
    partial void OnEndDateChanged(string value) => OnPropertyChanged(nameof(SummaryText));
    partial void OnDaysChanged(string value) => OnPropertyChanged(nameof(SummaryText));

    [RelayCommand]
    private async Task GoToLeaves()
    {
        await Shell.Current.GoToAsync("//MainPage");
    }
}

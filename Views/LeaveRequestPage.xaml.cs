using CommunityToolkit.Maui.Markup;
using IzinTakip.Converters;
using IzinTakip.ViewModels;
using Microsoft.Maui.Controls.Shapes;

namespace IzinTakip.Views;

public class LeaveRequestPage : ContentPage
{
    private readonly LeaveRequestViewModel _vm;

    public LeaveRequestPage(LeaveRequestViewModel vm)
    {
        BindingContext = _vm = vm;
        Shell.SetNavBarIsVisible(this, false);
        BackgroundColor = AppColors.AppBackground;

        var invertedBool = new InvertedBoolConverter();

        // Geri oku
        var backArrow = new Label
        {
            Text = "←",
            FontSize = 22,
            TextColor = AppColors.TextWhite,
            VerticalOptions = LayoutOptions.Center
        };
        backArrow.GestureRecognizers.Add(
            new TapGestureRecognizer().Bind(TapGestureRecognizer.CommandProperty, nameof(vm.GoToLeaveListCommand))
        );

        Content = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition(GridLength.Star),
                new RowDefinition(GridLength.Auto)
            },
            Children =
            {
                // Kaydırılabilir form
                new ScrollView
                {
                    Content = new VerticalStackLayout
                    {
                        Padding = new Thickness(20, 16),
                        Spacing = 14,
                        Children =
                        {
                            // Başlık
                            new HorizontalStackLayout
                            {
                                Spacing = 12,
                                Margin = new Thickness(0, 8, 0, 4),
                                Children =
                                {
                                    backArrow,
                                    new Label
                                    {
                                        Text = "Yeni İzin Talebi",
                                        FontSize = 18,
                                        FontAttributes = FontAttributes.Bold,
                                        TextColor = AppColors.TextWhite,
                                        VerticalOptions = LayoutOptions.Center
                                    }
                                }
                            },

                            // Kimlik kartı
                            new Border
                            {
                                StrokeShape = new RoundRectangle { CornerRadius = 14 },
                                BackgroundColor = Color.FromArgb("#1a2a3a"),
                                Stroke = new SolidColorBrush(AppColors.InputBorder),
                                StrokeThickness = 1,
                                Padding = new Thickness(16, 12),
                                Content = new VerticalStackLayout
                                {
                                    Spacing = 6,
                                    Children =
                                    {
                                        new HorizontalStackLayout
                                        {
                                            Spacing = 8,
                                            Children =
                                            {
                                                new Label { Text = "✉", FontSize = 14, TextColor = AppColors.Primary, VerticalOptions = LayoutOptions.Center },
                                                new Label { FontSize = 13, TextColor = AppColors.TextLight, VerticalOptions = LayoutOptions.Center }
                                                    .Bind(Label.TextProperty, nameof(vm.Email))
                                            }
                                        },
                                        new BoxView { HeightRequest = 1, Color = AppColors.InputBorder },
                                        new HorizontalStackLayout
                                        {
                                            Spacing = 8,
                                            Children =
                                            {
                                                new Label { Text = "👤", FontSize = 14, VerticalOptions = LayoutOptions.Center },
                                                new Label { FontSize = 14, FontAttributes = FontAttributes.Bold, TextColor = AppColors.TextWhite, VerticalOptions = LayoutOptions.Center }
                                                    .Bind(Label.TextProperty, nameof(vm.AdSoyad)),
                                                new Label { Text = "★", FontSize = 14, TextColor = AppColors.Primary, VerticalOptions = LayoutOptions.Center }
                                            }
                                        }
                                    }
                                }
                            },

                            // Başlangıç Tarihi
                            FieldLabel("Başlangıç Tarihi *"),
                            DateRow(nameof(vm.BaslangicTarihi), activeBorder: true),

                            // Bitiş Tarihi
                            FieldLabel("Bitiş Tarihi *"),
                            DateRow(nameof(vm.BitisTarihi)),

                            // Göreve Başlama Tarihi
                            FieldLabel("Göreve Başlama Tarihi *"),
                            DateRow(nameof(vm.IseDonus)),

                            // İzin Süresi
                            FieldLabel("İzin Süresi *"),
                            new Border
                            {
                                StrokeShape = new RoundRectangle { CornerRadius = 14 },
                                BackgroundColor = AppColors.InputBackground,
                                Stroke = new SolidColorBrush(AppColors.InputBorder),
                                StrokeThickness = 1,
                                Padding = new Thickness(14, 0),
                                HeightRequest = 48,
                                Content = new Picker
                                {
                                    Title = "İzin süresi seçin",
                                    TextColor = AppColors.TextWhite,
                                    TitleColor = AppColors.TextMuted,
                                    FontSize = 13,
                                    BackgroundColor = Colors.Transparent
                                }
                                .Bind(Picker.ItemsSourceProperty, nameof(vm.IzinSureleri))
                                .Bind(Picker.SelectedItemProperty, nameof(vm.SelectedIzinSuresi))
                            },

                            // Açıklama
                            FieldLabel("Açıklama / Not"),
                            new Border
                            {
                                StrokeShape = new RoundRectangle { CornerRadius = 14 },
                                BackgroundColor = AppColors.InputBackground,
                                Stroke = new SolidColorBrush(AppColors.InputBorder),
                                StrokeThickness = 1,
                                Padding = new Thickness(14, 8),
                                Content = new Editor
                                {
                                    Placeholder = "İzin talebinizle ilgili eklemek istedikleriniz…",
                                    PlaceholderColor = AppColors.TextMuted,
                                    TextColor = AppColors.TextWhite,
                                    FontSize = 13,
                                    HeightRequest = 80,
                                    AutoSize = EditorAutoSizeOption.TextChanges,
                                    BackgroundColor = Colors.Transparent
                                }
                                .Bind(Editor.TextProperty, nameof(vm.Aciklama))
                            },

                            // Uyarı
                            new HorizontalStackLayout
                            {
                                Spacing = 8,
                                Margin = new Thickness(0, 4, 0, 0),
                                Children =
                                {
                                    new Label { Text = "ℹ", FontSize = 13, TextColor = AppColors.TextMuted, VerticalOptions = LayoutOptions.Start },
                                    new Label
                                    {
                                        Text = "Aynı veya çakışan tarihler için ikinci bir talep oluşturulamaz.",
                                        FontSize = 11,
                                        TextColor = AppColors.TextMuted
                                    }
                                }
                            },

                            new ActivityIndicator { Color = AppColors.Primary }
                                .Bind(ActivityIndicator.IsRunningProperty, nameof(vm.IsBusy))
                                .Bind(ActivityIndicator.IsVisibleProperty, nameof(vm.IsBusy)),

                            new Label
                            {
                                FontSize = 13,
                                HorizontalOptions = LayoutOptions.Center,
                                HorizontalTextAlignment = TextAlignment.Center,
                                TextColor = AppColors.ErrorRed
                            }
                            .Bind(Label.TextProperty, nameof(vm.StatusMessage)),

                            new BoxView { HeightRequest = 70, Color = Colors.Transparent }
                        }
                    }
                }.Row(0),

                // Sabit alt buton
                new Border
                {
                    BackgroundColor = AppColors.AppBackground,
                    Padding = new Thickness(20, 12),
                    StrokeThickness = 0,
                    Content = new Button
                    {
                        Text = "💾  Kaydet",
                        BackgroundColor = AppColors.Primary,
                        TextColor = AppColors.AppBackground,
                        CornerRadius = 14,
                        HeightRequest = 52,
                        FontSize = 15,
                        FontAttributes = FontAttributes.Bold
                    }
                    .Bind(Button.CommandProperty, nameof(vm.SubmitLeaveCommand))
                    .Bind(Button.IsEnabledProperty, nameof(vm.IsBusy), converter: invertedBool)
                }.Row(1)
            }
        };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadUserCommand.ExecuteAsync(null);
    }

    static Label FieldLabel(string text) => new()
    {
        Text = text,
        FontSize = 11,
        FontAttributes = FontAttributes.Bold,
        TextColor = AppColors.TextMuted,
        CharacterSpacing = 1.2
    };

    static Border DateRow(string bindingPath, bool activeBorder = false) => new Border
    {
        StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 14 },
        BackgroundColor = AppColors.InputBackground,
        Stroke = new SolidColorBrush(activeBorder ? AppColors.Primary : AppColors.InputBorder),
        StrokeThickness = 1,
        Padding = new Thickness(14, 0),
        HeightRequest = 48,
        Content = new HorizontalStackLayout
        {
            Spacing = 10,
            Children =
            {
                new Label { Text = "📅", FontSize = 14, TextColor = AppColors.Primary, VerticalOptions = LayoutOptions.Center },
                new DatePicker
                {
                    Format = "dd MMMM yyyy",
                    TextColor = AppColors.TextWhite,
                    FontSize = 13,
                    BackgroundColor = Colors.Transparent,
                    VerticalOptions = LayoutOptions.Center
                }
                .Bind(DatePicker.DateProperty, bindingPath)
            }
        }
    };
}

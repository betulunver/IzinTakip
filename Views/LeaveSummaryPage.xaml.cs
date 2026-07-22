using CommunityToolkit.Maui.Markup;
using IzinTakip.Converters;
using IzinTakip.ViewModels;
using Microsoft.Maui.Controls.Shapes;

namespace IzinTakip.Views;

public class LeaveSummaryPage : ContentPage
{
    public LeaveSummaryPage(LeaveSummaryViewModel vm)
    {
        BindingContext = vm;
        Shell.SetNavBarIsVisible(this, false);
        BackgroundColor = AppColors.AppBackground;

        var invertedBool = new InvertedBoolConverter();
        var isNotNullOrEmpty = new IsNotNullOrEmptyConverter();
        var dateDisplay = new DateDisplayConverter();

        var backArrow = new Label
        {
            Text = "←",
            FontSize = 22,
            TextColor = AppColors.TextWhite,
            VerticalOptions = LayoutOptions.Center
        };
        backArrow.GestureRecognizers.Add(
            new TapGestureRecognizer().Bind(TapGestureRecognizer.CommandProperty, nameof(vm.GoBackCommand))
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
                new ScrollView
                {
                    Content = new VerticalStackLayout
                    {
                        Padding = new Thickness(20, 16),
                        Spacing = 16,
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
                                        Text = "Talebi Onayla",
                                        FontSize = 18,
                                        FontAttributes = FontAttributes.Bold,
                                        TextColor = AppColors.TextWhite,
                                        VerticalOptions = LayoutOptions.Center
                                    }
                                }
                            },

                            new Label
                            {
                                Text = "Göndermeden önce bilgileri kontrol edin.",
                                FontSize = 13,
                                TextColor = AppColors.TextLight
                            },

                            // Özet kartı
                            new Border
                            {
                                StrokeShape = new RoundRectangle { CornerRadius = 14 },
                                BackgroundColor = AppColors.CardBackground,
                                Stroke = new SolidColorBrush(AppColors.InputBorder),
                                StrokeThickness = 1,
                                Padding = new Thickness(18, 14),
                                Content = new VerticalStackLayout
                                {
                                    Spacing = 12,
                                    Children =
                                    {
                                        SummaryRow("Ad Soyad",       $"{nameof(vm.LeaveData)}.AdSoyad"),
                                        new BoxView { HeightRequest = 1, Color = AppColors.InputBorder },
                                        SummaryRow("Birim",          $"{nameof(vm.LeaveData)}.Birim"),
                                        new BoxView { HeightRequest = 1, Color = AppColors.InputBorder },
                                        SummaryRow("Başlangıç",      $"{nameof(vm.LeaveData)}.BaslangicTarihi", converter: dateDisplay),
                                        new BoxView { HeightRequest = 1, Color = AppColors.InputBorder },
                                        SummaryRow("Bitiş",          $"{nameof(vm.LeaveData)}.BitisTarihi", converter: dateDisplay),
                                        new BoxView { HeightRequest = 1, Color = AppColors.InputBorder },
                                        SummaryRow("Göreve Başlama", $"{nameof(vm.LeaveData)}.IseDonus", converter: dateDisplay),
                                        new BoxView { HeightRequest = 1, Color = AppColors.InputBorder },
                                        SummaryRow("İzin Süresi",    $"{nameof(vm.LeaveData)}.IzinSuresi", AppColors.Primary)
                                    }
                                }
                            },

                            // Açıklama kartı (sadece açıklama varsa görünür)
                            new Border
                            {
                                StrokeShape = new RoundRectangle { CornerRadius = 14 },
                                BackgroundColor = AppColors.CardBackground,
                                Stroke = new SolidColorBrush(AppColors.InputBorder),
                                StrokeThickness = 1,
                                Padding = new Thickness(18, 14),
                                Content = new VerticalStackLayout
                                {
                                    Spacing = 6,
                                    Children =
                                    {
                                        new Label
                                        {
                                            Text = "Açıklama / Not",
                                            FontSize = 11,
                                            TextColor = AppColors.TextMuted,
                                            CharacterSpacing = 1.2
                                        },
                                        new Label { FontSize = 13, TextColor = AppColors.TextLight }
                                            .Bind(Label.TextProperty, $"{nameof(vm.LeaveData)}.Aciklama")
                                    }
                                }
                            }
                            .Bind(Border.IsVisibleProperty, $"{nameof(vm.LeaveData)}.Aciklama", converter: isNotNullOrEmpty),

                            // Onay notu
                            new Grid
                            {
                                ColumnDefinitions =
                                {
                                    new ColumnDefinition(GridLength.Auto),
                                    new ColumnDefinition(GridLength.Star)
                                },
                                ColumnSpacing = 8,
                                Margin = new Thickness(0, 4, 0, 0),
                                Children =
                                {
                                    new Label { Text = "✓", FontSize = 14, TextColor = AppColors.SuccessGreen, VerticalOptions = LayoutOptions.Start }
                                        .Column(0),
                                    new Label
                                    {
                                        Text = "Çakışan tarih kontrolü yapıldı — uygun. Onayladığınızda talebiniz kayıt altına alınır.",
                                        FontSize = 12,
                                        TextColor = AppColors.TextLight,
                                        LineBreakMode = LineBreakMode.WordWrap
                                    }
                                    .Column(1)
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
                            .Bind(Label.TextProperty, nameof(vm.StatusMessage))
                        }
                    }
                }.Row(0),

                // Alt butonlar
                new VerticalStackLayout
                {
                    Padding = new Thickness(20, 12),
                    Spacing = 10,
                    BackgroundColor = AppColors.AppBackground,
                    Children =
                    {
                        new Button
                        {
                            Text = "✓  Onayla ve Gönder",
                            BackgroundColor = AppColors.Primary,
                            TextColor = AppColors.AppBackground,
                            CornerRadius = 14,
                            HeightRequest = 52,
                            FontSize = 15,
                            FontAttributes = FontAttributes.Bold
                        }
                        .Bind(Button.CommandProperty, nameof(vm.ConfirmAndSubmitCommand))
                        .Bind(Button.IsEnabledProperty, nameof(vm.IsBusy), converter: invertedBool),

                        new Button
                        {
                            Text = "Düzenle",
                            BackgroundColor = Colors.Transparent,
                            TextColor = AppColors.TextLight,
                            CornerRadius = 14,
                            HeightRequest = 42,
                            FontSize = 14,
                            BorderColor = AppColors.InputBorder,
                            BorderWidth = 1
                        }
                        .Bind(Button.CommandProperty, nameof(vm.GoBackCommand))
                    }
                }.Row(1)
            }
        };
    }

    static Grid SummaryRow(string labelText, string bindingPath, Color? valueColor = null, IValueConverter? converter = null) => new Grid
    {
        ColumnDefinitions =
        {
            new ColumnDefinition(GridLength.Star),
            new ColumnDefinition(GridLength.Star)
        },
        Children =
        {
            new Label { Text = labelText, FontSize = 12, TextColor = AppColors.TextMuted }
                .Column(0),
            new Label
            {
                FontSize = 13,
                FontAttributes = FontAttributes.Bold,
                TextColor = valueColor ?? AppColors.TextWhite,
                HorizontalTextAlignment = TextAlignment.End
            }
            .Bind(Label.TextProperty, bindingPath, converter: converter)
            .Column(1)
        }
    };
}

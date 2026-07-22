using CommunityToolkit.Maui.Markup;
using IzinTakip.ViewModels;
using Microsoft.Maui.Controls.Shapes;

namespace IzinTakip.Views;

public class SuccessPage : ContentPage
{
    public SuccessPage(SuccessViewModel vm)
    {
        BindingContext = vm;
        Shell.SetNavBarIsVisible(this, false);
        BackgroundColor = AppColors.AppBackground;

        Content = new VerticalStackLayout
        {
            Padding = new Thickness(30),
            Spacing = 20,
            VerticalOptions = LayoutOptions.Center,
            Children =
            {
                // Başarı ikonu
                new Frame
                {
                    BackgroundColor = AppColors.SuccessGreen,
                    CornerRadius = 36,
                    WidthRequest = 72,
                    HeightRequest = 72,
                    HorizontalOptions = LayoutOptions.Center,
                    Padding = 0,
                    HasShadow = false,
                    Content = new Label
                    {
                        Text = "✓",
                        FontSize = 32,
                        TextColor = Colors.White,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center
                    }
                },

                new Label
                {
                    Text = "Talebiniz Alındı",
                    FontSize = 22,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.Center,
                    TextColor = AppColors.TextWhite
                },

                new Label
                {
                    FontSize = 14,
                    HorizontalOptions = LayoutOptions.Center,
                    HorizontalTextAlignment = TextAlignment.Center,
                    TextColor = AppColors.TextLight
                }
                .Bind(Label.TextProperty, nameof(vm.SummaryText)),

                // Durum pill
                new Border
                {
                    StrokeShape = new RoundRectangle { CornerRadius = 20 },
                    BackgroundColor = AppColors.StatusPendingBg,
                    StrokeThickness = 0,
                    Padding = new Thickness(16, 8),
                    HorizontalOptions = LayoutOptions.Center,
                    Content = new HorizontalStackLayout
                    {
                        Spacing = 6,
                        Children =
                        {
                            new Ellipse
                            {
                                WidthRequest = 8,
                                HeightRequest = 8,
                                Fill = new SolidColorBrush(AppColors.StatusPending),
                                VerticalOptions = LayoutOptions.Center
                            },
                            new Label
                            {
                                Text = "Durum: Beklemede",
                                FontSize = 13,
                                FontAttributes = FontAttributes.Bold,
                                TextColor = AppColors.StatusPending
                            }
                        }
                    }
                },

                // CTA
                new Button
                {
                    Text = "İzinlerime Dön  →",
                    BackgroundColor = AppColors.Primary,
                    TextColor = AppColors.AppBackground,
                    CornerRadius = 14,
                    HeightRequest = 52,
                    FontSize = 15,
                    FontAttributes = FontAttributes.Bold,
                    Margin = new Thickness(0, 20, 0, 0)
                }
                .Bind(Button.CommandProperty, nameof(vm.GoToLeavesCommand))
            }
        };
    }
}

using CommunityToolkit.Maui.Markup;
using IzinTakip.Converters;
using IzinTakip.Models;
using IzinTakip.ViewModels;
using Microsoft.Maui.Controls.Shapes;

namespace IzinTakip.Views;

public class LeaveListPage : ContentPage
{
    private readonly LeaveListViewModel _vm;

    public LeaveListPage(LeaveListViewModel vm)
    {
        BindingContext = _vm = vm;
        Shell.SetNavBarIsVisible(this, false);
        BackgroundColor = AppColors.AppBackground;

        // Yeni talep - yüzen aksiyon butonu (FAB)
        var newLeaveFab = new Border
        {
            StrokeShape = new RoundRectangle { CornerRadius = 28 },
            StrokeThickness = 0,
            BackgroundColor = AppColors.Primary,
            WidthRequest = 56,
            HeightRequest = 56,
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.End,
            Margin = new Thickness(0, 0, 20, 20),
            Shadow = new Shadow { Radius = 12, Opacity = 0.4f, Brush = new SolidColorBrush(Colors.Black), Offset = new Point(0, 4) },
            Content = new Label
            {
                Text = "+",
                FontSize = 30,
                FontAttributes = FontAttributes.Bold,
                TextColor = AppColors.AppBackground,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, -3, 0, 0)
            }
        };
        newLeaveFab.GestureRecognizers.Add(
            new TapGestureRecognizer().Bind(TapGestureRecognizer.CommandProperty, nameof(vm.GoToNewLeaveCommand))
        );

        // Çıkış butonu
        var logoutBorder = new Border
        {
            StrokeShape = new RoundRectangle { CornerRadius = 10 },
            BackgroundColor = Colors.Transparent,
            Stroke = new SolidColorBrush(AppColors.InputBorder),
            StrokeThickness = 1,
            Padding = new Thickness(10, 8),
            VerticalOptions = LayoutOptions.Center,
            Content = new Image
            {
                Source = "cikis_yap.svg",
                WidthRequest = 20,
                HeightRequest = 20,
                Aspect = Aspect.AspectFit
            }
        };
        logoutBorder.GestureRecognizers.Add(
            new TapGestureRecognizer().Bind(TapGestureRecognizer.CommandProperty, nameof(vm.LogoutCommand))
        );

        Content = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Star)
            },
            Children =
            {
                // Profil başlık
                new VerticalStackLayout
                {
                    Padding = new Thickness(20, 16, 20, 8),
                    Spacing = 12,
                    Children =
                    {
                        new Grid
                        {
                            ColumnDefinitions =
                            {
                                new ColumnDefinition(GridLength.Auto),
                                new ColumnDefinition(GridLength.Star),
                                new ColumnDefinition(GridLength.Auto)
                            },
                            ColumnSpacing = 8,
                            Children =
                            {
                                // Avatar
                                new Frame
                                {
                                    BackgroundColor = AppColors.Primary,
                                    CornerRadius = 24,
                                    WidthRequest = 48,
                                    HeightRequest = 48,
                                    Padding = 0,
                                    HasShadow = false,
                                    VerticalOptions = LayoutOptions.Center,
                                    Content = new Label
                                    {
                                        FontSize = 16,
                                        FontAttributes = FontAttributes.Bold,
                                        TextColor = AppColors.AppBackground,
                                        HorizontalOptions = LayoutOptions.Center,
                                        VerticalOptions = LayoutOptions.Center
                                    }
                                    .Bind(Label.TextProperty, nameof(vm.Initials))
                                }.Column(0),

                                // İsim + birim
                                new VerticalStackLayout
                                {
                                    Margin = new Thickness(12, 0, 0, 0),
                                    VerticalOptions = LayoutOptions.Center,
                                    Spacing = 2,
                                    Children =
                                    {
                                        new Label { FontSize = 16, FontAttributes = FontAttributes.Bold, TextColor = AppColors.TextWhite }
                                            .Bind(Label.TextProperty, nameof(vm.UserName)),
                                        new Label { FontSize = 12, TextColor = AppColors.TextMuted }
                                            .Bind(Label.TextProperty, nameof(vm.UserBirim))
                                    }
                                }.Column(1),

                                logoutBorder.Column(2)
                            }
                        },

                        // İzinlerim başlık
                        new Grid
                        {
                            ColumnDefinitions =
                            {
                                new ColumnDefinition(GridLength.Star),
                                new ColumnDefinition(GridLength.Auto)
                            },
                            Children =
                            {
                                new Label { Text = "İzinlerim", FontSize = 16, FontAttributes = FontAttributes.Bold, TextColor = AppColors.TextWhite }
                                    .Column(0),
                                new Label { Text = "2026", FontSize = 13, TextColor = AppColors.TextMuted, VerticalOptions = LayoutOptions.Center }
                                    .Column(1)
                            }
                        }
                    }
                }.Row(0),

                // Liste
                new RefreshView
                {
                    Content = new CollectionView
                    {
                        EmptyView = new VerticalStackLayout
                        {
                            VerticalOptions = LayoutOptions.Center,
                            HorizontalOptions = LayoutOptions.Center,
                            Padding = new Thickness(40),
                            Children =
                            {
                                new Label
                                {
                                    Text = "Henüz izin talebi bulunmuyor.",
                                    FontSize = 14,
                                    TextColor = AppColors.TextMuted,
                                    HorizontalTextAlignment = TextAlignment.Center
                                }
                            }
                        },
                        ItemTemplate = new DataTemplate(BuildLeaveItem)
                    }
                    .Bind(CollectionView.ItemsSourceProperty, nameof(vm.Leaves))
                }
                .Bind(RefreshView.CommandProperty, nameof(vm.LoadLeavesCommand))
                .Bind(RefreshView.IsRefreshingProperty, nameof(vm.IsBusy))
                .Row(1),

                newLeaveFab.Row(1)
            }
        };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadLeavesCommand.ExecuteAsync(null);
    }

    static object BuildLeaveItem()
    {
        var statusColorConverter = new StatusColorConverter();
        var statusBgConverter = new StatusBgConverter();
        var dateDisplayConverter = new DateDisplayConverter();
        var pastLeaveOpacityConverter = new PastLeaveOpacityConverter();

        // Tarih aralığı (FormattedText)
        var dateLabel = new Label { FontSize = 14, FontAttributes = FontAttributes.Bold, TextColor = AppColors.TextWhite };
        dateLabel.FormattedText = new FormattedString
        {
            Spans =
            {
                new Span().Bind(Span.TextProperty, nameof(LeaveRequest.BaslangicTarihi), converter: dateDisplayConverter),
                new Span { Text = " – " },
                new Span().Bind(Span.TextProperty, nameof(LeaveRequest.BitisTarihi), converter: dateDisplayConverter)
            }
        };

        var grid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(4),
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto)
            },
            Padding = 0
        };

        // Durum şeridi
        grid.Add(
            new BoxView { WidthRequest = 4, VerticalOptions = LayoutOptions.Fill }
                .Bind(BoxView.ColorProperty, nameof(LeaveRequest.Durum), converter: statusColorConverter),
            0);

        // İçerik
        grid.Add(
            new VerticalStackLayout
            {
                Padding = new Thickness(14, 12),
                Spacing = 3,
                Children =
                {
                    dateLabel,
                    new Label { FontSize = 12, TextColor = AppColors.TextMuted }
                        .Bind(Label.TextProperty, nameof(LeaveRequest.IzinSuresi))
                }
            },
            1);

        // Durum rozeti
        grid.Add(
            new Border
            {
                StrokeShape = new RoundRectangle { CornerRadius = 12 },
                StrokeThickness = 0,
                Padding = new Thickness(10, 4),
                Margin = new Thickness(0, 0, 14, 0),
                VerticalOptions = LayoutOptions.Center,
                Content = new Label { FontSize = 11, FontAttributes = FontAttributes.Bold }
                    .Bind(Label.TextProperty, nameof(LeaveRequest.Durum))
                    .Bind(Label.TextColorProperty, nameof(LeaveRequest.Durum), converter: statusColorConverter)
            }
            .Bind(Border.BackgroundColorProperty, nameof(LeaveRequest.Durum), converter: statusBgConverter),
            2);

        return new Border
        {
            Margin = new Thickness(20, 6),
            Padding = 0,
            StrokeShape = new RoundRectangle { CornerRadius = 14 },
            BackgroundColor = AppColors.CardBackground,
            Stroke = new SolidColorBrush(AppColors.InputBorder),
            StrokeThickness = 1,
            Content = grid
        }
        .Bind(Border.OpacityProperty, nameof(LeaveRequest.BitisTarihi), converter: pastLeaveOpacityConverter);
    }
}

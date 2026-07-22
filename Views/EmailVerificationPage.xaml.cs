using CommunityToolkit.Maui.Markup;
using IzinTakip.Converters;
using IzinTakip.ViewModels;
using Microsoft.Maui.Controls.Shapes;

namespace IzinTakip.Views;

public class EmailVerificationPage : ContentPage
{
    public EmailVerificationPage(EmailVerificationViewModel vm)
    {
        BindingContext = vm;
        Shell.SetNavBarIsVisible(this, false);
        BackgroundColor = AppColors.AppBackground;

        var invertedBool = new InvertedBoolConverter();
        var statusMessageColor = new StatusMessageColorConverter();

        var resendSpan = new Span
        {
            Text = "Kodu tekrar gönder",
            TextColor = AppColors.Primary,
            FontSize = 12,
            FontAttributes = FontAttributes.Bold,
            TextDecorations = TextDecorations.Underline
        };
        resendSpan.GestureRecognizers.Add(
            new TapGestureRecognizer().Bind(TapGestureRecognizer.CommandProperty, nameof(vm.SendVerificationCodeCommand))
        );

        var resendActiveLabel = new Label { HorizontalOptions = LayoutOptions.Center, Margin = new Thickness(0, 4, 0, 0) };
        resendActiveLabel.FormattedText = new FormattedString
        {
            Spans =
            {
                new Span { Text = "Kod gelmedi mi? ", TextColor = AppColors.TextMuted, FontSize = 12 },
                resendSpan
            }
        };
        resendActiveLabel.Bind(Label.IsVisibleProperty, nameof(vm.CanResend));

        var resendCountdownLabel = new Label
        {
            FontSize = 12,
            TextColor = AppColors.TextMuted,
            HorizontalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 4, 0, 0)
        };
        resendCountdownLabel.Bind(Label.TextProperty, nameof(vm.ResendCountdownText));
        resendCountdownLabel.Bind(Label.IsVisibleProperty, nameof(vm.CanResend), converter: invertedBool);

        var resendLink = new VerticalStackLayout { Children = { resendActiveLabel, resendCountdownLabel } };
        resendLink.Bind(VerticalStackLayout.IsVisibleProperty, nameof(vm.IsCodeSent));

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = new Thickness(26),
                Spacing = 20,
                VerticalOptions = LayoutOptions.Center,
                Children =
                {
                    // Logo
                    new Frame
                    {
                        BackgroundColor = Colors.Transparent,
                        BorderColor = AppColors.Primary,
                        CornerRadius = 40,
                        WidthRequest = 80,
                        HeightRequest = 80,
                        HorizontalOptions = LayoutOptions.Center,
                        Padding = 0,
                        HasShadow = false,
                        Content = new Image
                        {
                            Source = "iuc_arma.png",
                            WidthRequest = 60,
                            HeightRequest = 60,
                            Aspect = Aspect.AspectFit,
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalOptions = LayoutOptions.Center
                        }
                    },

                    new Label
                    {
                        Text = "Proje Geliştirme Yönetimi Birimi İzin Sistemi",
                        FontSize = 22,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalOptions = LayoutOptions.Center,
                        HorizontalTextAlignment = TextAlignment.Center,
                        TextColor = AppColors.TextWhite
                    },

                    new Label
                    {
                        Text = "İSTANBUL ÜNİVERSİTESİ-CERRAHPAŞA",
                        FontSize = 10,
                        CharacterSpacing = 1.5,
                        HorizontalOptions = LayoutOptions.Center,
                        TextColor = AppColors.TextMuted
                    },

                    new Label
                    {
                        Text = "İzin işlemlerinizi yönetmek için kurumsal hesabınızla giriş yapın.",
                        FontSize = 13,
                        HorizontalOptions = LayoutOptions.Center,
                        HorizontalTextAlignment = TextAlignment.Center,
                        TextColor = AppColors.TextLight
                    },

                    // E-posta input
                    new Border
                    {
                        Stroke = new SolidColorBrush(AppColors.InputBorder),
                        StrokeThickness = 1,
                        StrokeShape = new RoundRectangle { CornerRadius = 14 },
                        BackgroundColor = AppColors.InputBackground,
                        Padding = new Thickness(14, 0),
                        HeightRequest = 52,
                        Content = new Entry
                        {
                            Placeholder = "ad.soyad@iuc.edu.tr",
                            PlaceholderColor = AppColors.TextMuted,
                            TextColor = AppColors.TextWhite,
                            Keyboard = Keyboard.Email,
                            FontSize = 14,
                            BackgroundColor = Colors.Transparent
                        }
                        .Bind(Entry.TextProperty, nameof(vm.Email))
                        .Bind(Entry.IsEnabledProperty, nameof(vm.IsCodeSent), converter: invertedBool)
                    },

                    // Doğrulama Kodu Gönder
                    new Button
                    {
                        Text = "Doğrulama Kodu Gönder  →",
                        BackgroundColor = AppColors.Primary,
                        TextColor = AppColors.AppBackground,
                        CornerRadius = 14,
                        HeightRequest = 52,
                        FontSize = 15,
                        FontAttributes = FontAttributes.Bold
                    }
                    .Bind(Button.CommandProperty, nameof(vm.SendVerificationCodeCommand))
                    .Bind(Button.IsVisibleProperty, nameof(vm.IsCodeSent), converter: invertedBool)
                    .Bind(Button.IsEnabledProperty, nameof(vm.IsBusy), converter: invertedBool),

                    // Kod girişi
                    new Border
                    {
                        Stroke = new SolidColorBrush(AppColors.Primary),
                        StrokeThickness = 1,
                        StrokeShape = new RoundRectangle { CornerRadius = 14 },
                        BackgroundColor = AppColors.InputBackground,
                        Padding = new Thickness(14, 0),
                        HeightRequest = 52,
                        Content = new Entry
                        {
                            Placeholder = "6 haneli doğrulama kodu",
                            PlaceholderColor = AppColors.TextMuted,
                            TextColor = AppColors.TextWhite,
                            Keyboard = Keyboard.Numeric,
                            MaxLength = 6,
                            FontSize = 16,
                            BackgroundColor = Colors.Transparent
                        }
                        .Bind(Entry.TextProperty, nameof(vm.VerificationCode))
                    }
                    .Bind(Border.IsVisibleProperty, nameof(vm.IsCodeSent)),

                    // Doğrula ve Giriş Yap
                    new Button
                    {
                        Text = "Doğrula ve Giriş Yap  ✓",
                        BackgroundColor = AppColors.Primary,
                        TextColor = AppColors.AppBackground,
                        CornerRadius = 14,
                        HeightRequest = 52,
                        FontSize = 15,
                        FontAttributes = FontAttributes.Bold
                    }
                    .Bind(Button.CommandProperty, nameof(vm.ConfirmVerificationCommand))
                    .Bind(Button.IsVisibleProperty, nameof(vm.IsCodeSent))
                    .Bind(Button.IsEnabledProperty, nameof(vm.IsBusy), converter: invertedBool),

                    new ActivityIndicator { Color = AppColors.Primary }
                        .Bind(ActivityIndicator.IsRunningProperty, nameof(vm.IsBusy))
                        .Bind(ActivityIndicator.IsVisibleProperty, nameof(vm.IsBusy)),

                    new Label
                    {
                        FontSize = 13,
                        HorizontalOptions = LayoutOptions.Center,
                        HorizontalTextAlignment = TextAlignment.Center
                    }
                    .Bind(Label.TextProperty, nameof(vm.StatusMessage))
                    .Bind(Label.TextColorProperty, nameof(vm.StatusIsError), converter: statusMessageColor),

                    resendLink
                }
            }
        };
    }
}

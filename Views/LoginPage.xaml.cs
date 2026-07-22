using CommunityToolkit.Maui.Markup;
using IzinTakip.Converters;
using IzinTakip.ViewModels;
using Microsoft.Maui.Controls.Shapes;

namespace IzinTakip.Views;

public class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel vm)
    {
        BindingContext = vm;
        Shell.SetNavBarIsVisible(this, false);
        BackgroundColor = AppColors.AppBackground;

        var invertedBool = new InvertedBoolConverter();
        var statusMessageColor = new StatusMessageColorConverter();

        var otpInfoLabel = new Label { FontSize = 13, TextColor = AppColors.TextLight };
        otpInfoLabel.FormattedText = new FormattedString
        {
            Spans =
            {
                new Span { TextColor = AppColors.Primary }
                    .Bind(Span.TextProperty, nameof(vm.Email)),
                new Span { Text = " adresine 6 haneli kod gönderildi.", TextColor = AppColors.TextLight }
            }
        };

        var verifySpan = new Span
        {
            Text = "Doğrulama Yap",
            TextColor = AppColors.Primary,
            FontSize = 12,
            FontAttributes = FontAttributes.Bold,
            TextDecorations = TextDecorations.Underline
        };
        verifySpan.GestureRecognizers.Add(
            new TapGestureRecognizer().Bind(TapGestureRecognizer.CommandProperty, nameof(vm.GoToVerificationCommand))
        );

        var resendSpan = new Span
        {
            Text = "Kodu tekrar gönder",
            TextColor = AppColors.Primary,
            FontSize = 12,
            FontAttributes = FontAttributes.Bold,
            TextDecorations = TextDecorations.Underline
        };
        resendSpan.GestureRecognizers.Add(
            new TapGestureRecognizer().Bind(TapGestureRecognizer.CommandProperty, nameof(vm.SendOtpCommand))
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
        resendLink.Bind(VerticalStackLayout.IsVisibleProperty, nameof(vm.IsOtpSent));

        var verifyLink = new Label { HorizontalOptions = LayoutOptions.Center, Margin = new Thickness(0, 10, 0, 0) };
        verifyLink.FormattedText = new FormattedString
        {
            Spans =
            {
                new Span { Text = "E-postanız doğrulanmadı mı? ", TextColor = AppColors.TextMuted, FontSize = 12 },
                verifySpan
            }
        };

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
                        .Bind(Entry.IsEnabledProperty, nameof(vm.IsOtpSent), converter: invertedBool)
                    },

                    // OTP Gönder butonu
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
                    .Bind(Button.CommandProperty, nameof(vm.SendOtpCommand))
                    .Bind(Button.IsVisibleProperty, nameof(vm.IsOtpSent), converter: invertedBool)
                    .Bind(Button.IsEnabledProperty, nameof(vm.IsBusy), converter: invertedBool),

                    // OTP başlık + bilgi
                    new VerticalStackLayout
                    {
                        Spacing = 6,
                        Children =
                        {
                            new Label
                            {
                                Text = "Doğrulama kodu",
                                FontSize = 18,
                                FontAttributes = FontAttributes.Bold,
                                TextColor = AppColors.TextWhite
                            },
                            otpInfoLabel
                        }
                    }
                    .Bind(VerticalStackLayout.IsVisibleProperty, nameof(vm.IsOtpSent)),

                    // OTP giriş alanı
                    new Border
                    {
                        Stroke = new SolidColorBrush(AppColors.Primary),
                        StrokeThickness = 1,
                        StrokeShape = new RoundRectangle { CornerRadius = 14 },
                        BackgroundColor = AppColors.InputBackground,
                        Padding = new Thickness(14, 0),
                        HeightRequest = 58,
                        Content = new Entry
                        {
                            Placeholder = "6 haneli giriş kodu",
                            PlaceholderColor = AppColors.TextMuted,
                            TextColor = AppColors.TextWhite,
                            Keyboard = Keyboard.Numeric,
                            MaxLength = 6,
                            FontSize = 20,
                            HorizontalTextAlignment = TextAlignment.Center,
                            BackgroundColor = Colors.Transparent
                        }
                        .Bind(Entry.TextProperty, nameof(vm.OtpCode))
                    }
                    .Bind(Border.IsVisibleProperty, nameof(vm.IsOtpSent)),

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
                    .Bind(Button.CommandProperty, nameof(vm.ConfirmOtpCommand))
                    .Bind(Button.IsVisibleProperty, nameof(vm.IsOtpSent))
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

                    resendLink,

                    verifyLink
                }
            }
        };
    }
}

using System.Globalization;

namespace IzinTakip.Converters;

public class InvertedBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool b ? !b : value;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool b ? !b : value;
}

public class StatusMessageColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is true ? AppColors.ErrorRed : AppColors.SuccessGreen;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

public class StatusColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value?.ToString() switch
        {
            "Onaylandı" => Color.FromArgb("#5fcb9b"),
            "Reddedildi" => Color.FromArgb("#e8857b"),
            _ => Color.FromArgb("#e0b75e")
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

public class StatusBgConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value?.ToString() switch
        {
            "Onaylandı" => Color.FromArgb("#1a2e25"),
            "Reddedildi" => Color.FromArgb("#2e1c1a"),
            _ => Color.FromArgb("#2a2518")
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

public class IsNotNullOrEmptyConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => !string.IsNullOrEmpty(value?.ToString());

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

public class DateDisplayConverter : IValueConverter
{
    private static readonly CultureInfo TrCulture = new("tr-TR");

    private static readonly TimeZoneInfo TrTimeZone = ResolveTurkeyTimeZone();

    private static TimeZoneInfo ResolveTurkeyTimeZone()
    {
        try { return TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time"); }
        catch (TimeZoneNotFoundException) { }
        catch (InvalidTimeZoneException) { }

        try { return TimeZoneInfo.FindSystemTimeZoneById("Europe/Istanbul"); }
        catch (TimeZoneNotFoundException) { }
        catch (InvalidTimeZoneException) { }

        return TimeZoneInfo.CreateCustomTimeZone("TRT", TimeSpan.FromHours(3), "Türkiye Saati", "Türkiye Saati");
    }

    // "yyyy-MM-dd" (uygulamanın kendi ürettiği tarih) veya Google Sheets'ten dönen
    // tam ISO 8601 UTC zaman damgasını ("2026-06-28T21:00:00.000Z") tarihe çözer.
    public static DateTime? Parse(string s)
    {
        if (DateTime.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var simpleDate))
            return simpleDate.Date;

        if (DateTimeOffset.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var offset))
            return TimeZoneInfo.ConvertTime(offset, TrTimeZone).Date;

        return null;
    }

    public static string Format(string s)
        => Parse(s)?.ToString("dd MMMM yyyy", TrCulture) ?? s;

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is string s && !string.IsNullOrWhiteSpace(s) ? Format(s) : value;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

public class PastLeaveOpacityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string s && !string.IsNullOrWhiteSpace(s))
        {
            var date = DateDisplayConverter.Parse(s);
            if (date.HasValue && date.Value < DateTime.Today)
                return 0.55;
        }

        return 1.0;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

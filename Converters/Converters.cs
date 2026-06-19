using System.Globalization;

namespace Przypominajka.Converters;

/// <summary>Zmienia bool na kolor: true = zielony, false = szary (lub inne przez ConverterParameter).</summary>
public class BoolToColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool b = value is true;
        if (parameter is string p)
        {
            var parts = p.Split('|');
            return Color.FromArgb(b ? parts[0] : parts[1]);
        }
        return b ? Color.FromArgb("#27AE60") : Color.FromArgb("#BDC3C7");
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

/// <summary>Zmienia bool na string: ConverterParameter="TrueText|FalseText"</summary>
public class BoolToStringConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool b = value is true;
        if (parameter is string p)
        {
            var parts = p.Split('|');
            return b ? parts[0] : (parts.Length > 1 ? parts[1] : string.Empty);
        }
        return b ? "Tak" : "Nie";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

/// <summary>Odwraca wartość bool.</summary>
public class InvertBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is not true;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is not true;
}

/// <summary>Zwraca true gdy wartość nie jest null.</summary>
public class NotNullConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is not null && value.ToString() != string.Empty;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

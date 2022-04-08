using System;
using System.Windows.Data;

namespace Pantheon.ViewModel.Converter;

internal class IntToStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object parameter,
        System.Globalization.CultureInfo culture)
    {
        return value switch
        {
            null => null,
            string => value,
            _ => value.ToString()
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter,
        System.Globalization.CultureInfo culture)
    {
        if (value is not string s) return value;

        return int.TryParse(s, out var i) ? i : 0;
    }
}
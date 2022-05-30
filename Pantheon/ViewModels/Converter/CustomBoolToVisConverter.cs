using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Pantheon.ViewModels.Converter;

internal class CustomBoolToVisConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not bool b) return Visibility.Hidden;

        return b ? Visibility.Hidden : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
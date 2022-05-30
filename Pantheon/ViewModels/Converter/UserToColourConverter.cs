using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Pantheon.ViewModels.Converter;

internal class UserToColourConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var val = (bool)value;
        return val ? new SolidColorBrush(Colors.DarkOrange) : new SolidColorBrush(Colors.Gray);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
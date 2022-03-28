using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Pantheon.ViewModel.Converter;

internal class BoolToColourConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var val = (bool)value;
        return val ? new SolidColorBrush(Colors.GreenYellow) : new SolidColorBrush(Colors.Red);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
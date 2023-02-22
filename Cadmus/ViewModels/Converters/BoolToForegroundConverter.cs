using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Cadmus.ViewModels.Converters;

internal class BoolToForegroundConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var val = (bool)value;
        return val ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Black);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
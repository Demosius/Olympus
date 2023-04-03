using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Pantheon.ViewModels.Converter;

internal class BoolToEmployeeBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var val = (bool)value;
        return val ? new SolidColorBrush(Colors.Gray) : new SolidColorBrush(Colors.Aqua);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
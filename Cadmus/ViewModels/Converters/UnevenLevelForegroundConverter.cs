using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Cadmus.ViewModels.Converters;

internal class UnevenLevelForegroundConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var val = (bool)value;
        return val ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.GreenYellow);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
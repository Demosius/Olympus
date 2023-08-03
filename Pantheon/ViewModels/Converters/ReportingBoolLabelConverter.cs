using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Pantheon.ViewModels.Converters;

internal class ReportingBoolLabelConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null) return new SolidColorBrush(Colors.White);
        var val = (bool)value;
        return val ? new SolidColorBrush(Colors.Cyan) : new SolidColorBrush(Colors.OrangeRed);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
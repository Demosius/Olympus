using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Deimos.Models;

namespace Deimos.ViewModels.Converters;

internal class DataCompColourConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not DataDateComparison comp) return new SolidColorBrush(Colors.Orange);
        return !comp.HasPickEvents ? new SolidColorBrush(Colors.Red) :
            !comp.HasMissPicks ? new SolidColorBrush(Colors.Yellow) : 
            new SolidColorBrush(Colors.AliceBlue);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
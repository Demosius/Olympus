using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Aion.ViewModel.Converters;

public class InOutColourConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var val = (value as string)?.ToUpper();
        return val == "IN" 
            ? new SolidColorBrush(Colors.Green) 
            : new SolidColorBrush(Colors.Red);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
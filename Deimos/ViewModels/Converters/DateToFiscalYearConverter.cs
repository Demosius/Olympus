using System;
using System.Globalization;
using System.Windows.Data;
using Uranus.Extensions;

namespace Deimos.ViewModels.Converters;

[ValueConversion(typeof(DateTime), typeof(string))]
public class DateToFiscalYearConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var date = (DateTime)value;
        return date.EBFiscalYear().ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
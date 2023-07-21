using System;
using System.Globalization;
using System.Windows.Data;
using Uranus.Extensions;

// ReSharper disable StringLiteralTypo

namespace Deimos.ViewModels.Converters;

[ValueConversion(typeof(EQAView), typeof(string))]
internal class QAViewConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var printable = (EQAView)value;
        return printable.GetDescription();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            string typeString => typeString.ToLower() switch
            {
                "qa errors" => EQAView.Errors,
                "errors" => EQAView.Errors,
                "error management" => EQAView.Errors,
                "stats" => EQAView.Stats,
                "operator stats" => EQAView.Stats,
                "reports" => EQAView.Reports,
                "stats/reports" => EQAView.Reports,
                "statreports" => EQAView.Reports,
                _ => EQAView.Errors
            },
            _ => EQAView.Errors
        };
    }
}
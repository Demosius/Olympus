using System;
using System.Globalization;
using System.Windows.Data;
using Uranus.Extensions;

namespace Cadmus.ViewModels.Converters;

[ValueConversion(typeof(EPrintable), typeof(string))]
internal class PrintableEnumConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var printable = (EPrintable)value;
        return printable.GetDescription();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            string typeString => typeString.ToLower() switch
            {
                // ReSharper disable once StringLiteralTypo
                "receivingputawaylabels" => EPrintable.ReceivingPutAwayLabels,
                "receiving put away labels" => EPrintable.ReceivingPutAwayLabels,
                "receiving put-away labels" => EPrintable.ReceivingPutAwayLabels,
                _ => EPrintable.ReceivingPutAwayLabels
            },
            _ => EPrintable.ReceivingPutAwayLabels
        };
    }
}
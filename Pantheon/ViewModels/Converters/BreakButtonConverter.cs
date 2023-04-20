using System;
using System.Globalization;
using System.Windows.Data;

namespace Pantheon.ViewModels.Converters;

/// <summary>
/// Used to determine what to display on the appropriate button, based on the associated break's name.
/// Lunch is permanent and not to be removed.
/// </summary>
internal class BreakButtonConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (string)value == "Lunch" ? "+" : "-";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (string)value == "+" ? "Lunch" : "Break";
    }
}
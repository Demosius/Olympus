using System;
using System.Globalization;
using System.Windows.Data;

namespace Pantheon.ViewModels.Converter;

/// <summary>
/// Converts a nullable bool to a string representing attendance for a roster rule.
/// </summary>
internal class NBoolToAttendanceStringConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not bool b) return "(Optional)";
        return b ? "(Required)" : "(Cannot)";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
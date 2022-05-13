using System;
using System.Globalization;
using System.Windows.Data;

namespace Pantheon.ViewModel.Converter;

/// <summary>
/// Converts a bool, expecting to be whether or not a shift rule VM is in edit mode, and returns
/// the appropriate button label for the appropriate command description.
/// </summary>
internal class EditBoolToLabelConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not bool b) return "Add";
        return b ? "Confirm" : "Add";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
﻿using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Pantheon.ViewModels.Converter;

internal class BoolToColourConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null) return new SolidColorBrush(Colors.Orange);
        var val = (bool)value;
        return val ? new SolidColorBrush(Colors.GreenYellow) : new SolidColorBrush(Colors.Red);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
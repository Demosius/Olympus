﻿using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Aion.ViewModels.Converters;

public class BoolColourConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value != null && (bool)value)
            return new SolidColorBrush(Colors.Green);
        return new SolidColorBrush(Colors.Red);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
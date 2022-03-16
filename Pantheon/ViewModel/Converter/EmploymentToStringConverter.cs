﻿using System;
using System.Globalization;
using System.Windows.Data;
using Uranus.Staff.Model;

namespace Pantheon.ViewModel.Converter;

[ValueConversion(typeof(EEmploymentType), typeof(string))]
internal class EmploymentToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null) return string.Empty;

        var employmentType = (EEmploymentType) value;
        return employmentType.GetDescription();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            EEmploymentType et => et,
            string typeString => typeString.ToLower() switch
            {
                "sa" => EEmploymentType.SA,
                "salary" => EEmploymentType.SA,
                "ca" => EEmploymentType.CA,
                "casual" => EEmploymentType.CA,
                "fp" => EEmploymentType.FP,
                "full-time permanent" => EEmploymentType.FP,
                "pp" => EEmploymentType.PP,
                "part-time permanent" => EEmploymentType.PP,
                _ => EEmploymentType.CA
            },
            _ => EEmploymentType.CA
        };
    }
}
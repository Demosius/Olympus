using System;
using System.Globalization;
using System.Windows.Data;
using Uranus.Extensions;
// ReSharper disable StringLiteralTypo

namespace Quest.ViewModels.Converters;

[ValueConversion(typeof(EQuestPage), typeof(string))]
internal class QuestPageConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var printable = (EQuestPage)value;
        return printable.GetDescription();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            string typeString => typeString.ToLower() switch
            {
                "pick screen" => EQuestPage.PickRateTracker,
                "pickrate tracker" => EQuestPage.PickRateTracker,
                "pickratetracker" => EQuestPage.PickRateTracker,
                _ => EQuestPage.PickRateTracker
            },
            _ => EQuestPage.PickRateTracker
        };
    }
}
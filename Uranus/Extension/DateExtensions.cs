using System;
using System.Globalization;

namespace Uranus.Extension;

public static class DateExtensions
{
    public static string FiscalWeek(this DateTime date)
    {
        var week = ISOWeek.GetWeekOfYear(date);

        var quarter = (int)Math.Ceiling((double)week / 13);

        if (quarter > 4) quarter = 4;

        var weekInQuarter = week - (quarter - 1) * 13;

        var monthInQuarter = Math.Ceiling((double)weekInQuarter / 4);

        if (monthInQuarter > 3) monthInQuarter = 3;

        var month = (int)((quarter - 1) * 3 + monthInQuarter);

        var weekInMonth = (int)(week - (quarter - 1) * 13 - (monthInQuarter - 1) * 4);

        return $"{DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(month)}-Wk{weekInMonth}";
    }
}
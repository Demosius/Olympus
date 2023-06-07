using System;
using System.Globalization;

namespace Uranus.Extensions;

public static class DateExtensions
{
    private static readonly string[] number = { "", "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT", "NINE" };

    public static int EB_ISOWeek(this DateTime date)
    {
        var checkDate = date.DayOfWeek == DayOfWeek.Sunday ? date.AddDays(1) : date;
        return ISOWeek.GetWeekOfYear(checkDate);
    }

    public static string EBFiscalWeek(this DateTime date)
    {
        var week = date.EB_ISOWeek();

        var quarter = (int)Math.Ceiling((double)week / 13);

        if (quarter > 4) quarter = 4;

        var weekInQuarter = week - (quarter - 1) * 13;

        var monthInQuarter = Math.Ceiling((double)weekInQuarter / 4);

        if (monthInQuarter > 3) monthInQuarter = 3;

        var month = (int)((quarter - 1) * 3 + monthInQuarter);

        var weekInMonth = (int)(week - (quarter - 1) * 13 - (monthInQuarter - 1) * 4);

        return $"{DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(month)}-Wk{weekInMonth}";
    }

    /// <summary>
    /// Based on EBGames definition of fiscal year beginning in Feb.
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static int EBFiscalQuarter(this DateTime date)
    {
        var month = (date.EBFiscalMonth() - 2) % 12 + 1;

        var quarter = (int)Math.Ceiling((double)month / 3);

        return quarter;
    }

    public static int Quarter(this DateTime date)
    {
        var week = date.EB_ISOWeek();

        var quarter = (int)Math.Ceiling((double)week / 13);

        return quarter;
    }

    /// <summary>
    /// Based on EBGames definition of fiscal year beginning in Feb.
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static string EBFiscalQuarterString(this DateTime date)
    {
        return $"Q{date.EBFiscalQuarter()}";
    }

    public static string QuarterString(this DateTime date)
    {
        return $"Q{date.Quarter()}";
    }

    public static int EBFiscalMonth(this DateTime date)
    {
        var week = date.EB_ISOWeek();

        var quarter = (int)Math.Ceiling((double)week / 13);

        if (quarter > 4) quarter = 4;

        var weekInQuarter = week - (quarter - 1) * 13;

        var monthInQuarter = Math.Ceiling((double)weekInQuarter / 4);

        if (monthInQuarter > 3) monthInQuarter = 3;

        var month = (int)((quarter - 1) * 3 + monthInQuarter);

        return month;
    }

    public static string EBFiscalMonthString(this DateTime date)
    {
        return DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(date.EBFiscalMonth());
    }

    public static DateTime WeekStartSunday(this DateTime date)
    {
        return date.AddDays(((int) DayOfWeek.Monday - ((int) date.DayOfWeek + 1)) % 7).Date;
    }

    public static DateTime WeekEndSaturday(this DateTime date)
    {
        return date.WeekStartSunday().AddDays(6);
    }

    public static int EBFiscalYear(this DateTime date)
    {
        // Fiscal year will only change compared to calender year in december and january.
        var fiscalMonth = date.EBFiscalMonth();
        var calenderMonth = date.Month;
        int fiscalYear;
        var calenderYear = date.Year;

        if (fiscalMonth is not 1 and not 12) 
            fiscalYear = calenderYear;
        else switch (fiscalMonth)
        {
            case 1 when calenderMonth is 12 or 2:
                fiscalYear = calenderYear;
                break;
            case 1:
            case 12 when calenderMonth is 1:
                fiscalYear = calenderYear - 1;
                break;
            default:
                fiscalYear = calenderYear;
                break;
        }
        return fiscalYear;
    }

    public static (DateTime sunday, int year, string qtr, string month, string week) GetEBFiscalData(this DateTime date)
    {
        var week = date.EB_ISOWeek();
        
        var quarter = (int)Math.Ceiling((double)week / 13);

        if (quarter > 4) quarter = 4;

        var weekInQuarter = week - (quarter - 1) * 13;

        var monthInQuarter = Math.Ceiling((double)weekInQuarter / 4);

        if (monthInQuarter > 3) monthInQuarter = 3;

        var month = (int)((quarter - 1) * 3 + monthInQuarter);

        var fiscalMonthString = DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(month);
        
        // +10 is the same as -2 for our purposes, except that % doesn't handle negative numbers strictly correctly.
        var fiscalMonthOffset = (month +10) % 12 + 1;

        var fiscalQuarter = (int)Math.Ceiling((double)fiscalMonthOffset / 3);
        var quarterString = $"Q{fiscalQuarter}";

        var weekInMonth = (int)(week - (quarter - 1) * 13 - (monthInQuarter - 1) * 4);

        var fiscalWeek = $"WEEK {number[weekInMonth]}";
        var sunday = date.WeekStartSunday();

        return (sunday, date.EBFiscalYear(), quarterString, fiscalMonthString, fiscalWeek);
    }
}
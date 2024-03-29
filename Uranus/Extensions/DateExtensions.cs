﻿using System;
using System.Globalization;

namespace Uranus.Extensions;

public static class DateExtensions
{
    private static readonly string[] number = { "", "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT", "NINE" };

    public static DateTime Monday(this DateTime date) =>
        date.AddDays(0 - (int) date.DayOfWeek + (int) DayOfWeek.Monday);

    public static int EB_ISOWeek(this DateTime date)
    {
        var checkDate = date.DayOfWeek == DayOfWeek.Sunday ? date.AddDays(1) : date;
        return ISOWeek.GetWeekOfYear(checkDate);
    }

    public static int EBFiscalWeek(this DateTime date) => (date.EB_ISOWeek() + 47) % ISOWeek.GetWeeksInYear(date.Year) + 1;

    public static string EBFiscalWeekString(this DateTime date)
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

    public static string EBFiscalWeekStringFull(this DateTime date) => $"{date.EBFiscalWeekString()} {date.EBFiscalYear()}";
    
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

    public static string EBFiscalMonthString(this DateTime date) => DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(date.EBFiscalMonth());
    
    public static string EBFiscalMonthStringFull(this DateTime date) => $"{date.EBFiscalMonthString()} {date.EBFiscalYear()}";

    public static DateTime WeekStartSunday(this DateTime date) => date.AddDays(0 - (int) date.DayOfWeek).Date;

    public static DateTime WeekEndSaturday(this DateTime date) => date.WeekStartSunday().AddDays(6);

    public static DateTime EBFiscalMonthStart(this DateTime date) => date.AddDays(0 - (int) date.DayOfWeek - (date.WeekInMonth() - 1) * 7);

    public static DateTime EBFiscalMonthEnd(this DateTime date) => date.EBFiscalMonthStart().AddDays(35).EBFiscalMonthStart().AddDays(-1);

    public static DateTime EBFiscalYearStart(this DateTime date) => date.AddDays(0 - (int) date.DayOfWeek - (date.EBFiscalWeek() - 1) * 7);

    public static DateTime EBFiscalYearEnd(this DateTime date) => date.EBFiscalYearStart().AddDays(372).EBFiscalYearStart().AddDays(-1);

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

    public static int WeekInMonth(this DateTime date)
    {
        var week = date.EB_ISOWeek();

        var quarter = (int)Math.Ceiling((double)week / 13);

        if (quarter > 4) quarter = 4;

        var weekInQuarter = week - (quarter - 1) * 13;

        var monthInQuarter = Math.Ceiling((double)weekInQuarter / 4);

        if (monthInQuarter > 3) monthInQuarter = 3;

        var weekInMonth = (int)(week - (quarter - 1) * 13 - (monthInQuarter - 1) * 4);

        return weekInMonth;
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
        var fiscalMonthOffset = (month + 10) % 12 + 1;

        var fiscalQuarter = (int)Math.Ceiling((double)fiscalMonthOffset / 3);
        var quarterString = $"Q{fiscalQuarter}";

        var weekInMonth = (int)(week - (quarter - 1) * 13 - (monthInQuarter - 1) * 4);

        var fiscalWeek = $"WEEK {number[weekInMonth]}";
        var sunday = date.WeekStartSunday();

        return (sunday, date.EBFiscalYear(), quarterString, fiscalMonthString, fiscalWeek);
    }

    public static bool IsBetween(this DateTime dateTime, DateTime fromDateTime, DateTime toDateTime) =>
        dateTime >= fromDateTime && dateTime <= toDateTime;

}
using System;
using System.Collections.Generic;
using Uranus.Extensions;
using Uranus.Staff.Models;

namespace Deimos.Models;

public class WeeklyStatisticReport : EmployeeStatisticsReport
{
    public string MonthWeek { get; set; }
    public string FiscalWeek { get; set; }
    public int FiscalYear { get; set; }
    public string FiscalQuarter { get; set; }
    public string FiscalMonth { get; set; }

    public string DateRange { get; set; }

    public string Clan => Employee?.ClanName ?? string.Empty;

    public WeeklyStatisticReport(Employee? employee, string rfID, List<PickSession> pickSessions, List<Mispick> mispicks, DateTime start, DateTime? end, ETechType? tech) : base(employee, rfID, pickSessions, mispicks, start, end, tech)
    {
        // Make sure that we a properly handling dates.
        // Turn start date into sunday.
        (start, FiscalYear, FiscalQuarter, FiscalMonth, MonthWeek) = start.GetEBFiscalData();
        FiscalWeek = start.EBFiscalWeek();
        StartDate = start;
        EndDate = StartDate.AddDays(6);

        DateRange = $"{start:ddd dd/MM/yyyy} to {end:ddd dd/MM/yyyy}";
    }
}
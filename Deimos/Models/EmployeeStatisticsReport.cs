using System;
using System.Collections.Generic;
using System.Linq;
using Uranus.Staff.Models;

namespace Deimos.Models;

public class EmployeeStatisticsReport
{
    public Employee? Employee { get; set; }
    public string EmployeeName => Employee?.FullName ?? string.Empty;
    public string DisplayName => Employee?.FullName ?? RFID;
    public string RFID { get; set; }
    public ETechType TechType { get; set; }
    public TimeSpan TimeTaken { get; set; }
    public int Hits { get; set; }
    public double HitsPerHour { get; set; }
    public int Units { get; set; }
    public double UnitsPerHour { get; set; }
    public int ErrorCount { get; set; }
    public int UnitErrors { get; set; }

    public double Accuracy { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public List<PickSession> PickSessions { get; set; }
    public List<Mispick> Mispicks { get; set; }

    private int? ptlHits;
    public int PTLHits => ptlHits ??= PickSessions.Where(s => s.PickLocation == EPickLocation.PTL).Sum(s => s.Hits);
    private int? spo01Hits;
    public int SP01Hits => spo01Hits ??= PickSessions.Where(s => s.PickLocation == EPickLocation.SP01).Sum(s => s.Hits);
    private int? aBulkHits;
    public int ABulkHits => aBulkHits ??= PickSessions.Where(s => s.PickLocation == EPickLocation.ABULK).Sum(s => s.Hits);

    private double? ptlHitsPerHour;
    public double PTLHitsPerHour => ptlHitsPerHour ??= PTLHits / TimeTaken.TotalHours;
    private double? sp01HitsPerHour;
    public double SP01HitsPerHour => sp01HitsPerHour ??= SP01Hits / TimeTaken.TotalHours;
    private double? aBulkHitsPerHour;
    public double ABulkHitsPerHour => aBulkHitsPerHour ??= ABulkHits / TimeTaken.TotalHours;

    public EmployeeStatisticsReport(Employee? employee, string rfID, List<PickSession> pickSessions, DateTime? start, DateTime? end)
    {
        RFID = rfID;
        Employee = employee;
        PickSessions = pickSessions; // Assume accurate list.

        var techs = PickSessions.Select(s => s.TechType).Distinct().ToList();
        TechType = techs.Count > 1 ? ETechType.All : techs.First();

        TimeTaken = TimeSpan.Zero;
        foreach (var pickSession in PickSessions)
            TimeTaken = TimeTaken.Add(pickSession.Duration);

        Hits = PickSessions.Sum(s => s.Hits);
        Units = PickSessions.Sum(s => s.Units);

        HitsPerHour = Hits / TimeTaken.TotalHours;
        UnitsPerHour = Units / TimeTaken.TotalHours;

        StartDate = start ?? PickSessions.Min(s => s.Date);
        EndDate = end ?? PickSessions.Max(s => s.Date);

        Mispicks = PickSessions.SelectMany(s => s.Mispicks).ToList();

        ErrorCount = Mispicks.Count;
        UnitErrors = Mispicks.Sum(m => Math.Abs(m.VarianceQty));

        Accuracy = 1 - (double)UnitErrors / Units;
    }


    public EmployeeStatisticsReport(Employee? employee, string rfID, List<PickSession> pickSessions, List<Mispick> mispicks, DateTime? start, DateTime? end, ETechType? tech = null)
    {
        RFID = rfID;
        Employee = employee;
        // Assume accurate list.
        PickSessions = pickSessions;
        Mispicks = mispicks;

        if (tech is null)
        {
            var techs = PickSessions.Select(s => s.TechType).ToList();
            techs.AddRange(Mispicks.Select(m => m.TechType));
            techs = techs.Distinct().ToList();
            TechType = techs.Count == 1 ? techs.First() : ETechType.All;
        }
        else
            TechType = (ETechType)tech;

        TimeTaken = TimeSpan.Zero;
        foreach (var pickSession in PickSessions)
            TimeTaken = TimeTaken.Add(pickSession.Duration);

        Hits = PickSessions.Sum(s => s.Hits);
        Units = PickSessions.Sum(s => s.Units);

        HitsPerHour = Hits / TimeTaken.TotalHours;
        UnitsPerHour = Units / TimeTaken.TotalHours;

        StartDate = start ?? PickSessions.Min(s => s.Date);
        EndDate = end ?? PickSessions.Max(s => s.Date);

        ErrorCount = Mispicks.Count;
        UnitErrors = Mispicks.Sum(m => Math.Abs(m.VarianceQty));

        Accuracy = 1 - (double)UnitErrors / Units;
    }


}
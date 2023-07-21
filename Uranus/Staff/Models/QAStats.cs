using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using SQLite;

namespace Uranus.Staff.Models;

public enum EDatePeriod
{
    Day,
    Week,
    Month,
    Year
}

public class QAStats
{
    [PrimaryKey] public string ID { get; set; } // [StartYear].[StartMonth].[StartDay]::[EndYear].[EndMonth].[EndDay], e.g. 2022.07.02::2022.07.08
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public string DateDescription { get; set; }
    public EDatePeriod DatePeriod { get; set; }

    /* Base Stats */
    public int RestockQty { get; set; }
    public int RestockHits { get; set; }
    public int RestockCartons { get; set; }
    public int QAUnits { get; set; }
    public int QAQty { get; set; }
    public int QALines { get; set; }
    public int QACartons { get; set; }
    public double AverageStaff { get; set; }

    /* Total Errors */
    public int ErrorUnits { get; set; }
    public int ErrorQty { get; set; }
    public int ErrorLines { get; set; }
    public int ErrorCartons { get; set; }

    /* External Errors */
    public int ExternalErrorUnits { get; set; }
    public int ExternalErrorQty { get; set; }
    public int ExternalErrorLines { get; set; }
    public int ExternalErrorCartons { get; set; }
    public int SupplierErrorUnits { get; set; }
    public int SupplierErrorQty { get; set; }
    public int SupplierErrorLines { get; set; }
    public int SupplierErrorCartons { get; set; }
    public int SystemErrorUnits { get; set; }
    public int SystemErrorQty { get; set; }
    public int SystemErrorLines { get; set; }
    public int SystemErrorCartons { get; set; }
    public int OtherExternalErrorUnits { get; set; }
    public int OtherExternalErrorQty { get; set; }
    public int OtherExternalErrorLines { get; set; }
    public int OtherExternalErrorCartons { get; set; }

    /* Warehouse Errors */
    // Total
    public int WHErrorUnits { get; set; }
    public int WHErrorQty { get; set; }
    public int WHErrorLines { get; set; }
    public int WHErrorCartons { get; set; }
    // Pickers
    public int PickerErrorUnits { get; set; }
    public int PickerErrorQty { get; set; }
    public int PickerErrorLines { get; set; }
    public int PickerErrorCartons { get; set; }
    public int PTVErrorUnits { get; set; }
    public int PTVErrorQty { get; set; }
    public int PTVErrorLines { get; set; }
    public int PTVErrorCartons { get; set; }
    public int PTLErrorUnits { get; set; }
    public int PTLErrorQty { get; set; }
    public int PTLErrorLines { get; set; }
    public int PTLErrorCartons { get; set; }
    // Others
    public int OtherDeptErrorUnits { get; set; }
    public int OtherDeptErrorQty { get; set; }
    public int OtherDeptErrorLines { get; set; }
    public int OtherDeptErrorCartons { get; set; }
    public int ReplenErrorUnits { get; set; }
    public int ReplenErrorQty { get; set; }
    public int ReplenErrorLines { get; set; }
    public int ReplenErrorCartons { get; set; }
    public int StockingErrorUnits { get; set; }
    public int StockingErrorQty { get; set; }
    public int StockingErrorLines { get; set; }
    public int StockingErrorCartons { get; set; }
    public int ReceivingErrorUnits { get; set; }
    public int ReceivingErrorQty { get; set; }
    public int ReceivingErrorLines { get; set; }
    public int ReceivingErrorCartons { get; set; }
    public int HeatMapErrorUnits { get; set; }
    public int HeatMapErrorQty { get; set; }
    public int HeatMapErrorLines { get; set; }
    public int HeatMapErrorCartons { get; set; }
    public int QAErrorUnits { get; set; }
    public int QAErrorQty { get; set; }
    public int QAErrorLines { get; set; }
    public int QAErrorCartons { get; set; }

    /* Seeking */
    public int SeekUnits { get; set; }
    public int SeekQty { get; set; }
    public int SeekLines { get; set; }
    public int SeekCartons { get; set; }
    public int SeekUnitsFixed { get; set; }
    public int SeekQtyFixed { get; set; }
    public int SeekLinesFixed { get; set; }
    public int SeekCartonsFixed { get; set; }

    /* Adjustable Variables */
    public string DevelopmentOpportunity { get; set; }
    public string Notes { get; set; }
    public double TargetAccuracy { get; set; }

    public QAStats()
    {
        ID = string.Empty;
        DevelopmentOpportunity = string.Empty;
        Notes = string.Empty;
        DateDescription = string.Empty;
    }

    public QAStats(DateTime startDate, DateTime endDate, string dateDescription = "")
    {
        ID = GetID(startDate, endDate);
        StartDate = startDate;
        EndDate = endDate;
        var days = (EndDate - StartDate).Days + 1;

        DatePeriod = days switch
        {
            <= 1 => EDatePeriod.Day,
            <= 7 => EDatePeriod.Week,
            <= 35 => EDatePeriod.Month,
            _ => EDatePeriod.Year
        };

        DateDescription = dateDescription;
        DevelopmentOpportunity = string.Empty;
        Notes = string.Empty;
    }

    public void SetValues(List<QACarton> qaCartons, List<QALine> qaLines)
    {
        QAUnits = qaLines.Sum(l => l.QAQtyBase);
        QAQty = qaLines.Sum(l => l.QAQty);
        QALines = qaLines.Count;
        QACartons = Math.Max(qaLines.Select(l => l.CartonID).Distinct().Count(), qaCartons.Count);

        var staffDict = qaCartons.GroupBy(c => c.Date)
            .ToDictionary(g => g.Key, g => g.Select(c => c.EmployeeID).Distinct().ToList());

        AverageStaff = staffDict.Any() ? staffDict.Values.Average(empList => empList.Count) : 0;

        var errorLines = qaLines.Where(l => l.HasError).ToList();

        SetErrorValues(errorLines);
    }

    private void SetErrorValues(List<QALine> errorLines)
    {
        ErrorUnits = errorLines.Sum(l => l.ErrorUnitQty);
        ErrorQty = errorLines.Sum(l => l.ErrorQty);
        ErrorLines = errorLines.Count;
        ErrorCartons = errorLines.Select(l => l.CartonID).Distinct().Count();

        var externalErrorLines = errorLines.Where(l => l.ErrorAllocation == EErrorAllocation.External).ToList();
        var whErrorLines = errorLines.Where(l => l.ErrorAllocation == EErrorAllocation.Warehouse).ToList();

        SetExternalErrorValues(externalErrorLines);
        SetWarehouseErrorValues(whErrorLines);
    }

    private void SetExternalErrorValues(List<QALine> externalErrorLines)
    {
        ExternalErrorUnits = externalErrorLines.Sum(l => l.ErrorUnitQty);
        ExternalErrorQty = externalErrorLines.Sum(l => l.ErrorQty);
        ExternalErrorLines = externalErrorLines.Count;
        ExternalErrorCartons = externalErrorLines.Select(l => l.CartonID).Distinct().Count();

        var supplierErrorLines = externalErrorLines.Where(l => l.SupplierError).ToList();
        var systemErrorLines = externalErrorLines.Where(l => l.SystemError).ToList();
        var otherExtErrorLines = externalErrorLines.Where(l => l.ErrorCategory == EErrorCategory.OtherExternal).ToList();

        SetSupplierErrorValues(supplierErrorLines);
        SetSystemErrorValues(systemErrorLines);
        SetOtherExternalErrorValues(otherExtErrorLines);
    }

    private void SetSupplierErrorValues(List<QALine> supplierErrorLines)
    {
        SupplierErrorUnits = supplierErrorLines.Sum(l => l.ErrorUnitQty);
        SupplierErrorQty = supplierErrorLines.Sum(l => l.ErrorQty);
        SupplierErrorLines = supplierErrorLines.Count;
        SupplierErrorCartons = supplierErrorLines.Select(l => l.CartonID).Distinct().Count();
    }

    private void SetSystemErrorValues(List<QALine> systemErrorLines)
    {
        SystemErrorUnits = systemErrorLines.Sum(l => l.ErrorUnitQty);
        SystemErrorQty = systemErrorLines.Sum(l => l.ErrorQty);
        SystemErrorLines = systemErrorLines.Count;
        SystemErrorCartons = systemErrorLines.Select(l => l.CartonID).Distinct().Count();
    }

    private void SetOtherExternalErrorValues(List<QALine> otherExternalErrorLines)
    {
        OtherExternalErrorUnits = otherExternalErrorLines.Sum(l => l.ErrorUnitQty);
        OtherExternalErrorQty = otherExternalErrorLines.Sum(l => l.ErrorQty);
        OtherExternalErrorLines = otherExternalErrorLines.Count;
        OtherExternalErrorCartons = otherExternalErrorLines.Select(l => l.CartonID).Distinct().Count();
    }

    private void SetWarehouseErrorValues(List<QALine> whErrorLines)
    {
        WHErrorUnits = whErrorLines.Sum(l => l.ErrorUnitQty);
        WHErrorQty = whErrorLines.Sum(l => l.ErrorQty);
        WHErrorLines = whErrorLines.Count;
        WHErrorCartons = whErrorLines.Select(l => l.CartonID).Distinct().Count();

        var pickerErrorLines = whErrorLines.Where(l => l.PickerError).ToList();
        var replenErrorLines = whErrorLines.Where(l => l.ReplenError).ToList();
        var stockingErrorLines = whErrorLines.Where(l => l.StockingError).ToList();
        var receivingErrorLines = whErrorLines.Where(l => l.ReceiveError).ToList();
        var heatMapErrorLines = whErrorLines.Where(l => l.HeatMapError).ToList();
        var qaErrorLines = whErrorLines.Where(l => l.QAError).ToList();
        var otherDeptErrorLines = whErrorLines.Where(l => l.ErrorCategory == EErrorCategory.OtherDept).ToList();

        SetPickerErrorValues(pickerErrorLines);
        SetReplenErrorValues(replenErrorLines);
        SetStockingErrorValues(stockingErrorLines);
        SetReceiveErrorValues(receivingErrorLines);
        SetHeatMapErrorValues(heatMapErrorLines);
        SetQAErrorValues(qaErrorLines);
        SetOtherDeptErrorValues(otherDeptErrorLines);
    }

    private void SetPickerErrorValues(List<QALine> pickerErrorLines)
    {
        PickerErrorUnits = pickerErrorLines.Sum(l => l.ErrorUnitQty);
        PickerErrorQty = pickerErrorLines.Sum(l => l.ErrorQty);
        PickerErrorLines = pickerErrorLines.Count;
        PickerErrorCartons = pickerErrorLines.Select(l => l.CartonID).Distinct().Count();

        var ptlErrorLines = pickerErrorLines.Where(l => l.PTL).ToList();
        var ptvErrorLines = pickerErrorLines.Where(l => l.PTV).ToList();

        SetPTLErrorValues(ptlErrorLines);
        SetPTVErrorValues(ptvErrorLines);
    }

    private void SetPTLErrorValues(List<QALine> ptlErrorLines)
    {
        PTLErrorUnits = ptlErrorLines.Sum(l => l.ErrorUnitQty);
        PTLErrorQty = ptlErrorLines.Sum(l => l.ErrorQty);
        PTLErrorLines = ptlErrorLines.Count;
        PTLErrorCartons = ptlErrorLines.Select(l => l.CartonID).Distinct().Count();
    }

    private void SetPTVErrorValues(List<QALine> ptvErrorLines)
    {
        PTVErrorUnits = ptvErrorLines.Sum(l => l.ErrorUnitQty);
        PTVErrorQty = ptvErrorLines.Sum(l => l.ErrorQty);
        PTVErrorLines = ptvErrorLines.Count;
        PTVErrorCartons = ptvErrorLines.Select(l => l.CartonID).Distinct().Count();
    }

    private void SetReplenErrorValues(List<QALine> replenErrorLines)
    {
        ReplenErrorUnits = replenErrorLines.Sum(l => l.ErrorUnitQty);
        ReplenErrorQty = replenErrorLines.Sum(l => l.ErrorQty);
        ReplenErrorLines = replenErrorLines.Count;
        ReplenErrorCartons = replenErrorLines.Select(l => l.CartonID).Distinct().Count();
    }

    private void SetStockingErrorValues(List<QALine> stockingErrorLines)
    {
        StockingErrorUnits = stockingErrorLines.Sum(l => l.ErrorUnitQty);
        StockingErrorQty = stockingErrorLines.Sum(l => l.ErrorQty);
        StockingErrorLines = stockingErrorLines.Count;
        StockingErrorCartons = stockingErrorLines.Select(l => l.CartonID).Distinct().Count();
    }

    private void SetReceiveErrorValues(List<QALine> receiveErrorLines)
    {
        ReceivingErrorUnits = receiveErrorLines.Sum(l => l.ErrorUnitQty);
        ReceivingErrorQty = receiveErrorLines.Sum(l => l.ErrorQty);
        ReceivingErrorLines = receiveErrorLines.Count;
        ReceivingErrorCartons = receiveErrorLines.Select(l => l.CartonID).Distinct().Count();
    }

    private void SetHeatMapErrorValues(List<QALine> heatMapErrorLines)
    {
        HeatMapErrorUnits = heatMapErrorLines.Sum(l => l.ErrorUnitQty);
        HeatMapErrorQty = heatMapErrorLines.Sum(l => l.ErrorQty);
        HeatMapErrorLines = heatMapErrorLines.Count;
        HeatMapErrorCartons = heatMapErrorLines.Select(l => l.CartonID).Distinct().Count();
    }

    private void SetQAErrorValues(List<QALine> qaErrorLines)
    {
        QAErrorUnits = qaErrorLines.Sum(l => l.ErrorUnitQty);
        QAErrorQty = qaErrorLines.Sum(l => l.ErrorQty);
        QAErrorLines = qaErrorLines.Count;
        QAErrorCartons = qaErrorLines.Select(l => l.CartonID).Distinct().Count();
    }

    private void SetOtherDeptErrorValues(List<QALine> otherDeptErrorLines)
    {
        OtherDeptErrorUnits = otherDeptErrorLines.Sum(l => l.ErrorUnitQty);
        OtherDeptErrorQty = otherDeptErrorLines.Sum(l => l.ErrorQty);
        OtherDeptErrorLines = otherDeptErrorLines.Count;
        OtherDeptErrorCartons = otherDeptErrorLines.Select(l => l.CartonID).Distinct().Count();
    }

    public static string GetID(DateTime startDate, DateTime endDate) =>
        $"{startDate.Year:0000}.{startDate.Month:00}.{startDate.Day:00}::{endDate.Year:0000}.{endDate.Month:00}.{endDate.Day:00}";

    public static DataTable GetDataTable(List<QAStats> statList)
    {
        var dt = new DataTable();

        dt.Columns.Add(new DataColumn("Date Description"));
        dt.Columns.Add(new DataColumn("Start Date"));
        dt.Columns.Add(new DataColumn("End Date"));
        dt.Columns.Add(new DataColumn("Restock Qty"));
        dt.Columns.Add(new DataColumn("Restock Hits"));
        dt.Columns.Add(new DataColumn("Restock Cartons"));
        dt.Columns.Add(new DataColumn("QA Units"));
        dt.Columns.Add(new DataColumn("QA Qty"));
        dt.Columns.Add(new DataColumn("QA Lines"));
        dt.Columns.Add(new DataColumn("QA Cartons"));
        dt.Columns.Add(new DataColumn("Average Staff"));
        dt.Columns.Add(new DataColumn("Error Units"));
        dt.Columns.Add(new DataColumn("Error Qty"));
        dt.Columns.Add(new DataColumn("Error Lines"));
        dt.Columns.Add(new DataColumn("Error Cartons"));
        dt.Columns.Add(new DataColumn("External Error Units"));
        dt.Columns.Add(new DataColumn("External Error Qty"));
        dt.Columns.Add(new DataColumn("External Error Lines"));
        dt.Columns.Add(new DataColumn("External Error Cartons"));
        dt.Columns.Add(new DataColumn("Supplier Error Units"));
        dt.Columns.Add(new DataColumn("Supplier Error Qty"));
        dt.Columns.Add(new DataColumn("Supplier Error Lines"));
        dt.Columns.Add(new DataColumn("Supplier Error Cartons"));
        dt.Columns.Add(new DataColumn("System Error Units"));
        dt.Columns.Add(new DataColumn("System Error Qty"));
        dt.Columns.Add(new DataColumn("System Error Lines"));
        dt.Columns.Add(new DataColumn("System Error Cartons"));
        dt.Columns.Add(new DataColumn("Other External Error Units"));
        dt.Columns.Add(new DataColumn("Other External Error Qty"));
        dt.Columns.Add(new DataColumn("Other External Error Lines"));
        dt.Columns.Add(new DataColumn("Other External Error Cartons"));
        dt.Columns.Add(new DataColumn("Warehouse Error Units"));
        dt.Columns.Add(new DataColumn("Warehouse Error Qty"));
        dt.Columns.Add(new DataColumn("Warehouse Error Lines"));
        dt.Columns.Add(new DataColumn("Warehouse Error Cartons"));
        dt.Columns.Add(new DataColumn("Picker Error Units"));
        dt.Columns.Add(new DataColumn("Picker Error Qty"));
        dt.Columns.Add(new DataColumn("Picker Error Lines"));
        dt.Columns.Add(new DataColumn("Picker Error Cartons"));
        dt.Columns.Add(new DataColumn("PTL Error Units"));
        dt.Columns.Add(new DataColumn("PTL Error Qty"));
        dt.Columns.Add(new DataColumn("PTL Error Lines"));
        dt.Columns.Add(new DataColumn("PTL Error Cartons"));
        dt.Columns.Add(new DataColumn("PTV Error Units"));
        dt.Columns.Add(new DataColumn("PTV Error Qty"));
        dt.Columns.Add(new DataColumn("PTV Error Lines"));
        dt.Columns.Add(new DataColumn("PTV Error Cartons"));
        dt.Columns.Add(new DataColumn("Replen Error Units"));
        dt.Columns.Add(new DataColumn("Replen Error Qty"));
        dt.Columns.Add(new DataColumn("Replen Error Lines"));
        dt.Columns.Add(new DataColumn("Replen Error Cartons"));
        dt.Columns.Add(new DataColumn("Stocking Error Units"));
        dt.Columns.Add(new DataColumn("Stocking Error Qty"));
        dt.Columns.Add(new DataColumn("Stocking Error Lines"));
        dt.Columns.Add(new DataColumn("Stocking Error Cartons"));
        dt.Columns.Add(new DataColumn("Receiving Error Units"));
        dt.Columns.Add(new DataColumn("Receiving Error Qty"));
        dt.Columns.Add(new DataColumn("Receiving Error Lines"));
        dt.Columns.Add(new DataColumn("Receiving Error Cartons"));
        dt.Columns.Add(new DataColumn("QA Error Units"));
        dt.Columns.Add(new DataColumn("QA Error Qty"));
        dt.Columns.Add(new DataColumn("QA Error Lines"));
        dt.Columns.Add(new DataColumn("QA Error Cartons"));
        dt.Columns.Add(new DataColumn("HeatMap Error Units"));
        dt.Columns.Add(new DataColumn("HeatMap Error Qty"));
        dt.Columns.Add(new DataColumn("HeatMap Error Lines"));
        dt.Columns.Add(new DataColumn("HeatMap Error Cartons"));
        dt.Columns.Add(new DataColumn("OtherDept Error Units"));
        dt.Columns.Add(new DataColumn("OtherDept Error Qty"));
        dt.Columns.Add(new DataColumn("OtherDept Error Lines"));
        dt.Columns.Add(new DataColumn("OtherDept Error Cartons"));
        dt.Columns.Add(new DataColumn("Seek Units"));
        dt.Columns.Add(new DataColumn("Seek Qty"));
        dt.Columns.Add(new DataColumn("Seek Lines"));
        dt.Columns.Add(new DataColumn("Seek Cartons"));
        dt.Columns.Add(new DataColumn("Seek Units Fixed"));
        dt.Columns.Add(new DataColumn("Seek Qty Fixed"));
        dt.Columns.Add(new DataColumn("Seek Lines Fixed"));
        dt.Columns.Add(new DataColumn("Seek Cartons Fixed"));
        dt.Columns.Add(new DataColumn("Development Opportunity"));
        dt.Columns.Add(new DataColumn("Notes"));
        dt.Columns.Add(new DataColumn("Target Accuracy"));
        dt.Columns.Add(new DataColumn("Date Period"));

        foreach (var stats in statList)
        {
            var row = dt.NewRow();

            row["Date Description"] = stats.DateDescription;
            row["Start Date"] = stats.StartDate;
            row["End Date"] = stats.EndDate;
            row["Average Staff"] = stats.AverageStaff;
            row["Development Opportunity"] = stats.DevelopmentOpportunity;
            row["Notes"] = stats.Notes;
            row["Target Accuracy"] = stats.TargetAccuracy;
            row["Date Period"] = stats.DatePeriod;
            row["Restock Qty"] = stats.RestockQty;
            row["Restock Hits"] = stats.RestockHits;
            row["Restock Cartons"] = stats.RestockCartons;
            row["QA Units"] = stats.QAUnits;
            row["QA Qty"] = stats.QAQty;
            row["QA Lines"] = stats.QALines;
            row["QA Cartons"] = stats.QACartons;
            row["Error Units"] = stats.ErrorUnits;
            row["Error Qty"] = stats.ErrorQty;
            row["Error Lines"] = stats.ErrorLines;
            row["Error Cartons"] = stats.ErrorCartons;
            row["External Error Units"] = stats.ExternalErrorUnits;
            row["External Error Qty"] = stats.ExternalErrorQty;
            row["External Error Lines"] = stats.ExternalErrorLines;
            row["External Error Cartons"] = stats.ExternalErrorCartons;
            row["Supplier Error Units"] = stats.SupplierErrorUnits;
            row["Supplier Error Qty"] = stats.SupplierErrorQty;
            row["Supplier Error Lines"] = stats.SupplierErrorLines;
            row["Supplier Error Cartons"] = stats.SupplierErrorCartons;
            row["System Error Units"] = stats.SystemErrorUnits;
            row["System Error Qty"] = stats.SystemErrorQty;
            row["System Error Lines"] = stats.SystemErrorLines;
            row["System Error Cartons"] = stats.SystemErrorCartons;
            row["Other External Error Units"] = stats.OtherExternalErrorUnits;
            row["Other External Error Qty"] = stats.OtherExternalErrorQty;
            row["Other External Error Lines"] = stats.OtherExternalErrorLines;
            row["Other External Error Cartons"] = stats.OtherExternalErrorCartons;
            row["Warehouse Error Units"] = stats.WHErrorUnits;
            row["Warehouse Error Qty"] = stats.WHErrorQty;
            row["Warehouse Error Lines"] = stats.WHErrorLines;
            row["Warehouse Error Cartons"] = stats.WHErrorCartons;
            row["Picker Error Units"] = stats.PickerErrorUnits;
            row["Picker Error Qty"] = stats.PickerErrorQty;
            row["Picker Error Lines"] = stats.PickerErrorLines;
            row["Picker Error Cartons"] = stats.PickerErrorCartons;
            row["PTL Error Units"] = stats.PTLErrorUnits;
            row["PTL Error Qty"] = stats.PTLErrorQty;
            row["PTL Error Lines"] = stats.PTLErrorLines;
            row["PTL Error Cartons"] = stats.PTLErrorCartons;
            row["PTV Error Units"] = stats.PTVErrorUnits;
            row["PTV Error Qty"] = stats.PTVErrorQty;
            row["PTV Error Lines"] = stats.PTVErrorLines;
            row["PTV Error Cartons"] = stats.PTVErrorCartons;
            row["Replen Error Units"] = stats.ReplenErrorUnits;
            row["Replen Error Qty"] = stats.ReplenErrorQty;
            row["Replen Error Lines"] = stats.ReplenErrorLines;
            row["Replen Error Cartons"] = stats.ReplenErrorCartons;
            row["Stocking Error Units"] = stats.StockingErrorUnits;
            row["Stocking Error Qty"] = stats.StockingErrorQty;
            row["Stocking Error Lines"] = stats.StockingErrorLines;
            row["Stocking Error Cartons"] = stats.StockingErrorCartons;
            row["Receiving Error Units"] = stats.ReceivingErrorUnits;
            row["Receiving Error Qty"] = stats.ReceivingErrorQty;
            row["Receiving Error Lines"] = stats.ReceivingErrorLines;
            row["Receiving Error Cartons"] = stats.ReceivingErrorCartons;
            row["HeatMap Error Units"] = stats.HeatMapErrorUnits;
            row["HeatMap Error Qty"] = stats.HeatMapErrorQty;
            row["HeatMap Error Lines"] = stats.HeatMapErrorLines;
            row["HeatMap Error Cartons"] = stats.HeatMapErrorCartons;
            row["QA Error Units"] = stats.QAErrorUnits;
            row["QA Error Qty"] = stats.QAErrorQty;
            row["QA Error Lines"] = stats.QAErrorLines;
            row["QA Error Cartons"] = stats.QAErrorCartons;
            row["OtherDept Error Units"] = stats.OtherDeptErrorUnits;
            row["OtherDept Error Qty"] = stats.OtherDeptErrorQty;
            row["OtherDept Error Lines"] = stats.OtherDeptErrorLines;
            row["OtherDept Error Cartons"] = stats.OtherDeptErrorCartons;
            row["Seek Units"] = stats.SeekUnits;
            row["Seek Qty"] = stats.SeekQty;
            row["Seek Lines"] = stats.SeekLines;
            row["Seek Cartons"] = stats.SeekCartons;
            row["Seek Units Fixed"] = stats.SeekUnitsFixed;
            row["Seek Qty Fixed"] = stats.SeekQtyFixed;
            row["Seek Lines Fixed"] = stats.SeekLinesFixed;
            row["Seek Cartons Fixed"] = stats.SeekCartonsFixed;

            dt.Rows.Add(row);
        }

        return dt;
    }
}
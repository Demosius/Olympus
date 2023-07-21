using System;
using System.Collections.Generic;
using System.Linq;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Uranus.Staff.Models;

public enum ECartonStatus
{
    FullyShipped,
    PartlyShipped,
    NotShipped
}

public class QACarton
{
    [PrimaryKey] public string ID { get; set; }
    public string StoreNo { get; set; }
    public string StoreName { get; set; }
    public ECartonStatus CartonStatus { get; set; }
    public string ShipmentNumber { get; set; }
    public string WarehouseCode { get; set; }
    public string CartonType { get; set; }
    public string EmployeeID { get; set; }  // PC ID
    [Indexed] public DateTime Date { get; set; }
    public TimeSpan Time { get; set; }
    public bool Pass { get; set; }
    public string QAStatus { get; set; }
    public string BatchID { get; set; }
    public double Depth { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public double MaxWeight { get; set; }
    public double MaxCube { get; set; }
    public double CurrentWeight { get; set; }
    public double CurrentCube { get; set; }

    [Ignore] public string QABy => EmployeeID;
    [Ignore] public Employee? QAOperator { get; set; }

    [OneToMany(nameof(QALine.CartonID), nameof(QALine.QACarton), CascadeOperations = CascadeOperation.CascadeRead)]
    public List<QALine> QALines { get; set; }

    [Ignore] public DateTime DateTime => Date.AddTicks(Time.Ticks);
    private QACarton? nextCarton;
    [Ignore] public QACarton? NextCarton
    {
        get => nextCarton;
        set
        {
            nextCarton = value;
            QATime = NextCarton is null ? TimeSpan.Zero : NextCarton.DateTime.Subtract(DateTime);
        }
    }

    [Ignore] public TimeSpan QATime { get; set; }

    [Ignore] public int QAScans => QALines.Sum(l => l.QAQty);
    [Ignore] public int QAUnits => QALines.Sum(l => l.PickQtyBase);
    [Ignore] public bool QAError => QALines.Any(l => l.QAError);
    [Ignore] public int QAErrorItems => QALines.Count(l => l.QAError);
    [Ignore] public int QAErrorScans => QALines.Where(l => l.QAError).Sum(l => Math.Abs(l.VarianceQty));
    [Ignore] public int QAErrorUnits => QALines.Where(l => l.QAError).Sum(l => Math.Abs(l.UnitVariance));

    public QACarton()
    {
        ID = Guid.NewGuid().ToString();
        StoreNo = string.Empty;
        StoreName = string.Empty;
        ShipmentNumber = string.Empty;
        WarehouseCode = string.Empty;
        CartonType = string.Empty;
        EmployeeID = string.Empty;
        QAStatus = string.Empty;
        BatchID = string.Empty;

        QALines = new List<QALine>();
    }

    public void AddQALine(QALine line)
    {
        line.QACarton = this;
        QALines.Add(line);
    }
}
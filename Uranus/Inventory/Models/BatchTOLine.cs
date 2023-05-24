using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Uranus.Inventory.Models;

public class BatchTOLine
{
    [PrimaryKey] public string CCN { get; set; }
    [ForeignKey(typeof(Store))] public string StoreNo { get; set; }
    public int Cartons { get; set; }
    public double Weight { get; set; }
    public double Cube { get; set; }
    public string CartonType { get; set; }
    public string StartingPickZone { get; set; }
    public string EndingPickZone { get; set; }
    public string StartingPickBin { get; set; }
    public string EndingPickBin { get; set; }
    [ForeignKey(typeof(Batch))] public string BatchID { get; set; }
    public DateTime Date { get; set; }
    public int UnitsBase { get; set; }
    public string WaveNo { get; set; }

    // Used for x cartons.
    public int ItemNumber { get; set; }
    public string Description { get; set; } // Item description for x Carton, [CCN] [BatchID] F[file number] for others.

    public string OriginalFileName { get; set; }    // Not including extension.

    public BatchTOLine()
    {
        StoreNo = string.Empty;
        CCN = string.Empty;
        CartonType = string.Empty;
        StartingPickZone = string.Empty;
        EndingPickZone = string.Empty;
        StartingPickBin = string.Empty;
        EndingPickBin = string.Empty;
        BatchID = string.Empty;
        WaveNo = string.Empty;
        Description = string.Empty;
        OriginalFileName = string.Empty;
    }
}
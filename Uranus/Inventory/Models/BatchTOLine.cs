using System;
using System.Collections.Generic;
using System.IO;
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
    public string StartZone { get; set; }
    public string EndZone { get; set; }
    public string StartBin { get; set; }
    public string EndBin { get; set; }
    [ForeignKey(typeof(Batch))] public string BatchID { get; set; }
    public DateTime Date { get; set; }
    public int UnitsBase { get; set; }
    public string WaveNo { get; set; }
    public string FreightRegion { get; set; }

    // Calculated from Zone and Bin, used for many checks.
    public string StartBay { get; set; }
    public string EndBay { get; set; }

    // Used for x cartons.
    public int ItemNumber { get; set; } = 111111;
    public string Description { get; set; } // Item description for x Carton, [CCN] [BatchID] F[file number] for others.

    // Used to track file locations and process for dispatch and label printing.
    public string OriginalFileDirectory { get; set; }
    public string OriginalFileName { get; set; }    // Not including extension.
    public string FinalFileDirectory { get; set; }
    public string FinalFileName { get; set; }

    // For managing processing and finalization.
    public DateTime OriginalProcessingTime { get; set; }
    public DateTime? FinalProcessingTime { get; set; }

    public bool IsFinalised { get; set; }

    // For splitting/merging into groups.
    [ForeignKey(typeof(BatchTOGroup))] public Guid GroupID { get; set; }

    [ManyToOne(nameof(BatchID), nameof(Models.Batch.TOLines), CascadeOperations = CascadeOperation.CascadeRead)]
    public Batch? Batch { get; set; }
    [ManyToOne(nameof(GroupID), nameof(BatchTOGroup.Lines), CascadeOperations = CascadeOperation.CascadeRead)]
    public BatchTOGroup? Group { get; set; }
    [ManyToOne(nameof(StoreNo), nameof(Models.Store.BatchTOLines), CascadeOperations = CascadeOperation.CascadeRead)]
    public Store? Store { get; set; }

    [OneToMany(nameof(PickLine.CartonID), nameof(PickLine.BatchTOLine), CascadeOperations = CascadeOperation.CascadeRead)]
    public List<PickLine> PickLines { get; set; }

    public BatchTOLine()
    {
        StoreNo = string.Empty;
        CCN = string.Empty;
        CartonType = string.Empty;
        StartZone = string.Empty;
        EndZone = string.Empty;
        StartBin = string.Empty;
        EndBin = string.Empty;
        BatchID = string.Empty;
        WaveNo = string.Empty;
        Description = string.Empty;
        OriginalFileDirectory = string.Empty;
        OriginalFileName = string.Empty;
        FinalFileDirectory = string.Empty;
        FinalFileName = string.Empty;
        OriginalProcessingTime = DateTime.Now;
        FinalProcessingTime = null;
        StartBay = string.Empty;
        EndBay = string.Empty;
        FreightRegion = string.Empty;

        PickLines = new List<PickLine>();
    }

    public void Finalise(string fileDirectory, string fileName)
    {
        FinalFileDirectory = fileDirectory;
        FinalFileName = fileName;
        IsFinalised = true;
        FinalProcessingTime = DateTime.Now;
    }

    public void Finalise(string filePath) =>
        Finalise(Path.GetDirectoryName(filePath) ?? string.Empty, Path.GetFileName(filePath));


    public void SetBays()
    {
        StartBay = GetBay(StartZone, StartBin);
        EndBay = GetBay(EndZone, EndBin);
    }

    public static string GetBay(string zone, string bin) =>
        zone.StartsWith("A") ? "A" :
        zone.StartsWith("S") ? "SP01" :
        bin.Length > 0 ? bin[..1] : 
        string.Empty;

    public void SetStore(Store store)
    {
        Store = store;
        StoreNo = store.Number;
        FreightRegion = Store.CCNRegion;
    }
}
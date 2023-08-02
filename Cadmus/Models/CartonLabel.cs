using System;

namespace Cadmus.Models;

public class CartonLabel
{
    public string StoreNo { get; set; }
    public int Cartons { get; set; }
    public double Weight { get; set; }
    public double Cube { get; set; }
    public string CCN { get; set; }
    public string Barcode { get; set; }
    public string CartonType { get; set; }
    public string StartZone { get; set; }
    public string EndZone { get; set; }
    public string StartBin { get; set; }
    public string EndBin { get; set; }
    public string TOBatchNo { get; set; }
    public DateTime Date { get; set; }
    public int TotalUnits { get; set; }
    public string WaveNo { get; set; }
    public string StockDescriptor { get; set; }
    public string Carrier { get; set; }

    public CartonLabel()
    {
        StoreNo = string.Empty;
        CCN = string.Empty;
        Barcode = string.Empty;
        CartonType = string.Empty;
        StartZone = string.Empty;
        EndZone = string.Empty;
        StartBin = string.Empty;
        EndBin = string.Empty;
        TOBatchNo = string.Empty;
        WaveNo = string.Empty;
        StockDescriptor = string.Empty;
        Carrier = string.Empty;
    }
}
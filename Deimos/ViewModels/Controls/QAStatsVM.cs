using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Deimos.Interfaces;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;
using Uranus.Staff.Models;

namespace Deimos.ViewModels.Controls;

public class QAStatsVM : INotifyPropertyChanged, IDBInteraction, IQAStatsVM
{
    public Helios Helios { get; set; }
    public QAStats Stats { get; set; }

    public string Description => $"{StartDate:dd-MMM-yyyy} to {EndDate:dd-MMM-yyyy}";
    public bool Actual => true;

    #region QAStats Access

    public string ID => Stats.ID;
    public DateTime StartDate => Stats.StartDate;
    public DateTime EndDate => Stats.EndDate;

    /* Base Stats */
    public int RestockQty => Stats.RestockQty;
    public int RestockHits => Stats.RestockHits;
    public int RestockCartons => Stats.RestockCartons;
    public int QAUnits => Stats.QAUnits;
    public int QAQty => Stats.QAQty;
    public int QALines => Stats.QALines;
    public int QACartons => Stats.QACartons;
    public double AverageStaff => Stats.AverageStaff;

    /* Total Errors */
    public int ErrorUnits => Stats.ErrorUnits;
    public int ErrorQty => Stats.ErrorQty;
    public int ErrorLines => Stats.ErrorLines;
    public int ErrorCartons => Stats.ErrorCartons;

    /* External Errors */
    public int ExternalErrorUnits => Stats.ExternalErrorUnits;
    public int ExternalErrorQty => Stats.ExternalErrorQty;
    public int ExternalErrorLines => Stats.ExternalErrorLines;
    public int ExternalErrorCartons => Stats.ExternalErrorCartons;
    public int SupplierErrorUnits => Stats.SupplierErrorUnits;
    public int SupplierErrorQty => Stats.SupplierErrorQty;
    public int SupplierErrorLines => Stats.SupplierErrorLines;
    public int SupplierErrorCartons => Stats.SupplierErrorCartons;
    public int SystemErrorUnits => Stats.SystemErrorUnits;
    public int SystemErrorQty => Stats.SystemErrorQty;
    public int SystemErrorLines => Stats.SystemErrorLines;
    public int SystemErrorCartons => Stats.SystemErrorCartons;
    public int OtherExternalErrorUnits => Stats.OtherExternalErrorUnits;
    public int OtherExternalErrorQty => Stats.OtherExternalErrorQty;
    public int OtherExternalErrorLines => Stats.OtherExternalErrorLines;
    public int OtherExternalErrorCartons => Stats.OtherExternalErrorCartons;

    /* Warehouse Errors */
    // Total
    public int WHErrorUnits => Stats.WHErrorUnits;
    public int WHErrorQty => Stats.WHErrorQty;
    public int WHErrorLines => Stats.WHErrorLines;
    public int WHErrorCartons => Stats.WHErrorCartons;
    // Pickers
    public int PickerErrorUnits => Stats.PickerErrorUnits;
    public int PickerErrorQty => Stats.PickerErrorQty;
    public int PickerErrorLines => Stats.PickerErrorLines;
    public int PickerErrorCartons => Stats.PickerErrorCartons;
    public int PTVErrorUnits => Stats.PTVErrorUnits;
    public int PTVErrorQty => Stats.PTVErrorQty;
    public int PTVErrorLines => Stats.PTVErrorLines;
    public int PTVErrorCartons => Stats.PTVErrorCartons;
    public int PTLErrorUnits => Stats.PTLErrorUnits;
    public int PTLErrorQty => Stats.PTLErrorQty;
    public int PTLErrorLines => Stats.PTLErrorLines;
    public int PTLErrorCartons => Stats.PTLErrorCartons;
    // Other
    public int OtherDeptErrorUnits => Stats.OtherDeptErrorUnits;
    public int OtherDeptErrorQty => Stats.OtherDeptErrorQty;
    public int OtherDeptErrorLines => Stats.OtherDeptErrorLines;
    public int OtherDeptErrorCartons => Stats.OtherDeptErrorCartons;
    public int ReplenErrorUnits => Stats.ReplenErrorUnits;
    public int ReplenErrorQty => Stats.ReplenErrorQty;
    public int ReplenErrorLines => Stats.ReplenErrorLines;
    public int ReplenErrorCartons => Stats.ReplenErrorCartons;
    public int StockingErrorUnits => Stats.StockingErrorUnits;
    public int StockingErrorQty => Stats.StockingErrorQty;
    public int StockingErrorLines => Stats.StockingErrorLines;
    public int StockingErrorCartons => Stats.StockingErrorCartons;
    public int ReceivingErrorUnits => Stats.ReceivingErrorUnits;
    public int ReceivingErrorQty => Stats.ReceivingErrorQty;
    public int ReceivingErrorLines => Stats.ReceivingErrorLines;
    public int ReceivingErrorCartons => Stats.ReceivingErrorCartons;
    public int HeatMapErrorUnits => Stats.HeatMapErrorUnits;
    public int HeatMapErrorQty => Stats.HeatMapErrorQty;
    public int HeatMapErrorLines => Stats.HeatMapErrorLines;
    public int HeatMapErrorCartons => Stats.HeatMapErrorCartons;
    public int QAErrorUnits => Stats.QAErrorUnits;
    public int QAErrorQty => Stats.QAErrorQty;
    public int QAErrorLines => Stats.QAErrorLines;
    public int QAErrorCartons => Stats.QAErrorCartons;

    /* Seeking */
    public int SeekUnits => Stats.SeekUnits;
    public int SeekQty => Stats.SeekQty;
    public int SeekLines => Stats.SeekLines;
    public int SeekCartons => Stats.SeekCartons;
    public int SeekUnitsFixed => Stats.SeekUnitsFixed;
    public int SeekQtyFixed => Stats.SeekQtyFixed;
    public int SeekLinesFixed => Stats.SeekLinesFixed;
    public int SeekCartonsFixed => Stats.SeekCartonsFixed;


    /* Adjustable Variables*/
    public string DevelopmentOpportunity
    {
        get => Stats.DevelopmentOpportunity;
        set
        {
            Stats.DevelopmentOpportunity = value;
            OnPropertyChanged();
            _ = Save();
        }
    }

    public string Notes
    {
        get => Stats.Notes;
        set
        {
            Stats.Notes = value;
            OnPropertyChanged();
            _ = Save();
        }
    }

    public double TargetAccuracy
    {
        get => Stats.TargetAccuracy;
        set
        {
            Stats.TargetAccuracy = value;
            OnPropertyChanged();
            _ = Save();
        }
    }
    
    public string TargetAccuracyString
    {
        get => TargetAccuracy.ToString("#0.##%");
        set
        {
            if (double.TryParse(value, out var target))
                TargetAccuracy = target;
            OnPropertyChanged();
        }
    }

    public double PctQtyQA => (double)QAQty / NZ(RestockQty, NZ(QAQty));
    public double PctLinesQA => (double)QALines / NZ(RestockHits, NZ(QALines));
    public double PctCartonsQA => (double)QACartons / NZ(RestockCartons, NZ(QACartons));

    public double OverallUnitAccuracy => (double)(QAUnits - ErrorUnits) / NZ(QAUnits);
    public double OverallQtyAccuracy => (double)(QAQty - ErrorQty) / NZ(QAQty);
    public double OverallLineAccuracy => (double)(QALines - ErrorLines) / NZ(QALines);
    public double OverallCartonAccuracy => (double)(QACartons - ErrorCartons) / NZ(QACartons);

    public double WHUnitAccuracy => (double)(QAUnits - WHErrorUnits) / NZ(QAUnits);
    public double WHQtyAccuracy => (double)(QAQty - WHErrorQty) / NZ(QAQty);
    public double WHLineAccuracy => (double)(QALines - WHErrorLines) / NZ(QALines);
    public double WHCartonAccuracy => (double)(QACartons - WHErrorCartons) / NZ(QACartons);

    public double PickerUnitAccuracy => (double)(QAUnits - PickerErrorUnits) / NZ(QAUnits);
    public double PickerQtyAccuracy => (double)(QAQty - PickerErrorQty) / NZ(QAQty);
    public double PickerLineAccuracy => (double)(QALines - PickerErrorLines) / NZ(QALines);
    public double PickerCartonAccuracy => (double)(QACartons - PickerErrorCartons) / NZ(QACartons);

    public double SupplierUnitPct => (double)SupplierErrorUnits / NZ(ExternalErrorUnits);
    public double SupplierQtyPct => (double)SupplierErrorQty / NZ(ExternalErrorQty);
    public double SupplierLinePct => (double)SupplierErrorLines / NZ(ExternalErrorLines);
    public double SupplierCartonPct => (double)SupplierErrorCartons / NZ(ExternalErrorCartons);

    public double SystemUnitPct => (double)SystemErrorUnits / NZ(ExternalErrorUnits);
    public double SystemQtyPct => (double)SystemErrorQty / NZ(ExternalErrorQty);
    public double SystemLinePct => (double)SystemErrorLines / NZ(ExternalErrorLines);
    public double SystemCartonPct => (double)SystemErrorCartons / NZ(ExternalErrorCartons);

    public double OtherExternalUnitPct => (double)OtherExternalErrorUnits / NZ(ExternalErrorUnits);
    public double OtherExternalQtyPct => (double)OtherExternalErrorQty / NZ(ExternalErrorQty);
    public double OtherExternalLinePct => (double)OtherExternalErrorLines / NZ(ExternalErrorLines);
    public double OtherExternalCartonPct => (double)OtherExternalErrorCartons / NZ(ExternalErrorCartons);

    public double PTLUnitPct => (double)PTLErrorUnits / NZ(WHErrorUnits);
    public double PTLQtyPct => (double)PTLErrorQty / NZ(WHErrorQty);
    public double PTLLinePct => (double)PTLErrorLines / NZ(WHErrorLines);
    public double PTLCartonPct => (double)PTLErrorCartons / NZ(WHErrorCartons);

    public double PTVUnitPct => (double)PTVErrorUnits / NZ(WHErrorUnits);
    public double PTVQtyPct => (double)PTVErrorQty / NZ(WHErrorQty);
    public double PTVLinePct => (double)PTVErrorLines / NZ(WHErrorLines);
    public double PTVCartonPct => (double)PTVErrorCartons / NZ(WHErrorCartons);

    public double ReplenUnitPct => (double)ReplenErrorUnits / NZ(WHErrorUnits);
    public double ReplenQtyPct => (double)ReplenErrorQty / NZ(WHErrorQty);
    public double ReplenLinePct => (double)ReplenErrorLines / NZ(WHErrorLines);
    public double ReplenCartonPct => (double)ReplenErrorCartons / NZ(WHErrorCartons);

    public double StockingUnitPct => (double)StockingErrorUnits / NZ(WHErrorUnits);
    public double StockingQtyPct => (double)StockingErrorQty / NZ(WHErrorQty);
    public double StockingLinePct => (double)StockingErrorLines / NZ(WHErrorLines);
    public double StockingCartonPct => (double)StockingErrorCartons / NZ(WHErrorCartons);

    public double ReceivingUnitPct => (double)ReceivingErrorUnits / NZ(WHErrorUnits);
    public double ReceivingQtyPct => (double)ReceivingErrorQty / NZ(WHErrorQty);
    public double ReceivingLinePct => (double)ReceivingErrorLines / NZ(WHErrorLines);
    public double ReceivingCartonPct => (double)ReceivingErrorCartons / NZ(WHErrorCartons);

    public double HeatMapUnitPct => (double)HeatMapErrorUnits / NZ(WHErrorUnits);
    public double HeatMapQtyPct => (double)HeatMapErrorQty / NZ(WHErrorQty);
    public double HeatMapLinePct => (double)HeatMapErrorLines / NZ(WHErrorLines);
    public double HeatMapCartonPct => (double)HeatMapErrorCartons / NZ(WHErrorCartons);

    public double QAUnitPct => (double)QAErrorUnits / NZ(WHErrorUnits);
    public double QAQtyPct => (double)QAErrorQty / NZ(WHErrorQty);
    public double QALinePct => (double)QAErrorLines / NZ(WHErrorLines);
    public double QACartonPct => (double)QAErrorCartons / NZ(WHErrorCartons);

    public double OtherDeptUnitPct => (double)OtherDeptErrorUnits / NZ(WHErrorUnits);
    public double OtherDeptQtyPct => (double)OtherDeptErrorQty / NZ(WHErrorQty);
    public double OtherDeptLinePct => (double)OtherDeptErrorLines / NZ(WHErrorLines);
    public double OtherDeptCartonPct => (double)OtherDeptErrorCartons / NZ(WHErrorCartons);

    #endregion

    #region INotifyPropertyChanged Members



    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }

    #endregion

    public QAStatsVM(QAStats stats, Helios helios)
    {
        Stats = stats;
        Helios = helios;

        RefreshDataCommand = new RefreshDataCommand(this);
    }

    public Task RefreshDataAsync()
    {
        throw new NotImplementedException();
    }

    private static int NZ(int n, int alternate = 1) => n == 0 ? alternate : n;

    public async Task Save() => await Helios.StaffUpdater.QAStatsAsync(Stats);

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
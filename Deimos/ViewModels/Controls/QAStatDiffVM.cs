using System.ComponentModel;
using System.Runtime.CompilerServices;
using Deimos.Interfaces;
using Uranus.Annotations;

namespace Deimos.ViewModels.Controls;

public class QAStatDiffVM : INotifyPropertyChanged, IQAStatsVM
{
    public string Description => $"";
    public bool Actual => false;

    public string TargetAccuracyString
    {
        get => (Stats2.TargetAccuracy - Stats1.TargetAccuracy).ToString("#0.##%");
        set { }
    }
    public string DevelopmentOpportunity { get; set; }
    public string Notes { get; set; }

    #region INotifyPropertyChangedMembers

    private QAStatsVM stats1;
    public QAStatsVM Stats1
    {
        get => stats1;
        set
        {
            stats1 = value;
            OnPropertyChanged();
            Refresh();
        }
    }

    private QAStatsVM stats2;
    public QAStatsVM Stats2
    {
        get => stats2;
        set
        {
            stats2 = value;
            OnPropertyChanged();
            Refresh();
        }
    }

    #endregion

    #region DiffValues

    public double AverageStaff => Stats2.AverageStaff - Stats1.AverageStaff;

    public int RestockQty => Stats2.RestockQty - Stats1.RestockQty;
    public int RestockHits => Stats2.RestockHits - Stats1.RestockHits;
    public int RestockCartons => Stats2.RestockCartons - Stats1.RestockCartons;

    public int QAUnits => Stats2.QAUnits - Stats1.QAUnits;
    public int QAQty => Stats2.QAQty - Stats1.QAQty;
    public int QALines => Stats2.QALines - Stats1.QALines;
    public int QACartons => Stats2.QACartons - Stats1.QACartons;

    public double PctQtyQA => Stats2.PctQtyQA - Stats1.PctQtyQA;
    public double PctLinesQA => Stats2.PctLinesQA - Stats1.PctLinesQA;
    public double PctCartonsQA => Stats2.PctCartonsQA - Stats1.PctCartonsQA;

    public int ErrorUnits => Stats2.ErrorUnits - Stats1.ErrorUnits;
    public int ErrorQty => Stats2.ErrorQty - Stats1.ErrorQty;
    public int ErrorLines => Stats2.ErrorLines - Stats1.ErrorLines;
    public int ErrorCartons => Stats2.ErrorCartons - Stats1.ErrorCartons;

    public double OverallUnitAccuracy => Stats2.OverallUnitAccuracy - Stats1.OverallUnitAccuracy;
    public double OverallQtyAccuracy => Stats2.OverallQtyAccuracy - Stats1.OverallQtyAccuracy;
    public double OverallLineAccuracy => Stats2.OverallLineAccuracy - Stats1.OverallLineAccuracy;
    public double OverallCartonAccuracy => Stats2.OverallCartonAccuracy - Stats1.OverallCartonAccuracy;

    public int ExternalErrorUnits => Stats2.ExternalErrorUnits - Stats1.ExternalErrorUnits;
    public int ExternalErrorQty => Stats2.ExternalErrorQty - Stats1.ExternalErrorQty;
    public int ExternalErrorLines => Stats2.ExternalErrorLines - Stats1.ExternalErrorLines;
    public int ExternalErrorCartons => Stats2.ExternalErrorCartons - Stats1.ExternalErrorCartons;

    public int WHErrorUnits => Stats2.WHErrorUnits - Stats1.WHErrorUnits;
    public int WHErrorQty => Stats2.WHErrorQty - Stats1.WHErrorQty;
    public int WHErrorLines => Stats2.WHErrorLines - Stats1.WHErrorLines;
    public int WHErrorCartons => Stats2.WHErrorCartons - Stats1.WHErrorCartons;

    public double WHUnitAccuracy => Stats2.WHUnitAccuracy - Stats1.WHUnitAccuracy;
    public double WHQtyAccuracy => Stats2.WHQtyAccuracy - Stats1.WHQtyAccuracy;
    public double WHLineAccuracy => Stats2.WHLineAccuracy - Stats1.WHLineAccuracy;
    public double WHCartonAccuracy => Stats2.WHCartonAccuracy - Stats1.WHCartonAccuracy;

    public int PickerErrorUnits => Stats2.PickerErrorUnits - Stats1.PickerErrorUnits;
    public int PickerErrorQty => Stats2.PickerErrorQty - Stats1.PickerErrorQty;
    public int PickerErrorLines => Stats2.PickerErrorLines - Stats1.PickerErrorLines;
    public int PickerErrorCartons => Stats2.PickerErrorCartons - Stats1.PickerErrorCartons;

    public double PickerUnitAccuracy => Stats2.PickerUnitAccuracy - Stats1.PickerUnitAccuracy;
    public double PickerQtyAccuracy => Stats2.PickerQtyAccuracy - Stats1.PickerQtyAccuracy;
    public double PickerLineAccuracy => Stats2.PickerLineAccuracy - Stats1.PickerLineAccuracy;
    public double PickerCartonAccuracy => Stats2.PickerCartonAccuracy - Stats1.PickerCartonAccuracy;

    public int SeekUnits => Stats2.SeekUnits - Stats1.SeekUnits;
    public int SeekQty => Stats2.SeekQty - Stats1.SeekQty;
    public int SeekLines => Stats2.SeekLines - Stats1.SeekLines;
    public int SeekCartons => Stats2.SeekCartons - Stats1.SeekCartons;

    public int SeekUnitsFixed => Stats2.SeekUnitsFixed - Stats1.SeekUnitsFixed;
    public int SeekQtyFixed => Stats2.SeekQtyFixed - Stats1.SeekQtyFixed;
    public int SeekLinesFixed => Stats2.SeekLinesFixed - Stats1.SeekLinesFixed;
    public int SeekCartonsFixed => Stats2.SeekCartonsFixed - Stats1.SeekCartonsFixed;

    public int SupplierErrorUnits => Stats2.SupplierErrorUnits - Stats1.SupplierErrorUnits;
    public int SupplierErrorQty => Stats2.SupplierErrorQty - Stats1.SupplierErrorQty;
    public int SupplierErrorLines => Stats2.SupplierErrorLines - Stats1.SupplierErrorLines;
    public int SupplierErrorCartons => Stats2.SupplierErrorCartons - Stats1.SupplierErrorCartons;

    public double SupplierUnitPct => Stats2.SupplierUnitPct - Stats1.SupplierUnitPct;
    public double SupplierQtyPct => Stats2.SupplierQtyPct - Stats1.SupplierQtyPct;
    public double SupplierLinePct => Stats2.SupplierLinePct - Stats1.SupplierLinePct;
    public double SupplierCartonPct => Stats2.SupplierCartonPct - Stats1.SupplierCartonPct;

    public int SystemErrorUnits => Stats2.SystemErrorUnits - Stats1.SystemErrorUnits;
    public int SystemErrorQty => Stats2.SystemErrorQty - Stats1.SystemErrorQty;
    public int SystemErrorLines => Stats2.SystemErrorLines - Stats1.SystemErrorLines;
    public int SystemErrorCartons => Stats2.SystemErrorCartons - Stats1.SystemErrorCartons;

    public double SystemUnitPct => Stats2.SystemUnitPct - Stats1.SystemUnitPct;
    public double SystemQtyPct => Stats2.SystemQtyPct - Stats1.SystemQtyPct;
    public double SystemLinePct => Stats2.SystemLinePct - Stats1.SystemLinePct;
    public double SystemCartonPct => Stats2.SystemCartonPct - Stats1.SystemCartonPct;

    public int OtherExternalErrorUnits => Stats2.OtherExternalErrorUnits - Stats1.OtherExternalErrorUnits;
    public int OtherExternalErrorQty => Stats2.OtherExternalErrorQty - Stats1.OtherExternalErrorQty;
    public int OtherExternalErrorLines => Stats2.OtherExternalErrorLines - Stats1.OtherExternalErrorLines;
    public int OtherExternalErrorCartons => Stats2.OtherExternalErrorCartons - Stats1.OtherExternalErrorCartons;

    public double OtherExternalUnitPct => Stats2.OtherExternalUnitPct - Stats1.OtherExternalUnitPct;
    public double OtherExternalQtyPct => Stats2.OtherExternalQtyPct - Stats1.OtherExternalQtyPct;
    public double OtherExternalLinePct => Stats2.OtherExternalLinePct - Stats1.OtherExternalLinePct;
    public double OtherExternalCartonPct => Stats2.OtherExternalCartonPct - Stats1.OtherExternalCartonPct;

    public int PTLErrorUnits => Stats2.PTLErrorUnits - Stats1.PTLErrorUnits;
    public int PTLErrorQty => Stats2.PTLErrorQty - Stats1.PTLErrorQty;
    public int PTLErrorLines => Stats2.PTLErrorLines - Stats1.PTLErrorLines;
    public int PTLErrorCartons => Stats2.PTLErrorCartons - Stats1.PTLErrorCartons;

    public double PTLUnitPct => Stats2.PTLUnitPct - Stats1.PTLUnitPct;
    public double PTLQtyPct => Stats2.PTLQtyPct - Stats1.PTLQtyPct;
    public double PTLLinePct => Stats2.PTLLinePct - Stats1.PTLLinePct;
    public double PTLCartonPct => Stats2.PTLCartonPct - Stats1.PTLCartonPct;

    public int PTVErrorUnits => Stats2.PTVErrorUnits - Stats1.PTVErrorUnits;
    public int PTVErrorQty => Stats2.PTVErrorQty - Stats1.PTVErrorQty;
    public int PTVErrorLines => Stats2.PTVErrorLines - Stats1.PTVErrorLines;
    public int PTVErrorCartons => Stats2.PTVErrorCartons - Stats1.PTVErrorCartons;

    public double PTVUnitPct => Stats2.PTVUnitPct - Stats1.PTVUnitPct;
    public double PTVQtyPct => Stats2.PTVQtyPct - Stats1.PTVQtyPct;
    public double PTVLinePct => Stats2.PTVLinePct - Stats1.PTVLinePct;
    public double PTVCartonPct => Stats2.PTVCartonPct - Stats1.PTVCartonPct;

    public int StockingErrorUnits => Stats2.StockingErrorUnits - Stats1.StockingErrorUnits;
    public int StockingErrorQty => Stats2.StockingErrorQty - Stats1.StockingErrorQty;
    public int StockingErrorLines => Stats2.StockingErrorLines - Stats1.StockingErrorLines;
    public int StockingErrorCartons => Stats2.StockingErrorCartons - Stats1.StockingErrorCartons;

    public double StockingUnitPct => Stats2.StockingUnitPct - Stats1.StockingUnitPct;
    public double StockingQtyPct => Stats2.StockingQtyPct - Stats1.StockingQtyPct;
    public double StockingLinePct => Stats2.StockingLinePct - Stats1.StockingLinePct;
    public double StockingCartonPct => Stats2.StockingCartonPct - Stats1.StockingCartonPct;

    public int ReplenErrorUnits => Stats2.ReplenErrorUnits - Stats1.ReplenErrorUnits;
    public int ReplenErrorQty => Stats2.ReplenErrorQty - Stats1.ReplenErrorQty;
    public int ReplenErrorLines => Stats2.ReplenErrorLines - Stats1.ReplenErrorLines;
    public int ReplenErrorCartons => Stats2.ReplenErrorCartons - Stats1.ReplenErrorCartons;

    public double ReplenUnitPct => Stats2.ReplenUnitPct - Stats1.ReplenUnitPct;
    public double ReplenQtyPct => Stats2.ReplenQtyPct - Stats1.ReplenQtyPct;
    public double ReplenLinePct => Stats2.ReplenLinePct - Stats1.ReplenLinePct;
    public double ReplenCartonPct => Stats2.ReplenCartonPct - Stats1.ReplenCartonPct;

    public int ReceivingErrorUnits => Stats2.ReceivingErrorUnits - Stats1.ReceivingErrorUnits;
    public int ReceivingErrorQty => Stats2.ReceivingErrorQty - Stats1.ReceivingErrorQty;
    public int ReceivingErrorLines => Stats2.ReceivingErrorLines - Stats1.ReceivingErrorLines;
    public int ReceivingErrorCartons => Stats2.ReceivingErrorCartons - Stats1.ReceivingErrorCartons;

    public double ReceivingUnitPct => Stats2.ReceivingUnitPct - Stats1.ReceivingUnitPct;
    public double ReceivingQtyPct => Stats2.ReceivingQtyPct - Stats1.ReceivingQtyPct;
    public double ReceivingLinePct => Stats2.ReceivingLinePct - Stats1.ReceivingLinePct;
    public double ReceivingCartonPct => Stats2.ReceivingCartonPct - Stats1.ReceivingCartonPct;

    public int HeatMapErrorUnits => Stats2.HeatMapErrorUnits - Stats1.HeatMapErrorUnits;
    public int HeatMapErrorQty => Stats2.HeatMapErrorQty - Stats1.HeatMapErrorQty;
    public int HeatMapErrorLines => Stats2.HeatMapErrorLines - Stats1.HeatMapErrorLines;
    public int HeatMapErrorCartons => Stats2.HeatMapErrorCartons - Stats1.HeatMapErrorCartons;

    public double HeatMapUnitPct => Stats2.HeatMapUnitPct - Stats1.HeatMapUnitPct;
    public double HeatMapQtyPct => Stats2.HeatMapQtyPct - Stats1.HeatMapQtyPct;
    public double HeatMapLinePct => Stats2.HeatMapLinePct - Stats1.HeatMapLinePct;
    public double HeatMapCartonPct => Stats2.HeatMapCartonPct - Stats1.HeatMapCartonPct;

    public int QAErrorUnits => Stats2.QAErrorUnits - Stats1.QAErrorUnits;
    public int QAErrorQty => Stats2.QAErrorQty - Stats1.QAErrorQty;
    public int QAErrorLines => Stats2.QAErrorLines - Stats1.QAErrorLines;
    public int QAErrorCartons => Stats2.QAErrorCartons - Stats1.QAErrorCartons;

    public double QAUnitPct => Stats2.QAUnitPct - Stats1.QAUnitPct;
    public double QAQtyPct => Stats2.QAQtyPct - Stats1.QAQtyPct;
    public double QALinePct => Stats2.QALinePct - Stats1.QALinePct;
    public double QACartonPct => Stats2.QACartonPct - Stats1.QACartonPct;

    public int OtherDeptErrorUnits => Stats2.OtherDeptErrorUnits - Stats1.OtherDeptErrorUnits;
    public int OtherDeptErrorQty => Stats2.OtherDeptErrorQty - Stats1.OtherDeptErrorQty;
    public int OtherDeptErrorLines => Stats2.OtherDeptErrorLines - Stats1.OtherDeptErrorLines;
    public int OtherDeptErrorCartons => Stats2.OtherDeptErrorCartons - Stats1.OtherDeptErrorCartons;

    public double OtherDeptUnitPct => Stats2.OtherDeptUnitPct - Stats1.OtherDeptUnitPct;
    public double OtherDeptQtyPct => Stats2.OtherDeptQtyPct - Stats1.OtherDeptQtyPct;
    public double OtherDeptLinePct => Stats2.OtherDeptLinePct - Stats1.OtherDeptLinePct;
    public double OtherDeptCartonPct => Stats2.OtherDeptCartonPct - Stats1.OtherDeptCartonPct;

    #endregion

    public QAStatDiffVM(QAStatsVM firstStats, QAStatsVM secondStats)
    {
        stats1 = firstStats;
        stats2 = secondStats;
        DevelopmentOpportunity = string.Empty;
        Notes = string.Empty;
    }

    public void Refresh()
    {
        OnPropertyChanged(nameof(TargetAccuracyString));
        OnPropertyChanged(nameof(Description));
        OnPropertyChanged(nameof(AverageStaff));
        OnPropertyChanged(nameof(RestockQty));
        OnPropertyChanged(nameof(RestockHits));
        OnPropertyChanged(nameof(RestockCartons));
        OnPropertyChanged(nameof(QAUnits));
        OnPropertyChanged(nameof(QAQty));
        OnPropertyChanged(nameof(QALines));
        OnPropertyChanged(nameof(QACartons));
        OnPropertyChanged(nameof(PctQtyQA));
        OnPropertyChanged(nameof(PctLinesQA));
        OnPropertyChanged(nameof(PctCartonsQA));
        OnPropertyChanged(nameof(ErrorUnits));
        OnPropertyChanged(nameof(ErrorQty));
        OnPropertyChanged(nameof(ErrorLines));
        OnPropertyChanged(nameof(ErrorCartons));
        OnPropertyChanged(nameof(OverallUnitAccuracy));
        OnPropertyChanged(nameof(OverallQtyAccuracy));
        OnPropertyChanged(nameof(OverallLineAccuracy));
        OnPropertyChanged(nameof(OverallCartonAccuracy));
        OnPropertyChanged(nameof(ExternalErrorUnits));
        OnPropertyChanged(nameof(ExternalErrorQty));
        OnPropertyChanged(nameof(ExternalErrorLines));
        OnPropertyChanged(nameof(ExternalErrorCartons));
        OnPropertyChanged(nameof(WHErrorUnits));
        OnPropertyChanged(nameof(WHErrorQty));
        OnPropertyChanged(nameof(WHErrorLines));
        OnPropertyChanged(nameof(WHErrorCartons));
        OnPropertyChanged(nameof(WHUnitAccuracy));
        OnPropertyChanged(nameof(WHLineAccuracy));
        OnPropertyChanged(nameof(WHQtyAccuracy));
        OnPropertyChanged(nameof(WHCartonAccuracy));
        OnPropertyChanged(nameof(PickerErrorUnits));
        OnPropertyChanged(nameof(PickerErrorQty));
        OnPropertyChanged(nameof(PickerErrorLines));
        OnPropertyChanged(nameof(PickerErrorCartons));
        OnPropertyChanged(nameof(PickerUnitAccuracy));
        OnPropertyChanged(nameof(PickerQtyAccuracy));
        OnPropertyChanged(nameof(PickerLineAccuracy));
        OnPropertyChanged(nameof(PickerCartonAccuracy));
        OnPropertyChanged(nameof(SeekUnits));
        OnPropertyChanged(nameof(SeekQty));
        OnPropertyChanged(nameof(SeekLines));
        OnPropertyChanged(nameof(SeekCartons));
        OnPropertyChanged(nameof(SeekUnitsFixed));
        OnPropertyChanged(nameof(SeekQtyFixed));
        OnPropertyChanged(nameof(SeekLinesFixed));
        OnPropertyChanged(nameof(SeekCartonsFixed));
        OnPropertyChanged(nameof(SupplierErrorUnits));
        OnPropertyChanged(nameof(SupplierErrorQty));
        OnPropertyChanged(nameof(SupplierErrorLines));
        OnPropertyChanged(nameof(SupplierErrorCartons));
        OnPropertyChanged(nameof(SupplierUnitPct));
        OnPropertyChanged(nameof(SupplierQtyPct));
        OnPropertyChanged(nameof(SupplierLinePct));
        OnPropertyChanged(nameof(StockingCartonPct));
        OnPropertyChanged(nameof(SystemErrorUnits));
        OnPropertyChanged(nameof(SystemErrorQty));
        OnPropertyChanged(nameof(SystemErrorLines));
        OnPropertyChanged(nameof(SystemErrorCartons));
        OnPropertyChanged(nameof(SystemUnitPct));
        OnPropertyChanged(nameof(SystemQtyPct));
        OnPropertyChanged(nameof(SystemLinePct));
        OnPropertyChanged(nameof(SupplierCartonPct));
        OnPropertyChanged(nameof(OtherExternalErrorUnits));
        OnPropertyChanged(nameof(OtherExternalErrorQty));
        OnPropertyChanged(nameof(OtherExternalErrorLines));
        OnPropertyChanged(nameof(OtherExternalErrorCartons));
        OnPropertyChanged(nameof(OtherExternalUnitPct));
        OnPropertyChanged(nameof(OtherExternalQtyPct));
        OnPropertyChanged(nameof(OtherExternalLinePct));
        OnPropertyChanged(nameof(OtherExternalCartonPct));
        OnPropertyChanged(nameof(PTLErrorUnits));
        OnPropertyChanged(nameof(PTLErrorQty));
        OnPropertyChanged(nameof(PTLErrorLines));
        OnPropertyChanged(nameof(PTLErrorCartons));
        OnPropertyChanged(nameof(PTLUnitPct));
        OnPropertyChanged(nameof(PTLQtyPct));
        OnPropertyChanged(nameof(PTLLinePct));
        OnPropertyChanged(nameof(PTLCartonPct));
        OnPropertyChanged(nameof(PTVErrorUnits));
        OnPropertyChanged(nameof(PTVErrorQty));
        OnPropertyChanged(nameof(PTVErrorLines));
        OnPropertyChanged(nameof(PTVErrorCartons));
        OnPropertyChanged(nameof(PTVUnitPct));
        OnPropertyChanged(nameof(PTVQtyPct));
        OnPropertyChanged(nameof(PTVLinePct));
        OnPropertyChanged(nameof(PTVCartonPct));
        OnPropertyChanged(nameof(StockingErrorUnits));
        OnPropertyChanged(nameof(StockingErrorQty));
        OnPropertyChanged(nameof(StockingErrorLines));
        OnPropertyChanged(nameof(StockingErrorCartons));
        OnPropertyChanged(nameof(StockingUnitPct));
        OnPropertyChanged(nameof(StockingQtyPct));
        OnPropertyChanged(nameof(StockingLinePct));
        OnPropertyChanged(nameof(StockingCartonPct));
        OnPropertyChanged(nameof(ReceivingErrorUnits));
        OnPropertyChanged(nameof(ReceivingErrorQty));
        OnPropertyChanged(nameof(ReceivingErrorLines));
        OnPropertyChanged(nameof(ReceivingErrorCartons));
        OnPropertyChanged(nameof(ReceivingUnitPct));
        OnPropertyChanged(nameof(ReceivingQtyPct));
        OnPropertyChanged(nameof(ReceivingLinePct));
        OnPropertyChanged(nameof(ReceivingCartonPct));
        OnPropertyChanged(nameof(ReplenErrorUnits));
        OnPropertyChanged(nameof(ReplenErrorQty));
        OnPropertyChanged(nameof(ReplenErrorLines));
        OnPropertyChanged(nameof(ReplenErrorCartons));
        OnPropertyChanged(nameof(ReplenUnitPct));
        OnPropertyChanged(nameof(ReplenQtyPct));
        OnPropertyChanged(nameof(ReplenLinePct));
        OnPropertyChanged(nameof(ReplenCartonPct));
        OnPropertyChanged(nameof(HeatMapErrorUnits));
        OnPropertyChanged(nameof(HeatMapErrorQty));
        OnPropertyChanged(nameof(HeatMapErrorLines));
        OnPropertyChanged(nameof(HeatMapErrorCartons));
        OnPropertyChanged(nameof(HeatMapUnitPct));
        OnPropertyChanged(nameof(HeatMapQtyPct));
        OnPropertyChanged(nameof(HeatMapLinePct));
        OnPropertyChanged(nameof(HeatMapCartonPct));
        OnPropertyChanged(nameof(QAErrorUnits));
        OnPropertyChanged(nameof(QAErrorQty));
        OnPropertyChanged(nameof(QAErrorLines));
        OnPropertyChanged(nameof(QAErrorCartons));
        OnPropertyChanged(nameof(QAUnitPct));
        OnPropertyChanged(nameof(QAQtyPct));
        OnPropertyChanged(nameof(QALinePct));
        OnPropertyChanged(nameof(QACartonPct));
        OnPropertyChanged(nameof(OtherDeptErrorUnits));
        OnPropertyChanged(nameof(OtherDeptErrorQty));
        OnPropertyChanged(nameof(OtherDeptErrorLines));
        OnPropertyChanged(nameof(OtherDeptErrorCartons));
        OnPropertyChanged(nameof(OtherDeptUnitPct));
        OnPropertyChanged(nameof(OtherDeptQtyPct));
        OnPropertyChanged(nameof(OtherDeptLinePct));
        OnPropertyChanged(nameof(OtherDeptCartonPct));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
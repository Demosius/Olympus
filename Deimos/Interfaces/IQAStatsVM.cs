namespace Deimos.Interfaces;

public interface IQAStatsVM
{
    public string Description { get; }
    public bool Actual { get; }
    public string TargetAccuracyString { get; set; }
    public string DevelopmentOpportunity { get; set; }
    public string Notes { get; set; }
    public double AverageStaff { get; }

    public int RestockQty { get; }
    public int RestockHits { get; }
    public int RestockCartons { get; }

    public int QAUnits { get; }
    public int QAQty { get; }
    public int QALines { get; }
    public int QACartons { get; }

    public double PctQtyQA { get; }
    public double PctLinesQA { get; }
    public double PctCartonsQA { get; }

    public int ErrorUnits { get; }
    public int ErrorQty { get; }
    public int ErrorLines { get; }
    public int ErrorCartons { get; }

    public double OverallUnitAccuracy { get; }
    public double OverallQtyAccuracy { get; }
    public double OverallLineAccuracy { get; }
    public double OverallCartonAccuracy { get; }

    public int ExternalErrorUnits { get; }
    public int ExternalErrorQty { get; }
    public int ExternalErrorLines { get; }
    public int ExternalErrorCartons { get; }

    public int WHErrorUnits { get; }
    public int WHErrorQty { get; }
    public int WHErrorLines { get; }
    public int WHErrorCartons { get; }

    public double WHUnitAccuracy { get; }
    public double WHQtyAccuracy { get; }
    public double WHLineAccuracy { get; }
    public double WHCartonAccuracy { get; }

    public int PickerErrorUnits { get; }
    public int PickerErrorQty { get; }
    public int PickerErrorLines { get; }
    public int PickerErrorCartons { get; }

    public double PickerUnitAccuracy { get; }
    public double PickerQtyAccuracy { get; }
    public double PickerLineAccuracy { get; }
    public double PickerCartonAccuracy { get; }

    public int SeekUnits { get; }
    public int SeekQty { get; }
    public int SeekLines { get; }
    public int SeekCartons { get; }

    public int SeekUnitsFixed { get; }
    public int SeekQtyFixed { get; }
    public int SeekLinesFixed { get; }
    public int SeekCartonsFixed { get; }

    public int SupplierErrorUnits { get; }
    public int SupplierErrorQty { get; }
    public int SupplierErrorLines { get; }
    public int SupplierErrorCartons { get; }

    public double SupplierUnitPct { get; }
    public double SupplierQtyPct { get; }
    public double SupplierLinePct { get; }
    public double SupplierCartonPct { get; }

    public int SystemErrorUnits { get; }
    public int SystemErrorQty { get; }
    public int SystemErrorLines { get; }
    public int SystemErrorCartons { get; }

    public double SystemUnitPct { get; }
    public double SystemQtyPct { get; }
    public double SystemLinePct { get; }
    public double SystemCartonPct { get; }

    public int OtherExternalErrorUnits { get; }
    public int OtherExternalErrorQty { get; }
    public int OtherExternalErrorLines { get; }
    public int OtherExternalErrorCartons { get; }

    public double OtherExternalUnitPct { get; }
    public double OtherExternalQtyPct { get; }
    public double OtherExternalLinePct { get; }
    public double OtherExternalCartonPct { get; }

    public int PTLErrorUnits { get; }
    public int PTLErrorQty { get; }
    public int PTLErrorLines { get; }
    public int PTLErrorCartons { get; }

    public double PTLUnitPct { get; }
    public double PTLQtyPct { get; }
    public double PTLLinePct { get; }
    public double PTLCartonPct { get; }

    public int PTVErrorUnits { get; }
    public int PTVErrorQty { get; }
    public int PTVErrorLines { get; }
    public int PTVErrorCartons { get; }

    public double PTVUnitPct { get; }
    public double PTVQtyPct { get; }
    public double PTVLinePct { get; }
    public double PTVCartonPct { get; }

    public int StockingErrorUnits { get; }
    public int StockingErrorQty { get; }
    public int StockingErrorLines { get; }
    public int StockingErrorCartons { get; }

    public double StockingUnitPct { get; }
    public double StockingQtyPct { get; }
    public double StockingLinePct { get; }
    public double StockingCartonPct { get; }

    public int ReplenErrorUnits { get; }
    public int ReplenErrorQty { get; }
    public int ReplenErrorLines { get; }
    public int ReplenErrorCartons { get; }

    public double ReplenUnitPct { get; }
    public double ReplenQtyPct { get; }
    public double ReplenLinePct { get; }
    public double ReplenCartonPct { get; }

    public int ReceivingErrorUnits { get; }
    public int ReceivingErrorQty { get; }
    public int ReceivingErrorLines { get; }
    public int ReceivingErrorCartons { get; }

    public double ReceivingUnitPct { get; }
    public double ReceivingQtyPct { get; }
    public double ReceivingLinePct { get; }
    public double ReceivingCartonPct { get; }

    public int HeatMapErrorUnits { get; }
    public int HeatMapErrorQty { get; }
    public int HeatMapErrorLines { get; }
    public int HeatMapErrorCartons { get; }

    public double HeatMapUnitPct { get; }
    public double HeatMapQtyPct { get; }
    public double HeatMapLinePct { get; }
    public double HeatMapCartonPct { get; }

    public int QAErrorUnits { get; }
    public int QAErrorQty { get; }
    public int QAErrorLines { get; }
    public int QAErrorCartons { get; }

    public double QAUnitPct { get; }
    public double QAQtyPct { get; }
    public double QALinePct { get; }
    public double QACartonPct { get; }

    public int OtherDeptErrorUnits { get; }
    public int OtherDeptErrorQty { get; }
    public int OtherDeptErrorLines { get; }
    public int OtherDeptErrorCartons { get; }

    public double OtherDeptUnitPct { get; }
    public double OtherDeptQtyPct { get; }
    public double OtherDeptLinePct { get; }
    public double OtherDeptCartonPct { get; }

}
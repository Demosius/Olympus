using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Deimos.Models;
using Uranus.Annotations;
using Uranus.Staff.Models;

namespace Deimos.ViewModels.Controls;

public class QAOperatorVM : INotifyPropertyChanged
{
    public Employee Operator { get; set; }

    #region Operator Access

    public int ID => Operator.ID;
    public string PC_ID => Operator.PC_ID;
    public string FullName => Operator.FullName;
    public EmployeeIcon? Icon => Operator.Icon;

    #endregion

    #region INotifyPropertyChanged Members

    public ObservableCollection<QASession> QASessions { get; set; }


    private int cartonCount;
    public int CartonCount
    {
        get => cartonCount;
        set
        {
            cartonCount = value;
            OnPropertyChanged();
        }
    }

    private int items;
    public int Items
    {
        get => items;
        set
        {
            items = value;
            OnPropertyChanged();
        }
    }

    private int scans;
    public int Scans
    {
        get => scans;
        set
        {
            scans = value;
            OnPropertyChanged();
        }
    }

    private int units;
    public int Units
    {
        get => units;
        set
        {
            units = value;
            OnPropertyChanged();
        }
    }

    private int daysActive;
    public int DaysActive
    {
        get => daysActive;
        set
        {
            daysActive = value;
            OnPropertyChanged();
        }
    }

    private TimeSpan scanTime;
    public TimeSpan ScanTime
    {
        get => scanTime;
        set
        {
            scanTime = value;
            OnPropertyChanged();
        }
    }

    private string scanTimeString;
    public string ScanTimeString
    {
        get => scanTimeString;
        set
        {
            scanTimeString = value;
            OnPropertyChanged();
        }
    }

    private double cartonsPerHour;
    public double CartonsPerHour
    {
        get => cartonsPerHour;
        set
        {
            cartonsPerHour = value;
            OnPropertyChanged();
        }
    }

    private double itemsPerMinute;
    public double ItemsPerMinute
    {
        get => itemsPerMinute;
        set
        {
            itemsPerMinute = value;
            OnPropertyChanged();
        }
    }

    private double scansPerMinute;
    public double ScansPerMinute
    {
        get => scansPerMinute;
        set
        {
            scansPerMinute = value;
            OnPropertyChanged();
        }
    }

    private double unitsPerMinute;
    public double UnitsPerMinute
    {
        get => unitsPerMinute;
        set
        {
            unitsPerMinute = value;
            OnPropertyChanged();
        }
    }

    private int cartonErrors;
    public int CartonErrors
    {
        get => cartonErrors;
        set
        {
            cartonErrors = value;
            OnPropertyChanged();
        }
    }

    private int itemErrors;
    public int ItemErrors
    {
        get => itemErrors;
        set
        {
            itemErrors = value;
            OnPropertyChanged();
        }
    }

    private int scanErrors;
    public int ScanErrors
    {
        get => scanErrors;
        set
        {
            scanErrors = value;
            OnPropertyChanged();
        }
    }

    private int unitErrors;
    public int UnitErrors
    {
        get => unitErrors;
        set
        {
            unitErrors = value;
            OnPropertyChanged();
        }
    }

    private double cartonAccuracy;
    public double CartonAccuracy
    {
        get => cartonAccuracy;
        set
        {
            cartonAccuracy = value;
            OnPropertyChanged();
        }
    }

    private double itemAccuracy;
    public double ItemAccuracy
    {
        get => itemAccuracy;
        set
        {
            itemAccuracy = value;
            OnPropertyChanged();
        }
    }

    private double scanAccuracy;
    public double ScanAccuracy
    {
        get => scanAccuracy;
        set
        {
            scanAccuracy = value;
            OnPropertyChanged();
        }
    }

    private double unitAccuracy;
    public double UnitAccuracy
    {
        get => unitAccuracy;
        set
        {
            unitAccuracy = value;
            OnPropertyChanged();
        }
    }

    private double overallAccuracy;
    public double OverallAccuracy
    {
        get => overallAccuracy;
        set
        {
            overallAccuracy = value;
            OnPropertyChanged();
        }
    }

    private Uri? iconUri;
    public Uri? IconUri
    {
        get => iconUri;
        set
        {
            iconUri = value;
            OnPropertyChanged();
        }
    }

    private double cartonPerformance;
    public double CartonPerformance
    {
        get => cartonPerformance;
        set
        {
            cartonPerformance = value;
            OnPropertyChanged();
        }
    }

    private double itemPerformance;
    public double ItemPerformance
    {
        get => itemPerformance;
        set
        {
            itemPerformance = value;
            OnPropertyChanged();
        }
    }

    private double scanPerformance;
    public double ScanPerformance
    {
        get => scanPerformance;
        set
        {
            scanPerformance = value;
            OnPropertyChanged();
        }
    }

    private double unitPerformance;
    public double UnitPerformance
    {
        get => unitPerformance;
        set
        {
            unitPerformance = value;
            OnPropertyChanged();
        }
    }

    private double overallPerformance;
    public double OverallPerformance
    {
        get => overallPerformance;
        set
        {
            overallPerformance = value;
            OnPropertyChanged();
        }
    }

    private double pctTopCarton;
    public double PctTopCarton
    {
        get => pctTopCarton;
        set
        {
            pctTopCarton = value;
            OnPropertyChanged();
        }
    }

    private double pctTopItem;
    public double PctTopItem
    {
        get => pctTopItem;
        set
        {
            pctTopItem = value;
            OnPropertyChanged();
        }
    }

    private double pctTopScan;
    public double PctTopScan
    {
        get => pctTopScan;
        set
        {
            pctTopScan = value;
            OnPropertyChanged();
        }
    }

    private double pctTopUnit;
    public double PctTopUnit
    {
        get => pctTopUnit;
        set
        {
            pctTopUnit = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region Commands



    #endregion

    public QAOperatorVM(Employee employee)
    {
        Operator = employee;
        var sessionDict = QASession.GetSessions(Operator.QACartons);
        QASessions = sessionDict.TryGetValue(Operator, out var sessions) ?
            new ObservableCollection<QASession>(sessions) :
            new ObservableCollection<QASession>();

        cartonCount = QASessions.Sum(s => s.CartonCount);
        items = QASessions.Sum(s => s.Items);
        scans = QASessions.Sum(s => s.Scans);
        units = QASessions.Sum(s => s.Units);
        daysActive = QASessions.Select(s => s.StartTime.Date).Distinct().Count();
        scanTime = new TimeSpan(QASessions.Sum(s => s.Duration.Ticks));
        scanTimeString = $"{(scanTime < TimeSpan.FromDays(1) ? "" : $"{scanTime.Days}d ")}{scanTime.Hours:00}:{scanTime.Minutes:00}:{scanTime.Seconds:00}";

        cartonsPerHour = cartonCount / ScanTime.TotalHours;
        itemsPerMinute = Items / ScanTime.TotalMinutes;
        scansPerMinute = scans / ScanTime.TotalMinutes;
        unitsPerMinute = units / ScanTime.TotalMinutes;

        cartonErrors = QASessions.Sum(s => s.CartonErrors);
        itemErrors = QASessions.Sum(s => s.ItemErrors);
        scanErrors = QASessions.Sum(s => s.ScanErrors);
        unitErrors = QASessions.Sum(s => s.UnitErrors);

        cartonAccuracy = (cartonCount - cartonErrors) / (double)cartonCount;
        itemAccuracy = (items - itemErrors) / (double)items;
        scanAccuracy = (scans - scanErrors) / (double)scans;
        unitAccuracy = (units - unitErrors) / (double)units;
        overallAccuracy = (cartonAccuracy + itemAccuracy + scanAccuracy + unitAccuracy) / 4;

        IconUri = Icon is null ? null : new Uri(Icon.FullPath, UriKind.RelativeOrAbsolute);
    }

    public void SetPerformance(double averageCartons, double averageItems, double averageScans, double averageUnits,
        double topCartons, double topItems, double topScans, double topUnits)
    {
        CartonPerformance = CartonsPerHour / averageCartons;
        ItemPerformance = ItemsPerMinute / averageItems;
        ScanPerformance = ScansPerMinute / averageScans;
        UnitPerformance = UnitsPerMinute / averageUnits;

        OverallPerformance = (CartonPerformance + ScanPerformance + UnitPerformance) / 3;

        PctTopCarton = CartonsPerHour / topCartons * 100;
        PctTopItem = ItemsPerMinute / topItems * 100;
        PctTopScan = ScansPerMinute / topScans * 100;
        PctTopUnit = UnitsPerMinute / topUnits * 100;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
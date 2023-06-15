using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Uranus.Annotations;
using Uranus.Inventory.Models;

namespace Argos.ViewModels.Components;

public class BatchTOLineVM : INotifyPropertyChanged
{
    public BatchTOLine Line { get; set; }

    #region Object Access

    public string CCN => Line.CCN;

    public string StoreNo
    {
        get => Line.StoreNo;
        set
        {
            Line.StoreNo = value;
            OnPropertyChanged();
        }
    }

    public int Cartons
    {
        get => Line.Cartons;
        set
        {
            Line.Cartons = value;
            OnPropertyChanged();
        }
    }

    public double Weight
    {
        get => Line.Weight;
        set
        {
            Line.Weight = value;
            OnPropertyChanged();
        }
    }

    public double Cube
    {
        get => Line.Cube; set
        {
            Line.Cube = value;
            OnPropertyChanged();
        }
    }

    public string CartonType
    {
        get => Line.CartonType;
        set
        {
            Line.CartonType = value;
            OnPropertyChanged();
        }
    }

    public string StartingPickZone
    {
        get => Line.StartZone;
        set
        {
            Line.StartZone = value;
            OnPropertyChanged();
        }
    }

    public string EndingPickZone
    {
        get => Line.EndZone;
        set
        {
            Line.EndZone = value;
            OnPropertyChanged();
        }
    }

    public string StartingPickBin
    {
        get => Line.StartBin;
        set
        {
            Line.StartBin = value;
            OnPropertyChanged();
        }
    }

    public string EndingPickBin
    {
        get => Line.EndBin;
        set
        {
            Line.EndBin = value;
            OnPropertyChanged();
        }
    }

    public string BatchID
    {
        get => Line.BatchID;
        set
        {
            Line.BatchID = value;
            OnPropertyChanged();
        }
    }

    public DateTime Date
    {
        get => Line.Date; 
        set
        {
            Line.Date = value;
            OnPropertyChanged();
        }
    }

    public int UnitsBase
    {
        get => Line.UnitsBase;
        set
        {
            Line.UnitsBase = value;
            OnPropertyChanged();
        }
    }

    public string WaveNo
    {
        get => Line.WaveNo; 
        set
        {
            Line.WaveNo = value;
            OnPropertyChanged();
        }
    }

    public int ItemNumber
    {
        get => Line.ItemNumber; set
        {
            Line.ItemNumber = value;
            OnPropertyChanged();
        }
    }

    public string Description
    {
        get => Line.Description;
        set
        {
            Line.Description = value;
            OnPropertyChanged();
        }
    }

    public string OriginalFileDirectory
    {
        get => Line.OriginalFileDirectory; 
        set
        {
            Line.OriginalFileDirectory = value;
            OnPropertyChanged();
        }
    }

    public string OriginalFileName
    {
        get => Line.OriginalFileName; 
        set
        {
            Line.OriginalFileName = value;
            OnPropertyChanged();
        }
    }

    public string FinalFileDirectory
    {
        get => Line.FinalFileDirectory; 
        set
        {
            Line.FinalFileDirectory = value;
            OnPropertyChanged();
        }
    }

    public string FinalFileName
    {
        get => Line.FinalFileName; 
        set
        {
            Line.FinalFileName = value;
            OnPropertyChanged();
        }
    }

    public DateTime OriginalProcessingTime
    {
        get => Line.OriginalProcessingTime; 
        set
        {
            Line.OriginalProcessingTime = value;
            OnPropertyChanged();
        }
    }

    public DateTime? FinalProcessingTime
    {
        get => Line.FinalProcessingTime; 
        set
        {
            Line.FinalProcessingTime = value;
            OnPropertyChanged();
        }
    }

    #endregion

    public BatchTOLineVM(BatchTOLine line)
    {
        Line = line;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
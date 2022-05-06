using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SQLite;
using SQLiteNetExtensions.Attributes;
using Uranus.Annotations;

namespace Uranus.Staff.Model;

/// <summary>
/// Core shift counter.
/// </summary>
public class ShiftCounter : INotifyPropertyChanged
{
    [PrimaryKey] public Guid ID { get; set; }
    [ForeignKey(typeof(Shift))] public string ShiftID { get; set; }
    public DateTime Date { get; set; }

    [ManyToOne(nameof(ShiftID), CascadeOperations = CascadeOperation.None)]
    public Shift? Shift { get; set; }

    #region INotifyPropertyChanged Members

    // Do not store the shift count, but instead recount upon initialization.
    private int count;
    [Ignore] public int Count
    {
        get => count;
        set
        {
            count = value;
            OnPropertyChanged();
        }
    }

    private int target;
    public int Target
    {
        get => target;
        set
        {
            target = value;
            OnPropertyChanged();
        }
    }

    [Ignore] public int Discrepancy => Target - Count;

    [Ignore] public bool Lacking => Count < Target;

    #endregion

    public ShiftCounter()
    {
        ID = Guid.NewGuid();
        ShiftID = string.Empty;
    }

    public ShiftCounter(Shift shift, int target, DateTime date)
    {
        ID = Guid.NewGuid();
        ShiftID = shift.ID;
        Shift = shift;
        Target = target;
        Date = date;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
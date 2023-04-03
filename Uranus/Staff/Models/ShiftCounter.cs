using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Uranus.Annotations;

namespace Uranus.Staff.Models;

/// <summary>
/// Core shift counter.
/// </summary>
public abstract class ShiftCounter
{
    [PrimaryKey] public Guid ID { get; set; }
    [ForeignKey(typeof(Shift))] public string ShiftID { get; set; }
    public int Target { get; set; }

    [ManyToOne(nameof(ShiftID), CascadeOperations = CascadeOperation.None)]
    public Shift? Shift { get; set; }

    #region INotifyPropertyChanged Members

    // Do not store the shift count, but instead recount upon initialization.
    [Ignore] public int Count { get; set; }

    [Ignore] public int Discrepancy => Target - Count;

    [Ignore] public bool Lacking => Count < Target;

    #endregion

    protected ShiftCounter()
    {
        ID = Guid.NewGuid();
        ShiftID = string.Empty;
    }

    protected ShiftCounter(Shift shift, int target)
    {
        ID = Guid.NewGuid();
        ShiftID = shift.ID;
        Shift = shift;
        Target = target;
    }
}
using Pantheon.Annotations;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Uranus.Staff.Model;

namespace Pantheon.Model;

public class ShiftCounter : INotifyPropertyChanged
{
    public Shift Shift { get; set; }

    #region INotifyPropertyChanged Members

    private int count;
    public int Count
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

    public int Discrepancy => Target - Count;

    public bool Lacking => Count < Target;

    #endregion

    public ShiftCounter(Shift shift, int target)
    {
        Shift = shift;
        Target = target;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
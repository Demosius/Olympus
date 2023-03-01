using System.ComponentModel;
using System.Runtime.CompilerServices;
using Pantheon.Annotations;
using Uranus.Staff.Models;

namespace Pantheon.ViewModels.Controls.Shifts;

public class BreakVM : INotifyPropertyChanged
{
    public Break Break { get; }
    // Parent VM
    public ShiftVM ShiftVM { get; }

    #region Direct Break Access

    public string ID => Break.ID;

    public Shift? Shift => Break.Shift;

    public string StartString => Break.StartString;

    public int Length => Break.Length;

    public string Name
    {
        get => Break.Name;
        set
        {
            Break.Name = value;
            OnPropertyChanged();
        }
    }

    #endregion

    public BreakVM(Break @break, ShiftVM shiftVM)
    {
        Break = @break;
        ShiftVM = shiftVM;
    }

    public void Remove() => ShiftVM.RemoveBreak(this);

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Morpheus.Views.Windows;
using Pantheon.ViewModels.Commands.Shifts;
using Styx;
using Uranus;
using Uranus.Staff.Models;

namespace Pantheon.ViewModels.Controls.Shifts;

public class BreakVM : INotifyPropertyChanged
{
    public Break Break { get; }
    // Parent VM
    public ShiftVM ShiftVM { get; }

    public Department? Department => ShiftVM.Department;
    public Charon Charon => ShiftVM.Charon;
    public Helios Helios => ShiftVM.Helios;

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

    #region Commands

    public AddRemoveBreakCommand AddRemoveBreakCommand { get; set; }

    #endregion

    public BreakVM(Break @break, ShiftVM shiftVM)
    {
        Break = @break;
        ShiftVM = shiftVM;

        AddRemoveBreakCommand = new AddRemoveBreakCommand(this);
    }

    public void Remove()
    {
        ShiftVM.RemoveBreak(this);
        Helios.StaffDeleter.Break(Break);
    }

    public void AddBreak()
    {
        var inputBox = new InputWindow("Enter Break Name", "New Break");

        if (inputBox.ShowDialog() != true) return;

        var breakName = inputBox.VM.Input;

        if (ShiftVM.Breaks.Any(b => b.Name == breakName)) return;

        var @break = new Break(ShiftVM.Shift, breakName);

        ShiftVM.AddBreak(@break);

        Helios.StaffCreator.Break(@break);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
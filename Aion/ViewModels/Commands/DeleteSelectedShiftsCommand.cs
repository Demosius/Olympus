using System;
using System.Linq;
using System.Windows.Input;
using Uranus.Staff.Models;

namespace Aion.ViewModels.Commands;

public class DeleteSelectedShiftsCommand : ICommand
{
    public ShiftEntryPageVM VM { get; set; }

    public DeleteSelectedShiftsCommand(ShiftEntryPageVM vm) { VM = vm; }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object? parameter)
    {
        if (parameter == null) return false;
        var items = (System.Collections.IList)parameter;
        return items.Count > 0;
    }

    public void Execute(object? parameter)
    {
        if (parameter == null) return;
        var items = (System.Collections.IList)parameter;
        var selection = items.Cast<ShiftEntry>();
        VM.DeleteSelectedShifts(selection.ToList());
    }
}
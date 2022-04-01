using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Pantheon.ViewModel.Pages;
using Uranus.Staff.Model;

namespace Pantheon.ViewModel.Commands;

public class DeleteShiftCommand : ICommand
{
    public ShiftPageVM VM { get; set; }

    public DeleteShiftCommand(ShiftPageVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.SelectedDepartment is not null && (VM.Charon?.CanDeleteShift(VM.SelectedDepartment) ?? false);
    }

    public void Execute(object? parameter)
    {
        if (parameter is not Shift shift) return;
        VM.DeleteShift(shift);
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
using System;
using System.Windows;
using System.Windows.Input;
using Pantheon.ViewModels.Controls.Shifts;

namespace Pantheon.ViewModels.Commands.Shifts;

public class SaveShiftCommand : ICommand
{
    public ShiftVM VM { get; set; }

    public SaveShiftCommand(ShiftVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.Department is not null && VM.Charon.CanUpdateShift(VM.Department);
    }

    public void Execute(object? parameter)
    {
        VM.SortBreaks();

        if (VM.Helios.StaffUpdater.Shift(VM.Shift) > 0)
            MessageBox.Show("Shift saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
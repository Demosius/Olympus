using Pantheon.ViewModel.Controls;
using System;
using System.Windows.Input;

namespace Pantheon.ViewModel.Commands.Rosters;

public class LaunchPublicHolidayManagerCommand : ICommand
{
    public DepartmentRosterVM VM { get; set; }

    public LaunchPublicHolidayManagerCommand(DepartmentRosterVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.LaunchPublicHolidayManager();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
using Pantheon.ViewModels.PopUp.Rosters;
using System;
using System.Windows;
using System.Windows.Input;

namespace Pantheon.ViewModels.Commands.Rosters;

internal class ConfirmHolidaysCommand : ICommand
{
    public PublicHolidayVM VM { get; set; }

    public ConfirmHolidaysCommand(PublicHolidayVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        if (parameter is not Window w) throw new Exception("Command Parameter not properly set.");
        VM.ConfirmHolidays();
        w.Close();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
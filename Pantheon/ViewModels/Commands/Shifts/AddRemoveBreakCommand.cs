using System;
using System.Windows.Input;
using Pantheon.ViewModels.Controls.Shifts;

namespace Pantheon.ViewModels.Commands.Shifts;

public class AddRemoveBreakCommand : ICommand
{
    public BreakVM VM { get; set; }

    public AddRemoveBreakCommand(BreakVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.Department is not null && VM.Charon.CanUpdateShift(VM.Department);
    }

    public void Execute(object? parameter)
    {
        if (VM.Name == "Lunch")
            VM.AddBreak();
        else
            VM.Remove();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
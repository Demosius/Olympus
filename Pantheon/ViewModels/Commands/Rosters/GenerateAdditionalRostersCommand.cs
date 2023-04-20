using System;
using System.Windows.Input;
using Pantheon.ViewModels.Controls.Rosters;

namespace Pantheon.ViewModels.Commands.Rosters;

public class GenerateAdditionalRostersCommand : ICommand
{
    public DepartmentRosterVM VM { get; set; }

    public GenerateAdditionalRostersCommand(DepartmentRosterVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        if (parameter is not string p) return;

        if (p == "Loan")
            VM.GenerateLoanRosters();
        else 
            VM.GenerateMissingRosters();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
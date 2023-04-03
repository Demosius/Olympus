using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Pantheon.ViewModels.Controls.Rosters;

namespace Pantheon.ViewModels.Commands.Rosters;

public class DeleteEmployeeRosterCommand : ICommand
{
    public EmployeeRosterVM VM { get; set; }

    public DeleteEmployeeRosterCommand(EmployeeRosterVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.Delete();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
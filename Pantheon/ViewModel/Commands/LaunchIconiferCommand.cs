using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Pantheon.ViewModel.Pages;

namespace Pantheon.ViewModel.Commands;

public class LaunchIconiferCommand : ICommand
{
    public EmployeePageVM VM { get; set; }

    public LaunchIconiferCommand(EmployeePageVM vm)
    {
        VM = vm;
    }

    public bool CanExecute(object? parameter)
    {
        return VM.SelectedEmployee is not null;
    }

    public void Execute(object? parameter)
    {
        VM.LaunchIconifer();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
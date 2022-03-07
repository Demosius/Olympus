using System;
using System.Windows.Input;

namespace Aion.ViewModel.Commands;

public class LaunchEmployeePrinterCommand : ICommand
{
    public EmployeePageVM VM { get; set; }

    public LaunchEmployeePrinterCommand(EmployeePageVM vm) { VM = vm; }

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object parameter)
    {
        return true;
    }

    public void Execute(object parameter)
    {
        throw new NotImplementedException();
    }
}
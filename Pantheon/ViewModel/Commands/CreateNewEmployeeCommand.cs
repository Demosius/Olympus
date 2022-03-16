using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Pantheon.ViewModel.Pages;

namespace Pantheon.ViewModel.Commands;

internal class CreateNewEmployeeCommand : ICommand
{
    public EmployeePageVM VM { get; set; }

    public CreateNewEmployeeCommand(EmployeePageVM vm)
    {
        VM = vm;
    }

    public bool CanExecute(object parameter)
    {
        return VM.Charon.CanCreateEmployee();
    }

    public void Execute(object parameter)
    {
        VM.CreateNewEmployee();
    }

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
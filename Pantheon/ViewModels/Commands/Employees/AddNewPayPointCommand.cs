using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Pantheon.ViewModels.PopUp.Employees;

namespace Pantheon.ViewModels.Commands.Employees;


public class AddNewPayPointCommand : ICommand
{
    public PayPointSelectionVM VM { get; set; }

    public AddNewPayPointCommand(PayPointSelectionVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.CanCreatePayPoints;
    }

    public void Execute(object? parameter)
    {
        VM.AddNewPayPoint();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
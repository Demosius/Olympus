using Pantheon.ViewModel.PopUp.Employees;
using System;
using System.Windows.Input;

namespace Pantheon.ViewModel.Commands.Employees
{
    internal class CancelSingleRuleEditCommand : ICommand
    {
        public EmployeeShiftVM VM { get; set; }

        public CancelSingleRuleEditCommand(EmployeeShiftVM vm) { VM = vm; }

        public bool CanExecute(object? parameter)
        {
            return VM.SingleRule?.InEdit ?? false;
        }

        public void Execute(object? parameter)
        {
            VM.CancelSingleRuleEdit();
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}

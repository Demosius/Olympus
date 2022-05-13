using Pantheon.ViewModel.PopUp.Employees;
using System;
using System.Windows.Input;

namespace Pantheon.ViewModel.Commands.Employees
{
    internal class EditShiftRuleCommand : ICommand
    {
        public EmployeeShiftVM VM { get; set; }

        public EditShiftRuleCommand(EmployeeShiftVM vm) { VM = vm; }

        public bool CanExecute(object? parameter)
        {
            return VM.Charon is not null && VM.SelectedRule is not null && VM.Employee is not null &&
                   VM.Charon.CanUpdateEmployee(VM.Employee);
        }

        public void Execute(object? parameter)
        {
            VM.EditShiftRule();
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}

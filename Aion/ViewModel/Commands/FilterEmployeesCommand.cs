using System;
using System.Windows.Input;

namespace Aion.ViewModel.Commands
{
    public class FilterEmployeesCommand : ICommand
    {
        public EmployeePageVM VM { get; set; }

        public FilterEmployeesCommand(EmployeePageVM vm) { VM = vm; }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            return (VM.EmpSearchString ?? "") != "";
        }

        public void Execute(object parameter)
        {
            VM.ApplyFilters();
        }
    }
}

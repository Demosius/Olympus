using System;
using System.Windows;
using System.Windows.Input;

namespace Aion.ViewModel.Commands
{
    public class DeleteEmployeeCommand : ICommand
    {
        public EmployeePageVM VM { get; set; }

        public DeleteEmployeeCommand(EmployeePageVM vm) { VM = vm; }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            return VM.SelectedEmployee != null;
        }

        public void Execute(object parameter)
        {
            MessageBox.Show("Employee deletion not currently implemented.", "Not Implemented", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}

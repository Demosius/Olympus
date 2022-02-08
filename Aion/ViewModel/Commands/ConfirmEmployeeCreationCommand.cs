using System;
using System.Windows;
using System.Windows.Input;

namespace Aion.ViewModel.Commands
{
    public class ConfirmEmployeeCreationCommand : ICommand
    {
        public EmployeeCreationVM CreationVM { get; set; }

        public ConfirmEmployeeCreationCommand(EmployeeCreationVM creationVM) { CreationVM = creationVM; }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            return CreationVM.IsFiveChars && CreationVM.IsNumeric && CreationVM.IsUnique;
        }

        public void Execute(object parameter)
        {
            var w = (Window)parameter;
            CreationVM.ConfirmCreation();
            if (w == null) return;
            w.DialogResult = true;
            w.Close();
        }
    }
}

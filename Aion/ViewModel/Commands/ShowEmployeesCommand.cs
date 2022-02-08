using System;
using System.Windows.Input;

namespace Aion.ViewModel.Commands
{
    public class ShowEmployeesCommand : ICommand
    {
        public AionVM VM { get; set; }

        public ShowEmployeesCommand(AionVM vm) { VM = vm; }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            return VM.CurrentPage != VM.EmployeePage || VM.CurrentPage is null;
        }

        public void Execute(object parameter)
        {
            VM.ShowEmployeePage();
        }
    }
}

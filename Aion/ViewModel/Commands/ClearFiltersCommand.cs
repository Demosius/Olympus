using Aion.ViewModel.Interfaces;
using System;
using System.Windows.Input;

namespace Aion.ViewModel.Commands
{
    public class ClearFiltersCommand : ICommand
    {
        public IFilters VM { get; set; }

        public ClearFiltersCommand(IFilters vm) { VM = vm; }

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
            VM.ClearFilters();
        }
    }
}

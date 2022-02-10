using System;
using System.Windows.Input;
using Aion.ViewModel.Interfaces;

namespace Aion.ViewModel.Commands
{
    public class ApplyFiltersCommand : ICommand
    {
        public IFilters VM { get; set; }

        public ApplyFiltersCommand(IFilters vm) { VM = vm; }

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
            VM.ApplyFilters();
        }
    }
}

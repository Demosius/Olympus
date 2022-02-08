using System;
using System.Windows.Input;

namespace Aion.ViewModel.Commands
{
    public class GetFilteredEntriesCommand : ICommand
    {
        public EntryViewPageVM VM { get; set; }

        public GetFilteredEntriesCommand(EntryViewPageVM vm) { VM = vm; }

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

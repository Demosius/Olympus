using System;
using System.Windows.Input;

namespace Aion.ViewModel.Commands
{
    public class ShowEntryViewCommand : ICommand
    {
        public AionVM VM { get; set; }

        public ShowEntryViewCommand(AionVM vm) { VM = vm; }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            return VM.CurrentPage != VM.EntryViewPage || VM.CurrentPage is null;
        }

        public void Execute(object parameter)
        {
            VM.ShowEntryView();
        }
    }
}

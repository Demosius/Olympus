using System;
using System.Windows;
using System.Windows.Input;

namespace Aion.ViewModel.Commands
{
    public class ConfirmEntryEditCommand : ICommand
    {
        public EntryEditVM VM { get; set; }

        public ConfirmEntryEditCommand(EntryEditVM vm) { VM = vm; }

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
            var w = parameter as Window;
            VM.ConfirmEdit();
            w?.Close();
        }
    }
}

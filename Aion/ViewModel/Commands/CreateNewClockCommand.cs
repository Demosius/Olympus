using System;
using System.Windows.Input;

namespace Aion.ViewModel.Commands
{
    public class CreateNewClockCommand : ICommand
    {
        public EntryEditVM VM { get; set; }

        public CreateNewClockCommand(EntryEditVM vm) { VM = vm; }

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
            VM.CreateNewClock();
        }
    }
}

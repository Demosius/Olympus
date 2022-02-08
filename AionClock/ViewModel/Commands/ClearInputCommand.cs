using System;
using System.Windows.Input;

namespace AionClock.ViewModel.Commands
{
    public class ClearInputCommand : ICommand
    {
        public ClockInVM VM { get; set; }

        public ClearInputCommand(ClockInVM vm) { VM = vm; }

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
            VM.Input = "";
        }
    }
}

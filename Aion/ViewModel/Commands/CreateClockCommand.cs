using Aion.View;
using System;
using System.Windows;
using System.Windows.Input;

namespace Aion.ViewModel.Commands
{
    public class CreateClockCommand : ICommand
    {
        public ClockCreationVM VM { get; set; }

        public CreateClockCommand(ClockCreationVM vm) { VM = vm; }

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
            var w = parameter as Window;
            VM.CreateClock();
            if (VM.NewClock == null) return;
            w?.Close();
            ClockEditView editor = new(VM.Helios, VM.NewClock, true);
            editor.ShowDialog();
        }
    }
}

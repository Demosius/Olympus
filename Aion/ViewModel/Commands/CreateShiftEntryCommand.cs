using Aion.View;
using System;
using System.Windows;
using System.Windows.Input;

namespace Aion.ViewModel.Commands
{
    public class CreateShiftEntryCommand : ICommand
    {
        public EntryCreationVM VM { get; set; }

        public CreateShiftEntryCommand(EntryCreationVM vm) { VM = vm; }

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
            var w = (Window)parameter;
            VM.CreateShiftEntry();
            if (VM.NewEntry == null) return;
            w.Close();
            EntryEditView editor = new(VM.Helios, VM.NewEntry);
            editor.ShowDialog();
        }
    }
}

using System;
using System.Windows.Input;

namespace Aion.ViewModel.Commands
{
    public class ExportEntriesToCSVCommand : ICommand
    {
        public ShiftEntryPageVM VM { get; set; }

        public ExportEntriesToCSVCommand(ShiftEntryPageVM vm) { VM = vm; }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            return VM.Entries.Count > 0;
        }

        public void Execute(object parameter)
        {
            VM.ExportEntriesAsCSV();
        }
    }
}

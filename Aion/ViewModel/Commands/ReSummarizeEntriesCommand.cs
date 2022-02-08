using System;
using System.Windows;
using System.Windows.Input;

namespace Aion.ViewModel.Commands
{
    public class ReSummarizeEntriesCommand : ICommand
    {
        public EntryViewPageVM VM { get; set; }

        public ReSummarizeEntriesCommand(EntryViewPageVM vm) { VM = vm; }

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
            var result = MessageBox.Show("Re-summarizing could undo manually adjusted entry times and shift-types.\n\n" +
                                         "Do you want to continue?", "Continue?", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
                VM.ReSummarizeEntries();
        }
    }
}

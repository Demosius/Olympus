using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Aion.ViewModel.Commands
{
    public class ReSummarizeEntryCommand : ICommand
    {
        public ShiftEntryPageVM VM { get; set; }

        public ReSummarizeEntryCommand(ShiftEntryPageVM vm) { VM = vm; }

        public bool CanExecute(object parameter)
        {
            return VM.SelectedEntry is not null;
        }

        public void Execute(object parameter)
        {
            VM.ReSummarizeEntry();
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}

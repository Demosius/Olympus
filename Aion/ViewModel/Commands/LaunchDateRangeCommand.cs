using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Aion.ViewModel.Commands
{
    public class LaunchDateRangeCommand : ICommand
    {
        public ShiftEntryPageVM VM { get; set; }

        public LaunchDateRangeCommand(ShiftEntryPageVM vm)
        {
            VM = vm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            VM.LaunchDateRangeWindow();
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}

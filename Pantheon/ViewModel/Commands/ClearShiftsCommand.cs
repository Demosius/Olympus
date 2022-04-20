using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Pantheon.ViewModel.Pages;

namespace Pantheon.ViewModel.Commands
{
    class ClearShiftsCommand : ICommand
    {
        public RosterPageVM VM { get; set; }

        public ClearShiftsCommand(RosterPageVM vm ) { VM = vm; }

        public bool CanExecute(object? parameter)
        {
            return VM.LoadedRoster is not null;
        }

        public void Execute(object? parameter)
        {
            VM.ClearShifts();
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}

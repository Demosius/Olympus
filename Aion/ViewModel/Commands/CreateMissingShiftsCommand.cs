#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Aion.ViewModel.Commands
{
    public class CreateMissingShiftsCommand : ICommand
    {
        public ShiftEntryPageVM VM { get; set; }

        public CreateMissingShiftsCommand(ShiftEntryPageVM vm)
        {
            VM = vm;
        }

        public bool CanExecute(object? parameter)
        {
            return VM.MissingEntryCount > 0;
        }

        public void Execute(object? parameter)
        {
            VM.CreateMissingShifts();
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}

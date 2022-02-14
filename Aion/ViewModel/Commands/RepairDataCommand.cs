#nullable enable
using System;
using System.Windows.Input;
using Aion.ViewModel.Interfaces;

namespace Aion.ViewModel.Commands
{
    public class RepairDataCommand : ICommand
    {
        public IDBInteraction VM { get; set; }

        public RepairDataCommand(IDBInteraction vm)
        {
            VM = vm;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            VM.RepairData();
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}

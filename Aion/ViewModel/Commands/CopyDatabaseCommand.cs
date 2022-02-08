using System;
using System.Windows.Input;
using Aion.ViewModel.Utility;

namespace Aion.ViewModel.Commands
{
    public class CopyDatabaseCommand : ICommand
    {
        public DBManager VM { get; set; }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public CopyDatabaseCommand(DBManager vm)
        {
            VM = vm;
        }

        public bool CanExecute(object parameter)
        {
            return (parameter ?? "null") is string dbLocation && (App.Charon.CanCopyDatabase() || dbLocation.ToLower() == "local");
        }

        public void Execute(object parameter)
        {
            VM.CopyDatabase();
        }
    }
}

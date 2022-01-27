using Olympus.ViewModel.Utility;
using System;
using System.Windows.Input;

namespace Olympus.ViewModel.Commands
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
            var dbLocation = (parameter ?? "null") as string;
            return App.Charon.CanCopyDatabase() || dbLocation.ToLower() == "local";
        }

        public void Execute(object parameter)
        {
            VM.CopyDatabase();
        }
    }
}

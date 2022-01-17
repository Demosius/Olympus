using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Olympus.ViewModel.Commands
{
    public class LogInCommand : ICommand
    {
        public LogInVM VM;

        public LogInCommand(LogInVM vm) { VM = vm; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return VM.UserID > 0 && VM.Password.Length >= 6;
        }

        public void Execute(object parameter)
        {
            if (VM.LogIn())
            {
                Window window = parameter as Window;
                window.Close();
            }
        }
    }
}

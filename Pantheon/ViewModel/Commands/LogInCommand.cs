using Pantheon.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Pantheon.ViewModel.Commands
{
    public class LogInCommand : ICommand
    {
        public LoginVM VM { get; set; }

        public LogInCommand(LoginVM vm) { VM = vm; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return VM.UserCode.Length > 0 && VM.Password.Length > 0;
        }

        public void Execute(object parameter)
        {
            if (VM.LogIn())
            {
                Window w = parameter as Window;
                w.Close();
                PantheonWindow pw = new();
                pw.Show();
            }
        }
    }
}

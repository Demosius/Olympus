using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Pantheon.ViewModel.Interface;

namespace Pantheon.ViewModel.Commands
{
    internal class FindNewImageCommand : ICommand
    {
        public IImageSelector VM { get; set; }

        public FindNewImageCommand(IImageSelector vm)
        {
            VM = vm;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            VM.FindNewImage();
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}

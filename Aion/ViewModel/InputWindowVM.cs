﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using Aion.Annotations;

namespace Aion.ViewModel
{
    public class InputWindowVM : INotifyPropertyChanged
    {
        private string title;
        public string Title
        {
            get => title;
            set
            {
                title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        private string prompt;
        public string Prompt
        {
            get => prompt;
            set
            {
                prompt = value;
                OnPropertyChanged(nameof(Prompt));
            }
        }

        private string input;
        public string Input
        {
            get => input;
            set
            {
                input = value;
                OnPropertyChanged(nameof(Input));
            }
        }

        public InputWindowVM()
        {
            Title = "Input Window";
            Prompt = "Enter Text";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new(propertyName));
        }
    }
}

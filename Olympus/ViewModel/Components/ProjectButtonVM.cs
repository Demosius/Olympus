using Olympus.Uranus.Staff.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.ViewModel.Components
{
    public class ProjectButtonVM : INotifyPropertyChanged
    {
        private Project project;
        public Project Project 
        {
            get => project; 
            set
            {
                project = value;
                OnPropertyChanged(nameof(Project));
            } 
        }
        public ProjectButtonVM() { }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

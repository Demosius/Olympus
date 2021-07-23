using Olympus.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.ViewModel.Components
{
    public class DBSelectionVM : INotifyPropertyChanged
    {
        private string dbString;
        public string DBString
        {
            get => dbString; 
            set
            {
                if (value == App.BaseDirectory())
                    dbString = "Local";
                else
                    dbString = value;
                OnPropertyChanged(nameof(DBString));
            }
        }

        public ChangeDatabaseCommand ChangeDatabaseCommand { get; set; }
        public MoveDatabaseCommand MoveDatabaseCommand { get; set; }
        public CopyDatabaseCommand CopyDatabaseCommand { get; set; }

        public DBSelectionVM()
        {
            ChangeDatabaseCommand = new ChangeDatabaseCommand(this);
            MoveDatabaseCommand = new MoveDatabaseCommand(this);
            CopyDatabaseCommand = new CopyDatabaseCommand(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string SelectFolder()
        {
            throw new NotImplementedException();
        }

        public void ChangeDatabase()
        {

        }

    }
}

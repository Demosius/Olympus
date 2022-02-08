using Aion.ViewModel.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Uranus.Staff.Model;

namespace Aion.ViewModel
{
    public class SignInVM : INotifyPropertyChanged
    {


        private ObservableCollection<Employee> managers;
        public ObservableCollection<Employee> Managers 
        {
            get => managers; 
            set
            {
                managers = value;
                OnPropertyChanged(nameof(Managers));
            }
        }

        private Employee selectedManager;
        public Employee SelectedManager
        {
            get  => selectedManager; 
            set 
            {
                selectedManager = value;
                OnPropertyChanged(nameof(SelectedManager));
                Code = "";
            }
        }

        private string code;
        public string Code
        {
            get => code;
            set
            {
                code = value;
                OnPropertyChanged(nameof(Code));
            }
        }

        // Command(s)
        public SignInCommand SignInCommand { get; set; }

        public SignInVM()
        {
            Managers = new(App.Helios.StaffReader.GetManagers());
            SignInCommand = new(this);
        }

        /// <summary>
        /// Compares input code to Selected manager.
        /// </summary>
        /// <returns>Bool: true if Code is correct, otherwise - false.</returns>
        public bool SignIn()
        {
            if (SelectedManager is null || Code?.Length <= 0) return false;
            _ = int.TryParse(Code, out var tryID);
            return SelectedManager.ID == tryID;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new(propertyName));
        }
    }
}

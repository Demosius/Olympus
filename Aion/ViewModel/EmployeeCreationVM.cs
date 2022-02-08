using Aion.ViewModel.Commands;
using System.Collections.Generic;
using System.ComponentModel;
using Uranus;
using Uranus.Staff.Model;

namespace Aion.ViewModel
{
    public class EmployeeCreationVM : INotifyPropertyChanged
    {
        public Helios Helios { get; set; }

        public Employee NewEmployee { get; set; }

        private List<int> existingCodes;

        private string newCode;
        public string NewCode
        {
            get => newCode;
            set
            {
                newCode = value;
                OnPropertyChanged(nameof(NewCode));
                CheckCode();
            }
        }

        private bool isFiveChars;
        public bool IsFiveChars
        {
            get => isFiveChars;
            set
            {
                isFiveChars = value;
                OnPropertyChanged(nameof(IsFiveChars));
            }
        }

        private bool isUnique;
        public bool IsUnique
        {
            get => isUnique;
            set
            {
                isUnique = value;
                OnPropertyChanged(nameof(IsUnique));
            }
        }

        private bool isNumeric;
        public bool IsNumeric
        {
            get => isNumeric;
            set
            {
                isNumeric = value;
                OnPropertyChanged(nameof(isNumeric));
            }
        }

        public ConfirmEmployeeCreationCommand ConfirmEmployeeCreationCommand { get; set; }

        public EmployeeCreationVM()
        {
            ConfirmEmployeeCreationCommand = new(this);
        }

        public void SetDataSource(Helios helios)
        {
            Helios = helios;
            existingCodes = Helios.StaffReader.EmployeeIDs();
        }

        private void CheckCode()
        {
            IsFiveChars = NewCode.Length == 5;
            IsNumeric = int.TryParse(NewCode, out var intCode) && intCode is >= 0 and <= 99999;
            IsUnique = existingCodes.BinarySearch(intCode) < 0;
        }

        public void ConfirmCreation()
        {
            NewEmployee = new() { ID = int.Parse(NewCode) };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new(propertyName));
        }
    }
}

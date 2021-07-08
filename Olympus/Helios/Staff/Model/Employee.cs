using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Olympus.Helios.Staff.Model
{
    public class Employee
    {
        public int Number { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        private decimal? _payRate;
        public decimal? PayRate
        {
            get { return _payRate; }
            set { _payRate = value; }
        }
        public string RF_ID { get; set; }
        public string ID { get; set; }
        public Department Department { get; set; }
        public Role Role { get; set; }
        private Locker _locker;
        public Locker Locker 
        {
            get { return _locker; }
            set
            {
                _locker.Employee = null;
                _locker = value;
            }
        }
        private string _phone;
        public string Phone
        {
            get { return _phone; }
            set { _phone = value; }
        }
        private string _email;
        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }
        private string _address;
        public string Address
        {
            get { return _address; }
            set { _address = value; }
        }
        public List<Vehicle> Vehicles { get; set; } = new List<Vehicle> { };

        public Employee() { }

        public Employee(int number, string firstName, string lastName, string displayName, decimal payRate, 
                        string rf_id, string id, Department department, Role role, Locker locker, string phone,
                        string email, string address)
        {
            Number = number;
            FirstName = firstName;
            LastName = lastName;
            DisplayName = displayName;
            _payRate = payRate;
            RF_ID = rf_id;
            ID = id;
            Department = department;
            Role = role;
            Locker = locker;
            _phone = phone;
            _email = email;
            _address = address;
        }
    }
}

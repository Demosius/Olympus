using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Olympus.Helios.Staff.Model
{
    public class Employee
    {
        [PrimaryKey]
        public int ID { get; set; } // Employee number (e.g. 60853)
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public decimal PayRate { get; set; }
        public string RF_ID { get; set; }
        public string PC_ID { get; set; }
        [ForeignKey(typeof(Department))]
        public string DepartmentName { get; set; }
        [ForeignKey(typeof(Role))]
        public string RoleName { get; set; }
        [ForeignKey(typeof(Locker))]
        public string LockerID { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }

        [ManyToOne]
        public Department Department { get; set; }
        [ManyToOne]
        public Role Role { get; set; }
        [OneToOne]
        public Locker Locker { get; set; }
        [ManyToMany(typeof(EmployeeVehicle))]
        public List<Vehicle> Vehicles { get; set; }
        [ManyToMany(typeof(EmployeeShift))]
        public List<Shift> Shifts { get; set; }
        [ManyToMany(typeof(EmployeeDepartmentLoaning))]
        public List<Department> DepartmentsCanWorkIn { get; set; }
        [OneToMany]
        public List<EmployeeInductionReference> InductionReferences { get; set; }

        public void SetDepartment(Department department)
        {
            Department = department;
            DepartmentName = department.Name;
            department.Employees.Add(this);
        }

        public void SetRole(Role role)
        {
            Role = role;
            RoleName = role.Name;
            role.Employees.Add(this);
        }

        public override bool Equals(object obj) => this.Equals(obj as Employee);

        public bool Equals(Employee employee)
        {
            if (employee is null) return false;

            if (Object.ReferenceEquals(this, employee)) return true;

            if (this.GetType() != employee.GetType()) return false;

            return ID == employee.ID;
        }

        public override int GetHashCode() => (ID, FirstName, LastName, DisplayName, PayRate,
                                              RF_ID, PC_ID, DepartmentName, RoleName, LockerID, 
                                              PhoneNumber, Email, Address).GetHashCode();

        public static bool operator ==(Employee lhs, Employee rhs)
        {
            if (lhs is null)
            {
                if (rhs is null)
                {
                    return true;
                }
                return false;
            }
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Employee lhs, Employee rhs) => !(lhs == rhs);

    }
}

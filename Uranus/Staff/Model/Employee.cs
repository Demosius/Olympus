using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

namespace Uranus.Staff.Model
{
    public enum EEmploymentType
    {
        SA, // Salaried
        CA, // Casual
        FP, // Full-time Permanent
        PP, // Part-time Permanent
    }

    public class Employee
    {
        [PrimaryKey]
        public int ID { get; set; } // Employee number (e.g. 60853)
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [NotNull]
        public string DisplayName { get; set; }
        public decimal? PayRate { get; set; }
        public string RF_ID { get; set; }
        public string PC_ID { get; set; }
        public string Location { get; set; }
        [ForeignKey(typeof(Department))]
        public string DepartmentName { get; set; }
        [ForeignKey(typeof(Role))]
        public string RoleName { get; set; }    // Also known as Job Classification.
        public int ReportsToID { get; set; }    // Specific Employee this employee reports to, bypassing Role and RoleReports.
        public string PayPoint { get; set; }
        public EEmploymentType EmploymentType { get; set; }
        [ForeignKey(typeof(Locker))]
        public string LockerID { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        [ForeignKey(typeof(EmployeeIcon))]
        public string IconName { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public Department Department { get; set; }
        [ManyToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public Role Role { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public Locker Locker { get; set; }
        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public Licence Licence { get; set; }
        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public EmployeeIcon Icon { get; set; }

        [ManyToMany(typeof(EmployeeVehicle), "VehicleRego" , "Owners" , CascadeOperations = CascadeOperation.All)]
        public List<Vehicle> Vehicles { get; set; }
        [ManyToMany(typeof(EmployeeShift), "ShiftName", "Employees", CascadeOperations = CascadeOperation.All)]
        public List<Shift> Shifts { get; set; }
        [ManyToMany(typeof(EmployeeDepartmentLoaning), "DepartmentName", "EmployeesCanLoan", CascadeOperations = CascadeOperation.All)]
        public List<Department> DepartmentsCanWorkIn { get; set; }
        [ManyToMany(typeof(EmployeeProject), "ProjectName", "Employees", CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public List<Project> Projects { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<EmployeeInductionReference> InductionReferences { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<ShiftRule> Rules { get; set; }

        /*public override bool Equals(object obj) => Equals(obj as Employee);

        public bool Equals(Employee employee)
        {
            if (employee is null) return false;

            if (ReferenceEquals(this, employee)) return true;

            if (GetType() != employee.GetType()) return false;

            return ID == employee.ID;
        }

        public static bool operator ==(Employee lhs, Employee rhs)
        {
            if (lhs is not null) return lhs.Equals(rhs);
            return rhs is null;
        }

        public static bool operator !=(Employee lhs, Employee rhs) => !(lhs == rhs);*/

    }
}

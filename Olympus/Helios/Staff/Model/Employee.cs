﻿using SQLite;
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
        [NotNull]
        public string DisplayName { get; set; }
        public decimal? PayRate { get; set; } = null;
        public string RF_ID { get; set; }
        public string PC_ID { get; set; }
        [ForeignKey(typeof(Department)), NotNull]
        public string DepartmentName { get; set; }
        [ForeignKey(typeof(Role)), NotNull]
        public string RoleName { get; set; }
        [ForeignKey(typeof(Locker))]
        public string LockerID { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }

        private Department department;
        private Role role;

        [ManyToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public Department Department
        {
            get => department;
            set
            {
                department = value;
                DepartmentName = value.Name;
                value.Employees.Add(this);
            }
        }
        [ManyToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public Role Role
        {
            get => role; 
            set
            {
                role = value;
                RoleName = value.Name;
                value.Employees.Add(this);
            }
        }
        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public Locker Locker { get; set; }
        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public Licence Licence { get; set; }
        [ManyToMany(typeof(EmployeeVehicle), "VehicleRego" , "Owners" , CascadeOperations = CascadeOperation.All)]
        public List<Vehicle> Vehicles { get; set; }
        [ManyToMany(typeof(EmployeeShift), "ShiftName", "Employees", CascadeOperations = CascadeOperation.All)]
        public List<Shift> Shifts { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<EmployeeInductionReference> InductionReferences { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<ShiftRule> Rules { get; set; }
        [ManyToMany(typeof(EmployeeDepartmentLoaning), "DepartmentName", "EmployeesCanLoan", CascadeOperations = CascadeOperation.All)]
        public List<Department> DepartmentsCanWorkIn { get; set; }


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

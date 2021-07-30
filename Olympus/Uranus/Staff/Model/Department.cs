﻿using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Olympus.Uranus.Staff.Model
{
    public class Department
    {
        [PrimaryKey]
        public string Name { get; set; }
        [ForeignKey(typeof(Employee))]
        public int HeadID { get; set; }

        [OneToOne]
        public Employee Head { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Shift> Shifts { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Employee> Employees { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Clan> Clans { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Role> Roles { get; set; }
        
        [ManyToMany(typeof(EmployeeDepartmentLoaning), "EmployeeID", "DepartmentsCanWorkIn", CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public List<Employee> EmployeesCanLoan { get; set; }
        [ManyToMany(typeof(DepartmentProject), "ProjectName", "Departments", CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public List<Project> Projects { get; set; }

        /* Constructor */
        public Department() { }

        public override bool Equals(object obj) => this.Equals(obj as Department);

        public bool Equals(Department department)
        {
            if (department is null) return false;

            if (Object.ReferenceEquals(this, department)) return true;

            if (this.GetType() != department.GetType()) return false;

            return Name == department.Name;
        }

        public override int GetHashCode() => (Name, HeadID).GetHashCode();

        public static bool operator ==(Department lhs, Department rhs)
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

        public static bool operator !=(Department lhs, Department rhs) => !(lhs == rhs);

    }
}

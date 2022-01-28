using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Uranus.Staff.Model
{
    public class DepartmentProject
    {
        public string DepartmentName { get; set; }
        public string ProjectName { get; set; }
    }
}

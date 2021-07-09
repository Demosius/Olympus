using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Helios.Staff.Model
{
    public class EmployeeInductionReference
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        [ForeignKey(typeof(Employee))]
        public int EmployeeID { get; set; }
        [ForeignKey(typeof(Induction))]
        public string InductionType { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpiryDate { get; set; }

        [OneToMany]
        public Employee Employee { get; set; }
        [OneToMany]
        public Induction Induction { get; set; }
    }
}

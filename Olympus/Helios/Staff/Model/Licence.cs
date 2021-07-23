using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Olympus.Helios.Staff.Model
{
    public class Licence
    {
        [PrimaryKey]
        public string Number { get; set; } 
        public DateTime IssueDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool LF { get; set; }
        public bool LO { get; set; }
        public bool WP { get; set; }
        public string ImageName { get; set; }
        [ForeignKey(typeof(Employee))]
        public int EmployeeID { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public Employee Employee { get; set; }
        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public LicenceImage Image { get; set; }

    }
}

using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Helios.Staff.Model
{
    public class TempTag
    {
        [PrimaryKey]
        public string RF_ID { get; set; }
        [ForeignKey(typeof(Employee))]
        public int EmployeeID { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public TagUse TagUse { get; set; }
        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public Employee Employee { get; set; }
    }
}

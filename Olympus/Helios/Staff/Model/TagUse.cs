using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Helios.Staff.Model
{
    public class TagUse
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        [ForeignKey(typeof(Employee))]
        public int EmployeeID { get; set; }
        [ForeignKey(typeof(TempTag))]
        public string TempTagRF_ID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [ManyToOne]
        public TempTag TempTag { get; set; }
        [ManyToOne]
        public Employee Employee { get; set; }
    }
}

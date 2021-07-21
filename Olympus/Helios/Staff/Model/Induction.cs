using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Helios.Staff.Model
{
    public class Induction
    {
        [PrimaryKey]
        public string Type { get; set; }
        public string Description { get; set; }
        public string Period { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<EmployeeInductionReference> EmployeeReferences { get; set; }
    }
}

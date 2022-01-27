using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

namespace Uranus.Staff.Model
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

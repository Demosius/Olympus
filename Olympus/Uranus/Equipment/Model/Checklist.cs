using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Olympus.Uranus.Equipment.Model
{
    public class Checklist
    {
        [PrimaryKey]
        public string Name { get; set; }
        [ForeignKey(typeof(MachineType))]
        public string TypeCode { get; set; }
        public string CheckCode { get; set; }

        [Ignore]
        public List<Check> Checks { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public List<Machine> Machines { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public MachineType MachineType { get; set; }

        public Checklist() { }

        public Checklist(string name, string typeCode, List<Check> checks)
        {
            Name = name;
            TypeCode = typeCode;
            Checks = checks;
        }
    }

    public class Check
    {
        public string Description { get; set; }
        public bool DesiredResponse { get; set; }
        public bool? Response { get; set; }
        public bool FaultFails { get; set; }

        public bool? IsFault()
        {
            return Response is null ? Response : Response != DesiredResponse;
        }

        public string FaultString()
        {
            return $"{Description}: {Response}";
        }
    }

}
